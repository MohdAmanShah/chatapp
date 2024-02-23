export class User {
    constructor(Id, Name) {
        this.Id = Id;
        this.Name = Name;
        this.Chats = [];
        this.ChatElements = null;
    }
}

export class Chat {
    constructor({ ChatId, RecipientId, RecipientName }) {
        this.ChatId = ChatId;
        this.RecipientId = RecipientId;
        this.RecipientName = RecipientName;
        this.LastMessage = "";
        this.Messages = [];
        this.ChatElement = null;
    }
}

export class Message {
    constructor({ DestinationId, SourceId, Body, ChatId }) {
        this.DestinationId = DestinationId;
        this.SourceId = SourceId;
        this.Body = Body;
        this.ChatId = ChatId;
    }
}