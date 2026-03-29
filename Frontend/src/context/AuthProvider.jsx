import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axiosInstance';
import AuthContext from './AuthContext';

export function AuthProvider({ children }) {
  const navigate = useNavigate();

  const [accessToken, setAccessToken] = useState(
    localStorage.getItem('accessToken') || null,
  );
  const [role, setRole] = useState(localStorage.getItem('role') || null);
  const [user, setUser] = useState(null);

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
    });
  };

  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('role');
    setAccessToken(null);
    setRole(null);
    setUser(null);
    navigate('/login');
  };

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    const userRole = localStorage.getItem('role');
    if (token) {
      setAccessToken(token);
      setRole(userRole);
    }
  }, []);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');

    if (token) {
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
        .catch(() => {
          logout();
        });
    }
  }, []);

  return (
    <AuthContext.Provider
      value={{
        accessToken,
        role,
        user,
        login,
        logout,
        setAccessToken,
        setRole,
        setUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
