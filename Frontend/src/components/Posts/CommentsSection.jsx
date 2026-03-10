import { useState, useEffect } from "react";

import { getCommentsByPost, createComment } from "../../api/commentAPI";
import Avatar from "../Avatar/Avatar";
import './Comment.css'

export const CommentsSection = ({ postId, currentUser, open, onCommentAdded }) => {
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
        <div className="comments-list">
          {comments.map((c) => (
            <div className="comment-item" key={c.id}>
              <Avatar photoUrl={c.authorAvatar} size={28} />
              <div className="comment-content">
                <div className="comment-user">{c.authorName}</div>
                <p className="comment-description">{c.content}</p>
              </div>
            </div>
          ))}
        </div>
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
        <button type="button" className="create-comment-button" onClick={handleAddComment}>
          Відправити
        </button>
      </div>
    </div>
  );
};