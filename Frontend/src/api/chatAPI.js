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


export const getOrCreateDirectChat = async (currentUserId, targetUserId) => {
  try {
    const res = await getChatsByUser(currentUserId);
    const chats = res.data;

    // якщо members - масив об'єктів з id
    let chat = chats.find(c => 
      c.members.length === 2 &&
      c.members.map(m => m.id).includes(currentUserId) &&
      c.members.map(m => m.id).includes(targetUserId)
    );

    if (chat) return chat;

    // створення нового чату
    const newChatRes = await createChat({
      name: `Чат з користувачем`,
      members: [{ id: currentUserId }, { id: targetUserId }]
    });

    return newChatRes.data;
  } catch (err) {
    console.error("Помилка getOrCreateDirectChat:", err);
    throw err;
  }
};

