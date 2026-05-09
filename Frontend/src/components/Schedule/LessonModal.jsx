import { useState, useEffect } from 'react';
import './LessonModal.css';
import { LessonType } from './scheduleEnums';

export default function LessonModal({ isOpen, onClose, onSave, cell }) {
  const [form, setForm] = useState({
    subjectName: '',
    teacherName: '',
    location:    '',
    type:        LessonType.Lecture,
  });

  useEffect(() => {
    if (isOpen) {
      setForm({
        subjectName: '',
        teacherName: '',
        location:    '',
        type:        LessonType.Lecture,
      });
    }
  }, [isOpen]);

  if (!isOpen) {return null;}

  const handleSubmit = () => {
    if (!form.subjectName.trim()) { alert('Введіть назву дисципліни'); return; }
    if (!form.teacherName.trim()) { alert('Введіть ПІБ викладача');    return; }
    onSave({ ...form, ...cell });
    onClose();
  };

  return (
    <div
      className="modal-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
    >
      <div className="modal">
        <h3>Нова пара</h3>

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

        <div className="modal-actions">
          <button onClick={handleSubmit}>Зберегти</button>
          <button onClick={onClose}>Скасувати</button>
        </div>
      </div>
    </div>
  );
}