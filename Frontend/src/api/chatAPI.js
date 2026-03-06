import api from "./axiosInstance";

/* =========================
   Chat
========================= */

export const getChat = (chatId) => api.get(`/Chat/${chatId}`);
export const getChatsByUser = (userId) => api.get(`/Chat/user/${userId}`);
export const createChat = (data) => api.post("/Chat", data);
export const addMembers = (chatId, data) => api.post(`/Chat/${chatId}/members`, data);

export const getMessages = (chatId) => api.get(`/Chat/${chatId}/messages`);
export const sendMessage = (chatId, data) => api.post(`/Chat/${chatId}/messages`, data);
export const markMessageRead = (chatId, messageId, userId) =>
  api.post(`/Chat/${chatId}/messages/${messageId}/read/${userId}`);
