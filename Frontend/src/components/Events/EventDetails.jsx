import { useEffect, useState, React } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getEventById, joinEvent } from '../../api/eventAPI';
import './Events.css';
import Loading from '../Loading/Loading';

export default function EventDetails() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [joining, setJoining] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEvent = async () => {
      try {
        const response = await getEventById(id);
        setEvent(response.data);
      } catch (err) {
        console.error('Error loading event:', err);
        setError(err.response?.data?.message || 'Помилка завантаження події');
      } finally {
        setLoading(false);
      }
    };

    fetchEvent();
  }, [id]);

  const handleJoin = async () => {
    try {
      setJoining(true);
      await joinEvent(id);
      alert('Ви успішно приєдналися до події');
    } catch (err) {
      console.error(err);
      alert(err.response?.data?.message || 'Не вдалося приєднатися');
    } finally {
      setJoining(false);
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

          <div className="event-card">
            <div className="event-info">
              <div className="event-field">
                <strong>Назва:</strong> {event.title}
              </div>

              <div className="event-field">
                <strong>Опис:</strong> {event.description}
              </div>

              <div className="event-field">
                <strong>Початок:</strong>{' '}
                {new Date(event.startTime).toLocaleString()}
              </div>

              <div className="event-field">
                <strong>Завершення:</strong>{' '}
                {new Date(event.endTime).toLocaleString()}
              </div>

              <div className="event-field">
                <strong>Створено:</strong>{' '}
                {new Date(event.createdAt).toLocaleString()}
              </div>

              <div className="event-field">
                <strong>Тип події:</strong>{' '}
                {event.isPublic ? 'Публічна' : 'Приватна'}
              </div>
            </div>

            <div className="event-actions">
              <button
                className="btn-participate"
                onClick={handleJoin}
                disabled={joining}
              >
                {joining ? 'Приєднання...' : 'Взяти участь'}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
