import React from 'react';
import { LikeIcon, CommentIcon } from '../Icons';
import Avatar from '../Avatar/Avatar';
import './Post.css';

const PostActions = ({ likes, comments, liked, onLikeToggle }) => (
    <div className="post-actions">
        <button
            className={`action-button like ${liked ? 'liked' : ''}`}
            onClick={onLikeToggle}
        >
            <img src={LikeIcon} alt="Like" className="action-icon" /> {likes}
        </button>
        <button className="action-button">
            <img src={CommentIcon} alt="Comment" className="action-icon" /> {comments}
        </button>
        <button className="action-button">🔗Share</button>
    </div>
);

const Post = ({ posts, onLikeToggle }) => {

    return (
        <>
            {posts.map(post => (
                <div className="post-card" key={post.id}>
                    <div className="post-header">
                        <Avatar 
                            photoUrl={post.authorAvatar} 
                            size={35} 
                            className="avatar" 
                        />
                        <strong className="author-name">{post.authorName}</strong>
                    </div>
                    <p className="post-content">{post.content}</p>
                    <PostActions
                        likes={post.likeCount}
                        comments={post.comments?.length || 0}
                        liked={post.liked || false}
                        onLikeToggle={() => onLikeToggle(post.id)}
                    />
                </div>
            ))}
        </>
    );
};

export default Post;
