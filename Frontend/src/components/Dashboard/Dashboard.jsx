import React, { useContext, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AuthContext from '../../context/AuthContext';
import { getMySchedule, getLessonSlots } from '../../api/scheduleAPI';
import { getGroups, getCuratedGroups } from '../../api/groupAPI';
import { getEvents } from '../../api/eventAPI';
import './Dashboard.css';

/* ── helpers ── */
const DAY_MAP = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6,
};
const DAY_UK = [
  'Неділя',
  'Понеділок',
  'Вівторок',
  'Середа',
  'Четвер',
  "П'ятниця",
  'Субота',
];
const MON_UK = [
  'січня',
  'лютого',
  'березня',
  'квітня',
  'травня',
  'червня',
  'липня',
  'серпня',
  'вересня',
  'жовтня',
  'листопада',
  'грудня',
];

function getISOWeek(date) {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  d.setDate(d.getDate() + 3 - ((d.getDay() + 6) % 7));
  const w1 = new Date(d.getFullYear(), 0, 4);
  return (
    1 +
    Math.round(
      ((d.getTime() - w1.getTime()) / 86400000 - 3 + ((w1.getDay() + 6) % 7)) /
        7,
    )
  );
}

function todayLabel() {
  const d = new Date();
  return `${DAY_UK[d.getDay()]}, ${d.getDate()} ${MON_UK[d.getMonth()]}`;
}

function toDay(raw) {
  if (typeof raw === 'number') {return raw;}
  return DAY_MAP[raw] ?? 0;
}

function matchesWeek(s, weekType) {
  if (s.isEveryWeek) {return true;}
  const w = s.week;
  if (typeof w === 'number') {return w === weekType;}
  return (
    (weekType === 1 && w === 'First') || (weekType === 2 && w === 'Second')
  );
}

function getTodayClasses(schedule, slots) {
  const today = new Date().getDay();
  const weekType = getISOWeek(new Date()) % 2 === 1 ? 1 : 2;
  return schedule
    .filter((s) => toDay(s.day) === today && matchesWeek(s, weekType))
    .sort((a, b) => a.pairNumber - b.pairNumber)
    .map((s) => {
      const slot = slots.find((sl) => sl.id === s.slotId);
      return {
        ...s,
        startTime: slot?.startTime ?? '',
        endTime: slot?.endTime ?? '',
      };
    });
}

function fmtTime(t) {
  if (!t) {return '';}
  return typeof t === 'string' ? t.slice(0, 5) : '';
}

function upcomingEvents(events) {
  const now = new Date();
  return events
    .filter((e) => new Date(e.startTime) >= now)
    .sort((a, b) => new Date(a.startTime) - new Date(b.startTime))
    .slice(0, 3);
}

function fmtEventDate(iso) {
  const d = new Date(iso);
  return `${d.getDate()} ${MON_UK[d.getMonth()]}`;
}

const ROLE_LABELS = {
  student: 'Студент',
  teacher: 'Викладач',
  admin: 'Адміністратор',
  superAdmin: 'Супер-адмін',
};
const ROLE_COLORS = {
  student: '#16a34a',
  teacher: '#0369a1',
  admin: '#7c3aed',
  superAdmin: '#b91c1c',
};

/* ── cards ── */
function ScheduleCard({ schedule, slots }) {
  const today = getTodayClasses(schedule, slots);
  return (
    <div className="db-card">
      <div className="db-card-header">
        <span className="db-card-icon">📅</span>
        <h3>Сьогодні</h3>
      </div>
      {today.length === 0 ? (
        <p className="db-empty">Пар немає</p>
      ) : (
        <ul className="db-list">
          {today.slice(0, 4).map((s) => (
            <li key={s.id} className="db-list-item">
              <span className="db-item-time">{fmtTime(s.startTime)}</span>
              <span className="db-item-main">{s.subjectName}</span>
              {s.location && <span className="db-item-sub">{s.location}</span>}
            </li>
          ))}
        </ul>
      )}
      <Link to="/groups" className="db-card-link">
        Мій розклад →
      </Link>
    </div>
  );
}

function GroupsCard({ groups }) {
  const navigate = useNavigate();
  return (
    <div className="db-card">
      <div className="db-card-header">
        <span className="db-card-icon">👥</span>
        <h3>Мої групи</h3>
      </div>
      {groups.length === 0 ? (
        <p className="db-empty">Немає груп</p>
      ) : (
        <ul className="db-list">
          {groups.slice(0, 4).map((g) => (
            <li
              key={g.id}
              className="db-list-item db-list-item--clickable"
              onClick={() => navigate('/groups')}
            >
              <span className="db-item-main">{g.name}</span>
            </li>
          ))}
        </ul>
      )}
      <Link to="/groups" className="db-card-link">
        Всі групи →
      </Link>
    </div>
  );
}

function CuratedGroupsCard({ groups }) {
  const navigate = useNavigate();
  return (
    <div className="db-card db-card--wide">
      <div className="db-card-header">
        <span className="db-card-icon">🎓</span>
        <h3>Куровані групи</h3>
      </div>
      {groups.length === 0 ? (
        <p className="db-empty">Немає курованих груп</p>
      ) : (
        <ul className="db-list">
          {groups.slice(0, 5).map((g) => (
            <li key={g.id} className="db-list-item db-list-item--row">
              <span className="db-item-main">{g.name}</span>
              <button
                className="db-mini-btn"
                onClick={() => navigate(`/group/edit/schedule/${g.id}`)}
              >
                Розклад
              </button>
            </li>
          ))}
        </ul>
      )}
      <Link to="/groups" className="db-card-link">
        Всі групи →
      </Link>
    </div>
  );
}

function EventsCard({ events }) {
  const upcoming = upcomingEvents(events);
  return (
    <div className="db-card">
      <div className="db-card-header">
        <span className="db-card-icon">📢</span>
        <h3>Найближчі події</h3>
      </div>
      {upcoming.length === 0 ? (
        <p className="db-empty">Немає подій</p>
      ) : (
        <ul className="db-list">
          {upcoming.map((e) => (
            <li key={e.id} className="db-list-item">
              <span className="db-item-time">{fmtEventDate(e.startTime)}</span>
              <span className="db-item-main">{e.title}</span>
            </li>
          ))}
        </ul>
      )}
      <Link to="/events" className="db-card-link">
        Всі події →
      </Link>
    </div>
  );
}

function AdminCard() {
  return (
    <div className="db-card db-card--wide">
      <div className="db-card-header">
        <span className="db-card-icon">⚡</span>
        <h3>Швидкі дії</h3>
      </div>
      <div className="db-quick-actions">
        <Link to="/admin-panel" className="db-action-btn">
          Адмін панель
        </Link>
        <Link to="/events/create" className="db-action-btn">
          + Подія
        </Link>
        <Link to="/groups/create" className="db-action-btn">
          + Група
        </Link>
      </div>
    </div>
  );
}

/* ── main ── */
export default function Dashboard() {
  const { user, role } = useContext(AuthContext);
  const [schedule, setSchedule] = useState([]);
  const [slots, setSlots] = useState([]);
  const [groups, setGroups] = useState([]);
  const [events, setEvents] = useState([]);

  const isStudent = role === 'student';
  const isTeacher = role === 'teacher';
  const isAdmin = role === 'admin' || role === 'superAdmin';

  useEffect(() => {
    const loads = [
      getEvents()
        .then((r) => setEvents(r.data || []))
        .catch(() => {}),
    ];

    if (isStudent) {
      loads.push(
        getMySchedule()
          .then((r) => setSchedule(r.data || []))
          .catch(() => {}),
        getLessonSlots()
          .then((r) => setSlots(r.data || []))
          .catch(() => {}),
        getGroups()
          .then((r) => setGroups(r.data || []))
          .catch(() => {}),
      );
    }
    if (isTeacher) {
      loads.push(
        getCuratedGroups()
          .then((r) => setGroups(r.data || []))
          .catch(() => {}),
      );
    }
    if (isAdmin) {
      loads.push(
        getGroups()
          .then((r) => setGroups(r.data || []))
          .catch(() => {}),
      );
    }

    Promise.all(loads);
  }, [role]);

  const roleColor = ROLE_COLORS[role] ?? '#374151';
  const roleLabel = ROLE_LABELS[role] ?? role;

  return (
    <div className="db-wrap">
      {/* greeting bar */}
      <div className="db-greeting">
        <div className="db-greeting-left">
          <span className="db-welcome">
            Привіт, {user?.name || 'Користувач'}!
          </span>
          <span
            className="db-role-badge"
            style={{
              background: roleColor + '18',
              color: roleColor,
              borderColor: roleColor + '40',
            }}
          >
            {roleLabel}
          </span>
        </div>
        <span className="db-date">{todayLabel()}</span>
      </div>

      {/* cards */}
      <div className="db-cards">
        {isStudent && (
          <>
            <ScheduleCard schedule={schedule} slots={slots} />
            <GroupsCard groups={groups} />
          </>
        )}
        {isTeacher && <CuratedGroupsCard groups={groups} />}
        {isAdmin && <AdminCard />}
        <EventsCard events={events} />
      </div>
    </div>
  );
}
