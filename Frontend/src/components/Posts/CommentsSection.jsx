import { useState, useEffect } from 'react';
import {
  getCommentsByPost,
  createComment,
  deleteComment,
  updateComment,
} from '../../api/commentAPI';
import Avatar from '../Avatar/Avatar';
import './Comment.css';
import Loading from '../Loading/Loading';

export const CommentsSection = ({
  postId,
  currentUser,
  open,
  onCommentAdded,
  onCommentDeleted,
}) => {
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [newComment, setNewComment] = useState('');

  const [editingId, setEditingId] = useState(null);
  const [editingText, setEditingText] = useState('');

  useEffect(() => {
    if (open) {
      fetchComments();
    }
  }, [open]);

  const fetchComments = async () => {
    setLoading(true);
    try {
      const res = await getCommentsByPost(postId);
      setComments(res.data || []);
    } catch (err) {
      console.error('Failed to fetch comments:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddComment = async () => {
    if (!newComment.trim()) {
      return;
    }
    if (!currentUser?.id) {
      return;
    }

    try {
      const res = await createComment({
        postId,
        userId: currentUser.id,
        content: newComment.trim(),
      });

      setComments((prev) => [...prev, res.data]);
      setNewComment('');
      onCommentAdded?.();
    } catch (err) {
      console.error('Failed to add comment:', err);
    }
  };

  const handleDelete = async (id) => {
    const confirmDelete = window.confirm('Видалити цей коментар?');
    if (!confirmDelete) {
      return;
    }

    try {
      await deleteComment(id);
      setComments((prev) => prev.filter((c) => c.id !== id));

      if (onCommentDeleted) {
        onCommentDeleted();
      }
    } catch (err) {
      console.error('Delete failed:', err);
    }
  };

  const handleEditStart = (comment) => {
    setEditingId(comment.id);
    setEditingText(comment.content);
  };

  const handleEditSave = async (id) => {
    if (!editingText.trim()) {
      return;
    }

    const oldComment = comments.find((c) => c.id === id);

    try {
      const res = await updateComment(id, {
        postId: oldComment.postId,
        userId: oldComment.userId,
        content: editingText.trim(),
      });

      setComments((prev) => prev.map((c) => (c.id === id ? res.data : c)));

      setEditingId(null);
      setEditingText('');
    } catch (err) {
      console.error('Update failed:', err);
    }
  };

  if (!open) {
    return null;
  }

  return (
    <div className="comments-section">
      {loading ? (
        <Loading />
      ) : comments.length === 0 ? (
        <p>Ще ніхто не коментував.</p>
      ) : (
        <div className="comments-list">
          {comments.map((c) => {
            const isOwner = currentUser?.id === c.userId;

            return (
              <div className="comment-item" key={c.id}>
                <Avatar photoUrl={c.authorAvatar} size={28} />

                <div className="comment-content">
                  <div className="comment-user">{c.authorName}</div>

                  {editingId === c.id ? (
                    <>
                      <input
                        value={editingText}
                        onChange={(e) => setEditingText(e.target.value)}
                      />
                      <div className="comment-actions">
                        <button onClick={() => handleEditSave(c.id)}>
                          Зберегти
                        </button>
                        <button onClick={() => setEditingId(null)}>
                          Скасувати
                        </button>
                      </div>
                    </>
                  ) : (
                    <>
                      <p className="comment-description">{c.content}</p>

                      {isOwner && (
                        <div className="comment-actions">
                          <button onClick={() => handleEditStart(c)}>
                            Редагувати
                          </button>
                          <button onClick={() => handleDelete(c.id)}>
                            Видалити
                          </button>
                        </div>
                      )}
                    </>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      <div className="comment-form">
        <input
          type="text"
          placeholder="Написати коментар..."
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              handleAddComment();
            }
          }}
        />
        <button
          type="button"
          className="create-comment-button"
          onClick={handleAddComment}
        >
          Відправити
        </button>
      </div>
    </div>
  );
};
