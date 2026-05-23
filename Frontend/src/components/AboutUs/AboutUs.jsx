import React from 'react';
import './AboutUs.css';

const teamMembers = [
  { name: 'Станіслав Кушнір', role: 'PM', color: '#7c3aed' },
  { name: 'Дмитро Скальський', role: 'Team Lead', color: '#2563eb' },
  { name: 'Габрієль Спелчук', role: 'Full-stack Developer', color: '#059669' },
  { name: 'Ярослав Сапсай', role: 'Business Analytic', color: '#d97706' },
  { name: 'Олексій Дорошенко', role: 'Business Analytic', color: '#dc2626' },
];

const getInitials = (name) => {
  const parts = name.trim().split(' ');
  if (parts.length === 1) {return parts[0].charAt(0).toUpperCase();}
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
};

const AboutUs = () => {
  return (
    <div className="aboutus-container">
      <h1 className="aboutus-title">Про нас</h1>
      <p className="aboutus-description">
        Ми — команда розробників &quot;Prime&quot; і студенти Чернівецького
        національного університету імені Федьковича, які розробляють
        веб-додатки, зручні та сучасні для студентів і університетів. Наша мета
        — полегшити взаємодію користувачів з інформацією та забезпечити комфорт
        і ефективність.
      </p>

      <h2 className="aboutus-subtitle">Наша команда</h2>
      <div className="team-grid">
        {teamMembers.map((member, index) => (
          <div key={index} className="team-card">
            <div
              className="team-avatar"
              style={{ backgroundColor: member.color }}
            >
              {getInitials(member.name)}
            </div>
            <div className="team-info">
              <h3 className="team-name">{member.name}</h3>
              <p className="team-role">{member.role}</p>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default AboutUs;
