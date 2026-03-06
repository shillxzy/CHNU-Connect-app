import api from "./axiosInstance";

/* =========================
   Comment
========================= */

export const getComment = (id) => api.get(`/Comment/${id}`);
export const updateComment = (id, data) => api.put(`/Comment/${id}`, data);
export const deleteComment = (id) => api.delete(`/Comment/${id}`);
export const getCommentsByPost = (postId) => api.get(`/Comment/post/${postId}`);
export const getCommentsByUser = (userId) => api.get(`/Comment/user/${userId}`);
export const createComment = (data) => api.post("/Comment", data);
