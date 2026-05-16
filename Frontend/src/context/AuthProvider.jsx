import React, { useState, useEffect, useRef, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import * as signalR from '@microsoft/signalr';
import api from '../api/axiosInstance';
import { getMyPermissions } from '../api/permissionAPI';
import AuthContext from './AuthContext';

export function AuthProvider({ children }) {
  const navigate = useNavigate();

  const [accessToken, setAccessToken] = useState(
    localStorage.getItem('accessToken') || null,
  );
  const [role, setRole] = useState(localStorage.getItem('role') || null);
  const [user, setUser] = useState(null);
  const [permissions, setPermissions] = useState([]);
  const [onlineUsers, setOnlineUsers] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);

  const hubRef = useRef(null);

  // ==================== AUTH ====================

  const login = async ({ email, password }) => {
    const res = await api.post('/Auth/login', { email, password });
    localStorage.setItem('accessToken', res.data.accessToken);
    localStorage.setItem('refreshToken', res.data.refreshToken);
    localStorage.setItem('role', res.data.role);

    setAccessToken(res.data.accessToken);
    setRole(res.data.role);
    setUser({
      id: res.data.userId,
      email: res.data.email,
      name: res.data.userName,
      role: res.data.role,
    });
  };

  const logout = useCallback(() => {
    hubRef.current?.stop().catch(() => {});
    hubRef.current = null;

    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('role');

    setAccessToken(null);
    setRole(null);
    setUser(null);
    setPermissions([]);
    setOnlineUsers([]);
    setUnreadCount(0);

    navigate('/login');
  }, [navigate]);

  // ==================== LOAD PROFILE ====================

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) {return;}

    api
      .get('/User/profile')
      .then((res) => {
        setUser({
          id: res.data.id,
          email: res.data.email,
          name: res.data.fullName,
          role: res.data.role,
        });
      })
      .catch(() => logout());
  }, []);

  // ==================== LOAD PERMISSIONS ====================

  useEffect(() => {
    if (!user?.id) {return;}
    const isAdminOrSuper = user.role === 'admin' || user.role === 'superAdmin';
    if (!isAdminOrSuper) {
      setPermissions([]);
      return;
    }

    getMyPermissions()
      .then((res) => setPermissions(Array.isArray(res.data) ? res.data : []))
      .catch(() => setPermissions([]));
  }, [user?.id, user?.role]);

  // ==================== SIGNALR ====================

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token || hubRef.current) {return;}

    const API_URL = import.meta.env.VITE_API_BASE_URL;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_URL}/hubs/chat`, {
        accessTokenFactory: () => localStorage.getItem('accessToken'),
      })
      .withAutomaticReconnect()
      .build();

    connection.on('OnlineUsers', (ids) => setOnlineUsers(ids));
    connection.on('UserOnline', (id) =>
      setOnlineUsers((prev) => [...new Set([...prev, id])]),
    );
    connection.on('UserOffline', (id) =>
      setOnlineUsers((prev) => prev.filter((x) => x !== id)),
    );

    connection.on('ReceiveNotification', () => {
      setUnreadCount((prev) => prev + 1);
    });

    connection
      .start()
      .then(async () => {
        await connection.invoke('SubscribeToNotifications').catch(() => {});
      })
      .catch((err) => console.error('SignalR error:', err));

    hubRef.current = connection;

    return () => {
      connection.stop().catch(() => {});
      hubRef.current = null;
    };
  }, [accessToken]);

  // ==================== UNREAD COUNT ON LOAD ====================

  useEffect(() => {
    if (!user?.id) {return;}
    api
      .get(`/Notification/count/${user.id}`)
      .then((res) => setUnreadCount(res.data))
      .catch(() => {});
  }, [user?.id]);

  const isOnline = useCallback(
    (userId) => onlineUsers.includes(userId),
    [onlineUsers],
  );

  const hasPermission = useCallback(
    (type) => {
      if (role === 'superAdmin') {return true;}
      return permissions.includes(type);
    },
    [role, permissions],
  );

  return (
    <AuthContext.Provider
      value={{
        accessToken,
        role,
        user,
        permissions,
        hasPermission,
        onlineUsers,
        unreadCount,
        setUnreadCount,
        isOnline,
        login,
        logout,
        setAccessToken,
        setRole,
        setUser,
        setPermissions,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
