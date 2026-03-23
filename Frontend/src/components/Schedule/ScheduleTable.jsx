import {
  DndContext,
  useDraggable,
  useDroppable
} from "@dnd-kit/core";

import { CSS } from "@dnd-kit/utilities";

function Lesson({ lesson }) {
  const { attributes, listeners, setNodeRef, transform, transition } =
    useDraggable({
      id: lesson.id,
      data: lesson,
    });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...listeners}
      {...attributes}
      className="lesson-card"
    >
      {lesson.subjectName}
    </div>
  );
}

function Cell({ day, pair, children }) {
  const { setNodeRef, isOver } = useDroppable({
    id: `${day}-${pair}`,
  });

  return (
    <div
      ref={setNodeRef}
      className={`day-column ${isOver ? "drop-hover" : ""}`}
    >
      {children}
    </div>
  );
}

export default function ScheduleTable({
  schedule,
  onUpdateLesson
}) {
  const days = [0, 1, 2, 3, 4];

  const handleDragEnd = (event) => {
    const { active, over } = event;

    if (!over) return;

    const [day, pair] = over.id.split("-");

    const lesson = active.data.current;

    onUpdateLesson({
      ...lesson,
      day: Number(day),
      pairNumber: Number(pair),
    });
  };

  return (
    <DndContext onDragEnd={handleDragEnd}>
      <div className="schedule-wrapper">

        {/* HEADER */}
        <div className="schedule-header">
          <div className="corner"></div>

          {["Пн","Вт","Ср","Чт","Пт"].map(d => (
            <div key={d} className="day-cell">{d}</div>
          ))}
        </div>

        {/* BODY */}
        {[1,2,3,4,5].map(pair => (
          <div key={pair} className="schedule-row">

            <div className="pair-cell">{pair}</div>

            {days.map(day => (
              <Cell key={day} day={day} pair={pair}>

                {schedule
                  .filter(l => l.day === day && l.pairNumber === pair)
                  .map(l => (
                    <Lesson key={l.id} lesson={l} />
                  ))}

              </Cell>
            ))}

          </div>
        ))}

      </div>
    </DndContext>
  );
}
