const Form = document.querySelector('[data-form]');
let user = {
    Name: null,
    Id: 0
};


async function loginHandler(e) {
    e.preventDefault();
    user.Name = e.target[0].value;
    const res = await fetch('https://localhost:44344/users/adduser', {
        method: "POST",
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(user)
    });
    user = await res.json();
    console.log(user);
    localStorage.setItem('user', JSON.stringify(user));
    window.location.href = '/pages/userlist.html';
}

Form.addEventListener('submit', loginHandler);