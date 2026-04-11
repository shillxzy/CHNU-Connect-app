import { useState, useEffect } from 'react';
import './LessonModal.css';

export default function LessonModal({ isOpen, onClose, onSave, cell }) {
  const [form, setForm] = useState({
    subjectName: '',
    teacherName: '',
    location: '',
    type: 0, // 0 = lecture, 1 = practice
    isEveryWeek: false, // <--- НОВЕ ПОЛЕ
  });

  // Очищаємо форму при кожному відкритті модалки
  useEffect(() => {
    if (isOpen) {
      setForm({
        subjectName: '',
        teacherName: '',
        location: '',
        type: 0,
        isEveryWeek: false,
      });
    }
  }, [isOpen]);

  if (!isOpen) {
    return null;
  }

  const handleSubmit = () => {
    onSave({
      ...form,
      ...cell,
    });
    onClose();
  };

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h3>Нова пара</h3>

        <input
          placeholder="Назва дисципліни"
          value={form.subjectName}
          onChange={(e) => setForm({ ...form, subjectName: e.target.value })}
        />

        <input
          placeholder="ПІБ викладача"
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
          <option value={0}>Лекція (на всю групу)</option>
          <option value={1}>Практика (для підгрупи)</option>
        </select>

        {/* НОВИЙ ЧЕКБОКС */}
        <label className="modal-checkbox">
          <input
            type="checkbox"
            checked={form.isEveryWeek}
            onChange={(e) =>
              setForm({ ...form, isEveryWeek: e.target.checked })
            }
          />
          <span>Кожного тижня (ігнорувати чисельник/знаменник)</span>
        </label>

        <div className="modal-actions">
          <button onClick={handleSubmit}>Зберегти</button>
          <button onClick={onClose}>Скасувати</button>
        </div>
      </div>
    </div>
  );
}
