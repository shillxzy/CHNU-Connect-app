import { useState, useEffect, useContext } from 'react';
import { getPosts, createPostWithImage, likePost } from '../../api/postAPI';
import { getProfile } from '../../api/userAPI';
import Post from './Post';
import Loading from '../Loading/Loading';
import AuthContext from '../../context/AuthContext';
import UserTooltip from '../ToolTip/UserTooltip';
import '../NewsFeed/NewsFeed.css';

export default function PostsList() {
  const { user: currentUser } = useContext(AuthContext);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [profile, setProfile] = useState(null);
  const [newPostContent, setNewPostContent] = useState('');
  const [newPostImage, setNewPostImage] = useState(null);
  const [previewImage, setPreviewImage] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [postsData, profileRes] = await Promise.all([
          getPosts(),
          getProfile(),
        ]);

        if (Array.isArray(postsData)) {
          setPosts(postsData);
        } else if (postsData && Array.isArray(postsData.data)) {
          setPosts(postsData.data);
        } else {
          setPosts([]);
        }

        setProfile(profileRes.data);
      } catch (error) {
        console.error('Error fetching posts:', error);
        setPosts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

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
    if (!newPostContent.trim() && !newPostImage) {return;}
    try {
      const formData = new FormData();
      formData.append('content', newPostContent);
      if (newPostImage) {formData.append('image', newPostImage);}

      const res = await createPostWithImage(formData);
      const newPost = {
        ...res.data,
        authorName: profile?.fullName || currentUser?.fullName,
        authorAvatar: profile?.photoUrl || '../Icons/default-avatar-icon.png',
        hasCurrentUserLiked: false,
        likeCount: 0,
      };

      setPosts((prev) => [newPost, ...prev]);
      setNewPostContent('');
      setNewPostImage(null);
      setPreviewImage(null);
    } catch (error) {
      console.error('Помилка створення поста:', error);
    }
  };

  if (loading) {return <Loading />;}

  return (
    <div className="posts-feed">
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

      <Post
        posts={posts}
        onLikeToggle={handleLikeToggle}
        currentUser={currentUser}
      />
    </div>
  );
}
