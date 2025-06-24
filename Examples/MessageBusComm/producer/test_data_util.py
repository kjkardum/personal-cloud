import os
import datetime
from dotenv import load_dotenv
from sqlalchemy import create_engine, MetaData, Table, insert, select
from sqlalchemy.orm import sessionmaker

# Load environment variables
load_dotenv()
DB_CONNECTION_STRING = os.getenv("PRODUCER_DB_CONNECTION_STRING")

# Connect to database
engine = create_engine(DB_CONNECTION_STRING)
metadata = MetaData()
metadata.reflect(bind=engine)

# Get the post_comments table
post_comments = metadata.tables['post_comments']

# Create a session
Session = sessionmaker(bind=engine)
session = Session()

def add_test_post_comment(post_name, post_content, commenter, comment, like_count=0):
    # Insert a new post comment
    stmt = insert(post_comments).values(
        post_name=post_name,
        post_content=post_content,
        commenter=commenter,
        comment=comment,
        like_count=like_count,
        created_at=datetime.datetime.utcnow(),
        updated_at=datetime.datetime.utcnow()
    )
    
    result = session.execute(stmt)
    session.commit()
    
    print(f"Added post comment: {post_name} - {commenter}")

def list_all_post_comments():
    # Select all post comments
    stmt = select(post_comments)
    result = session.execute(stmt)
    
    for row in result:
        print(f"ID: {row.id}")
        print(f"Post: {row.post_name}")
        print(f"Content: {row.post_content}")
        print(f"Commenter: {row.commenter}")
        print(f"Comment: {row.comment}")
        print(f"Likes: {row.like_count}")
        print(f"Created: {row.created_at}")
        print(f"Updated: {row.updated_at}")
        print("-" * 50)

def update_post_comment(id, **kwargs):
    # Update a post comment
    stmt = post_comments.update().where(post_comments.c.id == id).values(**kwargs)
    session.execute(stmt)
    session.commit()
    
    print(f"Updated post comment ID: {id}")

def delete_post_comment(id):
    # Delete a post comment
    stmt = post_comments.delete().where(post_comments.c.id == id)
    session.execute(stmt)
    session.commit()
    
    print(f"Deleted post comment ID: {id}")

def main():
    print("Test Data Utility")
    print("1. Add test post comment")
    print("2. List all post comments")
    print("3. Update post comment")
    print("4. Delete post comment")
    print("5. Exit")
    
    choice = input("Enter your choice: ")
    
    if choice == "1":
        post_name = input("Post name: ")
        post_content = input("Post content: ")
        commenter = input("Commenter: ")
        comment = input("Comment: ")
        like_count = int(input("Like count: ") or "0")
        
        add_test_post_comment(post_name, post_content, commenter, comment, like_count)
    
    elif choice == "2":
        list_all_post_comments()
    
    elif choice == "3":
        id = int(input("Post comment ID: "))
        print("Enter new values (leave blank to keep current value)")
        
        post_name = input("Post name: ")
        post_content = input("Post content: ")
        commenter = input("Commenter: ")
        comment = input("Comment: ")
        like_count_str = input("Like count: ")
        
        update_dict = {}
        if post_name:
            update_dict["post_name"] = post_name
        if post_content:
            update_dict["post_content"] = post_content
        if commenter:
            update_dict["commenter"] = commenter
        if comment:
            update_dict["comment"] = comment
        if like_count_str:
            update_dict["like_count"] = int(like_count_str)
        
        if update_dict:
            update_dict["updated_at"] = datetime.datetime.utcnow()
            update_post_comment(id, **update_dict)
        else:
            print("No changes to update")
    
    elif choice == "4":
        id = int(input("Post comment ID: "))
        delete_post_comment(id)
    
    elif choice == "5":
        print("Exiting...")
        return
    
    else:
        print("Invalid choice")
    
    # Recursively call main to show menu again
    main()

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\nExiting...")
    finally:
        session.close()
