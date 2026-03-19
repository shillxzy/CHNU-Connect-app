import React, { useEffect, useState} from 'react';
import { NavLink, Link, useNavigate } from "react-router-dom";
import { CHNUConnectIcon, UserIcon, SearchIcon } from '../Icons';
import { getProfile } from "../../api/userAPI";
import './Header.css';
import { getUnreadNotifications } from "../../api/notificationAPI";


const Header = () => {
    const navigate = useNavigate();
    const [isProfileOpen, setIsProfileOpen] = useState(false);
    const [user, setUser] = useState(null);   
    const [loading, setLoading] = useState(true);
    const [unreadCount, setUnreadCount] = useState(0);

    const handleProfile = () => {
    navigate(`/profile/${encodeURIComponent(user.fullName)}`);
  };

  useEffect(() => {
      const fetchData = async () => {
        try {
          const profileResponse = await getProfile();
          const userData = profileResponse.data;
          setUser(userData);

        } catch (err) {
          console.error("Error fetching profile:", err);
         
        } finally {
          setLoading(false);
        }
      };
  
      fetchData();
    }, []);

    useEffect(() => {
  if (!user?.id) return;

  const fetchUnread = async () => {
    try {
      const res = await getUnreadNotifications(user.id);
      setUnreadCount(res.data.length);
    } catch (err) {
      console.error("Error fetching unread notifications:", err);
    }
  };

  fetchUnread();
}, [user]);


    const handleLogout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        navigate("/login");
    };

    const navIsActive = (ctx) => {
      if (ctx.isActive) {
        return 'nav-link active';
      } else {
        return 'nav-link';
      }
    }

    if (loading) return <p>Завантаження хедеру...</p>;

    return (
        <header className="header">
            <div className="header-left">
                <img src={CHNUConnectIcon} alt="CHNU Connect" className="logo-icon" />
                <span className="logo">CHNU Connect</span>
            </div>

            <div className="header-right">
                <nav className="header-nav">
                    <NavLink to="/" className={(ctx) => navIsActive(ctx)}>Головна</NavLink>
                    <NavLink to="/events" className={(ctx) => navIsActive(ctx)}>Події</NavLink>
                    <NavLink to="/groups" className={(ctx) => navIsActive(ctx)}>Групи</NavLink>
                    <NavLink to="/about" className={(ctx) => navIsActive(ctx)}>Про Нас</NavLink>
                </nav>

                {/* --- Profile Wrapper --- */}
                <div className="profile-wrapper" onClick={() => setIsProfileOpen(prev => !prev)}>
                  <img src={UserIcon} alt="User" className="user-icon" />
                  {unreadCount > 0 && <span className="notification-badge-header">{unreadCount}</span>}

                  <div className={`profile-dropdown ${isProfileOpen ? 'active' : ''}`}>
                      <button onClick={handleProfile}>Профіль</button>
                      <Link to="/settings">Налаштування</Link>
                      <button onClick={handleLogout}>Вихід</button>
                  </div>
                </div>


                <div className="search-container">
                    <img src={SearchIcon} alt="Search" className="search-icon" />
                    <input type="text" placeholder="Пошук..." className="search-input" />
                </div>
            </div>
        </header>
    );
};

export default Header;
