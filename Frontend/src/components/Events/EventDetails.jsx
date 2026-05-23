import React, { useContext, useEffect, useRef, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import AuthContext from '../../context/AuthContext';
import {
  getEventById,
  joinEvent,
  leaveEvent,
  inviteToEvent,
} from '../../api/eventAPI';
import { searchUsers } from '../../api/userAPI';

const API_BASE = import.meta.env.VITE_API_BASE_URL;
import './Events.css';
import Loading from '../Loading/Loading';

export default function EventDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user, role } = useContext(AuthContext);

  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [busy, setBusy] = useState(false);
  const [feedback, setFeedback] = useState(null);

  // invite modal state
  const [showInvite, setShowInvite] = useState(false);
  const [inviteQuery, setInviteQuery] = useState('');
  const [inviteResults, setInviteResults] = useState([]);
  const [inviteBusy, setInviteBusy] = useState(false);
  const [inviteFeedback, setInviteFeedback] = useState(null);
  const searchTimer = useRef(null);

  useEffect(() => {
    getEventById(id)
      .then((res) => setEvent(res.data))
      .catch((err) =>
        setError(err.response?.data?.message || 'Помилка завантаження події'),
      )
      .finally(() => setLoading(false));
  }, [id]);

  const canInvite =
    event &&
    (role === 'admin' ||
      role === 'superAdmin' ||
      role === 'teacher' ||
      user?.id === event.createdById);

  const handleToggle = async () => {
    setBusy(true);
    setFeedback(null);
    try {
      if (event.isJoinedByCurrentUser) {
        await leaveEvent(id);
        setEvent((prev) => ({
          ...prev,
          isJoinedByCurrentUser: false,
          participantCount: Math.max(0, prev.participantCount - 1),
        }));
        setFeedback({ type: 'info', text: 'Ви покинули подію' });
      } else {
        await joinEvent(id);
        setEvent((prev) => ({
          ...prev,
          isJoinedByCurrentUser: true,
          participantCount: prev.participantCount + 1,
        }));
        setFeedback({
          type: 'success',
          text: 'Ви успішно приєдналися до події',
        });
      }
    } catch (err) {
      setFeedback({
        type: 'error',
        text: err.response?.data?.message || 'Помилка',
      });
    } finally {
      setBusy(false);
    }
  };

  const handleInviteSearch = (q) => {
    setInviteQuery(q);
    setInviteFeedback(null);
    clearTimeout(searchTimer.current);
    if (!q.trim()) {
      setInviteResults([]);
      return;
    }
    searchTimer.current = setTimeout(async () => {
      try {
        const res = await searchUsers(q);
        setInviteResults(Array.isArray(res.data) ? res.data : []);
      } catch {
        setInviteResults([]);
      }
    }, 350);
  };

  const handleInviteUser = async (targetUser) => {
    setInviteBusy(true);
    setInviteFeedback(null);
    try {
      await inviteToEvent(id, targetUser.id);
      setInviteFeedback({
        type: 'success',
        text: `${targetUser.fullName} запрошено!`,
      });
      setInviteResults([]);
      setInviteQuery('');
    } catch (err) {
      setInviteFeedback({
        type: 'error',
        text: err.response?.data?.message || 'Помилка',
      });
    } finally {
      setInviteBusy(false);
    }
  };

  if (loading) {
    return <Loading />;
  }
  if (error) {
    return <p>Помилка: {error}</p>;
  }
  if (!event) {
    return <p>Подію не знайдено</p>;
  }

  const isCreator = user?.id === event.createdById;

  return (
    <div className="events-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <button className="btn-back" onClick={() => navigate(-1)}>
              ← Назад
            </button>
            <h1 className="page-title">{event.title}</h1>
          </div>

          <div className="event-card event-card--detail">
            {event.imageUrl && (
              <img
                src={`${API_BASE}${event.imageUrl}`}
                alt={event.title}
                style={{
                  width: '100%',
                  maxHeight: 320,
                  objectFit: 'cover',
                  borderRadius: 12,
                  marginBottom: 16,
                }}
              />
            )}
            <div className="event-info">
              {event.description && (
                <div className="event-field">
                  <strong>Опис:</strong> {event.description}
                </div>
              )}
              <div className="event-field">
                <strong>Початок:</strong>{' '}
                {new Date(event.startTime).toLocaleString('uk-UA')}
              </div>
              <div className="event-field">
                <strong>Завершення:</strong>{' '}
                {new Date(event.endTime).toLocaleString('uk-UA')}
              </div>
              <div className="event-field">
                <strong>Тип:</strong> {event.isPublic ? 'Публічна' : 'Приватна'}
              </div>
              <div className="event-field">
                <strong>Учасників:</strong> {event.participantCount}
              </div>
            </div>

            {feedback && (
              <p className={`ev-feedback ev-feedback--${feedback.type}`}>
                {feedback.text}
              </p>
            )}

            <div className="event-actions">
              {!isCreator && (
                <button
                  className={
                    event.isJoinedByCurrentUser
                      ? 'btn-leave'
                      : 'btn-participate'
                  }
                  onClick={handleToggle}
                  disabled={busy}
                >
                  {busy
                    ? '...'
                    : event.isJoinedByCurrentUser
                      ? 'Покинути подію'
                      : 'Взяти участь'}
                </button>
              )}

              {canInvite && (
                <button
                  className="btn-details"
                  onClick={() => {
                    setShowInvite((v) => !v);
                    setInviteFeedback(null);
                  }}
                >
                  {showInvite ? 'Сховати' : '+ Запросити учасника'}
                </button>
              )}
            </div>

            {/* ── Invite panel ── */}
            {showInvite && canInvite && (
              <div className="ev-invite-panel">
                <p className="ev-invite-title">Запросити учасника</p>
                <div className="ev-invite-search">
                  <input
                    className="form-input"
                    placeholder="Пошук за іменем або email..."
                    value={inviteQuery}
                    onChange={(e) => handleInviteSearch(e.target.value)}
                  />
                </div>
                {inviteResults.length > 0 && (
                  <ul className="ev-invite-list">
                    {inviteResults.map((u) => (
                      <li key={u.id} className="ev-invite-item">
                        <span className="ev-invite-name">
                          {u.fullName || u.email}
                        </span>
                        <button
                          className="btn-participate"
                          style={{ padding: '6px 14px', fontSize: 13 }}
                          onClick={() => handleInviteUser(u)}
                          disabled={inviteBusy}
                        >
                          Запросити
                        </button>
                      </li>
                    ))}
                  </ul>
                )}
                {inviteFeedback && (
                  <p
                    className={`ev-feedback ev-feedback--${inviteFeedback.type}`}
                  >
                    {inviteFeedback.text}
                  </p>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
