const Form = document.querySelector('[data-form]');

async function loginHandler(e) {
    e.preventDefault();
    const formData = {
        Name: e.target[0].value,
    };
    const res = await fetch('https://65c99ef73b05d29307deaf46.mockapi.io/app/api/users', {
        method: "POST",
        headers: {
            'content-type': 'application/json'
        },
        body: JSON.stringify(formData)
    });
    const user = await res.json();
    localStorage.setItem('user', JSON.stringify(user));
    window.location.href = '/pages/userlist.html';
}

Form.addEventListener('submit', loginHandler)