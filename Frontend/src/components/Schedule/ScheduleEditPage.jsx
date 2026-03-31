import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';

import { getScheduleByGroup, createSchedule } from '../../api/scheduleAPI';

import ScheduleTable from './ScheduleTable';

export default function ScheduleEditPage() {
  const { id } = useParams();

  const [schedule, setSchedule] = useState([]);

  useEffect(() => {
    load();
  }, []);

  const load = async () => {
    const res = await getScheduleByGroup(id);
    setSchedule(res.data);
  };

  const handleCreate = async (lesson) => {
    await createSchedule({
      ...lesson,
      groupId: id,
    });

    load();
  };

  return (
    <div>
      <h2>Редагування розкладу</h2>

      <ScheduleTable schedule={schedule} onUpdateLesson={handleCreate} />
    </div>
  );
}
