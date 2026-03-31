// notificationAPI.js
import api from './axiosInstance';

export const getUnreadNotifications = (userId) =>
  api.get(`/Notification/unread/${userId}`);
export const getUserNotifications = (userId) =>
  api.get(`/Notification/user/${userId}`);
export const markNotificationAsRead = (notificationId) =>
  api.post(`/Notification/${notificationId}/read`);
