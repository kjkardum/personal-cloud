-- Sample data for testing the Kafka producer-consumer system
-- These statements are meant to be run on the producer database

-- Clear tables (if they exist)
DELETE FROM processed_posts;
DELETE FROM post_comments;

-- Insert sample posts with comments
-- Post 1: Programming Tips
INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Programming Tips', 'Here are some tips for programming effectively...', 'Alice', 'Great tips! I especially liked the one about code reviews.', 15);

INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Programming Tips', 'Here are some tips for programming effectively...', 'Bob', 'I would add that regular breaks are important too!', 7);

INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Programming Tips', 'Here are some tips for programming effectively...', 'Charlie', 'Do you have any specific tips for Python?', 3);

-- Post 2: Kafka Tutorial
INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Kafka Tutorial', 'Kafka is a distributed streaming platform...', 'David', 'This tutorial helped me understand Kafka better.', 10);

INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Kafka Tutorial', 'Kafka is a distributed streaming platform...', 'Eva', 'Could you elaborate more on Kafka Connect?', 5);

-- Post 3: FastAPI Guide
INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('FastAPI Guide', 'FastAPI is a modern web framework for building APIs...', 'Frank', 'FastAPI has been a game-changer for my projects!', 20);

-- Wait 30 seconds to allow the producer to process these insertions, then run the updates

-- Update statements (run these after the producer has processed the initial inserts)
-- Update a comment's like count
UPDATE post_comments 
SET like_count = 25 
WHERE post_name = 'Programming Tips' AND commenter = 'Alice';

-- Update post content
UPDATE post_comments 
SET post_content = 'Here are some updated tips for programming effectively...' 
WHERE post_name = 'Programming Tips';

-- Update commenter name
UPDATE post_comments 
SET commenter = 'Eva Updated' 
WHERE post_name = 'Kafka Tutorial' AND commenter = 'Eva';

-- Wait 30 seconds to allow the producer to process these updates, then run the deletes

-- Delete statements (run these after the producer has processed the updates)
-- Delete a single comment
DELETE FROM post_comments 
WHERE post_name = 'Programming Tips' AND commenter = 'Charlie';

-- Delete an entire post (all comments for a post)
DELETE FROM post_comments 
WHERE post_name = 'FastAPI Guide';

-- Additional statements to test various scenarios

-- Add a new comment to an existing post
INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('Kafka Tutorial', 'Kafka is a distributed streaming platform...', 'Grace', 'How does Kafka compare to RabbitMQ?', 0);

-- Add a completely new post with a comment
INSERT INTO post_comments (post_name, post_content, commenter, comment, like_count)
VALUES ('PostgreSQL Tips', 'PostgreSQL is a powerful open-source database...', 'Henry', 'I love the JSON features in PostgreSQL!', 12);

-- Update multiple fields at once
UPDATE post_comments 
SET commenter = 'Bob Updated', comment = 'Updated comment text!', like_count = 30 
WHERE post_name = 'Programming Tips' AND commenter = 'Bob';

-- Test script to verify the consumer database state
-- Run these queries against the consumer database to verify the data was properly processed

/*
-- Check posts in consumer database
SELECT * FROM posts;

-- Check comments in consumer database
SELECT * FROM comments;

-- Join posts and comments to see the relationship
SELECT p.post_name, p.post_content, c.commenter, c.comment, c.like_count
FROM posts p
JOIN comments c ON p.id = c.post_id
ORDER BY p.post_name, c.commenter;
*/
