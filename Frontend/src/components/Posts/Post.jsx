import React, { useState, useEffect } from 'react';
import './Post.css';
import UserTooltip from '../ToolTip/UserTooltip';
import { PostActions } from './PostAction';
import { CommentsSection } from './CommentsSection';
import { deletePost, updatePost } from '../../api/postAPI';
import { getCommentsByPost } from '../../api/commentAPI';

const Post = ({ posts, onLikeToggle, currentUser }) => {
  const API_BASE = import.meta.env.VITE_API_BASE_URL;

  const [openComments, setOpenComments] = useState({});
  const [commentsCount, setCommentsCount] = useState({});
  const [openMenu, setOpenMenu] = useState({});
  const [localPosts, setLocalPosts] = useState(posts);
  const [editingPostId, setEditingPostId] = useState(null);
  const [editedContent, setEditedContent] = useState('');

  const getComments = async (postId) => {
    try {
      const res = await getCommentsByPost(postId);

      if (res) {
        return res?.data;
      }
    } catch {
      console.error('Can`t get comments');
      return [];
    }
  };

  useEffect(() => {
    const isMounted = true;
    setLocalPosts(posts);
    const fetchAllComments = async () => {
      const counts = {};

      const promises = posts.map(async (post) => {
        const data = await getComments(post.id);
        counts[post.id] = data.length;
      });

      await Promise.all(promises);

      if (isMounted) {
        setCommentsCount(counts);
      }
    };

    if (posts.length > 0) {
      fetchAllComments();
    }
  }, [posts]);

  const toggleComments = (postId) => {
    setOpenComments((prev) => ({
      ...prev,
      [postId]: !prev[postId],
    }));
  };

  const toggleMenu = (postId) => {
    setOpenMenu((prev) => ({
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

  const handleCommentDeleted = (postId) => {
    setCommentsCount((prev) => ({
      ...prev,
      [postId]: Math.max(0, (prev[postId] || 1) - 1),
    }));
  };

  const handleDelete = async (postId) => {
    const confirmDelete = window.confirm('Видалити пост?');
    if (!confirmDelete) {
      return;
    }

    try {
      await deletePost(postId);
      setLocalPosts((prev) => prev.filter((p) => p.id !== postId));
    } catch (e) {
      console.error('Помилка видалення:', e);
    }
  };

  const startEditing = (postId, currentContent) => {
    setEditingPostId(postId);
    setEditedContent(currentContent);
    setOpenMenu({});
  };

  const saveEdit = async (postId) => {
    if (!editedContent.trim()) {
      return;
    }

    try {
      const updated = await updatePost(postId, { content: editedContent });
      setLocalPosts((prev) =>
        prev.map((p) =>
          p.id === postId ? { ...p, content: updated.data.content } : p,
        ),
      );
      setEditingPostId(null);
      setEditedContent('');
    } catch (e) {
      console.error('Помилка редагування:', e);
    }
  };

  const cancelEdit = () => {
    setEditingPostId(null);
    setEditedContent('');
  };

  return (
    <>
      {localPosts.map((post) => {
        const imageUrl = post.imageUrl ? `${API_BASE}${post.imageUrl}` : null;
        const isOpen = !!openComments[post.id];
        const isEditing = editingPostId === post.id;

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
              <div className="author-name">{post.authorName || 'Unknown'}</div>

              {/* ТРИКРАПКА */}
              {currentUser?.id === post.authorId && !isEditing && (
                <div className="post-menu">
                  <button
                    className="menu-btn"
                    onClick={() => toggleMenu(post.id)}
                  >
                    ⋯
                  </button>
                  {openMenu[post.id] && (
                    <div className="menu-dropdown">
                      <button
                        onClick={() => startEditing(post.id, post.content)}
                      >
                        Редагувати
                      </button>
                      <button
                        className="delete-btn"
                        onClick={() => handleDelete(post.id)}
                      >
                        Видалити
                      </button>
                    </div>
                  )}
                </div>
              )}
            </div>

            {/* Контент або редагування */}
            {isEditing ? (
              <div className="edit-post" key={post.id}>
                <textarea
                  className="edit-textarea"
                  value={editedContent}
                  onChange={(e) => setEditedContent(e.target.value)}
                />
                <div className="edit-post-buttons">
                  <button
                    className="change-post-button submit"
                    onClick={() => saveEdit(post.id)}
                  >
                    Зберегти
                  </button>
                  <button
                    className="change-post-button cancel"
                    onClick={cancelEdit}
                    style={{ marginLeft: '5px' }}
                  >
                    Скасувати
                  </button>
                </div>
              </div>
            ) : (
              <p className="post-content">{post.content}</p>
            )}

            {imageUrl && (
              <div className="post-image">
                <img src={imageUrl} alt="post" />
              </div>
            )}

            <PostActions
              likes={post.likeCount || 0}
              comments={commentsCount[post.id] || 0}
              liked={post.hasCurrentUserLiked || false}
              onLikeToggle={() =>
                onLikeToggle(post.id, post.hasCurrentUserLiked)
              }
              onCommentToggle={() => toggleComments(post.id)}
            />

            <CommentsSection
              postId={post.id}
              currentUser={currentUser}
              open={isOpen}
              onCommentAdded={() => handleCommentAdded(post.id)}
              onCommentDeleted={() => handleCommentDeleted(post.id)}
            />
          </div>
        );
      })}
    </>
  );
};

export default Post;
