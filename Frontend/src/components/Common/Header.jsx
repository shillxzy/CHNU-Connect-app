import React, { useEffect, useState, useContext, useRef } from 'react';
import { NavLink, Link, useNavigate } from 'react-router-dom';
import { CHNUConnectIcon, UserIcon, SearchIcon } from '../Icons';
import { getProfile } from '../../api/userAPI';
import { getUnreadNotifications, markAllNotificationsAsRead } from '../../api/notificationAPI';
import AuthContext from '../../context/AuthContext';
import './Header.css';
import notificationIcon from '../Icons/notification.png';

const Header = () => {
  const navigate = useNavigate();
  const { unreadCount, setUnreadCount, logout, user: ctxUser } = useContext(AuthContext);

  const [isProfileOpen,      setIsProfileOpen]      = useState(false);
  const [isNotifOpen,        setIsNotifOpen]         = useState(false);
  const [user,               setUser]               = useState(null);
  const [notifications,      setNotifications]      = useState([]);

  const notifRef  = useRef(null);
  const profileRef = useRef(null);

  // Закрити дропдауни при кліку поза ними
  useEffect(() => {
    const handler = (e) => {
      if (notifRef.current  && !notifRef.current.contains(e.target))  {setIsNotifOpen(false);}
      if (profileRef.current && !profileRef.current.contains(e.target)) {setIsProfileOpen(false);}
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, []);

  useEffect(() => {
    getProfile()
      .then((res) => setUser(res.data))
      .catch(() => {});
  }, []);

  const openNotifications = async () => {
    setIsNotifOpen((prev) => !prev);
    if (!isNotifOpen && user?.id) {
      try {
        const res = await getUnreadNotifications(user.id);
        setNotifications(res.data || []);
      } catch { /* empty */ }
    }
  };

  const handleMarkAllRead = async () => {
    try {
      await markAllNotificationsAsRead();
      setUnreadCount(0);
      setNotifications([]);
      setIsNotifOpen(false);
    } catch { /* empty */ }
  };

  const getNotifLabel = (type) => {
    if (type === 'message')    {return '💬 Нове повідомлення';}
    if (type === 'like')       {return '❤️ Хтось вpodобав ваш пост';}
    if (type === 'comment')    {return '💬 Новий коментар';}
    if (type === 'event')      {return '📅 Нова подія';}
    return '🔔 Сповіщення';
  };

  // Вихід з облікового запису
  const handleLogout = () => {
    logout();
  };

  const navIsActive = (ctx) => ctx.isActive ? 'nav-link active' : 'nav-link';

  return (
    <header className="header">
      <div className="header-left">
        <Link to="/" className="logo-link">
          <img src={CHNUConnectIcon} alt="CHNU Connect" className="logo-icon" />
          <span className="logo-text">CHNU Connect</span>
        </Link>
      </div>

      <div className="header-right">
        <nav className="header-nav">
          <NavLink to="/"       className={navIsActive} end>Головна</NavLink>
          <NavLink to="/events" className={navIsActive}>Події</NavLink>
          <NavLink to="/groups" className={navIsActive}>Групи</NavLink>
          <NavLink to="/about"  className={navIsActive}>Про Нас</NavLink>
        </nav>

        {/* ===== NOTIFICATIONS BELL ===== */}
        <div className="notif-wrapper" ref={notifRef}>
          <button className="notif-btn" onClick={openNotifications}>
            <img src={notificationIcon} alt="Notifications" className="notif-icon" />
            {unreadCount > 0 && (
              <span className="notif-badge">{unreadCount > 99 ? '99+' : unreadCount}</span>
            )}
          </button>

          {isNotifOpen && (
            <div className="notif-dropdown">
              <div className="notif-header">
                <span>Сповіщення</span>
                {notifications.length > 0 && (
                  <button className="notif-mark-all" onClick={handleMarkAllRead}>
                    Прочитати всі
                  </button>
                )}
              </div>

              <div className="notif-list">
                {notifications.length === 0 ? (
                  <div className="notif-empty">Немає нових сповіщень</div>
                ) : (
                  notifications.slice(0, 10).map((n) => (
                    <div key={n.id} className="notif-item">
                      <span className="notif-text">{getNotifLabel(n.type)}</span>
                      <span className="notif-time">
                        {new Date(n.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                      </span>
                    </div>
                  ))
                )}
              </div>
            </div>
          )}
        </div>

        {/* ===== PROFILE ===== */}
        <div className="profile-wrapper" ref={profileRef} onClick={() => setIsProfileOpen((p) => !p)}>
          <img src={UserIcon} alt="User" className="user-icon" />

          <div className={`profile-dropdown ${isProfileOpen ? 'active' : ''}`}>
            <button onClick={(e) => { e.stopPropagation(); navigate(`/profile/${encodeURIComponent(user?.fullName)}`); }}>
              Профіль
            </button>
            <Link to="/settings" onClick={(e) => e.stopPropagation()}>Налаштування</Link>
            <button onClick={(e) => { e.stopPropagation(); handleLogout(); }}>Вихід</button>
          </div>
        </div>

        {/* ===== SEARCH ===== */}
        <div className="search-container">
          <img src={SearchIcon} alt="Search" className="search-icon" />
          <input type="text" placeholder="Пошук..." className="search-input" />
        </div>
      </div>
    </header>
  );
};

export default Header;
