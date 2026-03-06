import React, { useContext } from "react";
import { Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";
import AuthContext from "./context/AuthContext";

import Login from "./components/Auth/Login";
import HomePage from "./components/HomePage";
import HomePageNewsFeed from "./components/NewsFeed/NewsFeed";
import GroupsList from "./components/Groups/GroupsList";
import EventsList from "./components/Events/EventsList";
import Profile from "./components/Profile/Profile";
import ProfileEdit from "./components/Profile/ProfileEdit";
import AboutUs from "./components/AboutUs/AboutUs";

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
        <Route path="profile" element={<Profile />} />
        <Route path="profile/:fullname" element={<ProfileEdit />} />
        <Route path="about" element={<AboutUs />} />
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
