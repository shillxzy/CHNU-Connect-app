import api from './axiosInstance';

/* =========================
   Group
========================= */

export const getGroups = () => api.get('/Group/my');
export const getAllGroups = () => api.get('/Group');
export const getCuratedGroups = () => api.get('/Group/curated');
export const getGroupById = (id) => api.get(`/Group/${id}`);
export const createGroup = (data) => api.post('/Group', data);
export const updateGroup = (id, data) => api.put(`/Group/${id}`, data);
export const deleteGroup = (id) => api.delete(`/Group/${id}`);

/* =========================
   Members
========================= */

export const assignCurator = (groupId, userId) =>
  api.post(`/Group/${groupId}/assign-curator`, userId, {
    headers: {
      'Content-Type': 'application/json',
    },
  });

export const addUserToGroup = (groupId, userId, role) =>
  api.post(`/Group/${groupId}/add-user`, {
    userId,
    role,
  });
