import { useEffect } from "react";
import { useLocation } from "react-router-dom";

export default function RouteLogger() {
  const location = useLocation();

  useEffect(() => {
    console.log("NAVIGATED TO:", location.pathname);
  }, [location]);

  return null;
}
