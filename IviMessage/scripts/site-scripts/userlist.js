const user = localStorage.getItem('user');
const userName = document.querySelector('.user-name');
const messageelement = document.getElementById("message");
const sendmessagebutton = document.getElementById("sendmessagebutton");
let socket;
if (user) {

    let userObject = JSON.parse(user);
    console.dir(userObject.Id);
    socket = new WebSocket(`wss://localhost:44344/users/ws/${userObject.Id}`);

    // Handle WebSocket events
    socket.onopen = () => {
        console.log('WebSocket connection established');
    };

    socket.onmessage = (event) => {
        console.log(event);
        console.log('Received message:', event.data);
    };

    socket.onclose = () => {
        console.log('WebSocket connection closed');
    };

    socket.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
}
else {
    console.log(`Data not found.`);
}

function sendMessage() {
    let message = messageelement.value;
    console.log(message);
    if (socket.readyState === WebSocket.OPEN) {
        socket.send(message);
        console.log('Message sent to server:', message);
    } else {
        console.error('WebSocket connection is not open. Unable to send message:', message);
    }
}


sendmessagebutton.onclick = sendMessage;