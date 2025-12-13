import React, { useEffect, useState } from 'react';
import './Profile.css';
import { getUserProfile } from '../../api/getUserProfile';
import { getPostUserById } from '../../api/getPostUserById';

const Profile = () => {
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const token = localStorage.getItem("token");
        if (!token) throw new Error("User is not authenticated");

        const userData = await getUserProfile(token);
        setUser(userData);

        const userPosts = await getPostUserById(userData.id, token);
        setPosts(userPosts || []);
      } catch (err) {
        console.error("Error fetching user:", err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchUser();
  }, []);

  if (loading) return <p>Завантаження профілю...</p>;
  if (error) return <p>Помилка: {error}</p>;
  if (!user) return <p>Користувач не знайдений</p>;

  return (
    <div className="profile-page-container">
      <div className="profile-header">
        <div className="profile-photo-area">
          {user.photoUrl 
            ? <img src={user.photoUrl} alt="Фото профілю" className="profile-photo" /> 
            : <span className="profile-photo-placeholder">Фото профілю</span>}
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
            <button className="btn btn-edit">Редагувати профіль</button>
            <button className="btn btn-messages">
              Повідомлення <span className="notification-badge">{user.unreadMessages || 0}</span>
            </button>
            <button className="btn btn-logout" onClick={() => {
              localStorage.removeItem("token");
              window.location.href = "/login";
            }}>Вихід</button>
          </div>
        </div>
      </div>

      <div className="recent-posts-section">
        <h3 className="section-title">Останні пости</h3>
        {posts.length > 0 ? (
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
