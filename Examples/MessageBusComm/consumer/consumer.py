import os
import json
import datetime
from dotenv import load_dotenv
from sqlalchemy import create_engine, Column, Integer, String, Text, DateTime, ForeignKey, MetaData, Table, select, insert, update, delete
from sqlalchemy.orm import relationship, declarative_base, Session
from kafka import KafkaConsumer

# Load environment variables
load_dotenv()
KAFKA_HOST = os.getenv("KAFKA_HOST")
KAFKA_PORT = os.getenv("KAFKA_PORT")
KAFKA_TOPIC = os.getenv("KAFKA_TOPIC", "post_comments")
DB_CONNECTION_STRING = os.getenv("CONSUMER_DB_CONNECTION_STRING")

# Connect to database
engine = create_engine(DB_CONNECTION_STRING)
Base = declarative_base()

# Define models with relationships
class Post(Base):
    __tablename__ = 'posts'
    
    id = Column(Integer, primary_key=True)
    post_name = Column(String(100), nullable=False)
    post_content = Column(Text, nullable=False)
    created_at = Column(DateTime, default=datetime.datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.datetime.utcnow, onupdate=datetime.datetime.utcnow)
    
    comments = relationship("Comment", back_populates="post", cascade="all, delete-orphan")

class Comment(Base):
    __tablename__ = 'comments'
    
    id = Column(Integer, primary_key=True)
    post_id = Column(Integer, ForeignKey('posts.id'), nullable=False)
    commenter = Column(String(100), nullable=False)
    comment = Column(Text, nullable=False)
    like_count = Column(Integer, default=0)
    created_at = Column(DateTime, default=datetime.datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.datetime.utcnow, onupdate=datetime.datetime.utcnow)
    
    post = relationship("Post", back_populates="comments")

# Create tables
Base.metadata.create_all(engine)

# Connect to Kafka
consumer = KafkaConsumer(
    KAFKA_TOPIC,
    bootstrap_servers=f'{KAFKA_HOST}:{KAFKA_PORT}',
    auto_offset_reset='earliest',
    enable_auto_commit=True,
    group_id='post_comment_consumer',
    value_deserializer=lambda x: json.loads(x.decode('utf-8'))
)

def process_message(message):
    """Process a message from Kafka and store it in the database"""
    with Session(engine) as session:
        message_type = message.get('type')
        message_id = message.get('id')
        
        if message_type == 'delete':
            # Handle delete - we don't have post details in delete messages
            # So we need to find posts associated with this message ID
            # For this example, we'll just log it
            print(f"Received delete message for ID: {message_id}")
            # In a real system, you would need additional logic to identify which record to delete
            
        elif message_type in ['create', 'update']:
            # Handle create or update
            for post_data in message.get('posts', []):
                # Check if post exists (by name and content for simplicity)
                post = session.query(Post).filter_by(
                    post_name=post_data['post_name'],
                    post_content=post_data['post_content']
                ).first()
                
                if not post:
                    # Create new post
                    post = Post(
                        post_name=post_data['post_name'],
                        post_content=post_data['post_content']
                    )
                    session.add(post)
                    session.flush()  # To get the post ID
                
                # Process comments
                for comment_data in post_data.get('comments', []):
                    # Check if comment exists
                    comment = None
                    if post.id:
                        comment = session.query(Comment).filter_by(
                            post_id=post.id,
                            commenter=comment_data['commenter'],
                            comment=comment_data['comment']
                        ).first()
                    
                    if not comment:
                        # Create new comment
                        comment = Comment(
                            post_id=post.id,
                            commenter=comment_data['commenter'],
                            comment=comment_data['comment'],
                            like_count=comment_data['like_count']
                        )
                        session.add(comment)
                    else:
                        # Update existing comment
                        comment.like_count = comment_data['like_count']
                
                session.commit()
                print(f"Processed {message_type} message for post: {post_data['post_name']}")

def main():
    print("Starting consumer...")
    
    try:
        for message in consumer:
            try:
                print(f"Received message: {message.value}")
                process_message(message.value)
            except Exception as e:
                print(f"Error processing message: {e}")
    except KeyboardInterrupt:
        print("Consumer shutting down...")
    finally:
        consumer.close()

if __name__ == "__main__":
    main()
