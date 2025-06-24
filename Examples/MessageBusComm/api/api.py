import os
from dotenv import load_dotenv
from fastapi import FastAPI, HTTPException
from sqlalchemy import create_engine, Column, Integer, String, Text, DateTime, ForeignKey
from sqlalchemy.orm import declarative_base, relationship, Session
from pydantic import BaseModel
from typing import List, Optional
import datetime

# Load environment variables
load_dotenv()
DB_CONNECTION_STRING = os.getenv("CONSUMER_DB_CONNECTION_STRING")

# Create FastAPI application
app = FastAPI(title="Post Comments API", description="API for retrieving post comments")

# Connect to database
engine = create_engine(DB_CONNECTION_STRING)
Base = declarative_base()

# Define models with relationships - same as in consumer but without creating tables
class Post(Base):
    __tablename__ = 'posts'
    
    id = Column(Integer, primary_key=True)
    post_name = Column(String(100), nullable=False)
    post_content = Column(Text, nullable=False)
    created_at = Column(DateTime, default=datetime.datetime.utcnow)
    updated_at = Column(DateTime, default=datetime.datetime.utcnow, onupdate=datetime.datetime.utcnow)
    
    comments = relationship("Comment", back_populates="post")

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

# Define Pydantic models for response
class CommentResponse(BaseModel):
    id: int
    commenter: str
    comment: str
    like_count: int
    
    model_config = {
        "from_attributes": True
    }

class PostResponse(BaseModel):
    id: int
    post_name: str
    post_content: str
    comments: List[CommentResponse]
    
    model_config = {
        "from_attributes": True
    }

@app.get("/posts", response_model=List[PostResponse])
def get_posts():
    """Get all posts with their comments"""
    with Session(engine) as session:
        # Query all posts with their comments
        posts = session.query(Post).all()
        return posts

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
