import React, { useContext } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthProvider';
import AuthContext from './context/AuthContext';
import { ThemeProvider } from './context/ThemeProvider';

import Login from './components/Auth/Login';
import HomePage from './components/HomePage/HomePage';
import HomePageNewsFeed from './components/NewsFeed/NewsFeed';
import GroupsPage from './components/Groups/GroupsPage';
import EventsList from './components/Events/EventsList';
import Profile from './components/Profile/Profile';
import AboutUs from './components/AboutUs/AboutUs';
import RefreshPassword from './components/Auth/RefreshPassword';
import Registration from './components/Auth/Registration';
import CreateEvent from './components/Events/CreateEvent';
import EventDetails from './components/Events/EventDetails';
import PostsList from './components/Posts/PostsList';
import ProfileView from './components/Profile/ProfileView';
import Chats from './components/Chats/Chats';
import AdminPanel from './components/AdminPanel/AdminPanel';
import GroupCreate from './components/Groups/GroupCreate';
import GroupEdit from './components/Groups/GroupEdit';
import ScheduleEditPage from './components/Schedule/ScheduleEditPage';
import ScheduleViewPage from './components/Schedule/ScheduleViewPage';
import MySchedulePage from './components/Schedule/MySchedulePage';
import SearchPage from './components/Search/SearchPage';
import SettingsPage from './components/Settings/SettingsPage';
import NotificationsPage from './components/Notifications/NotificationsPage';

function PublicRoute({ children }) {
  const { accessToken } = useContext(AuthContext);
  return accessToken ? <Navigate to="/" replace /> : children;
}

function ProtectedRoute({ children }) {
  const { accessToken } = useContext(AuthContext);
  return accessToken ? children : <Navigate to="/login" replace />;
}

function AdminRoute({ children }) {
  const { accessToken, role } = useContext(AuthContext);
  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }
  if (!role) {
    return null;
  }
  if (role !== 'admin' && role !== 'superAdmin') {
    return <Navigate to="/" replace />;
  }
  return children;
}

// Дозволяє редагувати розклад: admin+ManageSchedule або teacher (curator-check на бекенді)
function ScheduleEditorRoute({ children }) {
  const { accessToken, role, hasPermission } = useContext(AuthContext);
  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }
  if (!role) {
    return null;
  }
  if (role === 'superAdmin') {
    return children;
  }
  if (role === 'admin' && hasPermission('ManageSchedule')) {
    return children;
  }
  if (role === 'teacher') {
    return children;
  }
  return <Navigate to="/" replace />;
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
        <Route path="groups" element={<GroupsPage />} />
        <Route path="groups/:id" element={<GroupsPage />} />
        <Route path="events" element={<EventsList />} />
        <Route path="posts" element={<PostsList />} />
        <Route path="profile/:fullname" element={<Profile />} />
        <Route path="profile/view/:id" element={<ProfileView />} />
        <Route
          path="profile/edit/:fullname"
          element={<Navigate to="/settings" replace />}
        />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="about" element={<AboutUs />} />
        <Route path="events/create" element={<CreateEvent />} />
        <Route path="events/:id" element={<EventDetails />} />
        <Route path="chats" element={<Chats />} />
        <Route path="search" element={<SearchPage />} />
        <Route path="chats/:chatId" element={<Chats />} />
        <Route path="notifications" element={<NotificationsPage />} />

        {/* Перегляд розкладу конкретної групи */}
        <Route path="group/schedule/:id" element={<ScheduleViewPage />} />

        {/* Мій розклад — всі групи поточного користувача */}
        <Route path="schedule" element={<MySchedulePage />} />

        <Route
          path="admin-panel"
          element={
            <AdminRoute>
              <AdminPanel />
            </AdminRoute>
          }
        />
        <Route
          path="groups/create"
          element={
            <AdminRoute>
              <GroupCreate />
            </AdminRoute>
          }
        />
        <Route
          path="groups/edit/:id"
          element={
            <AdminRoute>
              <GroupEdit />
            </AdminRoute>
          }
        />

        {/* Редагування розкладу — admin+ManageSchedule або teacher (curator check на бекенді) */}
        <Route
          path="group/edit/schedule/:id"
          element={
            <ScheduleEditorRoute>
              <ScheduleEditPage />
            </ScheduleEditorRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
