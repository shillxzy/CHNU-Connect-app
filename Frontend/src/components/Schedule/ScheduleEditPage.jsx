import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

import ScheduleTable from './ScheduleTable';
import LessonModal   from './LessonModal';

import {
  getScheduleByGroup, getLessonSlots, seedLessonSlots,
  createSchedule, moveSchedule, deleteSchedule,
  getSubGroups, createSubGroup, updateSubGroup, deleteSubGroup,
} from '../../api/scheduleAPI';
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

export default function ScheduleEditPage() {
  const { id } = useParams();

  const [schedule,  setSchedule]  = useState([]);
  const [group,     setGroup]     = useState(null);
  const [slots,     setSlots]     = useState([]);
  const [subGroups, setSubGroups] = useState([]);

  const [modalOpen,    setModalOpen]    = useState(false);
  const [selectedCell, setSelectedCell] = useState(null);
  const [editLesson,   setEditLesson]   = useState(null);

  const [sgModalOpen,  setSgModalOpen]  = useState(false);
  const [sgEditTarget, setSgEditTarget] = useState(null);
  const [sgName,       setSgName]       = useState('');
  const [seeding,      setSeeding]      = useState(false);
  const [seedMsg,      setSeedMsg]      = useState('');

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

  const handleSeedSlots = async () => {
    setSeeding(true); setSeedMsg('');
    try {
      const res = await seedLessonSlots();
      setSeedMsg(res.data?.message || 'Готово!');
      const slotsRes = await getLessonSlots();
      setSlots(slotsRes.data || []);
    } catch (err) {
      setSeedMsg('Помилка: ' + (err.response?.data || err.message));
    } finally { setSeeding(false); }
  };

  const handleCreateClick = (cellId) => {
    const [day, slotId, subgroup, week] = cellId.split('-');
    setSelectedCell({ groupId: Number(id), day: Number(day), slotId: Number(slotId), subGroupId: subgroup === 'all' ? null : Number(subgroup), week: Number(week) });
    setEditLesson(null);
    setModalOpen(true);
  };

  const handleEditClick = (lesson) => {
    setSelectedCell({ groupId: lesson.groupId, day: lesson.day - 1, slotId: lesson.slotId, subGroupId: lesson.subGroupId, week: lesson.week });
    setEditLesson(lesson);
    setModalOpen(true);
  };

  const handleSaveLesson = async (lesson) => {
  console.log('LESSON отримано:', lesson);
  console.log('isEveryWeek:', lesson.isEveryWeek);

  try {
    const dotNetDay = Number(lesson.day) + 1;

    const payload = {
      groupId: Number(id),
      subGroupId: lesson.subGroupId ?? null,
      day: dotNetDay,
      week: Number(lesson.week),
      type: Number(lesson.type),
      slotId: Number(lesson.slotId),
      subjectName: lesson.subjectName ?? '',
      teacherName: lesson.teacherName ?? '',
      location: lesson.location ?? '',
      isEveryWeek: lesson.isEveryWeek ?? false,
    };

    console.log('PAYLOAD:', payload);

    if (editLesson) {
      // EDIT
      await deleteSchedule(editLesson.id);

      const res = await createSchedule(payload);

      console.log('CREATE RESPONSE:', res.data);

      const [norm] = normalizeSchedule([res.data]);

      setSchedule((prev) => [
        ...prev.filter((l) => l.id !== editLesson.id),
        norm,
      ]);
    } else {
      // CREATE
      const res = await createSchedule(payload);

      console.log('CREATE RESPONSE:', res.data);

      const [norm] = normalizeSchedule([res.data]);

      setSchedule((prev) => [...prev, norm]);
    }

    setModalOpen(false);
    setEditLesson(null);
  } catch (err) {
    console.error('SAVE ERROR:', err);

    alert(
      'Помилка: ' +
        (err.response?.data?.message ||
          err.response?.data ||
          err.message),
    );
  }
};

  const handleMove = async (lesson) => {
    try {
      const dotNetDay = Number(lesson.day) + 1;
      const res = await moveSchedule({ scheduleId: lesson.id, newDay: dotNetDay, newSlotId: Number(lesson.slotId), newWeek: Number(lesson.week) });
      if (!res.data) {return;}
      setSchedule((prev) => prev.map((l) => l.id === lesson.id ? { ...l, day: dotNetDay, slotId: lesson.slotId, week: lesson.week } : l));
    } catch (err) { console.error('MOVE ERROR:', err); }
  };

  const handleDelete = async (lesson) => {
    try {
      await deleteSchedule(lesson.id);
      setSchedule((prev) => prev.filter((l) => l.id !== lesson.id));
    } catch (err) { console.error('DELETE ERROR:', err); }
  };

  const openSgCreate = () => { setSgEditTarget(null); setSgName(''); setSgModalOpen(true); };
  const openSgEdit   = (sg) => { setSgEditTarget(sg); setSgName(sg.name); setSgModalOpen(true); };

  const handleSgSave = async () => {
    if (!sgName.trim()) { alert('Введіть назву підгрупи'); return; }
    try {
      if (sgEditTarget) {
        const res = await updateSubGroup(sgEditTarget.id, { name: sgName });
        setSubGroups((prev) => prev.map((sg) => sg.id === sgEditTarget.id ? res.data : sg));
      } else {
        const res = await createSubGroup({ groupId: Number(id), name: sgName });
        setSubGroups((prev) => [...prev, res.data]);
      }
      setSgModalOpen(false);
    } catch (err) { alert('Помилка: ' + (err.response?.data || err.message)); }
  };

  const handleSgDelete = async (sg) => {
    if (!confirm('Видалити підгрупу "' + sg.name + '"?')) {return;}
    try {
      await deleteSubGroup(sg.id);
      setSubGroups((prev) => prev.filter((s) => s.id !== sg.id));
    } catch (err) { alert('Помилка: ' + (err.response?.data || err.message)); }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h2>Редагування розкладу — {group?.name}</h2>

      <div style={{ display: 'flex', gap: '12px', marginBottom: '16px', flexWrap: 'wrap', alignItems: 'center' }}>
        <button onClick={handleSeedSlots} disabled={seeding || slots.length >= 7}
          style={{ padding: '8px 16px', background: slots.length >= 7 ? '#6c757d' : '#007bff', color: 'white', border: 'none', borderRadius: '8px', cursor: slots.length >= 7 ? 'default' : 'pointer', fontWeight: 600 }}>
          {seeding ? '⏳ Додаємо...' : slots.length >= 7 ? '✅ 7 пар вже є в БД' : '⚡ Автозаповнити 7 пар у БД'}
        </button>
        {seedMsg && <span style={{ color: '#28a745', fontWeight: 500 }}>{seedMsg}</span>}
        <button onClick={openSgCreate}
          style={{ padding: '8px 16px', background: '#28a745', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 600 }}>
          + Додати підгрупу
        </button>
      </div>

      {subGroups.length > 0 && (
        <div style={{ marginBottom: '16px', display: 'flex', gap: '8px', flexWrap: 'wrap' }}>
          {subGroups.map((sg) => (
            <div key={sg.id} style={{ display: 'flex', alignItems: 'center', gap: '6px', background: '#e9ecef', borderRadius: '20px', padding: '4px 12px', fontSize: '14px' }}>
              <span>{sg.name}</span>
              <button onClick={() => openSgEdit(sg)} style={{ background: 'none', border: 'none', cursor: 'pointer' }}>✏️</button>
              <button onClick={() => handleSgDelete(sg)} style={{ background: 'none', border: 'none', cursor: 'pointer' }}>🗑️</button>
            </div>
          ))}
        </div>
      )}

      {slots.length === 0 && (
        <div style={{ padding: '12px 16px', background: '#fff3cd', border: '1px solid #ffc107', borderRadius: '8px', marginBottom: '16px', color: '#856404' }}>
          ⚠️ У БД немає пар. Натисніть <strong>«Автозаповнити 7 пар у БД»</strong>.
        </div>
      )}

      <ScheduleTable
        schedule={schedule} subGroups={subGroups} slots={slots}
        onCreate={handleCreateClick} onMove={handleMove}
        onDelete={handleDelete} onEdit={handleEditClick}
        isReadonly={false}
      />

      <LessonModal
        isOpen={modalOpen}
        onClose={() => { setModalOpen(false); setEditLesson(null); }}
        onSave={handleSaveLesson}
        cell={selectedCell}
        initialData={editLesson}
      />

      {sgModalOpen && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.4)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}
          onClick={(e) => e.target === e.currentTarget && setSgModalOpen(false)}>
          <div style={{ background: 'white', borderRadius: '12px', padding: '24px', width: '340px', display: 'flex', flexDirection: 'column', gap: '12px', boxShadow: '0 10px 30px rgba(0,0,0,0.2)' }}>
            <h3 style={{ margin: 0 }}>{sgEditTarget ? 'Редагувати підгрупу' : 'Нова підгрупа'}</h3>
            <input placeholder="Назва (напр. 344(1))" value={sgName} onChange={(e) => setSgName(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleSgSave()} autoFocus
              style={{ padding: '10px', borderRadius: '8px', border: '1px solid #ddd', fontSize: '14px' }} />
            <div style={{ display: 'flex', gap: '8px' }}>
              <button onClick={handleSgSave} style={{ flex: 1, padding: '10px', background: '#28a745', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: 600 }}>Зберегти</button>
              <button onClick={() => setSgModalOpen(false)} style={{ flex: 1, padding: '10px', background: '#dee2e6', border: 'none', borderRadius: '8px', cursor: 'pointer' }}>Скасувати</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
