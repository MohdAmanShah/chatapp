export function HomePage() {
    let v = null;
    return `
    <div class="container home">
        <h1>Welcome!</h1>
        <form action="#" method="get" data-form>
            <input required placeholder="Name" type="text" name="user-name" id="user-name" data-user-name>
            <button type="submit" data-submit-button>Login</button>
        </form>
    </div>
    `;
}

export function ListPage(user) {
    return `
    <div class="container list">
        <div class="user-name" data-user>${user.Name}</div>
        <div class="user-list" data-active-users>
        </div>
    </div>
    <button type="button" class="find-new-chat-button" data-find-new-chat-button>New Chat</button>
    `;
}

export function ChatElement(chat) {
    const element = document.createElement('a');
    element.classList.add('user');
    element.innerHTML = ` 
    <h2 class="name">${chat.RecipientName}</h2>
    <div class="last-message">${chat.LastMessage}</div>`;
    return element;
}


export function ChatPage(chat) {
    return `  <div class="container chat">
    <header>
        <div class="invited-user-name" data-user=${chat.RecipientId}>${chat.RecipientName}</div>
        <button type="button" data-close-chat-button>Close Chat</button>
    </header>
    <div class="chats" data-chats data-chatId=${chat.ChatId}>
    </div>
    <form action="#" data-message-form>
        <div class="input-group">
            <input type="text" name="message" id="message" placeholder="How are you?">
            <button type="submit">Send</button>
        </div>
    </form>
</div>`;
}

export function MessageElement(Message) {
    const m = document.createElement('div');
    m.innerHTML = `<p>${Message.Body}</p>`;
    m.classList.add("chat-element");
    setTimeout(() => { // Add a slight delay to ensure the element is fully rendered
        m.scrollIntoView({
            behavior: "smooth",
            block: "end",
        });
    }, 100);
    return m;
}



export function NotFoundPage() {
    return `
        <h1 style="color=red;">Page Not Found </h1>
    `
}
