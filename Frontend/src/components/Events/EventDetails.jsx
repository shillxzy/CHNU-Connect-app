import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getEventById, joinEvent, leaveEvent } from '../../api/eventAPI';
import './Events.css';
import Loading from '../Loading/Loading';

export default function EventDetails() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [busy, setBusy] = useState(false);
  const [feedback, setFeedback] = useState(null);

  useEffect(() => {
    getEventById(id)
      .then((res) => setEvent(res.data))
      .catch((err) =>
        setError(err.response?.data?.message || 'Помилка завантаження події'),
      )
      .finally(() => setLoading(false));
  }, [id]);

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

  if (loading) {return <Loading />;}
  if (error) {return <p>Помилка: {error}</p>;}
  if (!event) {return <p>Подію не знайдено</p>;}

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
              <button
                className={
                  event.isJoinedByCurrentUser ? 'btn-leave' : 'btn-participate'
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
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
