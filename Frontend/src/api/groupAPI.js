import api from "./axiosInstance";

/* =========================
   Group
========================= */

export const getGroups = () => api.get("/Group");
export const getGroupById = (id) => api.get(`/Group/${id}`);
export const createGroup = (data) => api.post("/Group", data);
export const updateGroup = (id, data) => api.put(`/Group/${id}`, data);
export const deleteGroup = (id) => api.delete(`/Group/${id}`);

/* =========================
   Extra (тільки для деталей)
========================= */

export const getGroupSubjects = (groupId) =>
  api.get(`/Subject/group/${groupId}`);

export const getGroupSchedule = (groupId) =>
  api.get(`/Schedule/group/${groupId}`);
