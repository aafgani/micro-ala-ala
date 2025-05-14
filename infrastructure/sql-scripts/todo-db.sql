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
    UserId NVARCHAR(255) NOT NULL  -- foreign identifier, no FK constraint
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
    ParentTaskId INT NULL, -- for subtasks (self-referencing)
    FOREIGN KEY (ToDoListId) REFERENCES ToDoLists(Id) ON DELETE CASCADE,
    FOREIGN KEY (ParentTaskId) REFERENCES Tasks(Id) ON DELETE NO ACTION
);
GO

-- Create Tags table
CREATE TABLE Tags (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Many-to-many relationship between Tasks and Tags
CREATE TABLE TaskTags (
    TaskId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (TaskId, TagId),
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
);
GO

