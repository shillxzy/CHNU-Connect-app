import React from 'react';
import { LikeIcon, CommentIcon } from '../Icons';
import Avatar from '../Avatar/Avatar';
import '../HomePage.css';

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

const Post = ({ data, onLikeToggle }) => (
    <div className="post-card">
        <div className="post-header">
            <Avatar 
                photoUrl={data.authorAvatar} 
                size={35} 
                className="avatar" 
            />
            <strong className="author-name">{data.authorName}</strong>
        </div>
        <p className="post-content">{data.content}</p>
        <PostActions
            likes={data.likeCount}
            comments={data.comments?.length || 0}
            liked={data.liked || false}
            onLikeToggle={onLikeToggle}
        />
    </div>
);

export default Post;
