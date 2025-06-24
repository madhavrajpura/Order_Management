// Toggle New Password

const resetTogglePassword = document.querySelector('#resetTogglePassword');
const resetPassword = document.querySelector('#resetPassword');

if (resetTogglePassword) {
    resetTogglePassword.addEventListener('click', () => {

        const type = resetPassword
            .getAttribute('type') === 'password' ?
            'text' : 'password';
        resetPassword.setAttribute('type', type);
        resetTogglePassword.classList.toggle('bi-eye-fill');
        resetTogglePassword.classList.toggle('bi-eye-slash-fill');
    });
}

// Toggle New Confirm Password

const resetConfirmTogglePassword = document.querySelector('#resetConfirmTogglePassword');
const resetConfirmPassword = document.querySelector('#resetConfirmPassword');

if (resetConfirmTogglePassword) {
    resetConfirmTogglePassword.addEventListener('click', () => {

        const type = resetConfirmPassword
            .getAttribute('type') === 'password' ?
            'text' : 'password';
        resetConfirmPassword.setAttribute('type', type);
        resetConfirmTogglePassword.classList.toggle('bi-eye-fill');
        resetConfirmTogglePassword.classList.toggle('bi-eye-slash-fill');
    });
}