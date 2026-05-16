import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { joinEvent, leaveEvent } from '../../api/eventAPI';

const MON_UK = [
  'січ',
  'лют',
  'бер',
  'кві',
  'тра',
  'чер',
  'лип',
  'сер',
  'вер',
  'жов',
  'лис',
  'гру',
];

function fmtDate(iso) {
  const d = new Date(iso);
  return `${d.getDate()} ${MON_UK[d.getMonth()]} ${d.getFullYear()}, ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
}

export function EventCard({ event, onJoin, onLeave }) {
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState(null);

  const handleToggle = async () => {
    setBusy(true);
    setErr(null);
    try {
      if (event.isJoinedByCurrentUser) {
        await leaveEvent(event.id);
        onLeave?.(event.id);
      } else {
        await joinEvent(event.id);
        onJoin?.(event.id);
      }
    } catch (e) {
      setErr(e.response?.data?.message || 'Помилка');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="event-card">
      <div className="event-info">
        <div className="event-field ev-title">{event.title}</div>
        {event.description && (
          <div className="event-field ev-desc">{event.description}</div>
        )}
        <div className="event-field ev-meta">
          <span>{fmtDate(event.startTime)}</span>
          {event.isPublic !== undefined && (
            <span
              className={`ev-badge ${event.isPublic ? 'ev-badge--public' : 'ev-badge--private'}`}
            >
              {event.isPublic ? 'Публічна' : 'Приватна'}
            </span>
          )}
          {event.participantCount > 0 && (
            <span className="ev-participants">
              {event.participantCount} учасник
              {declension(event.participantCount)}
            </span>
          )}
        </div>
      </div>
      {err && <p className="ev-feedback ev-feedback--error">{err}</p>}
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
              ? 'Покинути'
              : 'Взяти участь'}
        </button>
        <Link to={`/events/${event.id}`} className="btn-details">
          Детальніше
        </Link>
      </div>
    </div>
  );
}

function declension(n) {
  if (n % 10 === 1 && n % 100 !== 11) {return '';}
  if (n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) {return 'и';}
  return 'ів';
}
