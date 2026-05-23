import React from 'react';
import { useLocation, Outlet } from 'react-router-dom';
import Header from '../Common/Header.jsx';
import GroupsPage from '../Groups/GroupsPage.jsx';
import EventsList from '../Events/EventsList.jsx';
import PostsList from '../Posts/PostsList.jsx';
import Dashboard from '../Dashboard/Dashboard.jsx';
import NewsFeed from '../NewsFeed/NewsFeed.jsx';
import './HomePage.css';
import Profile from '../Profile/Profile.jsx';
import AboutUs from '../AboutUs/AboutUs.jsx';
import ProfileEdit from '../Profile/ProfileEdit.jsx';
import Chats from '../Chats/Chats.jsx';
import GroupCreate from '../Groups/GroupCreate.jsx';
import GroupEdit from '../Groups/GroupEdit.jsx';

function HomePage({ onLogout }) {
  const location = useLocation();
  const isHome = location.pathname === '/';

  return (
    <div className="app-container">
      <Header onLogout={onLogout} />
      {isHome && <Dashboard />}

      <div className="main-content-area full-width">
        <Outlet />
      </div>
    </div>
  );
}

HomePage.NewsFeed = NewsFeed;
HomePage.GroupsPage = GroupsPage;
HomePage.GroupCreate = GroupCreate;
HomePage.GroupEdit = GroupEdit;
HomePage.EventsList = EventsList;
HomePage.PostsList = PostsList;
HomePage.Profile = Profile;
HomePage.AboutUs = AboutUs;
HomePage.ProfileEdit = ProfileEdit;
HomePage.Chats = Chats;

export default HomePage;
