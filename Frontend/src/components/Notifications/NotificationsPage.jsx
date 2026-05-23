import { useEffect, useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthContext from '../../context/AuthContext';
import Avatar from '../Avatar/Avatar';
import {
  getAllNotifications,
  markNotificationAsRead,
  markAllNotificationsAsRead,
} from '../../api/notificationAPI';

const TYPE_ROUTE = {
  message: (id) => `/chats/${id}`,
  event: (id) => `/events/${id}`,
  group: (id) => `/groups/${id}`,
  like: (id) => `/posts/${id}`,
  comment: (id) => `/posts/${id}`,
};

const TYPE_LABEL = {
  message: 'написав вам повідомлення',
  like: 'вподобав ваш пост',
  comment: 'прокоментував ваш пост',
  event: 'запросив на подію',
  group: 'додав вас до групи',
};

export default function NotificationsPage() {
  const { user, setUnreadCount } = useContext(AuthContext);
  const navigate = useNavigate();
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user?.id) {
      return;
    }
    getAllNotifications(user.id)
      .then((res) => setNotifications(res.data || []))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [user?.id]);

  const handleClick = async (n) => {
    if (!n.isRead) {
      await markNotificationAsRead(n.id).catch(() => {});
      setNotifications((prev) =>
        prev.map((x) => (x.id === n.id ? { ...x, isRead: true } : x)),
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    }
    const route = TYPE_ROUTE[n.type];
    if (route && n.entityId) {
      navigate(route(n.entityId));
    }
  };

  const handleDismiss = async (e, id) => {
    e.stopPropagation();
    await markNotificationAsRead(id).catch(() => {});
    setNotifications((prev) =>
      prev.map((x) => (x.id === id ? { ...x, isRead: true } : x)),
    );
    setUnreadCount((prev) => Math.max(0, prev - 1));
  };

  const handleMarkAll = async () => {
    await markAllNotificationsAsRead().catch(() => {});
    setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
    setUnreadCount(0);
  };

  if (loading) {
    return <div style={{ padding: 32 }}>Завантаження...</div>;
  }

  const unread = notifications.filter((n) => !n.isRead);

  return (
    <div style={{ maxWidth: 640, margin: '32px auto', padding: '0 16px' }}>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 20,
        }}
      >
        <h1 style={{ fontSize: 22, fontWeight: 700, margin: 0 }}>Сповіщення</h1>
        {unread.length > 0 && (
          <button
            onClick={handleMarkAll}
            style={{
              background: 'none',
              border: 'none',
              color: '#2563eb',
              cursor: 'pointer',
              fontSize: 13,
              fontWeight: 500,
            }}
          >
            Прочитати всі
          </button>
        )}
      </div>

      {notifications.length === 0 && (
        <p style={{ color: '#9ca3af', textAlign: 'center', marginTop: 40 }}>
          Немає сповіщень
        </p>
      )}

      <div style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
        {notifications.map((n) => (
          <div
            key={n.id}
            onClick={() => handleClick(n)}
            style={{
              display: 'flex',
              alignItems: 'flex-start',
              gap: 12,
              padding: '12px 16px',
              borderRadius: 12,
              background: n.isRead ? '#fff' : '#eff6ff',
              border: '1px solid #e5e7eb',
              cursor: TYPE_ROUTE[n.type] && n.entityId ? 'pointer' : 'default',
              transition: 'background 0.15s',
            }}
          >
            <Avatar photoUrl={n.actorAvatar} size={38} />
            <div style={{ flex: 1 }}>
              <span style={{ fontWeight: 600, fontSize: 14 }}>
                {n.actorName || 'Хтось'}
              </span>{' '}
              <span style={{ color: '#6b7280', fontSize: 13 }}>
                {TYPE_LABEL[n.type] || 'надіслав сповіщення'}
              </span>
              {n.body && (
                <div style={{ fontSize: 12, color: '#374151', marginTop: 3 }}>
                  {n.body}
                </div>
              )}
              <div style={{ fontSize: 11, color: '#9ca3af', marginTop: 4 }}>
                {new Date(n.createdAt).toLocaleString('uk-UA', {
                  day: 'numeric',
                  month: 'short',
                  hour: '2-digit',
                  minute: '2-digit',
                })}
              </div>
            </div>
            <div
              style={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'flex-end',
                gap: 6,
              }}
            >
              {!n.isRead && (
                <span
                  style={{
                    width: 8,
                    height: 8,
                    borderRadius: '50%',
                    background: '#2563eb',
                    display: 'inline-block',
                  }}
                />
              )}
              {!n.isRead && (
                <button
                  onClick={(e) => handleDismiss(e, n.id)}
                  style={{
                    background: 'none',
                    border: 'none',
                    color: '#9ca3af',
                    cursor: 'pointer',
                    fontSize: 12,
                    padding: 0,
                  }}
                  title="Позначити як прочитане"
                >
                  ✕
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
