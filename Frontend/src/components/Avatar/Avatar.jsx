import React from "react";
import defaultAvatar from "../../Icons/user.png";

const API_URL = import.meta.env.VITE_API_BASE_URL;

const Avatar = ({ photoUrl, size = 150, alt = "Фото профілю", className = "", style = {} }) => {
  const src = photoUrl ? `${API_URL}${photoUrl}` : defaultAvatar;

  const combinedStyle = {
    width: size,
    height: size,
    borderRadius: "50%",
    objectFit: "cover",
    ...style,
  };

  return <img src={src} alt={alt} className={className} style={combinedStyle} />;
};

export default Avatar;
