import { User, Chat, Message } from './datamodels.js';
import { HomePage, ChatPage, ListPage, NotFoundPage, ChatElement, MessageElement } from './components.js';

const ServerUrl = `https://localhost:44344`;
const SocketUrl = `wss://${new URL(ServerUrl).host}`;
const user = new User();
let socket = null;
const app = document.getElementById("app");

window.addEventListener('popstate', (event) => {
    const route = event.state ? event.state.route : '/home';
    switch (route) {
        case "/":
            {
                location.reload();
                break;
            }

        case "/user":
            {
                loadUserList();
                break;
            }

        case "/chat": {
            for (let i = 0; i < user.Chats.length; ++i) {
                if (event.state.chat == user.Chats[i].ChatId) {
                    loadChat(user.Chats[i]);
                    return;
                }
            }

            break;
        }
        default: break;
    }
});


window.history.pushState({ route: '/' }, '', '/');
function loadhome() {
    app.innerHTML = '';
    const loginform = new DOMParser().parseFromString(HomePage(), 'text/html').body.firstChild;
    app.appendChild(loginform);
    LoginFormHandler(loginform);
} loadhome();


function LoginFormHandler(loginform) {
    const Form = document.querySelector('[data-form]');
    async function loginHandler(e) {
        e.preventDefault();
        let u = {
            Name: e.target[0].value
        };
        const res = await fetch(`${ServerUrl}/user/add`, {
            method: "POST",
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify(u)
        });
        u = await res.json();
        user.Id = u.Id;
        user.Name = u.Name;
        window.history.pushState({ route: '/user' }, '', '/user');
        makewebsocketconnection();
        loadUserList();
    }
    Form.addEventListener('submit', loginHandler);
}


function loadUserList() {
    app.innerHTML = '';
    const listpage = new DOMParser().parseFromString(ListPage(user), 'text/html').body.children;
    app.append(...listpage);
    user.ChatElements = document.querySelector('[data-active-users]');
    AddChatsToList(user);
    HandlerNewChatButton(user.ChatElements, document.querySelector('[data-find-new-chat-button]'));
}

function AddChatsToList(user) {
    user.Chats.forEach(chat => {
        user.ChatElements.appendChild(chat.ChatElement);
    })
}

function HandlerNewChatButton(userlist, newchatbutton) {
    newchatbutton.addEventListener('click', async e => {
        const res = await fetch(`${ServerUrl}/user/getuser/${user.Id}`);
        const resChat = await res.json();
        if (resChat.ChatId != 0 && resChat.user.Id != 0 && resChat.user.Name != "Null") {
            let chat = new Chat({
                ChatId: resChat.ChatId,
                RecipientId: resChat.user.Id,
                RecipientName: resChat.user.Name
            });
            user.Chats.push(chat);
            const element = ChatElement(chat);
            userlist.insertBefore(element, userlist.firstChild);
            chat.ChatElement = element;
            element.addEventListener('click', e => {
                e.preventDefault();
                window.history.pushState({ route: `/chat`, chat: chat.ChatId }, '', `/chat`);
                loadChat(chat);
            })
        }
        else {
            const errorMessage = document.createElement('div');
            errorMessage.innerHTML = `
            <p>No more users active</p>`;
            errorMessage.classList.add('error-message');
            errorMessage.classList.add('no-user-online');
            document.body.appendChild(errorMessage);
            setTimeout(() => {
                document.body.removeChild(errorMessage);
            }, 3000);
        }
    })
}



function loadChat(chat) {
    app.innerHTML = '';
    const messages = new DOMParser().parseFromString(ChatPage(chat), 'text/html').body.children;
    app.append(...messages);

    const closeChatbutton = document.querySelector('[data-close-chat-button]');

    closeChatbutton.addEventListener('click', async e => {
        user.ChatElements.remove(chat.ChatElement);
        const res = await fetch(`${ServerUrl}/user/deleteChat/${chat.ChatId}?RecipientId=${chat.RecipientId}&InitiatorId=${user.Id}`, {
            method: "DELETE"
        });
        for (let i = 0; i < user.Chats.length; ++i) {
            if (chat.ChatId == user.Chats[i].ChatId) {
                user.Chats.splice(i, 1);
            }
        }
        history.back();
        loadUserList();
    })

    const messageList = document.querySelector('[data-chats]');
    LoadMessages(chat, messageList);

    const messageform = document.querySelector("[data-message-form]");
    messageform.addEventListener('submit', async e => {
        e.preventDefault();
        const mes = new Message({
            DestinationId: chat.RecipientId,
            SourceId: user.Id,
            Body: e.target[0].value,
            ChatId: chat.ChatId
        });
        chat.Messages.push(mes);
        await sendMessage(chat, mes);


        const me = MessageElement(mes);
        me.classList.add("sender");
        messageList.appendChild(me);
        chat.LastMessage = mes.Body;
        e.target.reset();
    });
}

function LoadMessages(chat, messageList) {
    chat.Messages.forEach(message => {
        const me = MessageElement(message);
        if (message.SourceId == user.Id) {
            me.classList.add("sender");
        }
        else {
            me.classList.add("reciever");
        }
        messageList.appendChild(me);
    })
}

function HandlerChatMessage(payload) {

    for (let i = 0; i < user.Chats.length; ++i) {
        if (user.Chats[i].ChatId == payload.ChatId) {
            const mes = new Message({
                DestinationId: payload.DestinationId,
                SourceId: payload.SourceId,
                Body: payload.Message,
                ChatId: payload.ChatId
            });
            user.Chats[i].Messages.push(mes);
            user.Chats[i].LastMessage = `${payload.Message.substring(0, 20)}`;


            let chatPage = document.querySelector('[data-chats]');
            if (chatPage) {
                let cid = chatPage.getAttribute('data-chatId');
                if (cid == user.Chats[i].ChatId) {
                    const me = MessageElement(mes);
                    me.classList.add("reciever");
                    document.querySelector('[data-chats]').appendChild(me);
                }
            }
            else {
                user.Chats[i].ChatElement.querySelector('.last-message').innerHTML = user.Chats[i].LastMessage;
            }
            return '';
        }
    }
    const chat = new Chat({
        ChatId: payload.ChatId,
        RecipientId: payload.SourceId,
        RecipientName: payload.SourceName
    });
    const mes = new Message({
        DestinationId: payload.DestinationId,
        SourceId: payload.SourceId,
        Body: payload.Message,
        ChatId: payload.ChatId
    });
    chat.Messages.push(mes);
    chat.LastMessage = payload.Message.substring(0, 20);
    chat.ChatElement = ChatElement(chat);
    chat.ChatElement.addEventListener('click', e => {
        e.preventDefault();
        window.history.pushState({ route: `/chat`, chat: chat.ChatId }, '', `/chat`);
        loadChat(chat);
    })
    user.ChatElements.insertBefore(chat.ChatElement, user.ChatElements.firstChild);
    user.Chats.push(chat);
}


function makewebsocketconnection() {
    socket = new WebSocket(`${SocketUrl}/user/ws/${user.Id}`);

    socket.onopen = () => {
        console.log('WebSocket connection established');
    };

    socket.onmessage = (event) => {
        const payload = JSON.parse(event.data);
        if (payload.close) {
            Closechats(payload.close);
        }
        else {
            HandlerChatMessage(payload);
        }

    };

    socket.onclose = () => {
        console.log('WebSocket connection closed');
    };

    socket.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
}


async function sendMessage(chat, mes) {
    const payload = {
        SourceId: user.Id,
        SourceName: user.Name,
        DestinationId: chat.RecipientId,
        ChatId: chat.ChatId,
        Message: mes.Body
    };
    const messageJson = JSON.stringify(payload)
    if (socket && socket.readyState === WebSocket.OPEN) {
        socket.send(messageJson);
    }
}

function Closechats(Id) {
    for (let i = 0; i < user.Chats.length; ++i) {
        if (user.Chats[i].RecipientId == Id) {
            user.Chats.splice(i, 1);
            if (history.state.route = '/chat') {
                history.back();
                return loadUserList();
            }
        }
    }
}