import { useState } from 'react';
import { DndContext, useDraggable, useDroppable } from '@dnd-kit/core';
import './Schedule.css';

const DAYS = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт'];

const WEEKS = [
  { id: 1, label: '1-й тиж.' },
  { id: 2, label: '2-й тиж.' },
];

const SUBGROUPS = [1, 2, 3];

const PAIRS = [
  { number: 1, time: '8:20-9:40' },
  { number: 2, time: '9:50-11:10' },
  { number: 3, time: '11:30-12:50' },
  { number: 4, time: '13:00-14:20' },
  { number: 5, time: '14:40-16:00' },
  { number: 6, time: '16:10-17:30' },
];

/* ================= LESSON ================= */

function Lesson({ lesson, onMenu }) {
  const { setNodeRef, listeners, attributes, transform } = useDraggable({
    id: lesson.id,
    data: lesson,
  });

  const style = transform
    ? { transform: `translate3d(${transform.x}px, ${transform.y}px, 0)` }
    : undefined;

  const typeClass = lesson.type === 0 ? 'lecture' : 'practice';

  return (
    <div
      ref={setNodeRef}
      {...listeners}
      {...attributes}
      style={style}
      onContextMenu={(e) => {
        e.preventDefault();
        onMenu(lesson, e.clientX, e.clientY);
      }}
      className={`chnu-schedule-lesson ${typeClass}`}
    >
      <div className="chnu-schedule-lesson-inner">
        <div className="chnu-schedule-title">{lesson.subjectName}</div>
        <div className="chnu-schedule-meta">👤 {lesson.teacherName}</div>
        <div className="chnu-schedule-meta">📍 {lesson.location}</div>
      </div>
    </div>
  );
}

/* ================= CELL ================= */

function Cell({ id, lesson, onCreate, onMenu, isFull }) {
  const { setNodeRef, isOver } = useDroppable({ id });
  const [hover, setHover] = useState(false);

  return (
    <div
      ref={setNodeRef}
      className={`chnu-schedule-cell ${isOver ? 'is-over' : ''} ${isFull ? 'is-full-width' : ''}`}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
    >
      {lesson && <Lesson lesson={lesson} onMenu={onMenu} />}

      {!lesson && hover && (
        <button className="chnu-schedule-plus" onClick={() => onCreate(id)}>
          +
        </button>
      )}
    </div>
  );
}

/* ================= HELPERS (ГОЛОВНЕ ВИПРАВЛЕННЯ) ================= */

function getLecture(schedule, dayIndex, pairNumber, weekId) {
  return schedule.find((l) => {
    if (l.type !== 0) {
      return false;
    }
    if (l.day !== dayIndex) {
      return false;
    }
    if (l.pairNumber !== pairNumber) {
      return false;
    }

    return l.isEveryWeek || l.week === weekId;
  });
}

function getPractice(schedule, dayIndex, pairNumber, subgroup, weekId) {
  return schedule.find((l) => {
    if (l.type !== 1) {
      return false;
    }
    if (l.day !== dayIndex) {
      return false;
    }
    if (l.pairNumber !== pairNumber) {
      return false;
    }
    if (l.subGroupId !== subgroup) {
      return false;
    }

    return l.isEveryWeek || l.week === weekId;
  });
}

/* ================= MAIN ================= */

export default function ScheduleTable({
  schedule = [],
  onCreate,
  onMove,
  onDelete,
}) {
  const [menu, setMenu] = useState(null);

  const handleDragEnd = ({ active, over }) => {
    if (!over) {
      return;
    }

    const lesson = active.data.current;
    const [day, pair, subgroup, week] = over.id.split('-');

    onMove({
      ...lesson,
      day: Number(day),
      pairNumber: Number(pair),
      subGroupId: subgroup === 'all' ? null : Number(subgroup),
      week: Number(week),
      isEveryWeek: lesson.isEveryWeek,
    });
  };

  return (
    <div className="chnu-schedule-root">
      <DndContext onDragEnd={handleDragEnd}>
        {/* HEADER */}
        <div className="chnu-schedule-header">
          <div className="chnu-schedule-header-corner">
            <div className="col-pair">Пари</div>
            <div className="col-week">Тиждень</div>
          </div>

          <div className="chnu-schedule-header-days">
            {DAYS.map((d) => (
              <div key={d} className="chnu-schedule-header-day">
                <div className="day-name">{d}</div>
                <div className="subgroups-row">
                  {SUBGROUPS.map((sg) => (
                    <div key={sg} className="subgroup-name">
                      {sg}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* BODY */}
        <div className="chnu-schedule-body">
          {PAIRS.map((p) => (
            <div key={p.number} className="chnu-schedule-pair-container">
              <div className="chnu-schedule-time-col">
                <div className="pair-number">{p.number}</div>
                <div className="pair-time">{p.time}</div>
              </div>

              <div className="chnu-schedule-weeks-container">
                {WEEKS.map((w) => (
                  <div key={w.id} className="chnu-schedule-week-row">
                    <div className="chnu-schedule-week-label">{w.label}</div>

                    <div className="chnu-schedule-days-container">
                      {DAYS.map((_, dayIndex) => {
                        const lecture = getLecture(
                          schedule,
                          dayIndex,
                          p.number,
                          w.id,
                        );

                        return (
                          <div
                            key={dayIndex}
                            className="chnu-schedule-day-cells"
                          >
                            {/* ===== LECTURE (1 на день) ===== */}
                            {lecture ? (
                              <Cell
                                id={`${dayIndex}-${p.number}-all-${w.id}`}
                                lesson={lecture}
                                onCreate={onCreate}
                                onMenu={(lesson, x, y) =>
                                  setMenu({ lesson, x, y })
                                }
                                isFull
                              />
                            ) : (
                              /* ===== PRACTICES (3 підгрупи) ===== */
                              SUBGROUPS.map((sg) => {
                                const id = `${dayIndex}-${p.number}-${sg}-${w.id}`;

                                const practice = getPractice(
                                  schedule,
                                  dayIndex,
                                  p.number,
                                  sg,
                                  w.id,
                                );

                                return (
                                  <Cell
                                    key={id}
                                    id={id}
                                    lesson={practice}
                                    onCreate={onCreate}
                                    onMenu={(lesson, x, y) =>
                                      setMenu({ lesson, x, y })
                                    }
                                  />
                                );
                              })
                            )}
                          </div>
                        );
                      })}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>

        {/* CONTEXT MENU */}
        {menu && (
          <div
            className="chnu-schedule-context"
            style={{ top: menu.y, left: menu.x }}
          >
            <div
              onClick={() => {
                onDelete(menu.lesson);
                setMenu(null);
              }}
            >
              Видалити
            </div>
          </div>
        )}
      </DndContext>
    </div>
  );
}
