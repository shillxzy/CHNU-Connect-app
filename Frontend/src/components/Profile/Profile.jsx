import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Profile.css';
import '../NewsFeed/NewsFeed.css';
import { getProfile } from '../../api/userAPI';
import { getPostsByUser } from '../../api/postAPI';
import { getCuratedGroups } from '../../api/groupAPI';
import Avatar from '../Avatar/Avatar.jsx';
import Post from '../Posts/Post.jsx';
import { getUnreadNotifications } from '../../api/notificationAPI';
import Loading from '../Loading/Loading.jsx';

const Profile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [unreadCount, setUnreadCount] = useState(0);
  const [curatedGroups, setCuratedGroups] = useState([]);

  const handleEditProfile = () => {
    navigate('/settings');
  };

  const handleChatsProfile = () => {
    navigate(`/chats/${encodeURIComponent(user.fullName)}`);
  };

  useEffect(() => {
    if (!user?.id) {
      return;
    }

    const fetchUnread = async () => {
      try {
        const res = await getUnreadNotifications(user.id);
        setUnreadCount(res.data.length);
      } catch (err) {
        console.error('Error fetching unread notifications:', err);
      }
    };

    fetchUnread();
  }, [user]);

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('role');
    window.location.href = '/login';
  };

  const handleLikeToggle = (postId) => {
    setPosts((prevPosts) =>
      prevPosts.map((post) =>
        post.id === postId
          ? {
              ...post,
              liked: !post.liked,
              likeCount: post.liked ? post.likeCount - 1 : post.likeCount + 1,
            }
          : post,
      ),
    );
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const profileResponse = await getProfile();
        const userData = profileResponse.data;
        setUser(userData);

        const postsResponse = await getPostsByUser(userData.id);
        const userPosts = Array.isArray(postsResponse.data)
          ? postsResponse.data
          : [];
        setPosts(userPosts);

        if (userData.role?.toLowerCase() === 'teacher') {
          try {
            const curatedRes = await getCuratedGroups();
            setCuratedGroups(
              Array.isArray(curatedRes.data) ? curatedRes.data : [],
            );
          } catch {
            setCuratedGroups([]);
          }
        }
      } catch (err) {
        console.error('Error fetching profile:', err);
        setError(
          err.response?.data?.message ||
            err.message ||
            'Помилка при завантаженні профілю',
        );
        setPosts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <Loading />;
  }
  if (error) {
    return <p>Помилка: {error}</p>;
  }
  if (!user) {
    return <p>Користувач не знайдений</p>;
  }

  return (
    <div className="profile-page-container">
      <div className="profile-header">
        <div className="profile-photo-area">
          <Avatar photoUrl={user.photoUrl} size={150} />
        </div>

        <div className="profile-info-actions">
          <div className="profile-details">
            <div
              style={{
                display: 'flex',
                alignItems: 'center',
                gap: '10px',
                flexWrap: 'wrap',
              }}
            >
              <h2 className="profile-name">{user.fullName}</h2>
              {user.role?.toLowerCase() === 'teacher' && (
                <span
                  style={{
                    background: '#e0f2fe',
                    color: '#0369a1',
                    fontSize: '12px',
                    fontWeight: 600,
                    padding: '2px 10px',
                    borderRadius: '12px',
                    border: '1px solid #bae6fd',
                  }}
                >
                  Викладач
                </span>
              )}
            </div>
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
            <button
              className="profile-btn btn-edit"
              onClick={handleEditProfile}
            >
              Редагувати профіль
            </button>
            <button
              className="profile-btn btn-messages"
              onClick={handleChatsProfile}
            >
              Повідомлення
              {unreadCount > 0 && (
                <span className="notification-badge">{unreadCount}</span>
              )}
            </button>

            <button className="profile-btn btn-logout" onClick={handleLogout}>
              Вихід
            </button>
          </div>
        </div>
      </div>

      {user.role?.toLowerCase() === 'teacher' && (
        <div className="recent-posts-section">
          <h3 className="section-title">Куровані групи</h3>
          {curatedGroups.length > 0 ? (
            <div
              style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}
            >
              {curatedGroups.map((g) => (
                <div
                  key={g.id}
                  style={{
                    background: '#f0f9ff',
                    border: '1px solid #bae6fd',
                    borderRadius: '8px',
                    padding: '10px 16px',
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                  }}
                >
                  <div>
                    <strong>{g.name}</strong>
                    {g.description && (
                      <p
                        style={{
                          margin: '2px 0 0',
                          fontSize: '13px',
                          color: '#64748b',
                        }}
                      >
                        {g.description}
                      </p>
                    )}
                  </div>
                  <button
                    onClick={() => navigate(`/group/edit/schedule/${g.id}`)}
                    style={{
                      padding: '5px 14px',
                      background: '#6610f2',
                      color: 'white',
                      border: 'none',
                      borderRadius: '6px',
                      cursor: 'pointer',
                      fontSize: '13px',
                      fontWeight: 600,
                    }}
                  >
                    Розклад
                  </button>
                </div>
              ))}
            </div>
          ) : (
            <p>Немає курованих груп</p>
          )}
        </div>
      )}

      <div className="recent-posts-section">
        <h3 className="section-title">Останні пости</h3>
        {Array.isArray(posts) && posts.length > 0 ? (
          <Post posts={posts} onLikeToggle={handleLikeToggle} />
        ) : (
          <p>Немає постів</p>
        )}
      </div>
    </div>
  );
};

export default Profile;
