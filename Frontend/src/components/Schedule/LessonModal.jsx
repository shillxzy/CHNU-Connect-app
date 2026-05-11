import { useState, useEffect } from 'react';
import './LessonModal.css';
import { LessonType } from './scheduleEnums';

export default function LessonModal({ isOpen, onClose, onSave, cell, initialData = null }) {
  const [form, setForm] = useState({
    subjectName: '',
    teacherName: '',
    location:    '',
    type:        LessonType.Lecture,
    isEveryWeek: false,
  });

  useEffect(() => {
    if (isOpen) {
      if (initialData) {
        setForm({
          subjectName: initialData.subjectName ?? '',
          teacherName: initialData.teacherName ?? '',
          location:    initialData.location    ?? '',
          type:        initialData.type        ?? LessonType.Lecture,
          isEveryWeek: initialData.isEveryWeek ?? false,
        });
      } else {
        setForm({
          subjectName: '',
          teacherName: '',
          location:    '',
          type:        LessonType.Lecture,
          isEveryWeek: false,
        });
      }
    }
  }, [isOpen, initialData]);

  if (!isOpen) {return null;}

const handleSubmit = () => {
    if (!form.subjectName.trim()) { alert('Введіть назву дисципліни'); return; }
    if (!form.teacherName.trim()) { alert('Введіть ПІБ викладача');    return; }
    
    console.log('FORM:', form);           // ← додай
    console.log('CELL:', cell);           // ← додай
    console.log('MERGED:', { ...cell, ...form }); // ← додай
    
    onSave({ ...cell, ...form });
    onClose();
};

  const isEdit = !!initialData;

  return (
    <div
      className="modal-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="modal">
        <h3>{isEdit ? 'Редагувати пару' : 'Нова пара'}</h3>

        <input
          placeholder="Назва дисципліни *"
          value={form.subjectName}
          onChange={(e) => setForm({ ...form, subjectName: e.target.value })}
          autoFocus
        />

        <input
          placeholder="ПІБ викладача *"
          value={form.teacherName}
          onChange={(e) => setForm({ ...form, teacherName: e.target.value })}
        />

        <input
          placeholder="Аудиторія"
          value={form.location}
          onChange={(e) => setForm({ ...form, location: e.target.value })}
        />

        <select
          value={form.type}
          onChange={(e) => setForm({ ...form, type: Number(e.target.value) })}
        >
          <option value={LessonType.Lecture}>Лекція (на всю групу)</option>
          <option value={LessonType.Practice}>Практика (для підгрупи)</option>
        </select>

        <label className="modal-checkbox">
          <input
            type="checkbox"
            checked={form.isEveryWeek}
            onChange={(e) => setForm({ ...form, isEveryWeek: e.target.checked })}
          />
          <span>Щотижня (показувати в обох тижнях)</span>
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>{isEdit ? 'Зберегти зміни' : 'Зберегти'}</button>
          <button onClick={onClose}>Скасувати</button>
        </div>
      </div>
    </div>
  );
}