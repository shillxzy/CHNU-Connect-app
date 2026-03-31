import { useEffect, useState, React } from 'react';
import { Link } from 'react-router-dom';
import { getEvents } from '../../api/eventAPI';
import { Event } from './Event';
import './Events.css';
import Loading from '../Loading/Loading';

export default function EventsList() {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        const response = await getEvents();

        const eventsData = Array.isArray(response.data) ? response.data : [];
        setEvents(eventsData);
      } catch (err) {
        console.error('Error fetching events:', err);
        setError(
          err.response?.data?.message ||
            err.message ||
            'Помилка при завантаженні подій',
        );
        setEvents([]);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  if (loading) {
    return <Loading />;
  }
  if (error) {
    return <p>Помилка: {error}</p>;
  }

  return (
    <div className="events-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <h1 className="page-title">Події</h1>
            <Link to="/events/create" className="btn-create">
              Створити подію
            </Link>
          </div>

          <h2 className="section-subtitle">Список подій</h2>

          <div className="events-list">
            <Event events={events} />
            <p>Немає подій</p>
          </div>
        </div>
      </div>
    </div>
  );
}
