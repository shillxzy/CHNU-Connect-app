import React, {
  useEffect,
  useState,
  useContext,
  useRef,
  useCallback,
} from 'react';
import { NavLink, Link, useNavigate } from 'react-router-dom';
import { CHNUConnectIcon, SearchIcon } from '../Icons';
import notificationIcon from '../Icons/notification.png';
import { getProfile, searchUsers } from '../../api/userAPI';
import {
  getUnreadNotifications,
  markAllNotificationsAsRead,
  markNotificationAsRead,
} from '../../api/notificationAPI';
import AuthContext from '../../context/AuthContext';
import Avatar from '../Avatar/Avatar';
import './Header.css';

const Header = () => {
  const navigate = useNavigate();
  const { unreadCount, setUnreadCount, logout, role } = useContext(AuthContext);

  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [isNotifOpen, setIsNotifOpen] = useState(false);
  const [user, setUser] = useState(null);
  const [notifications, setNotifications] = useState([]);

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [showSearchResults, setShowSearchResults] = useState(false);

  const profileRef = useRef(null);
  const notifRef = useRef(null);
  const searchRef = useRef(null);

  // Close dropdowns on outside click
  useEffect(() => {
    const handler = (e) => {
      if (profileRef.current && !profileRef.current.contains(e.target)) {
        setIsProfileOpen(false);
      }
      if (notifRef.current && !notifRef.current.contains(e.target)) {
        setIsNotifOpen(false);
      }
      if (searchRef.current && !searchRef.current.contains(e.target)) {
        setShowSearchResults(false);
      }
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
    const opening = !isNotifOpen;
    setIsNotifOpen(opening);
    setIsProfileOpen(false);
    if (opening && user?.id) {
      try {
        const res = await getUnreadNotifications(user.id);
        setNotifications(res.data || []);
      } catch {
        /* empty */
      }
    }
  };

  const openProfile = () => {
    setIsProfileOpen((prev) => !prev);
    setIsNotifOpen(false);
  };

  const handleMarkAllRead = async (e) => {
    e.stopPropagation();
    try {
      await markAllNotificationsAsRead();
      setUnreadCount(0);
      setNotifications([]);
    } catch {
      /* empty */
    }
  };

  const getNotifText = (type) => {
    if (type === 'message') {
      return 'написав вам повідомлення';
    }
    if (type === 'like') {
      return 'вподобав ваш пост';
    }
    if (type === 'comment') {
      return 'прокоментував ваш пост';
    }
    if (type === 'event') {
      return 'запросив на подію';
    }
    if (type === 'group') {
      return 'додав вас до групи';
    }
    return 'надіслав сповіщення';
  };

  const handleNotifClick = (n) => {
    setIsNotifOpen(false);
    markNotificationAsRead(n.id).catch(() => {});
    setNotifications((prev) => prev.filter((x) => x.id !== n.id));
    setUnreadCount((prev) => Math.max(0, prev - 1));
    if (n.type === 'message') {
      navigate(`/chats/${n.entityId}`);
    } else if (n.type === 'event') {
      navigate(`/events/${n.entityId}`);
    } else if (n.type === 'group') {
      navigate(`/groups/${n.entityId}`);
    } else if (n.entityId) {
      navigate(`/posts/${n.entityId}`);
    }
  };

  const handleDismiss = async (e, id) => {
    e.stopPropagation();
    await markNotificationAsRead(id).catch(() => {});
    setNotifications((prev) => prev.filter((n) => n.id !== id));
    setUnreadCount((prev) => Math.max(0, prev - 1));
  };

  const handleLogout = () => logout();

  // Debounced search
  const searchTimeout = useRef(null);
  const handleSearchChange = useCallback((e) => {
    const q = e.target.value;
    setSearchQuery(q);
    clearTimeout(searchTimeout.current);
    if (!q.trim()) {
      setSearchResults([]);
      setShowSearchResults(false);
      return;
    }
    searchTimeout.current = setTimeout(async () => {
      try {
        const res = await searchUsers(q);
        setSearchResults(res.data || []);
        setShowSearchResults(true);
      } catch {
        /* empty */
      }
    }, 300);
  }, []);

  const navIsActive = (ctx) => (ctx.isActive ? 'nav-link active' : 'nav-link');

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
          <NavLink to="/" className={navIsActive} end>
            Головна
          </NavLink>
          <NavLink to="/events" className={navIsActive}>
            Події
          </NavLink>
          <NavLink to="/groups" className={navIsActive}>
            Групи
          </NavLink>
          <NavLink to="/schedule" className={navIsActive}>
            Розклад
          </NavLink>
          <NavLink to="/about" className={navIsActive}>
            Про Нас
          </NavLink>
        </nav>

        {/* ===== SEARCH ===== */}
        <div className="search-wrapper" ref={searchRef}>
          <img
            src={SearchIcon}
            alt="Search"
            className="search-icon"
            onClick={() => {
              setShowSearchResults(false);
              navigate(
                `/search${searchQuery.trim() ? `?q=${encodeURIComponent(searchQuery.trim())}` : ''}`,
              );
            }}
          />
          {/* #9 FIX: плейсхолдер "Пошук користувачів..." */}
          <input
            type="text"
            placeholder="Пошук користувачів..."
            className="search-input"
            value={searchQuery}
            onChange={handleSearchChange}
            onFocus={() =>
              searchResults.length > 0 && setShowSearchResults(true)
            }
            onKeyDown={(e) => {
              if (e.key === 'Enter' && searchQuery.trim()) {
                setShowSearchResults(false);
                navigate(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
              }
            }}
          />
          {showSearchResults && searchResults.length > 0 && (
            <div className="search-results">
              {searchResults.map((u) => (
                <div
                  key={u.id}
                  className="search-result-item"
                  onMouseDown={() => {
                    setShowSearchResults(false);
                    setSearchQuery('');
                    // #6 FIX: навігація через id, а не fullName — щоб не відкривався свій профіль
                    navigate(`/profile/view/${u.id}`);
                  }}
                >
                  <Avatar photoUrl={u.photoUrl} size={28} />
                  <span className="search-result-name">
                    {u.fullName || u.email}
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* ===== NOTIFICATIONS BELL ===== */}
        <div className="notif-wrapper" ref={notifRef}>
          <button className="notif-btn" onClick={openNotifications}>
            <img
              src={notificationIcon}
              alt="Notifications"
              className="notif-icon"
            />
            {unreadCount > 0 && (
              <span className="notif-badge">
                {unreadCount > 99 ? '99+' : unreadCount}
              </span>
            )}
          </button>

          {isNotifOpen && (
            <div className="notif-dropdown">
              <div className="dropdown-notif-header">
                <span>Сповіщення</span>
                {notifications.length > 0 && (
                  <button
                    className="notif-mark-all"
                    onClick={handleMarkAllRead}
                  >
                    Прочитати всі
                  </button>
                )}
              </div>

              <div className="dropdown-notif-list">
                {notifications.length === 0 ? (
                  <div className="notif-empty">Немає нових сповіщень</div>
                ) : (
                  notifications.slice(0, 6).map((n) => (
                    <div
                      key={n.id}
                      className="notif-item"
                      onClick={() => handleNotifClick(n)}
                    >
                      <Avatar photoUrl={n.actorAvatar} size={30} />
                      <div className="notif-body">
                        <span className="notif-actor">
                          {n.actorName || 'Хтось'}
                        </span>{' '}
                        <span className="notif-action">
                          {getNotifText(n.type)}
                        </span>
                        {n.body && (
                          <div className="notif-preview">{n.body}</div>
                        )}
                      </div>
                      <div className="notif-right">
                        <span className="notif-time">
                          {new Date(n.createdAt).toLocaleTimeString([], {
                            hour: '2-digit',
                            minute: '2-digit',
                          })}
                        </span>
                        <button
                          className="notif-dismiss"
                          title="Закрити"
                          onClick={(e) => handleDismiss(e, n.id)}
                        >
                          ✕
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>
              <div className="notif-footer">
                <button
                  className="notif-all-btn"
                  onClick={() => {
                    setIsNotifOpen(false);
                    navigate('/notifications');
                  }}
                >
                  Усі сповіщення →
                </button>
              </div>
            </div>
          )}
        </div>

        {/* ===== PROFILE ===== */}
        <div className="profile-wrapper" ref={profileRef} onClick={openProfile}>
          <div className="profile-icon-wrap">
            <Avatar photoUrl={user?.photoUrl} size={32} />
          </div>

          <div className={`profile-dropdown ${isProfileOpen ? 'active' : ''}`}>
            <button
              onClick={(e) => {
                e.stopPropagation();
                navigate(`/profile/${encodeURIComponent(user?.fullName)}`);
                setIsProfileOpen(false);
              }}
            >
              Профіль
            </button>
            <button
              onClick={(e) => {
                e.stopPropagation();
                navigate('/chats');
                setIsProfileOpen(false);
              }}
            >
              Повідомлення
            </button>
            <Link to="/settings" onClick={(e) => e.stopPropagation()}>
              Налаштування
            </Link>
            {(role === 'admin' || role === 'superAdmin') && (
              <button
                onClick={(e) => {
                  e.stopPropagation();
                  navigate('/admin-panel');
                  setIsProfileOpen(false);
                }}
              >
                Адмін панель
              </button>
            )}
            <div className="dropdown-divider" />
            <button
              onClick={(e) => {
                e.stopPropagation();
                handleLogout();
              }}
            >
              Вихід
            </button>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
