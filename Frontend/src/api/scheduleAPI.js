import api from './axiosInstance';

/* =========================
   Lesson Slots
========================= */
export const getLessonSlots = () => api.get('/Schedule/slots');
export const seedLessonSlots = () => api.post('/Schedule/slots/seed');

/* =========================
   Schedule
========================= */
export const getScheduleByGroup = (groupId) =>
  api.get(`/Schedule/group/${groupId}`);
export const getMySchedule = () => api.get('/Schedule/my');
export const createSchedule = (data) => api.post('/Schedule', data);
export const deleteSchedule = (id) => api.delete(`/Schedule/${id}`);
export const moveSchedule = (data) => api.put('/Schedule/move', data);
export const swapSchedule = (data) => api.put('/Schedule/swap', data);

/* =========================
   SubGroups
========================= */
export const getSubGroups = (groupId) =>
  api.get(`/Schedule/subgroups/${groupId}`);
export const createSubGroup = (data) => api.post('/Schedule/subgroups', data);
export const updateSubGroup = (id, data) =>
  api.put(`/Schedule/subgroups/${id}`, data);
export const deleteSubGroup = (id) => api.delete(`/Schedule/subgroups/${id}`);
