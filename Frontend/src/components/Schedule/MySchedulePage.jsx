import { useEffect, useState } from 'react';
import {
  getMySchedule,
  getLessonSlots,
  getSubGroups,
} from '../../api/scheduleAPI';
import { getGroupById } from '../../api/groupAPI';
import ScheduleTable from './ScheduleTable';
import Loading from '../Loading/Loading';

const DAY_MAP = {
  Sunday: 0,
  Monday: 1,
  Tuesday: 2,
  Wednesday: 3,
  Thursday: 4,
  Friday: 5,
  Saturday: 6,
};
const WEEK_MAP = { First: 1, Second: 2 };
const TYPE_MAP = { Lecture: 0, Practice: 1 };

function normalizeSchedule(raw) {
  return raw.map((item) => ({
    ...item,
    day:
      typeof item.day === 'string' ? (DAY_MAP[item.day] ?? item.day) : item.day,
    week:
      typeof item.week === 'string'
        ? (WEEK_MAP[item.week] ?? item.week)
        : item.week,
    type:
      typeof item.type === 'string'
        ? (TYPE_MAP[item.type] ?? item.type)
        : item.type,
    isEveryWeek: item.isEveryWeek ?? false,
  }));
}

export default function MySchedulePage() {
  const [allSchedule, setAllSchedule] = useState([]);
  const [groups, setGroups] = useState([]);
  const [selectedGroupId, setSelectedGroupId] = useState(null);
  const [slots, setSlots] = useState([]);
  const [subGroups, setSubGroups] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const [schedRes, slotsRes] = await Promise.all([
          getMySchedule(),
          getLessonSlots(),
        ]);

        const rawSchedule = schedRes.data || [];
        const normalized = normalizeSchedule(rawSchedule);

        const groupIds = [...new Set(rawSchedule.map((s) => s.groupId))];

        const groupInfos = await Promise.all(
          groupIds.map(async (gId) => {
            try {
              const r = await getGroupById(gId);
              return { id: gId, name: r.data?.name || `Група ${gId}` };
            } catch {
              return { id: gId, name: `Група ${gId}` };
            }
          }),
        );

        setAllSchedule(normalized);
        setGroups(groupInfos);
        setSlots(slotsRes.data || []);

        if (groupIds.length > 0) {setSelectedGroupId(groupIds[0]);}
      } catch (err) {
        console.error('MySchedulePage load error:', err);
      } finally {
        setLoading(false);
      }
    };

    load();
  }, []);

  useEffect(() => {
    if (!selectedGroupId) {return;}
    getSubGroups(selectedGroupId)
      .then((r) => setSubGroups(r.data || []))
      .catch(() => setSubGroups([]));
  }, [selectedGroupId]);

  if (loading) {return <Loading />;}

  const filteredSchedule = allSchedule.filter(
    (s) => s.groupId === selectedGroupId,
  );
  const selectedGroupName =
    groups.find((g) => g.id === selectedGroupId)?.name || '';

  return (
    <div style={{ padding: '20px' }}>
      <h2 style={{ marginBottom: '16px' }}>Мій розклад</h2>

      {groups.length === 0 && (
        <p style={{ color: '#6c757d' }}>Ви не входите до жодної групи.</p>
      )}

      {groups.length > 0 && (
        <>
          {groups.length > 1 && (
            <div
              style={{
                marginBottom: '16px',
                display: 'flex',
                alignItems: 'center',
                gap: '10px',
              }}
            >
              <label htmlFor="group-select" style={{ fontWeight: 600 }}>
                Оберіть групу:
              </label>
              <select
                id="group-select"
                value={selectedGroupId ?? ''}
                onChange={(e) => setSelectedGroupId(Number(e.target.value))}
                style={{
                  padding: '6px 12px',
                  borderRadius: '8px',
                  border: '1px solid #ddd',
                  fontSize: '14px',
                }}
              >
                {groups.map((g) => (
                  <option key={g.id} value={g.id}>
                    {g.name}
                  </option>
                ))}
              </select>
            </div>
          )}

          {groups.length === 1 && (
            <p style={{ marginBottom: '12px', color: '#555' }}>
              Група: <strong>{selectedGroupName}</strong>
            </p>
          )}

          <ScheduleTable
            schedule={filteredSchedule}
            subGroups={subGroups}
            slots={slots}
            onCreate={() => {}}
            onMove={() => {}}
            onDelete={() => {}}
            isReadonly={true}
          />
        </>
      )}
    </div>
  );
}
