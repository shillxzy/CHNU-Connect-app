import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import AuthContext from '../context/AuthContext';
import Loading from '../components/Loading/Loading';

export default function AdminRoute({ children }) {
  const { accessToken, role } = useContext(AuthContext);

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }
  if (!role) {
    return <Loading />;
  } // НЕ null
  if (role !== 'admin') {
    return <Navigate to="/" replace />;
  }

  return children;
}
