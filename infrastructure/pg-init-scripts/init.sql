-- Create ToDoLists table
CREATE TABLE "ToDoLists" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "UserId" VARCHAR(255) NOT NULL  -- foreign identifier, no FK constraint
);

-- Create Tasks table
CREATE TABLE "Tasks" (
    "Id" SERIAL PRIMARY KEY,
    "Title" VARCHAR(255) NOT NULL,
    "Notes" TEXT,
    "IsCompleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DueDate" TIMESTAMPTZ,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "ToDoListId" INT NOT NULL,
    "ParentTaskId" INT,
    FOREIGN KEY ("ToDoListId") REFERENCES "ToDoLists"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("ParentTaskId") REFERENCES "Tasks"("Id") ON DELETE NO ACTION
);

-- Create Tags table
CREATE TABLE "Tags" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

-- Many-to-many relationship between Tasks and Tags
CREATE TABLE "TaskTags" (
    "TaskId" INT NOT NULL,
    "TagId" INT NOT NULL,
    PRIMARY KEY ("TaskId", "TagId"),
    FOREIGN KEY ("TaskId") REFERENCES "Tasks"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("TagId") REFERENCES "Tags"("Id") ON DELETE CASCADE
);

-- Insert sample ToDoLists
INSERT INTO "ToDoLists" ("Title", "Description", "UserId")
VALUES 
('Work Projects', 'Tasks related to work initiatives', 'user-123'),
('Personal Goals', 'Things I want to achieve personally', 'user-456');

-- Insert sample Tasks
INSERT INTO "Tasks" ("Title", "Notes", "IsCompleted", "DueDate", "ToDoListId")
VALUES 
('Finish presentation', 'Due by Friday', FALSE, '2025-05-16', 1),
('Plan team outing', NULL, FALSE, NULL, 1),
('Start running routine', 'Morning runs every other day', FALSE, '2025-06-01', 2),
('Read a book', 'Start with “Atomic Habits”', FALSE, NULL, 2);

-- Insert sample Tags
INSERT INTO "Tags" ("Name")
VALUES 
('Urgent'), 
('Low Priority'), 
('Health'), 
('Work'), 
('Reading');

-- Insert TaskTags (many-to-many)
INSERT INTO "TaskTags" ("TaskId", "TagId")
VALUES
(1, 1),  -- Finish presentation - Urgent
(1, 4),  -- Finish presentation - Work
(2, 2),  -- Plan team outing - Low Priority
(3, 3),  -- Running routine - Health
(4, 5);  -- Read a book - Reading

-- Insert SubTasks (using ParentTaskId)
INSERT INTO "Tasks" ("Title", "Notes", "IsCompleted", "DueDate", "ToDoListId", "ParentTaskId")
VALUES
('Create slides', 'Use company template', FALSE, '2025-05-14', 1, 1),
('Review with manager', NULL, FALSE, '2025-05-15', 1, 1);

INSERT INTO "Tasks" ("Title", "Notes", "IsCompleted", "DueDate", "ToDoListId", "ParentTaskId")
VALUES
('Buy book', NULL, TRUE, NULL, 2, 4),
('Read Chapter 1', NULL, FALSE, NULL, 2, 4);
