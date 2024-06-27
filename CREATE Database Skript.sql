-- Initialisieren der Datenbank
DROP DATABASE IF EXISTS localAuthREST_API;
CREATE DATABASE IF NOT EXISTS localAuthREST_API;

USE localAuthREST_API;

-- Initialisieren der Tabelle Users
CREATE TABLE IF NOT EXISTS users (
	User_ID INT PRIMARY KEY,
    Vorname VARCHAR(255),
    Nachname VARCHAR(255),
    Email VARCHAR(255),
    Benutzername VARCHAR(255),
    Passwort VARCHAR(255)
);

-- Initialisieren eines Testbenuters
INSERT INTO users VALUES (1, "Test", "Admin", "test@admin.de", "t.Admin123", "MISSING");