import React, { useState, useEffect } from "react";
import { LikeIcon, CommentIcon } from "../Icons";
import Avatar from "../Avatar/Avatar";
import "../HomePage.css";
import { getCommentsByPost, createComment } from "../../api/commentAPI";

/* ---------------- Post Actions ---------------- */
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
      onClick={onCommentToggle} // відкриття/закриття коментарів
    >
      <img src={CommentIcon} alt="Comment" className="action-icon" /> {comments || 0}
    </button>

    <button type="button" className="action-button">
      🔗 Share
    </button>
  </div>
);

/* ---------------- Comments Section ---------------- */
const CommentsSection = ({ postId, currentUser, open, onCommentAdded }) => {
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

      // Оновлюємо лічильник коментарів у Post
      if (onCommentAdded) onCommentAdded();
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
  const API_BASE = "http://localhost:5000";
  const [openComments, setOpenComments] = useState({});
  const [commentsCount, setCommentsCount] = useState({}); // ключ: postId, значення: кількість коментарів

  // Ініціалізація лічильника коментарів
  useEffect(() => {
    const counts = {};
    posts.forEach((post) => {
      counts[post.id] = post.comments?.length || 0;
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
              comments={commentsCount[post.id] || 0} 
              liked={post.liked || false}
              onLikeToggle={() => onLikeToggle(post.id)}
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
