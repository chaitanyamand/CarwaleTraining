const form = document.getElementById('contactForm');
const successToast = document.getElementById('successToast');

// Email validation regex
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function showError(fieldId, message) {
    const field = document.getElementById(fieldId);
    const errorElement = document.getElementById(fieldId + '-error');

    field.classList.add('error');
    errorElement.textContent = message;
    errorElement.classList.add('show');
}

function clearError(fieldId) {
    const field = document.getElementById(fieldId);
    const errorElement = document.getElementById(fieldId + '-error');

    field.classList.remove('error');
    errorElement.classList.remove('show');
}

function clearRadioError() {
    const radioGroup = document.getElementById('query-type-group');
    const errorElement = document.getElementById('query-type-error');

    radioGroup.classList.remove('error');
    errorElement.classList.remove('show');
}

function showRadioError() {
    const radioGroup = document.getElementById('query-type-group');
    const errorElement = document.getElementById('query-type-error');

    radioGroup.classList.add('error');
    errorElement.classList.add('show');
}

function validateField(field) {
    const fieldId = field.id;
    const value = field.value.trim();

    if (field.hasAttribute('required') && !value) {
        showError(fieldId, 'This field is required');
        return false;
    }

    if (field.type === 'email' && value && !emailRegex.test(value)) {
        showError(fieldId, 'Please enter a valid email address');
        return false;
    }

    clearError(fieldId);
    return true;
}

function validateRadioGroup() {
    const radioButtons = document.querySelectorAll('input[name="query-type"]');
    const isChecked = Array.from(radioButtons).some(radio => radio.checked);

    if (!isChecked) {
        showRadioError();
        return false;
    }

    clearRadioError();
    return true;
}

function validateCheckbox() {
    const checkbox = document.getElementById('consent');
    const errorElement = document.getElementById('consent-error');

    if (!checkbox.checked) {
        checkbox.classList.add('error');
        errorElement.classList.add('show');
        return false;
    }

    checkbox.classList.remove('error');
    errorElement.classList.remove('show');
    return true;
}

function showSuccessToast() {
    successToast.classList.add('show');
    setTimeout(() => {
        successToast.classList.remove('show');
    }, 5000);
}

function resetForm() {
    form.reset();
    // Clear all error states
    const errorElements = document.querySelectorAll('.error-message');
    const fieldElements = document.querySelectorAll('.error');

    errorElements.forEach(el => el.classList.remove('show'));
    fieldElements.forEach(el => el.classList.remove('error'));
}

// Add event listeners for real-time validation
const textInputs = document.querySelectorAll('input[type="text"], input[type="email"], textarea');
textInputs.forEach(input => {
    input.addEventListener('blur', () => validateField(input));
    input.addEventListener('input', () => {
        if (input.classList.contains('error')) {
            validateField(input);
        }
    });
});

const radioButtons = document.querySelectorAll('input[name="query-type"]');
radioButtons.forEach(radio => {
    radio.addEventListener('change', validateRadioGroup);
});

const checkbox = document.getElementById('consent');
checkbox.addEventListener('change', validateCheckbox);

// Form submission
form.addEventListener('submit', (e) => {
    e.preventDefault();

    let isValid = true;

    // Validate all text inputs
    textInputs.forEach(input => {
        if (!validateField(input)) {
            isValid = false;
        }
    });

    // Validate radio group
    if (!validateRadioGroup()) {
        isValid = false;
    }

    // Validate checkbox
    if (!validateCheckbox()) {
        isValid = false;
    }

    if (isValid) {
        showSuccessToast();
        setTimeout(() => {
            resetForm();
        }, 500);
    }
});