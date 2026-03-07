import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getEvents } from "../../api/eventAPI"; 
import { Event } from "./Event";
import "./Events.css";

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
        console.error("Error fetching events:", err);
        setError(err.response?.data?.message || err.message || "Помилка при завантаженні подій");
        setEvents([]);
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
<<<<<<< HEAD
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
=======
            <Event events={events} />
>>>>>>> 0e4775d29bd4491b9b45d492c39d7a397af1dcb2
          </div>
        </div>
      </div>
    </div>
  );
}
