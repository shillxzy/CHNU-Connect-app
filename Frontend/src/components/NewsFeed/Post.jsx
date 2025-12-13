import React from 'react';
import { LikeIcon, CommentIcon } from '../Icons';
import '../HomePage.css';

const PostActions = ({ likes, comments }) => (
    <div className="post-actions">
        <button className="action-button like">
            <img src={LikeIcon} alt="Like" className="action-icon" /> {likes}
        </button>
        <button className="action-button">
            <img src={CommentIcon} alt="Comment" className="action-icon" /> {comments}
        </button>
        <button className="action-button">
            🔗Share
        </button>
    </div>
);

const Post = ({ data }) => (
    <div className="post-card">
        <div className="post-header">
            <div className="avatar"></div>
            <strong className="author-name">{data.author}</strong>
        </div>
        <p className="post-content">{data.content}</p>
        <PostActions likes={data.likeCount} comments={data.comments} />
    </div>
);

export default Post;
