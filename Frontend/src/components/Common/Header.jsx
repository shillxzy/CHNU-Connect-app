import React, { useEffect, useState} from 'react';
import { Link, useLocation, useNavigate } from "react-router-dom";
import { CHNUConnectIcon, UserIcon, SearchIcon } from '../Icons';
import { getProfile } from "../../api/userAPI";
import './Header.css';

const Header = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [isProfileOpen, setIsProfileOpen] = useState(false);
    const [user, setUser] = useState(null);   
    const [loading, setLoading] = useState(true);

    const isActive = (path) => location.pathname === path;

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

    const handleLogout = () => {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        navigate("/login");
    };

    if (loading) return <p>Завантаження хедеру...</p>;

    return (
        <header className="header">
            <div className="header-left">
                <img src={CHNUConnectIcon} alt="CHNU Connect" className="logo-icon" />
                <span className="logo">CHNU Connect</span>
            </div>

            <div className="header-right">
                <nav className="header-nav">
                    <Link to="/" className={`nav-link ${isActive('/') ? 'active' : ''}`}>Головна</Link>
                    <Link to="/events" className={`nav-link ${isActive('/events') ? 'active' : ''}`}>Події</Link>
                    <Link to="/groups" className={`nav-link ${isActive('/groups') ? 'active' : ''}`}>Групи</Link>
                    <Link to="/about" className={`nav-link ${isActive('/about') ? 'active' : ''}`}>Про Нас</Link>
                </nav>

                {/* --- Profile Wrapper --- */}
                <div 
                    className="profile-wrapper" 
                    onClick={() => setIsProfileOpen(prev => !prev)}
                >
                    <img src={UserIcon} alt="User" className="user-icon" />

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
