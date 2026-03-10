import React, { useState, useEffect } from "react";
import './Post.css';
import UserTooltip from "../ToolTip/UserTooltip";
import { PostActions } from "./PostAction";
import { CommentsSection } from "./CommentsSection";

const Post = ({ posts, onLikeToggle, currentUser }) => {
  const API_BASE = import.meta.env.VITE_API_BASE_URL;
  const [openComments, setOpenComments] = useState({});
  const [commentsCount, setCommentsCount] = useState({}); // ключ: postId, значення: true/false

    useEffect(() => {
    const counts = {};
    posts.forEach((post) => {
      counts[post.id] = post.comments?.length || 0;
      console.log(post.comments?.length)
    });

    setCommentsCount(counts);
  }, [posts]);

  const toggleComments = (postId) => {
    setOpenComments((prev) => ({
      ...prev,
      [postId]: !prev[postId],
    }));
  };

  const handleCommentAdded = (postId) => {
    setCommentsCount((prev) => ({
      ...prev,
      [postId]: (prev[postId] || 0) + 1,
    }));
  };

  return (
    <>
      {posts.map((post) => {
        const imageUrl = post.imageUrl ? `${API_BASE}${post.imageUrl}` : null;
        const isOpen = !!openComments[post.id];

        return (
          <div className="post-card" key={post.id}>
            <div className="post-header">
              <UserTooltip
                userId={post.authorId}
                currentUserId={currentUser?.id}
                size={35}
                fallbackAvatar={post.authorAvatar}
                fallbackName={post.authorName}
              />
              <div className="author-name">{post.authorName || "Unknown"}</div>
            </div>


            <p className="post-content">{post.content}</p>

            {imageUrl && (
              <div className="post-image">
                <img src={imageUrl} alt="post" />
              </div>
            )}

            <PostActions
              likes={post.likeCount || 0}
              comments={commentsCount[post.id] || 0} 
              liked={post.hasCurrentUserLiked || false} 
              onLikeToggle={() => onLikeToggle(post.id, post.hasCurrentUserLiked)} 
              onCommentToggle={() => toggleComments(post.id)}
            />

            <CommentsSection
              postId={post.id}
              currentUser={currentUser}
              open={isOpen}
              onCommentAdded={() => handleCommentAdded(post.id)}
            />
          </div>
        );
      })}
    </>
  );
};

export default Post;
