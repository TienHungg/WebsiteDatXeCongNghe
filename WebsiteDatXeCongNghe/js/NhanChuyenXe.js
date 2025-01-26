// Wait for the HTML document to finish loading before running any JavaScript code
document.addEventListener('DOMContentLoaded', function () {
const chatPopup = document.querySelector('.chat-popup');
const chatBtn = document.querySelector('.fa-comment');
const closeBtn = document.querySelector('.close');

// Show chat popup when clicking the chat button
chatBtn.addEventListener('click', () => {
    chatPopup.style.display = 'block';
});

// Hide chat popup when clicking the close button
closeBtn.addEventListener('click', () => {
    chatPopup.style.display = 'none';
});

// Hide chat popup when clicking outside the chat popup
window.addEventListener('click', (e) => {
    if (e.target == chatPopup) {
        chatPopup.style.display = 'none';
    }
});

// Scroll to the bottom of the chat messages
const chatMessages = document.querySelector('.chat-messages');
chatMessages.scrollTop = chatMessages.scrollHeight;

// Send message when clicking the send button or pressing enter
const chatTextarea = document.querySelector('#chat-textarea');
const sendMessageBtn = document.querySelector('#send-message-btn');

//function sendMessage() {
//    const message = chatTextarea.value.trim();

//    if (message !== '') {
//        const newMessage = `
//      <div class="chat-message right">
//        <div class="chat-message-text">${message}</div>
//      </div>
//    `;
//        chatMessages.innerHTML += newMessage;
//        chatTextarea.value = '';
//        chatMessages.scrollTop = chatMessages.scrollHeight;
//    }
//}

//sendMessageBtn.addEventListener('click', sendMessage);

//chatTextarea.addEventListener('keydown', (e) => {
//    if (e.keyCode === 13) {
//        e.preventDefault();
//        sendMessage();
//    }
//});
});