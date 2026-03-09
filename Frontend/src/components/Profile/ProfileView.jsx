import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getUserById } from "../../api/userAPI";
import { getPostsByUser } from "../../api/postAPI";
import Avatar from "../Avatar/Avatar.jsx";
import Post from "../Posts/Post.jsx";
import "./Profile.css";

const ProfileView = ({ currentUser }) => {
  const { id } = useParams(); // id користувача з URL
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUserAndPosts = async () => {
      try {
        const resUser = await getUserById(id);
        const userData = resUser.data;
        setUser(userData);

        const resPosts = await getPostsByUser(userData.id);
        const userPosts = Array.isArray(resPosts.data) ? resPosts.data : [];

        const postsWithAuthor = userPosts.map(post => ({
          ...post,
          authorId: userData.id,
          authorName: userData.fullName,
          authorAvatar: userData.photoUrl || "../Icons/default-avatar-profile-icon.png",
          hasCurrentUserLiked: post.hasCurrentUserLiked ?? false,
        }));

        setPosts(postsWithAuthor);
      } catch (err) {
        console.error("Error fetching user or posts:", err);
        setError(err.response?.data?.message || "Помилка при завантаженні профілю");
      } finally {
        setLoading(false);
      }
    };

    fetchUserAndPosts();
  }, [id]);

  const handleLikeToggle = async (postId, currentlyLiked) => {
    setPosts(prevPosts =>
      prevPosts.map(post =>
        post.id === postId
          ? {
              ...post,
              hasCurrentUserLiked: !currentlyLiked,
              likeCount: currentlyLiked ? post.likeCount - 1 : post.likeCount + 1,
            }
          : post
      )
    );
  };

  if (loading) return <p>Завантаження профілю...</p>;
  if (error) return <p>Помилка: {error}</p>;
  if (!user) return <p>Користувач не знайдений</p>;

  return (
    <div className="profile-page-container">
      <div className="profile-header">
        <div className="profile-photo-area">
          <Avatar photoUrl={user.photoUrl} size={150} />
        </div>

        <div className="profile-info-actions">
          <div className="profile-details">
            <h2 className="profile-name">{user.fullName}</h2>
            <p className="profile-detail-line">
              <span className="profile-label">Факультет:</span>
              <span className="profile-value">{user.faculty}</span>
            </p>
            <p className="profile-detail-line">
              <span className="profile-label">Курс:</span>
              <span className="profile-value">{user.course}</span>
            </p>
            <p className="profile-detail-line">
              <span className="profile-label">Біо:</span>
              <span className="profile-value">{user.bio}</span>
            </p>
          </div>
        </div>
      </div>

      <div className="recent-posts-section">
        <h3 className="section-title">Пости користувача</h3>
        {posts.length > 0 ? (
          <Post posts={posts} onLikeToggle={handleLikeToggle} currentUser={currentUser} />
        ) : (
          <p>У цього користувача ще немає постів</p>
        )}
      </div>
    </div>
  );
};

export default ProfileView;
