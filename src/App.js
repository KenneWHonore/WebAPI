import React, { useState, useEffect } from "react";
import { BrowserRouter as Router, Route, Routes, Navigate, Link } from "react-router-dom";
import axios from "axios"; // Importation d'Axios
import "./styles.css"; // Fichier CSS pour le style global

const API_URL = "https://localhost:7134/api/Auth"; 

function Signup() {
  const [form, setForm] = useState({ username: "", email: "", password: "", role: "client" });
  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${API_URL}/register`, form, {
        headers: { "Content-Type": "application/json" },
      });
      setMessage(response.data.message || "Inscription réussie !");
    } catch (error) {
      console.error("Erreur API :", error);
      setMessage("Erreur lors de l'inscription");
    }
  };

  return (
    <div className="form-container">
      <h2>Inscription</h2>
      <form onSubmit={handleSubmit} className="form">
        <input type="text" name="username" placeholder="Nom d'utilisateur" onChange={handleChange} required />
        <input type="email" name="email" placeholder="Email" onChange={handleChange} required />
        <input type="password" name="password" placeholder="Mot de passe" onChange={handleChange} required />
        <select name="role" onChange={handleChange}>
          <option value="client">Client</option>
          <option value="admin">Admin</option>
        </select>
        <button type="submit" className="btn">S'inscrire</button>
      </form>
      <p>{message}</p>
      <p>J'ai déjà un compte: <Link to="/login">Connexion</Link></p>
    </div>
  );
}

function Login() {
  const [form, setForm] = useState({ username: "", password: "" });
  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${API_URL}/login`, {
        username: form.username,
        password: form.password,
      }, {
        headers: { "Content-Type": "application/json" },
      });

      if (response.data.token) {
        localStorage.setItem("token", response.data.token);

        // Extraire et stocker le rôle dans le localStorage
        const userRole = JSON.parse(atob(response.data.token.split(".")[1])).role;
        localStorage.setItem("role", userRole); // Stocker le rôle

        // Rediriger en fonction du rôle
        window.location.href = userRole === "admin" ? "/admin" : "/client";
      } else {
        setMessage("Identifiants incorrects");
      }
    } catch (error) {
      console.error("Erreur API :", error);
      setMessage("Erreur lors de la connexion");
    }
  };

  return (
    <div className="form-container">
      <h2>Connexion</h2>
      <form onSubmit={handleSubmit} className="form">
        <input type="text" name="username" placeholder="Nom d'utilisateur" onChange={handleChange} required />
        <input type="password" name="password" placeholder="Mot de passe" onChange={handleChange} required />
        <button type="submit" className="btn">Se connecter</button>
      </form>
      <p>{message}</p>
      <p>Je n'ai pas encore de compte: <Link to="/signup">Inscription</Link></p>
    </div>
  );
}

function Welcome() {
  const [role, setRole] = useState(null);

  useEffect(() => {
    // Vérifier si le token et le rôle sont stockés dans le localStorage
    const storedToken = localStorage.getItem("token");
    const storedRole = localStorage.getItem("role");

    // Si pas de token ou de rôle, rediriger vers la page de connexion
    if (!storedToken || !storedRole) {
      window.location.href = "/login";
      return;
    }

    setRole(storedRole);
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role"); // Supprimer aussi le rôle
    window.location.href = "/login";
  };

  if (!role) {
    return null; // Attendre que le rôle soit récupéré
  }

  return (
    <div className="welcome-container">
      <h2>Welcome to your page</h2>
      <button className="btn" onClick={handleLogout}>Déconnexion</button>
    </div>
  );
}

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/signup" element={<Signup />} />
        <Route path="/login" element={<Login />} />
        <Route path="/admin" element={<Welcome />} />
        <Route path="/client" element={<Welcome />} />
        <Route path="*" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;
