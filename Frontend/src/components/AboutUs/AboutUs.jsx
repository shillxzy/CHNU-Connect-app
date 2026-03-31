import React from 'react';
import './AboutUs.css';

const teamMembers = [
  { name: '###', role: '###' },
  { name: '###', role: '###' },
  { name: '###', role: '###' },
  { name: '###', role: '###' },
];

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
            <div className="team-avatar">{member.name.charAt(0)}</div>
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
