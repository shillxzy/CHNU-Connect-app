import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import AuthContext from '../context/AuthContext';

export default function ProtectedRoute({ children }) {
  const { isAuthenticated, loading } = useContext(AuthContext);

  if (loading) {
    return null;
  } // або loader

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
