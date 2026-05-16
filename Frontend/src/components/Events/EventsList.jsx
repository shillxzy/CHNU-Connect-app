import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../context/AuthContext';
import { getEvents } from '../../api/eventAPI';
import { EventCard } from './Event';
import './Events.css';
import Loading from '../Loading/Loading';

export default function EventsList() {
  const { role, hasPermission } = useContext(AuthContext);
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const canCreate = role === 'superAdmin' || hasPermission('ManageEvents');

  useEffect(() => {
    getEvents()
      .then((res) => setEvents(Array.isArray(res.data) ? res.data : []))
      .catch((err) =>
        setError(
          err.response?.data?.message || 'Помилка при завантаженні подій',
        ),
      )
      .finally(() => setLoading(false));
  }, []);

  const handleJoin = (eventId) => {
    setEvents((prev) =>
      prev.map((e) =>
        e.id === eventId
          ? {
              ...e,
              isJoinedByCurrentUser: true,
              participantCount: e.participantCount + 1,
            }
          : e,
      ),
    );
  };

  const handleLeave = (eventId) => {
    setEvents((prev) =>
      prev.map((e) =>
        e.id === eventId
          ? {
              ...e,
              isJoinedByCurrentUser: false,
              participantCount: Math.max(0, e.participantCount - 1),
            }
          : e,
      ),
    );
  };

  if (loading) {return <Loading />;}
  if (error) {return <p>Помилка: {error}</p>;}

  return (
    <div className="events-page">
      <div className="page-content">
        <div className="content-container">
          <div className="page-header">
            <h1 className="page-title">Події</h1>
            {canCreate && (
              <Link to="/events/create" className="btn-create">
                Створити подію
              </Link>
            )}
          </div>

          <div className="events-list">
            {events.length === 0 ? (
              <p>Немає подій</p>
            ) : (
              events.map((event) => (
                <EventCard
                  key={event.id}
                  event={event}
                  onJoin={handleJoin}
                  onLeave={handleLeave}
                />
              ))
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
