import api from './axiosInstance';

/* =========================
   Schedule
========================= */

export const getScheduleByGroup = (groupId) =>
  api.get(`/Schedule/group/${groupId}`);

export const getMySchedule = () => api.get('/Schedule/my');

export const createSchedule = (data) => api.post('/Schedule', data);

export const deleteSchedule = (id) => api.delete(`/Schedule/${id}`);

/* =========================
   Drag & Drop actions
========================= */

export const moveSchedule = (data) => api.put('/Schedule/move', data);

export const swapSchedule = (data) => api.put('/Schedule/swap', data);
