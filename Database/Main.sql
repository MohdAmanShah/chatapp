IF NOT EXISTS ( SELECT name FROM sys.databases WHERE name = 'ivimessage')
BEGIN
	CREATE DATABASE ivimessage;
END

CREATE TABLE ivimessage.dbo.users(
	Id INT PRIMARY KEY,
	Name nvarchar(100)	
);

CREATE TABLE ivimessage.dbo.chat(
	chatid INT PRIMARY KEY,
	creatorId INT NOT NULL,
	inviteduserId INT NOT NULL,
	CONSTRAINT Fk_creator FOREIGN KEY (creatorId) REFERENCES ivimessage.dbo.users (Id),
	CONSTRAINT Fk_inviteduser FOREIGN KEY (inviteduserId) REFERENCES ivimessage.dbo.users (Id)
	);

	CREATE TABLE ivimessage.dbo.Messages (
    MessageID INT PRIMARY KEY,
    Content NVARCHAR(MAX),
    SenderID INT NOT NULL,
    ReceiverID INT NOT NULL,
    Timestamp DATETIME,
    ChatSessionID INT,
    CONSTRAINT FK_Sender FOREIGN KEY (SenderID) REFERENCES dbo.users (Id),
    CONSTRAINT FK_Receiver FOREIGN KEY (ReceiverID) REFERENCES dbo.users (Id),
    CONSTRAINT FK_ChatSession FOREIGN KEY (ChatSessionID) REFERENCES dbo.chat (chatid)
);
