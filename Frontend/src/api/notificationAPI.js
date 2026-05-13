import api from './axiosInstance';

export const getUnreadNotifications = (userId) =>
  api.get(`/Notification/user/${userId}`);

export const getUnreadCount = (userId) =>
  api.get(`/Notification/count/${userId}`);

export const markNotificationAsRead = (id) =>
  api.post(`/Notification/${id}/read`);

export const markAllNotificationsAsRead = () =>
  api.post('/Notification/read-all');
