import api from './axiosInstance';

/* =========================
   Subject
========================= */

export const getSubjectsByGroup = (groupId) =>
  api.get(`/Subject/group/${groupId}`);

export const createSubject = (data) => api.post('/Subject', data);

export const updateSubject = (id, data) => api.put(`/Subject/${id}`, data);

export const deleteSubject = (id) => api.delete(`/Subject/${id}`);
