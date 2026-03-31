import api from './axiosInstance';

/* =========================
   User
========================= */

export const getProfile = () => api.get('/User/profile');
export const updateProfile = (data) => api.put('/User/profile', data);
export const uploadPhoto = (formData) =>
  api.post('/User/upload-photo', formData);
export const deletePhoto = () => api.delete('/User/photo');
export const getAllUsers = () => api.get('/User/all');
export const blockUser = (userId) => api.post(`/User/${userId}/block`);
export const unblockUser = (userId) => api.post(`/User/${userId}/unblock`);
export const getUserById = (userId) => api.get(`/User/${userId}`);
