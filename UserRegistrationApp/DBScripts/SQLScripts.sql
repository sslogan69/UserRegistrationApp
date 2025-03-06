-- Create Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Create WorkflowStates table
CREATE TABLE WorkflowStates (
    WorkflowStateId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    State NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Insert into Users table
INSERT INTO Users (UserName, Email, PasswordHash, CreatedDate)
VALUES ('testuser', 'testuser@example.com', 'hashedpassword', '2023-10-10 12:00:00');

-- Insert into WorkflowStates table
INSERT INTO WorkflowStates (UserId, State, CreatedDate)
VALUES (1, 'Started', '2023-10-10 12:00:00');

-- Update Users table
UPDATE Users
SET UserName = 'updateduser', Email = 'updateduser@example.com'
WHERE UserId = 1;

-- Update WorkflowStates table
UPDATE WorkflowStates
SET State = 'Completed'
WHERE WorkflowStateId = 1;

-- Delete from Users table
DELETE FROM Users
WHERE UserId = 1;

-- Delete from WorkflowStates table
DELETE FROM WorkflowStates
WHERE WorkflowStateId = 1;