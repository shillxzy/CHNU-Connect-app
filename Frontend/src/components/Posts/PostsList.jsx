import { useState, useEffect, useContext } from 'react';
import { getFeed, createPostWithImage, likePost } from '../../api/postAPI';
import { getProfile } from '../../api/userAPI';
import Post from './Post';
import Loading from '../Loading/Loading';
import AuthContext from '../../context/AuthContext';
import UserTooltip from '../ToolTip/UserTooltip';
import '../NewsFeed/NewsFeed.css';

const PAGE_SIZE = 10;

export default function PostsList() {
  const { user: currentUser } = useContext(AuthContext);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [profile, setProfile] = useState(null);
  const [newPostContent, setNewPostContent] = useState('');
  const [newPostImage, setNewPostImage] = useState(null);
  const [previewImage, setPreviewImage] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [feedRes, profileRes] = await Promise.all([
          getFeed(page, PAGE_SIZE),
          getProfile(),
        ]);

        const data = feedRes.data;
        setPosts(Array.isArray(data.items) ? data.items : []);
        setTotalCount(data.totalCount || 0);
        setProfile(profileRes.data);
      } catch (error) {
        // eslint-disable-next-line no-console
        console.error('Error fetching posts:', error);
        setPosts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [page]);

  const handleLikeToggle = async (postId, hasLiked) => {
    try {
      await likePost(postId, currentUser?.id, hasLiked);
      setPosts((prev) =>
        prev.map((p) =>
          p.id === postId
            ? {
                ...p,
                hasCurrentUserLiked: !hasLiked,
                likeCount: hasLiked ? p.likeCount - 1 : p.likeCount + 1,
              }
            : p,
        ),
      );
    } catch (e) {
      // eslint-disable-next-line no-console
      console.error('Like error:', e);
    }
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setNewPostImage(file);
      setPreviewImage(URL.createObjectURL(file));
    }
  };

  const handleCreatePost = async () => {
    if (!newPostContent.trim() && !newPostImage) {
      return;
    }
    try {
      const formData = new FormData();
      formData.append('content', newPostContent);
      if (newPostImage) {
        formData.append('image', newPostImage);
      }

      const res = await createPostWithImage(formData);
      const newPost = {
        ...res.data,
        authorName: profile?.fullName || currentUser?.fullName,
        authorAvatar: profile?.photoUrl || '../Icons/default-avatar-icon.png',
        hasCurrentUserLiked: false,
        likeCount: 0,
      };

      setPosts((prev) => [newPost, ...prev].slice(0, PAGE_SIZE));
      setTotalCount((prev) => prev + 1);
      setNewPostContent('');
      setNewPostImage(null);
      setPreviewImage(null);
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error('Помилка створення поста:', error);
    }
  };

  if (loading) {
    return <Loading />;
  }

  const canCreatePost = currentUser?.role !== 'student';

  return (
    <div className="posts-feed">
      {canCreatePost && (
        <div className="post-creator">
          <div className="post-creator-top">
            {profile && (
              <UserTooltip
                userId={profile.id}
                currentUserId={profile?.id}
                size={38}
              />
            )}
            <textarea
              className="creator-input"
              placeholder="Що нового?"
              value={newPostContent}
              onChange={(e) => setNewPostContent(e.target.value)}
            />
          </div>

          {previewImage && (
            <div className="post-image-preview">
              <img src={previewImage} alt="preview" />
            </div>
          )}

          <div className="post-creator-actions">
            <label className="creator-add-btn">
              📷 Фото
              <input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
                style={{ display: 'none' }}
              />
            </label>
            <button className="creator-post-btn" onClick={handleCreatePost}>
              Опублікувати
            </button>
          </div>
        </div>
      )}

      <Post
        posts={posts}
        onLikeToggle={handleLikeToggle}
        currentUser={currentUser}
      />

      {totalCount > PAGE_SIZE && (
        <div
          style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            gap: '12px',
            padding: '16px 0',
          }}
        >
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            style={{
              padding: '8px 16px',
              borderRadius: '8px',
              border: '1px solid #ddd',
              cursor: page === 1 ? 'default' : 'pointer',
              background: page === 1 ? '#f5f5f5' : '#fff',
            }}
          >
            ← Назад
          </button>
          <span style={{ fontSize: '14px', color: '#555' }}>
            Сторінка {page} з {Math.ceil(totalCount / PAGE_SIZE)}
          </span>
          <button
            onClick={() => setPage((p) => p + 1)}
            disabled={page >= Math.ceil(totalCount / PAGE_SIZE)}
            style={{
              padding: '8px 16px',
              borderRadius: '8px',
              border: '1px solid #ddd',
              cursor:
                page >= Math.ceil(totalCount / PAGE_SIZE)
                  ? 'default'
                  : 'pointer',
              background:
                page >= Math.ceil(totalCount / PAGE_SIZE) ? '#f5f5f5' : '#fff',
            }}
          >
            Вперед →
          </button>
        </div>
      )}
    </div>
  );
}
