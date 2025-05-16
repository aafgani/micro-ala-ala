-- Switch to the todo database
USE todo;
GO

-- Insert sample ToDoLists
INSERT INTO ToDoLists (Title, Description, UserId)
VALUES 
('Work Projects', 'Tasks related to work initiatives', 'user-123'),
('Personal Goals', 'Things I want to achieve personally', 'user-456');

-- Insert sample Tasks
INSERT INTO Tasks (Title, Notes, IsCompleted, DueDate, ToDoListId)
VALUES 
('Finish presentation', 'Due by Friday', 0, '2025-05-16', 1),
('Plan team outing', NULL, 0, NULL, 1),
('Start running routine', 'Morning runs every other day', 0, '2025-06-01', 2),
('Read a book', 'Start with “Atomic Habits”', 0, NULL, 2);

-- Insert sample Tags
INSERT INTO Tags (Name)
VALUES 
('Urgent'), 
('Low Priority'), 
('Health'), 
('Work'), 
('Reading');

-- Insert TaskTags (many-to-many)
-- Assuming task IDs 1–4 and tag IDs 1–5
INSERT INTO TaskTags (TaskId, TagId)
VALUES
(1, 1),  -- Finish presentation - Urgent
(1, 4),  -- Finish presentation - Work
(2, 2),  -- Plan team outing - Low Priority
(3, 3),  -- Running routine - Health
(4, 5);  -- Read a book - Reading

-- Insert SubTasks (using ParentTaskId)
-- Subtasks under 'Finish presentation' (TaskId = 1)
INSERT INTO Tasks (Title, Notes, IsCompleted, DueDate, ToDoListId, ParentTaskId)
VALUES
('Create slides', 'Use company template', 0, '2025-05-14', 1, 1),
('Review with manager', NULL, 0, '2025-05-15', 1, 1);

-- Subtasks under 'Read a book' (TaskId = 4)
INSERT INTO Tasks (Title, Notes, IsCompleted, DueDate, ToDoListId, ParentTaskId)
VALUES
('Buy book', NULL, 1, NULL, 2, 4),
('Read Chapter 1', NULL, 0, NULL, 2, 4);
