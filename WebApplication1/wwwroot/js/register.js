const passwordInput = document.getElementById('passwordInput');
const passwordToggle = document.getElementById('passwordToggle');

passwordToggle.addEventListener('click', function () {
    const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordInput.setAttribute('type', type);

    if (type === 'text') {
        passwordToggle.classList.remove('fa-eye-slash');
        passwordToggle.classList.add('fa-eye');
    } else {
        passwordToggle.classList.remove('fa-eye');
        passwordToggle.classList.add('fa-eye-slash');
    }
});

const confirmPasswordInput = document.getElementById('confirmPasswordInput');
const passwordToggle2 = document.getElementById('passwordToggle2');

passwordToggle2.addEventListener('click', function () {
    const type = confirmPasswordInput.getAttribute('type') === 'password' ? 'text' : 'password';
    confirmPasswordInput.setAttribute('type', type);

    if (type === 'text') {
        passwordToggle2.classList.remove('fa-eye-slash');
        passwordToggle2.classList.add('fa-eye');
    } else {
        passwordToggle2.classList.remove('fa-eye');
        passwordToggle2.classList.add('fa-eye-slash');
    }
});