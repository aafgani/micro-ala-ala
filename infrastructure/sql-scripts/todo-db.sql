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
