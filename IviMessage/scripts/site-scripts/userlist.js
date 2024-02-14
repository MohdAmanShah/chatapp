const user = localStorage.getItem('user');
const userName = document.querySelector('.user-name');
if (user) {
    const name = JSON.parse(user).Name;
    userName.innerText = name;

}
else {
    
    console.log(`Data not found.`);
}