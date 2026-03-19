import React, { useContext } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";
import AuthContext from "./context/AuthContext";

import Login from "./components/Auth/Login";
import HomePage from "./components/HomePage/HomePage";
import HomePageNewsFeed from "./components/NewsFeed/NewsFeed";
import GroupsList from "./components/Groups/GroupsList";
import EventsList from "./components/Events/EventsList";
import Profile from "./components/Profile/Profile";
import ProfileEdit from "./components/Profile/ProfileEdit";
import AboutUs from "./components/AboutUs/AboutUs";
import RefreshPassword from "./components/Auth/RefreshPassword";
import Registration from "./components/Auth/Registration";
import CreateEvent from "./components/Events/CreateEvent";
import EventDetails from "./components/Events/EventDetails";
import PostsList from "./components/Posts/PostsList";
import ProfileView from "./components/Profile/ProfileView";
import Chats from "./components/Chats/Chats";
import AdminPanel from "./components/AdminPanel/AdminPanel";


// Публічний маршрут
function PublicRoute({ children }) {
  const { accessToken } = useContext(AuthContext);
  return accessToken ? <Navigate to="/" replace /> : children;
}

// Захищений маршрут
function ProtectedRoute({ children }) {
  const { accessToken } = useContext(AuthContext);
  return accessToken ? children : <Navigate to="/login" replace />;
}

function AdminRoute({ children }) {
  const { accessToken, user } = useContext(AuthContext);

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  if (!user) {
    return null; // або loader
  }

  if (user.role !== "admin") {
    return <Navigate to="/" replace />;
  }

  return children;
}



function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          <PublicRoute>
            <Login />
          </PublicRoute>
        }
      />

      <Route
        path="/registration"
        element={
          <PublicRoute>
            <Registration />
          </PublicRoute>
        }
      />

      <Route
        path="/refresh-password"
        element={
          <PublicRoute>
            <RefreshPassword />
          </PublicRoute>
        }
      />

      <Route
        path="/"
        element={
          <ProtectedRoute>
            <HomePage />
          </ProtectedRoute>
        }
      >
        <Route index element={<HomePageNewsFeed />} />
        <Route path="groups" element={<GroupsList />} />
        <Route path="events" element={<EventsList />} />
        <Route path="posts" element={<PostsList />} />
        <Route path="profile/:fullname" element={<Profile />} />
        <Route path="profile/view/:id" element={<ProfileView />} />
        <Route path="profile/edit/:fullname" element={<ProfileEdit />} />
        <Route path="about" element={<AboutUs />} />
        <Route path="events/create" element={<CreateEvent />} />
        <Route path="events/:id" element={<EventDetails />} />
        <Route path="chats/:chatId" element={<Chats />} />
        
        <Route path="admin-panel" element={
            <AdminRoute>
              <AdminPanel />
            </AdminRoute>
          }
        />
      
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />

    </Routes>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppRoutes /> 
    </AuthProvider>
  );
}

export default App;
