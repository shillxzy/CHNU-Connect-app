import api from "./axiosInstance";

/* =========================
   Authentication
========================= */

export const register = (data) => api.post("/Auth/register", data);
export const login = (data) => api.post("/Auth/login", data);
export const refreshToken = (data) => api.post("/Auth/refresh-token", data);
export const forgotPassword = (data) => api.post("/Auth/forgot-password", data);
export const resetPassword = (data) => api.post("/Auth/reset-password", data);
export const logout = () => api.post("/Auth/logout");
