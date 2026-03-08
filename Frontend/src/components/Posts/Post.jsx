import React, { useState, useEffect } from "react";
import { LikeIcon, CommentIcon } from '../Icons';
import Avatar from '../Avatar/Avatar';
import './Post.css';
import {
  getCommentsByPost,
  createComment,
} from "../../api/commentAPI"; 


const PostActions = ({ likes, comments, liked, onLikeToggle, onCommentToggle }) => (
  <div className="post-actions">
    <button
      type="button"
      className={`action-button like ${liked ? "liked" : ""}`}
      onClick={onLikeToggle}
    >
      <img src={LikeIcon} alt="Like" className="action-icon" /> {likes || 0}
    </button>

    <button
      type="button"
      className="action-button"
      onClick={onCommentToggle} // керування відкриттям коментарів
    >
      <img src={CommentIcon} alt="Comment" className="action-icon" /> {comments || 0}
    </button>

    <button type="button" className="action-button">
      🔗 Share
    </button>
  </div>
);

/* ---------------- Comments Section ---------------- */
const CommentsSection = ({ postId, currentUser, open }) => {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [newComment, setNewComment] = useState("");

  useEffect(() => {
    if (open) fetchComments();
  }, [open]);

  const fetchComments = async () => {
    setLoading(true);
    try {
      const res = await getCommentsByPost(postId);
      setComments(res.data || []);
    } catch (err) {
      console.error("Failed to fetch comments:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddComment = async () => {
  if (!newComment.trim()) return;
  if (!currentUser?.id) {
    console.error("currentUser undefined, cannot add comment");
    return;
  }
  try {
    const res = await createComment({
      postId,
      userId: currentUser.id,
      content: newComment.trim(),
    });
    setComments((prev) => [...prev, res.data]);
    setNewComment("");
  } catch (err) {
    console.error("Failed to add comment:", err);
  }
};


  if (!open) return null;

  return (
    <div className="comments-section">
      {loading ? (
        <p>Завантаження коментарів...</p>
      ) : comments.length === 0 ? (
        <p>Ще ніхто не коментував.</p>
      ) : (
        comments.map((c) => (
          <div className="comment-item" key={c.id}>
            <Avatar photoUrl={c.authorAvatar} size={28} />
            <div className="comment-content">
              <strong>{c.authorName}</strong>
              <p>{c.content}</p>
            </div>
          </div>
        ))
      )}

      {/* Форма додавання коментаря */}
      <div className="comment-form">
        <input
          type="text"
          placeholder="Написати коментар..."
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") handleAddComment();
          }}
        />
        <button type="button" onClick={handleAddComment}>
          Відправити
        </button>
      </div>
    </div>
  );
};

/* ---------------- Post Component ---------------- */
const Post = ({ posts, onLikeToggle, currentUser }) => {
  const API_BASE = import.meta.env.VITE_API_BASE_URL;
  const [openComments, setOpenComments] = useState({}); // ключ: postId, значення: true/false

  const toggleComments = (postId) => {
    setOpenComments((prev) => ({
      ...prev,
      [postId]: !prev[postId],
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
              <Avatar
                photoUrl={post.authorAvatar || "/images/default-avatar-icon.png"}
                size={35}
              />
              <strong>{post.authorName || "Unknown"}</strong>
            </div>

            <p className="post-content">{post.content}</p>

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
              onCommentToggle={() => toggleComments(post.id)}
            />

            {/* Коментарі */}
            <CommentsSection
              postId={post.id}
              currentUser={currentUser}
              open={isOpen}
            />
          </div>
        );
      })}
    </>
  );
};

export default Post;
