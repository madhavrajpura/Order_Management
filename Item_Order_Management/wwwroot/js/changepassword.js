// Toggle Current Password

const togglecurrentPassword = document.querySelector('#togglecurrentPassword');
const currentpassword = document.querySelector('#currentPassword');

if (togglecurrentPassword) {
    togglecurrentPassword.addEventListener('click', () => {

        const type = currentpassword
            .getAttribute('type') === 'password' ?
            'text' : 'password';
        currentpassword.setAttribute('type', type);
        togglecurrentPassword.classList.toggle('bi-eye-fill');
        togglecurrentPassword.classList.toggle('bi-eye-slash-fill');
    });
}

// Toggle New Password

const togglenewPassword = document.querySelector('#togglenewPassword');
const newPassword = document.querySelector('#newPassword');

if (togglenewPassword) {
    togglenewPassword.addEventListener('click', () => {

        const type = newPassword
            .getAttribute('type') === 'password' ?
            'text' : 'password';
        newPassword.setAttribute('type', type);
        togglenewPassword.classList.toggle('bi-eye-fill');
        togglenewPassword.classList.toggle('bi-eye-slash-fill');
    });
}

// Toggle New Confirm Password

const togglenewconfirmPassword = document.querySelector('#togglenewconfirmPassword');
const newconfirmPassword = document.querySelector('#confirmPassword');

if (togglenewconfirmPassword) {
    togglenewconfirmPassword.addEventListener('click', () => {

        const type = newconfirmPassword
            .getAttribute('type') === 'password' ?
            'text' : 'password';
            newconfirmPassword.setAttribute('type', type);
        togglenewconfirmPassword.classList.toggle('bi-eye-fill');
        togglenewconfirmPassword.classList.toggle('bi-eye-slash-fill');
    });
}