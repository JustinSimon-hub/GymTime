  -- ========================================
-- Export Your Local Data First
-- ========================================

-- Run this in your local machine terminal:
-- mysqldump -u gymuser -p gymtime_db > backup_data.sql

-- ========================================
-- Then Import to Azure
-- ========================================

-- 1. Connect to Azure MySQL:
--    mysql -h gymtime-mysql-server.mysql.database.azure.com -u gymadmin -p --ssl-mode=REQUIRED

-- 2. Create database:
--    CREATE DATABASE gymtime_db;
--    USE gymtime_db;

-- 3. Import schema:
--    source schema.sql;

-- 4. Import your data:
--    source backup_data.sql;

-- ========================================
-- Quick Data Verification Queries
-- ========================================

USE gymtime_db;

-- Check all tables exist
SHOW TABLES;

-- Verify Users
SELECT COUNT(*) as TotalUsers FROM Users;
SELECT * FROM Users LIMIT 5;

-- Verify Diets
SELECT COUNT(*) as TotalDiets FROM Diets;
SELECT * FROM Diets LIMIT 5;

-- Verify Workouts
SELECT COUNT(*) as TotalWorkouts FROM Workouts;
SELECT * FROM Workouts LIMIT 5;

-- Check foreign key relationships
SELECT 
    u.ID as UserId,
    u.Email,
    COUNT(DISTINCT d.Id) as DietCount,
    COUNT(DISTINCT w.Id) as WorkoutCount
FROM Users u
LEFT JOIN Diets d ON u.ID = d.UserId
LEFT JOIN Workouts w ON u.ID = w.UserId
GROUP BY u.ID, u.Email;
