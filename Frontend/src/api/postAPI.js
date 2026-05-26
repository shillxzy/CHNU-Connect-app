import api from './axiosInstance';

/* =========================
   Post
========================= */

export const getPosts = async () => {
  const response = await api.get('/Post');
  return response.data;
};

export const getFirstFivePosts = async () => {
  const response = await api.get('/Post', {
    params: {
      _limit: 5,
    },
  });
  return response.data;
};
export const createPost = (data) => api.post('/Post', data);
export const getFeed = (page = 1, pageSize = 10) =>
  api.get('/Post/feed', { params: { page, pageSize } });
export const getPostById = (id) => api.get(`/Post/${id}`);
export const updatePost = (id, data) => api.put(`/Post/${id}`, data);
export const deletePost = (id) => api.delete(`/Post/${id}`);
export const getPostsByUser = (userId) => api.get(`/Post/user/${userId}`);
export const likePost = (id) => api.post(`/Post/${id}/like`);
export const unlikePost = (id) => api.delete(`/Post/${id}/like`);
export const getPostLikes = (id) => api.get(`/Post/${id}/likes`);
export const searchPosts = (query) =>
  api.get('/Post/search', { params: { q: query } });
export const createPostWithImage = (formData) =>
  api.post('/Post/with-image', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

export const updatePostImage = (id, formData) =>
  api.put(`/Post/${id}/image`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
