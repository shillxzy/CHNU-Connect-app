import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Profile.css";
import { getProfile } from "../../api/userAPI";
import { getPostsByUser } from "../../api/postAPI";
import Avatar from "../Avatar/Avatar.jsx";

const Profile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

const handleEditProfile = () => {
  navigate(`/profile/edit/${encodeURIComponent(user.fullName)}`);
};

  useEffect(() => {
    const fetchData = async () => {
      try {
        const profileResponse = await getProfile();
        const userData = profileResponse.data;
        setUser(userData);

        console.log("user.photoUrl:", userData.photoUrl);

        const postsResponse = await getPostsByUser(userData.id);
        const userPosts = Array.isArray(postsResponse.data) ? postsResponse.data : [];
        setPosts(userPosts);

      } catch (err) {
        console.error("Error fetching profile:", err);
        setError(err.response?.data?.message || err.message || "Помилка при завантаженні профілю");
        setPosts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("role");
    window.location.href = "/login";
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

          <div className="profile-actions">
            <button className="btn btn-edit" onClick={handleEditProfile}>
    Редагувати профіль
  </button>
            <button className="btn btn-messages">
              Повідомлення <span className="notification-badge">{user.unreadMessages || 0}</span>
            </button>
            <button className="btn btn-logout" onClick={handleLogout}>
              Вихід
            </button>
          </div>
        </div>
      </div>

      <div className="recent-posts-section">
        <h3 className="section-title">Останні пости</h3>
        {Array.isArray(posts) && posts.length > 0 ? (
          posts.map(post => (
            <div key={post.id} className="post-placeholder">
              {post.content}
            </div>
          ))
        ) : (
          <p>Немає постів</p>
        )}
      </div>
    </div>
  );
};

export default Profile;
