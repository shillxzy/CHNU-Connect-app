import api from "./axiosInstance";

/* =========================
   Group
========================= */

export const getGroups = () => api.get("/Group");
export const createGroup = (data) => api.post("/Group", data);
export const getPublicGroups = () => api.get("/Group/public");
export const getGroupById = (id) => api.get(`/Group/${id}`);
export const updateGroup = (id, data) => api.put(`/Group/${id}`, data);
export const deleteGroup = (id) => api.delete(`/Group/${id}`);
export const getGroupsByUser = (userId) => api.get(`/Group/user/${userId}`);
export const getMyGroups = () => api.get("/Group/my-groups");
export const joinGroup = (id) => api.post(`/Group/${id}/join`);
export const leaveGroup = (id) => api.delete(`/Group/${id}/leave`);
