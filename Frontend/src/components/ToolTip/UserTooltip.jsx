import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import Avatar from '../Avatar/Avatar';
import { getUserById } from '../../api/userAPI';
import './UserTooltip.css';

const UserTooltip = ({ userId, currentUserId, size = 38, fallbackAvatar }) => {
  const [hovered, setHovered] = useState(false);
  const [user, setUser] = useState(null);

  useEffect(() => {
    if (!userId) {
      return;
    }

    const fetchUser = async () => {
      try {
        const res = await getUserById(userId);
        setUser(res.data);
      } catch (err) {
        console.error('Error fetching user for tooltip:', err);
      }
    };

    fetchUser();
  }, [userId]);

  // Лінк на профіль
  const profileLink =
    userId === currentUserId
      ? `/profile/${encodeURIComponent(user?.fullName || '')}` // поточний користувач
      : `/profile/view/${userId}`; // інші користувачі

  return (
    <div
      className="user-tooltip-wrapper"
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <Link to={profileLink}>
        <Avatar
          photoUrl={fallbackAvatar || (user && user.photoUrl)}
          size={size}
        />
      </Link>

      {hovered && user && (
        <div className="user-tooltip-popup">
          <div className="tooltip-avatar">
            <Avatar photoUrl={user.photoUrl} size={50} />
          </div>
          <div className="tooltip-info">
            <strong>{user.fullName}</strong>
            <p>Факультет: {user.faculty}</p>
            <p>Курс: {user.course}</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default UserTooltip;
