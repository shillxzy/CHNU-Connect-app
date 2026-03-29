import React from 'react';
import { Link } from 'react-router-dom';

export const Event = ({ events }) => {
  return (
    <>
      {Array.isArray(events) && events.length > 0 ? (
        events.map((event) => (
          <div className="event-card" key={event.id}>
            <div className="event-info">
              <div className="event-field">
                <strong>Назва:</strong> {event.title}
              </div>
              <div className="event-field">
                <strong>Дата:</strong> {event.startTime}
              </div>
              <div className="event-field">
                <strong>Опис:</strong> {event.description}
              </div>
            </div>
            <div className="event-actions">
              <button className="btn-participate">Взяти участь</button>
              <Link to={`/events/${event.id}`} className="btn-details">
                Детальніше
              </Link>
            </div>
          </div>
        ))
      ) : (
        <p>Немає подій</p>
      )}
    </>
  );
};
