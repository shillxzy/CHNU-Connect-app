import React, { useState } from "react";
import defaultAvatar from "../Icons/default-avatar-profile-icon.png";

const API_URL = import.meta.env.VITE_API_BASE_URL;

const Avatar = ({ photoUrl, size = 150, alt = "Фото профілю", className = "", style = {} }) => {
  const [imgError, setImgError] = useState(false);

  const src = !photoUrl || imgError 
    ? defaultAvatar 
    : `${API_URL}${photoUrl}`;

  const combinedStyle = {
    width: size,
    height: size,
    borderRadius: "50%",
    objectFit: "cover",
    ...style,
  };

  return <img src={src} alt={alt} className={className} style={combinedStyle} onError={() => setImgError(true)} />;
};

export default Avatar;
