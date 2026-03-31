import React, { useEffect, useState } from 'react';
import defaultAvatar from '../Icons/default-avatar-profile-icon.png';

const API_URL = import.meta.env.VITE_API_BASE_URL;

const Avatar = ({
  photoUrl,
  size = 150,
  alt = 'Фото профілю',
  className = '',
  style = {},
}) => {
  const [imgError, setImgError] = useState(false);

  useEffect(() => {
    setImgError(false);
  }, [photoUrl]);

  const getSource = () => {
    if (!photoUrl || imgError) {
      return defaultAvatar;
    }

    if (photoUrl.startsWith('blob:') || photoUrl.startsWith('http')) {
      return photoUrl;
    }

    return `${API_URL}${photoUrl}`;
  };

  const combinedStyle = {
    width: size,
    height: size,
    borderRadius: '50%',
    objectFit: 'cover',
    ...style,
  };

  return (
    <img
      src={getSource()}
      alt={alt}
      className={className}
      style={combinedStyle}
      onError={() => setImgError(true)}
    />
  );
};

export default Avatar;
