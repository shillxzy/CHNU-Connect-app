import React from "react";
import { LikeIcon, CommentIcon } from "../Icons";
import Avatar from "../Avatar/Avatar";
import "../HomePage.css";

const PostActions = ({ likes, comments, liked, onLikeToggle }) => (
    <div className="post-actions">
        <button
            type="button"
            className={`action-button like ${liked ? "liked" : ""}`}
            onClick={onLikeToggle}
        >
            <img src={LikeIcon} alt="Like" className="action-icon" /> {likes || 0}
        </button>

        <button type="button" className="action-button">
            <img src={CommentIcon} alt="Comment" className="action-icon" />{" "}
            {comments || 0}
        </button>

        <button type="button" className="action-button">🔗 Share</button>
    </div>
);

const Post = ({ posts, onLikeToggle }) => {
    const API_BASE = "http://localhost:5000"; // адреса бекенду

    return (
        <>
            {posts.map((post) => {
                const imageUrl = post.imageUrl
                    ? `${API_BASE}${post.imageUrl}`
                    : null;

                return (
                    <div className="post-card" key={post.id}>
                        <div className="post-header">
                            <Avatar
                                photoUrl={
                                    post.authorAvatar ||
                                    "/images/default-avatar-icon.png"
                                }
                                size={35}
                            />
                            <strong>{post.authorName || "Unknown"}</strong>
                        </div>

                        <p className="post-content">{post.content}</p>

                        {/* Фото поста */}
                        {imageUrl && (
                            <div className="post-image">
                                <img src={imageUrl} alt="post" />
                            </div>
                        )}

                        <PostActions
                            likes={post.likeCount || 0}
                            comments={post.comments?.length || 0}
                            liked={post.liked || false}
                            onLikeToggle={() => onLikeToggle(post.id)}
                        />
                    </div>
                );
            })}
        </>
    );
};

export default Post;
