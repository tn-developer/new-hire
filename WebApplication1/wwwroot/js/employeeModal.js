// Function to hide the modal
function hideModal() {
    document.getElementById('myModal').style.display = 'none';
}

// Call hideModal function when the page loads to initially hide the modal
window.onload = function () {
    hideModal();
}

// Function to open the modal
function openModal() {
    document.getElementById('myModal').style.display = 'block';
}

// Function to close the modal
function closeModal() {
    document.getElementById('myModal').style.display = 'none';
}

// Function to validate form inputs
function validateForm() {
    // Add your validation logic here
    return true; // Return true if validation passes, false otherwise
}

// Function to handle form submission
function handleSubmit() {
    if (validateForm()) {
        // If form validation passes, you can submit the form
        // For example, you can use AJAX to submit the form data
        // Here, we're just closing the modal for demonstration purposes
        closeModal();
        alert('Form submitted successfully!');
    } else {
        // If form validation fails, you can display an error message or take other actions
        alert('Form validation failed. Please check your inputs.');
    }
}

// Close the modal when the user clicks outside of it
window.onclick = function (event) {
    var modal = document.getElementById('myModal');
    if (event.target == modal) {
        closeModal();
    }
}
