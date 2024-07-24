// document.addEventListener("DOMContentLoaded", function () {
//   const passwordInput = document.getElementById("passwordInput");
//   const passwordToggle = document.getElementById("passwordToggle");
//   const loginButton = document.getElementById("loginButton");
//   const usernameInput = document.getElementById("usernameInput");

//   // Toggle password visibility
//   passwordToggle.addEventListener("click", function () {
//     const type =
//       passwordInput.getAttribute("type") === "password" ? "text" : "password";
//     passwordInput.setAttribute("type", type);

//     if (type === "text") {
//       passwordToggle.classList.remove("fa-eye-slash");
//       passwordToggle.classList.add("fa-eye");
//     } else {
//       passwordToggle.classList.remove("fa-eye");
//       passwordToggle.classList.add("fa-eye-slash");
//     }
//   });
// });

document.addEventListener("DOMContentLoaded", function () {
  const passwordInput = document.getElementById("passwordInput");
  const passwordToggle = document.getElementById("passwordToggle");

  // Toggle password visibility
  passwordToggle.addEventListener("click", function () {
    const type = passwordInput.getAttribute("type") === "password" ? "text" : "password";
    passwordInput.setAttribute("type", type);

    if (type === "text") {
      passwordToggle.classList.remove("fa-eye-slash");
      passwordToggle.classList.add("fa-eye");
    } else {
      passwordToggle.classList.remove("fa-eye");
      passwordToggle.classList.add("fa-eye-slash");
    }
  });

  // Show or hide the eye icon based on password input value
  passwordInput.addEventListener("input", function () {
    if (passwordInput.value.length > 0) {
      passwordToggle.style.display = "inline";
    } else {
      passwordToggle.style.display = "none";
    }
  });
});


//Login Button == Enter Keyboard
//

//Sweet Alert Login Succesfully

// document.addEventListener("DOMContentLoaded", function() {
//   var loginButton = document.getElementById('loginButton');

//   loginButton.addEventListener('click', function(event) {
//       event.preventDefault(); 

//       Swal.fire({
//           position: "center",
//           icon: "success",
//           title: "Logged in Successfully!",
//           showConfirmButton: false,
//           timer: 1500,
//           customClass: {
//               container: 'custom-swal-container',
//               popup: 'custom-swal-popup',
//               content: 'custom-swal-content'
//           }
//       });
//   });
// });

document.addEventListener("DOMContentLoaded", function() {
  var loginButton = document.getElementById('loginButton');

  loginButton.addEventListener('click', function(event) {
      event.preventDefault(); 

      Swal.fire({
          position: "top",
          icon: "success",
          title: "Logged in Successfully!",
          showConfirmButton: false,
          timer: 1500,
          customClass: {
              container: 'custom-swal-container',
              popup: 'custom-swal-popup',
              content: 'custom-swal-content'
          }
      });
  });
});
// //