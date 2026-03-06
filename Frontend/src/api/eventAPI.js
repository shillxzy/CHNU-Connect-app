import api from "./axiosInstance";

/* =========================
   Event
========================= */

export const getEvents = () => api.get("/Event");
export const createEvent = (data) => api.post("/Event", data);
export const getPublicEvents = () => api.get("/Event/public");
export const getEventById = (id) => api.get(`/Event/${id}`);
export const updateEvent = (id, data) => api.put(`/Event/${id}`, data);
export const deleteEvent = (id) => api.delete(`/Event/${id}`);
export const getEventsByUser = (userId) => api.get(`/Event/user/${userId}`);
export const getMyEvents = () => api.get("/Event/my-events");
export const joinEvent = (id) => api.post(`/Event/${id}/join`);
export const leaveEvent = (id) => api.delete(`/Event/${id}/leave`);
