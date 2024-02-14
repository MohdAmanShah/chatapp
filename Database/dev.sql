-- Drop the existing foreign key constraint
ALTER TABLE ivimessage.dbo.Messages
DROP CONSTRAINT FK_ChatSession;

-- Recreate the foreign key constraint with ON DELETE CASCADE option
ALTER TABLE ivimessage.dbo.Messages
ADD CONSTRAINT FK_ChatSession
FOREIGN KEY (ChatSessionID)
REFERENCES ivimessage.dbo.chat (chatid)
ON DELETE CASCADE;
