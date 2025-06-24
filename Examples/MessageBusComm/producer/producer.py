import os
import json
import time
import hashlib
import datetime
from dotenv import load_dotenv
from sqlalchemy import create_engine, Column, Integer, String, Text, DateTime, MetaData, Table, select, insert, update
from kafka import KafkaProducer

# Load environment variables
load_dotenv()
KAFKA_HOST = os.getenv("KAFKA_HOST")
KAFKA_PORT = os.getenv("KAFKA_PORT")
KAFKA_TOPIC = os.getenv("KAFKA_TOPIC", "post_comments")
DB_CONNECTION_STRING = os.getenv("PRODUCER_DB_CONNECTION_STRING")

# Connect to database
engine = create_engine(DB_CONNECTION_STRING)
metadata = MetaData()

# Define tables
post_comments = Table(
    'post_comments', 
    metadata,
    Column('id', Integer, primary_key=True),
    Column('post_name', String(100), nullable=False),
    Column('post_content', Text, nullable=False),
    Column('commenter', String(100), nullable=False),
    Column('comment', Text, nullable=False),
    Column('like_count', Integer, default=0),
    Column('created_at', DateTime, default=datetime.datetime.utcnow),
    Column('updated_at', DateTime, default=datetime.datetime.utcnow, onupdate=datetime.datetime.utcnow)
)

processed_posts = Table(
    'processed_posts',
    metadata,
    Column('id', String(64), primary_key=True),
    Column('hash', String(64), nullable=False),
    Column('processed_at', DateTime, default=datetime.datetime.utcnow)
)

# Create tables if they don't exist
metadata.create_all(engine)

# Connect to Kafka
producer = KafkaProducer(
    bootstrap_servers=f'{KAFKA_HOST}:{KAFKA_PORT}',
    value_serializer=lambda x: json.dumps(x).encode('utf-8')
)

def validate_post_comment(post_comment):
    """Simple validation of post comments"""
    if not post_comment['post_name'] or len(post_comment['post_name']) > 100:
        return False
    
    if not post_comment['post_content']:
        return False
    
    if not post_comment['commenter'] or len(post_comment['commenter']) > 100:
        return False
    
    if not post_comment['comment']:
        return False
    
    if post_comment['like_count'] < 0:
        return False
    
    return True

def generate_hash(post_comment):
    """Generate a hash for a post comment based on its content"""
    data = f"{post_comment['post_name']}:{post_comment['post_content']}:{post_comment['commenter']}:{post_comment['comment']}:{post_comment['like_count']}"
    return hashlib.sha256(data.encode()).hexdigest()

def generate_id(post_comment):
    """Generate an ID based on the post comment content"""
    return hashlib.md5(f"{post_comment['id']}:{generate_hash(post_comment)}".encode()).hexdigest()

def group_by_post(post_comments):
    """Group comments by post to structure data properly"""
    posts = {}
    for comment in post_comments:
        post_name = comment['post_name']
        post_content = comment['post_content']
        post_key = f"{post_name}:{post_content}"
        
        if post_key not in posts:
            posts[post_key] = {
                'post_name': post_name,
                'post_content': post_content,
                'comments': []
            }
        
        posts[post_key]['comments'].append({
            'commenter': comment['commenter'],
            'comment': comment['comment'],
            'like_count': comment['like_count']
        })
    
    return list(posts.values())

def check_and_process_changes():
    with engine.connect() as connection:
        # Get all post comments
        result = connection.execute(select(post_comments))
        current_records = [row._asdict() for row in result]
        
        # Process each record
        for record in current_records:
            current_hash = generate_hash(record)
            record_id = generate_id(record)
            
            # Check if record was processed before
            processed = connection.execute(
                select(processed_posts).where(processed_posts.c.id == record_id)
            ).fetchone()
            
            message_type = None
            
            if not processed:
                # New record
                message_type = 'create'
            elif processed['hash'] != current_hash:
                # Updated record
                message_type = 'update'
            
            if message_type and validate_post_comment(record):
                # Group comments by post
                grouped_data = group_by_post([record])
                
                # Prepare message
                message = {
                    'id': record_id,
                    'type': message_type,
                    'timestamp': datetime.datetime.utcnow().isoformat(),
                    'posts': grouped_data
                }
                
                # Send to Kafka
                producer.send(KAFKA_TOPIC, value=message)
                print(f"Sent message: {message_type} for record {record_id}")
                
                # Update processed_posts table
                if not processed:
                    connection.execute(
                        insert(processed_posts).values(
                            id=record_id, 
                            hash=current_hash,
                            processed_at=datetime.datetime.utcnow()
                        )
                    )
                else:
                    connection.execute(
                        update(processed_posts)
                        .where(processed_posts.c.id == record_id)
                        .values(
                            hash=current_hash,
                            processed_at=datetime.datetime.utcnow()
                        )
                    )
        
        # Check for deleted records
        processed_ids = [row['id'] for row in connection.execute(select(processed_posts.c.id))]
        current_ids = [generate_id(record) for record in current_records]
        
        deleted_ids = set(processed_ids) - set(current_ids)
        
        for deleted_id in deleted_ids:
            # Send delete message
            message = {
                'id': deleted_id,
                'type': 'delete',
                'timestamp': datetime.datetime.utcnow().isoformat()
            }
            
            producer.send(KAFKA_TOPIC, value=message)
            print(f"Sent message: delete for record {deleted_id}")
            
            # Remove from processed_posts
            connection.execute(
                processed_posts.delete().where(processed_posts.c.id == deleted_id)
            )

def main():
    print("Starting producer...")
    
    try:
        while True:
            check_and_process_changes()
            print(f"Checked for changes at {datetime.datetime.now().isoformat()}")
            time.sleep(30)  # Check every 30 seconds
    except KeyboardInterrupt:
        print("Producer shutting down...")
    finally:
        producer.close()

if __name__ == "__main__":
    main()
