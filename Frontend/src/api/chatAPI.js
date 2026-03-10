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

/**
 * Створює чат між двома користувачами або повертає існуючий.
 */
export const getOrCreateDirectChat = async (currentUserId, targetUserId) => {
  try {

    const res = await getChatsByUser(currentUserId);
    const chats = res.data;

    const chat = chats.find(c =>
      c.members &&
      c.members.length === 2 &&
      c.members.some(m => m.userId === currentUserId) &&
      c.members.some(m => m.userId === targetUserId)
    );

    if (chat) return chat;

    const newChatRes = await createChat({
      type: "private",
      title: null,
      createdBy: currentUserId,
      memberIds: [currentUserId, targetUserId]
    });

    return newChatRes.data;

  } catch (err) {
    console.error("Помилка getOrCreateDirectChat:", err);
    throw err;
  }
};

