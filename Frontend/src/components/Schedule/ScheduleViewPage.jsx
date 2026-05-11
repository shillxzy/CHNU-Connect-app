import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

import ScheduleTable from './ScheduleTable';
import { getScheduleByGroup, getLessonSlots, getSubGroups } from '../../api/scheduleAPI';
import { getGroupById } from '../../api/groupAPI';

const DAY_MAP  = { Sunday:0, Monday:1, Tuesday:2, Wednesday:3, Thursday:4, Friday:5, Saturday:6 };
const WEEK_MAP = { First:1, Second:2 };
const TYPE_MAP = { Lecture:0, Practice:1 };

function normalizeSchedule(raw) {
  return raw.map((item) => ({
    ...item,
    day:         typeof item.day  === 'string' ? (DAY_MAP[item.day]   ?? item.day)  : item.day,
    week:        typeof item.week === 'string' ? (WEEK_MAP[item.week] ?? item.week) : item.week,
    type:        typeof item.type === 'string' ? (TYPE_MAP[item.type] ?? item.type) : item.type,
    isEveryWeek: item.isEveryWeek ?? false,
  }));
}

export default function ScheduleViewPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [schedule,  setSchedule]  = useState([]);
  const [group,     setGroup]     = useState(null);
  const [slots,     setSlots]     = useState([]);
  const [subGroups, setSubGroups] = useState([]);

  useEffect(() => { load(); }, []);

  const load = async () => {
    try {
      const [groupRes, scheduleRes, slotsRes, sgRes] = await Promise.all([
        getGroupById(id), getScheduleByGroup(id), getLessonSlots(), getSubGroups(id),
      ]);
      setGroup(groupRes.data);
      setSchedule(normalizeSchedule(scheduleRes.data || []));
      setSlots(slotsRes.data  || []);
      setSubGroups(sgRes.data || []);
    } catch (err) { console.error('LOAD ERROR:', err); }
  };

  return (
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '20px' }}>
        <button
          onClick={() => navigate(-1)}
          style={{ padding: '8px 16px', background: '#6c757d', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer' }}
        >
          ← Назад
        </button>
        <h2 style={{ margin: 0 }}>Розклад — {group?.name}</h2>
      </div>

      <ScheduleTable
        schedule={schedule}
        subGroups={subGroups}
        slots={slots}
        onCreate={() => {}}
        onMove={() => {}}
        onDelete={() => {}}
        isReadonly={true}
      />
    </div>
  );
}
