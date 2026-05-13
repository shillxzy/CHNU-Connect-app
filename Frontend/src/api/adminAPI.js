import api from './axiosInstance';

/* =========================
   Admin
========================= */

export const getActions = () => api.get('/Admin/actions');
export const createAction = (data) => api.post('/Admin/actions', data);
export const getActionById = (id) => api.get(`/Admin/actions/${id}`);
export const deleteActionById = (id) => api.delete(`/Admin/actions/${id}`);
export const getActionsByAdmin = (adminId) =>
  api.get(`/Admin/actions/admin/${adminId}`);
export const getActionsByTarget = (targetUserId) =>
  api.get(`/Admin/actions/target/${targetUserId}`);

export const blockUser = (userId) => api.post(`/Admin/users/${userId}/block`);
export const unblockUser = (userId) =>
  api.post(`/Admin/users/${userId}/unblock`);

export const getUsers = () => api.get('/Admin/users');
export const getUserById = (id) => api.get(`/Admin/users/${id}`);
export const setRole = (data) => api.post('/Admin/set-role', data);
export const updateUser = (id, data) => api.put(`/Admin/users/${id}`, data);
export const deleteEvent = (id) => api.delete(`/Event/${id}`);
export const deleteGroup = (id) => api.delete(`/Group/${id}`);
export const deletePost = (id) => api.delete(`/Post/${id}`);
