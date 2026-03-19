import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthProvider";  
import RouteLogger from "./context/RouteLogger";  

ReactDOM.createRoot(document.getElementById("root")).render(
  <BrowserRouter>
    <AuthProvider>
    <RouteLogger />
    <App />
  </AuthProvider>
  </BrowserRouter>
);
