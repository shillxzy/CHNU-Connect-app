import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';

import ScheduleTable from './ScheduleTable';
import LessonModal from './LessonModal';

import { getScheduleByGroup } from '../../api/scheduleAPI';

export default function ScheduleEditPage() {
  const { id } = useParams();

  const [schedule, setSchedule] = useState([]);
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedCell, setSelectedCell] = useState(null);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    const res = await getScheduleByGroup(id);
    setSchedule(res.data);
  };

  /* ================= CREATE ================= */

  const handleCreateClick = (cellId) => {
    const [day, pair, subgroup, week] = cellId.split('-');

    setSelectedCell({
      day: Number(day),
      pairNumber: Number(pair),
      subGroupId: Number(subgroup),
      week: Number(week),
      groupId: Number(id),
    });

    setModalOpen(true);
  };

  const handleSaveLesson = (lesson) => {
    setSchedule((prev) => [
      ...prev,
      {
        ...lesson,
        id: Date.now(),
      },
    ]);
  };

  /* ================= MOVE ================= */

  const handleMove = (updatedLesson) => {
    setSchedule((prev) =>
      prev.map((l) =>
        l.id === updatedLesson.id ? { ...l, ...updatedLesson } : l,
      ),
    );
  };

  return (
    <div>
      <h2>Редагування розкладу</h2>

      <ScheduleTable
        schedule={schedule}
        onCreate={handleCreateClick}
        onMove={handleMove}
      />

      <LessonModal
        isOpen={modalOpen}
        onClose={() => setModalOpen(false)}
        onSave={handleSaveLesson}
        cell={selectedCell}
      />
    </div>
  );
}
