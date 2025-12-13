import React from 'react';
import { Link, useLocation } from "react-router-dom";
import { CHNUConnectIcon, UserIcon, SearchIcon } from '../Icons';

const Header = () => {
    const location = useLocation();
    
    const isActive = (path) => location.pathname === path;
    
    return (
        <header className="header">
            <div className="header-left">
                <img src={CHNUConnectIcon} alt="CHNU Connect" className="logo-icon" />
                <span className="logo">CHNU Connect</span>
            </div>

            <div className="header-right">
                <nav className="header-nav">
                    <Link 
                        to="/" 
                        className={`nav-link ${isActive('/') ? 'active' : ''}`}
                    >
                        Головна
                    </Link>
                    <Link 
                        to="/events" 
                        className={`nav-link ${isActive('/events') ? 'active' : ''}`}
                    >
                        Події
                    </Link>
                    <Link 
                        to="/groups" 
                        className={`nav-link ${isActive('/groups') ? 'active' : ''}`}
                    >
                        Групи
                    </Link>
                    <Link 
                        to="/about" 
                        className={`nav-link ${isActive('/about') ? 'active' : ''}`}
                    >
                        Про Нас
                    </Link>
                </nav>

                <Link to="/profile">
                    <img src={UserIcon} alt="User" className="user-icon" />
                </Link>

                <div className="search-container">
                    <img src={SearchIcon} alt="Search" className="search-icon" />
                    <input 
                        type="text" 
                        placeholder="Пошук..." 
                        className="search-input" 
                    />
                </div>
            </div>
        </header>
    );
};

export default Header;