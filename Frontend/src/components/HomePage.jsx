import React from 'react';
import { Routes, Route, useLocation, Outlet } from "react-router-dom";
import Header from './Common/Header.jsx';
import GroupsList from './Groups/GroupsList.jsx';
import EventsList from './Events/EventsList.jsx';
import HeroBanner from './Common/HeroBanner.jsx';
import NewsFeed from './NewsFeed/NewsFeed.jsx';
import Sidebar from './SideBar/SideBarHomePage.jsx';
import Footer from './Common/Footer.jsx';
import './HomePage.css';
import Profile from './Profile/Profile.jsx';
import AboutUs from './AboutUs/AboutUs.jsx';
import ProfileEdit from './Profile/ProfileEdit.jsx';

function HomePage({ onLogout }) {
  const location = useLocation();
  const isHome = location.pathname === "/";

  return (
    <div className="app-container">
      <Header onLogout={onLogout} />
      {isHome && <HeroBanner />}

      <div className={`main-content-area ${isHome ? "with-sidebar" : "full-width"}`}>
        <Outlet /> 
        {isHome && <Sidebar />}
      </div>

      <Footer />
    </div>
  );
}

HomePage.NewsFeed = NewsFeed;
HomePage.GroupsList = GroupsList;
HomePage.EventsList = EventsList;
HomePage.Profile = Profile;
HomePage.AboutUs = AboutUs;
HomePage.ProfileEdit = ProfileEdit;

export default HomePage;