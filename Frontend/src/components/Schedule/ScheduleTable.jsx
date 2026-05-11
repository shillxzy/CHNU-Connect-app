import { useState } from 'react';
import { DndContext, useDraggable, useDroppable } from '@dnd-kit/core';
import './Schedule.css';

// .NET DayOfWeek: Sunday=0, Monday=1...Friday=5
// Grid dayIndex: 0=Пн..4=Пт → dotNetDay = gridDayIndex + 1

const DAYS = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт'];

const WEEKS = [
  { id: 1, label: '1-й тиждень' },
  { id: 2, label: '2-й тиждень' },
];

/* ================= LESSON CARD ================= */

function Lesson({ lesson, onMenu, isReadonly }) {
  const { setNodeRef, listeners, attributes, transform, isDragging } = useDraggable({
    id: lesson.id,
    data: lesson,
    disabled: isReadonly, // ✅ readonly — drag вимкнено
  });

  const style = transform
    ? { transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`, zIndex: 999, opacity: 0.85 }
    : undefined;

  const typeClass = lesson.type === 0 ? 'lecture' : 'practice';
  const everyWeekBadge = lesson.isEveryWeek;

  return (
    <div
      ref={setNodeRef}
      {...(isReadonly ? {} : listeners)}
      {...(isReadonly ? {} : attributes)}
      style={{ ...style, cursor: isReadonly ? 'default' : 'grab' }}
      onContextMenu={(e) => {
        if (isReadonly) {return;}
        e.preventDefault();
        onMenu(lesson, e.clientX, e.clientY);
      }}
      className={`chnu-schedule-lesson ${typeClass} ${isDragging ? 'dragging' : ''}`}
    >
      <div className="chnu-schedule-lesson-inner">
        <div className="chnu-schedule-title">{lesson.subjectName}</div>
        <div className="chnu-schedule-meta">👤 {lesson.teacherName}</div>
        {lesson.location && (
          <div className="chnu-schedule-meta">📍 {lesson.location}</div>
        )}
        <div className="chnu-schedule-badges">
          <span className="chnu-schedule-badge">
            {lesson.type === 0 ? 'Лекція' : 'Практика'}
          </span>
          {everyWeekBadge && (
            <span className="chnu-schedule-badge every-week">Щотижня</span>
          )}
        </div>
      </div>
    </div>
  );
}

/* ================= CELL ================= */

function Cell({ id, lesson, onCreate, onMenu, isFull, isReadonly }) {
  const { setNodeRef, isOver } = useDroppable({ id, disabled: isReadonly });
  const [hover, setHover] = useState(false);

  return (
    <div
      ref={setNodeRef}
      className={`chnu-schedule-cell ${isOver ? 'is-over' : ''} ${isFull ? 'is-full-width' : ''}`}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
    >
      {lesson ? (
        <Lesson lesson={lesson} onMenu={onMenu} isReadonly={isReadonly} />
      ) : (
        !isReadonly && hover && (
          <button className="chnu-schedule-plus" onClick={() => onCreate(id)}>
            +
          </button>
        )
      )}
    </div>
  );
}

/* ================= HELPERS ================= */

// isEveryWeek — пара показується в обох тижнях
function getLecture(schedule, dayIndex, slotId, weekId) {
  return schedule.find(
    (l) =>
      l.type === 0 &&
      l.day === dayIndex + 1 &&
      l.slotId === slotId &&
      (l.isEveryWeek || l.week === weekId),
  );
}

function getPractice(schedule, dayIndex, slotId, subGroupId, weekId) {
  return schedule.find(
    (l) =>
      l.type === 1 &&
      l.day === dayIndex + 1 &&
      l.slotId === slotId &&
      l.subGroupId === Number(subGroupId) &&
      (l.isEveryWeek || l.week === weekId),
  );
}

const fmt = (t) => (t ? String(t).slice(0, 5) : '');

/* ================= MAIN ================= */

export default function ScheduleTable({
  schedule  = [],
  subGroups = [],
  slots     = [],
  onCreate,
  onMove,
  onDelete,
  onEdit,
  isReadonly = false, // ✅ prop для студентів/викладачів
}) {
  const [menu, setMenu] = useState(null);

  const handleDragEnd = ({ active, over }) => {
    if (!over || isReadonly) {return;}
    const lesson = active.data.current;
    const [dayStr, slotIdStr, subgroup, weekStr] = over.id.split('-');
    onMove({
      ...lesson,
      day:        Number(dayStr),
      slotId:     Number(slotIdStr),
      subGroupId: subgroup === 'all' ? null : Number(subgroup),
      week:       Number(weekStr),
    });
  };

  if (slots.length === 0) {
    return (
      <div style={{ padding: '20px', color: '#6c757d', textAlign: 'center' }}>
        {isReadonly
          ? 'Розклад ще не налаштований.'
          : 'Слоти не завантажені. Натисніть «Автозаповнити 7 пар у БД».'}
      </div>
    );
  }

  return (
    <div className="chnu-schedule-root">
      <DndContext onDragEnd={handleDragEnd}>

        {/* HEADER */}
        <div className="chnu-schedule-header">
          <div className="chnu-schedule-header-corner">
            <div className="col-pair">Пара</div>
            <div className="col-week">Тиждень</div>
          </div>
          <div className="chnu-schedule-header-days">
            {DAYS.map((d) => (
              <div key={d} className="chnu-schedule-header-day">
                <div className="day-name">{d}</div>
                <div className="subgroups-row">
                  {subGroups.length > 0 ? (
                    subGroups.map((sg) => (
                      <div key={sg.id} className="subgroup-name">{sg.name}</div>
                    ))
                  ) : (
                    <div className="subgroup-name">Вся група</div>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* BODY */}
        <div className="chnu-schedule-body">
          {slots.map((slot) => (
            <div key={slot.id} className="chnu-schedule-pair-container">
              <div className="chnu-schedule-time-col">
                <div className="pair-number">{slot.pairNumber}</div>
                <div className="pair-time">{fmt(slot.startTime)}–{fmt(slot.endTime)}</div>
              </div>

              <div className="chnu-schedule-weeks-container">
                {WEEKS.map((w) => (
                  <div key={w.id} className="chnu-schedule-week-row">
                    <div className="chnu-schedule-week-label">{w.label}</div>
                    <div className="chnu-schedule-days-container">
                      {DAYS.map((_, dayIndex) => {
                        const lecture = getLecture(schedule, dayIndex, slot.id, w.id);
                        return (
                          <div key={dayIndex} className="chnu-schedule-day-cells">
                            {lecture ? (
                              <Cell
                                id={`${dayIndex}-${slot.id}-all-${w.id}`}
                                lesson={lecture}
                                onCreate={onCreate}
                                onMenu={(l, x, y) => setMenu({ lesson: l, x, y })}
                                isFull
                                isReadonly={isReadonly}
                              />
                            ) : subGroups.length > 0 ? (
                              subGroups.map((sg) => {
                                const cellId   = `${dayIndex}-${slot.id}-${sg.id}-${w.id}`;
                                const practice = getPractice(schedule, dayIndex, slot.id, sg.id, w.id);
                                return (
                                  <Cell
                                    key={cellId}
                                    id={cellId}
                                    lesson={practice}
                                    onCreate={onCreate}
                                    onMenu={(l, x, y) => setMenu({ lesson: l, x, y })}
                                    isReadonly={isReadonly}
                                  />
                                );
                              })
                            ) : (
                              <Cell
                                id={`${dayIndex}-${slot.id}-all-${w.id}`}
                                lesson={undefined}
                                onCreate={onCreate}
                                onMenu={(l, x, y) => setMenu({ lesson: l, x, y })}
                                isFull
                                isReadonly={isReadonly}
                              />
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

        {/* CONTEXT MENU — тільки для адміна */}
        {menu && !isReadonly && (
          <>
            <div
              style={{ position: 'fixed', inset: 0, zIndex: 9998 }}
              onClick={() => setMenu(null)}
            />
            <div className="chnu-schedule-context" style={{ top: menu.y, left: menu.x }}>
              <div
                onClick={() => {
                  onEdit && onEdit(menu.lesson);
                  setMenu(null);
                }}
              >
                ✏️ Редагувати
              </div>
              <div
                onClick={() => {
                  onDelete(menu.lesson);
                  setMenu(null);
                }}
                style={{ color: '#dc3545' }}
              >
                🗑 Видалити
              </div>
            </div>
          </>
        )}

      </DndContext>
    </div>
  );
}
