import api from './axiosInstance';

export const getMyPermissions = () => api.get('/Admin/my-permissions');
export const getUserPermissions = (userId) =>
  api.get(`/Admin/permissions/${userId}`);
export const grantPermission = (userId, type) =>
  api.post('/Admin/permissions/grant', { userId, type });
export const revokePermission = (userId, type) =>
  api.delete('/Admin/permissions/revoke', { data: { userId, type } });

export const ALL_PERMISSION_TYPES = [
  'ManageContent',
  'ManageSchedule',
  'ManageUsers',
  'ManageEvents',
];

export const PERMISSION_LABELS = {
  ManageContent: 'Контент (пости, коментарі)',
  ManageSchedule: 'Розклад',
  ManageUsers: 'Користувачі (блок, ролі)',
  ManageEvents: 'Події та новини',
};
