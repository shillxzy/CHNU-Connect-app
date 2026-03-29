import api from './axiosInstance';

/* =========================
   Schedule
========================= */

export const getScheduleByGroup = (groupId) =>
  api.get(`/Schedule/group/${groupId}`);

export const getMySchedule = () => api.get('/Schedule/my');

export const createSchedule = (data) => api.post('/Schedule', data);

export const deleteSchedule = (id) => api.delete(`/Schedule/${id}`);
