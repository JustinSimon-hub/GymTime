-- ========================================
-- GymTime Database Schema
-- Target: Azure Database for MySQL
-- ========================================

-- Create database (run separately if needed)
CREATE DATABASE IF NOT EXISTS gymtime_db;
USE gymtime_db;

-- ========================================
-- Table: Users
-- ========================================
CREATE TABLE IF NOT EXISTS Users (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ========================================
-- Table: Diets
-- ========================================
CREATE TABLE IF NOT EXISTS Diets (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    FoodName VARCHAR(255) NOT NULL,
    Proteins INT NOT NULL DEFAULT 0,
    Fats INT NOT NULL DEFAULT 0,
    Carbohydrates INT NOT NULL DEFAULT 0,
    Calories INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(ID) ON DELETE CASCADE,
    INDEX idx_user_diet (UserId),
    INDEX idx_created (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ========================================
-- Table: Workouts
-- ========================================
CREATE TABLE IF NOT EXISTS Workouts (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    WorkoutName VARCHAR(255) NOT NULL,
    Reps INT NOT NULL DEFAULT 0,
    Sets INT NOT NULL DEFAULT 0,
    PersonalRecord INT NOT NULL DEFAULT 0,
    Description TEXT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (UserId) REFERENCES Users(ID) ON DELETE CASCADE,
    INDEX idx_user_workout (UserId),
    INDEX idx_created (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ========================================
-- Verify Tables Created
-- ========================================
SHOW TABLES;

-- ========================================
-- Sample Data (Optional - for testing)
-- ========================================

-- Insert test user (password: Test123!)
-- INSERT INTO Users (Email, PasswordHash) 
-- VALUES ('test@gymtime.com', '$2a$11$hashedpasswordhere');

-- ========================================
-- Useful Queries for Verification
-- ========================================

-- Check table structures
-- DESCRIBE Users;
-- DESCRIBE Diets;
-- DESCRIBE Workouts;

-- Check row counts
-- SELECT 'Users' as TableName, COUNT(*) as RowCount FROM Users
-- UNION ALL
-- SELECT 'Diets', COUNT(*) FROM Diets
-- UNION ALL
-- SELECT 'Workouts', COUNT(*) FROM Workouts;
