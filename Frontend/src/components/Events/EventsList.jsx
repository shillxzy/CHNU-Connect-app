import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getAllEvents } from "../../api/getAllEventsApi"; // твій API для подій
import "./Events.css";

export default function EventsList() {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        const token = localStorage.getItem("token");
        const data = await getAllEvents(token);
        setEvents(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  if (loading) return <p>Завантаження подій...</p>;
  if (error) return <p>Помилка: {error}</p>;

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
            {events.map((event) => (
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
                  <button className="btn-details">Детальніше</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
