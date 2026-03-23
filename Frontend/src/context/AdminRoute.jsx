import { useContext } from "react";
import { Navigate } from "react-router-dom";
import AuthContext from "../context/AuthContext";

export default function AdminRoute({ children }) {
  const { accessToken, role } = useContext(AuthContext);

  if (!accessToken) return <Navigate to="/login" replace />;
  if (!role) return <div>Loading...</div>; // НЕ null
  if (role !== "admin") return <Navigate to="/" replace />;

  return children;
}


