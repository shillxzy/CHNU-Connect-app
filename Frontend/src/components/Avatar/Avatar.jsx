import React from "react";
import defaultAvatar from "../Icons/default-avatar-profile-icon.png";

const Avatar = ({ photoUrl, size = 150, alt = "Фото профілю", className = "", style = {} }) => {
  const BASE_URL = "http://localhost:5000"; 
const src = photoUrl ? `${BASE_URL}${photoUrl}` : defaultAvatar;

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
