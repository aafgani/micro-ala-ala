-- Create the database
IF DB_ID('todo') IS NULL
    CREATE DATABASE todo;
GO

-- Switch to the new database
USE todo;
GO

-- Create ToDoLists table
CREATE TABLE ToDoLists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UserId NVARCHAR(255) NOT NULL
);
GO

-- Create Tasks table
CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    IsCompleted BIT NOT NULL DEFAULT 0,
    DueDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ToDoListId INT NOT NULL,
    FOREIGN KEY (ToDoListId) REFERENCES ToDoLists(Id) ON DELETE CASCADE
);
GO

-- Optional: Indexes
CREATE INDEX IX_Tasks_ToDoListId ON Tasks(ToDoListId);
CREATE INDEX IX_ToDoLists_UserId ON ToDoLists(UserId);
GO
