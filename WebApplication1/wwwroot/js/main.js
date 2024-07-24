// //Sidebar Logo go to Dashboard when click
document.addEventListener("DOMContentLoaded", function () {
    const logoImage = document.querySelector(".logo-image img"); // Select the logo image
    const dashboardContent = document.getElementById('dash-board');
    const sidebarLinks = document.querySelectorAll('.menu-links li a'); // Select all sidebar links
    const dashboardLink = document.querySelector('.menu-links a[href="#dash-board"]');
    const dropdownLinks = document.querySelector('.dropdown-links'); // Select the dropdown links

    let originalDashboardHTML = null; // Store the original dashboard HTML

    // Function to load the original dashboard content
    function loadOriginalDashboardContent() {
        originalDashboardHTML = dashboardContent.innerHTML;
    }

    // Function to navigate back to the dashboard content
    function navigateToDashboard() {
        if (originalDashboardHTML === null) {
            console.error('Original dashboard content not loaded.');
            return;
        }
        dashboardContent.innerHTML = originalDashboardHTML;

        // Add "active" class to the dashboard link
        dashboardLink.classList.add('active');

        // Remove "active" class from all other sidebar links
        sidebarLinks.forEach(function (link) {
            if (link !== dashboardLink) {
                link.classList.remove('active');
            }
        });
    }

    // Load the original dashboard content when the page loads
    loadOriginalDashboardContent();

    // Function to close the dropdown menu
    function closeDropdown() {
        dropdownLinks.style.display = "none";
    }

    // Add event listener to the logo image
    if (logoImage) {
        logoImage.addEventListener("click", function () {
            navigateToDashboard();
            closeDropdown();
        });
    }
});
//

//Sidebar Toggles and Sidebar screen Big screen to Mobile screen
document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.querySelector(".sidebar .toggle-btn");
    const sidebar = document.querySelector(".sidebar");
    const dashboardSection = document.querySelector(".dash-board"); // Added this line

    toggleBtn.addEventListener("click", function () {
        sidebar.classList.toggle("active");
        // dashboardSection.classList.toggle("moved"); // Added this line
        toggleBtn.classList.toggle("active");
    });

    // Check if window width is <= 760px, then hide sidebar
    function checkWidth() {
        if (window.innerWidth <= 760) {
            sidebar.classList.remove("active");
            dashboardSection.classList.remove("moved");
            toggleBtn.classList.remove("active");
        } else {
            sidebar.classList.add("active"); // Reset sidebar state when width is > 760px
            // dashboardSection.classList.add("moved"); // Reset dashboard state when width is > 760px
            toggleBtn.classList.add("active"); // Reset toggle button state when width is > 760px
        }
    }

    // Check window width on page load
    checkWidth();

    // Check window width on resize
    window.addEventListener("resize", function () {
        checkWidth();
    });
});
//


//Sidebar Menu List....
document.addEventListener("DOMContentLoaded", function () {
    var dropdowns = document.querySelectorAll(".dropdown");

    dropdowns.forEach(function (dropdown) {
        var parentLink = dropdown.querySelector("a");
        var dropdownLinks = dropdown.querySelector(".dropdown-links");

        parentLink.addEventListener("click", function (event) {
            event.preventDefault();
            toggleDropdown();
        });

        function toggleDropdown() {
            // Toggle the display of the dropdown links
            if (dropdownLinks.style.display === "block") {
                dropdownLinks.style.display = "none";
            } else {
                dropdownLinks.style.display = "block";
            }
        }
    });

    // Add event listener to close dropdowns when clicking on other menu links
    document.querySelectorAll(".menu-links li a").forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();

            // Close all other dropdowns
            dropdowns.forEach(function (dropdown) {
                if (dropdown !== link.closest(".dropdown")) {
                    dropdown.querySelector(".dropdown-links").style.display = "none";
                }
            });

            // Remove active class from all links
            document.querySelectorAll(".menu-links li a").forEach(function (link) {
                link.classList.remove("active");
            });

            // Add active class to the clicked link
            link.classList.add("active");
        });
    });
});
//GANA TO PAREEEEEEEEEEEEEEEEEEEEEE



//   //Change Dashboard Content to another Page
document.addEventListener("DOMContentLoaded", function () {
    // Get reference to the "Add Employee" link
    var addEmployeeLink = document.querySelector('.menu-links a[href="/Dashboard-TK/Add-Employee.html"]');
    // Get reference to the "For Checking" link
    var forCheckingLink = document.querySelector('.menu-links a[href="/Dashboard-TK/For-Checking.html"]');
    // Get reference to the "Update Account" link
    var updateAccountLink = document.querySelector('.menu-links a[href="/Dashboard-TK/Update-Account.html"]');
    // Get reference to the dashboard content section
    var dashboardContent = document.getElementById('dash-board');
    // Save the original dashboard HTML
    var originalDashboardHTML = dashboardContent.innerHTML;

    // Function to handle clicking on the "Add Employee" link
    function handleAddEmployeeClick(event) {
        event.preventDefault(); // Prevent default link behavior
        // Load Add Employee content into the dashboard section
        loadAddEmployeeContent();
    }

    // Function to handle clicking on the "For Checking" link
    function handleForCheckingClick(event) {
        event.preventDefault(); // Prevent default link behavior
        // Load For Checking content into the dashboard section
        loadForCheckingContent();
    }

    // Function to handle clicking on the "Update Account" link
    function handleUpdateAccountClick(event) {
        event.preventDefault(); // Prevent default link behavior
        // Load Update Account content into the dashboard section
        loadUpdateAccountContent();
    }

    // Function to load Add Employee content into the dashboard section
    function loadAddEmployeeContent() {
        // Fetch the content of the Add Employee page
        fetch('/Dashboard-TK/Add-Employee.html')
            .then(response => response.text())
            .then(html => {
                // Replace the content of the dashboard section with the Add Employee HTML
                dashboardContent.innerHTML = html;
                // Add event listener to the dashboard sidebar list item
                var dashboardLink = document.querySelector('.menu-links a[href="#dash-board"]');
                if (dashboardLink) {
                    dashboardLink.addEventListener('click', handleDashboardClick);
                } else {
                    console.error('Dashboard link not found.');
                }
            })
            .catch(error => console.error('Error loading Add Employee content:', error));
    }

    // Function to load For Checking content into the dashboard section
    function loadForCheckingContent() {
        // Fetch the content of the For Checking page
        fetch('/Dashboard-TK/For-Checking.html')
            .then(response => response.text())
            .then(html => {
                // Replace the content of the dashboard section with the For Checking HTML
                dashboardContent.innerHTML = html;
                // Add event listener to the dashboard sidebar list item
                var dashboardLink = document.querySelector('.menu-links a[href="#dash-board"]');
                if (dashboardLink) {
                    dashboardLink.addEventListener('click', handleDashboardClick);
                } else {
                    console.error('Dashboard link not found.');
                }
            })
            .catch(error => console.error('Error loading For Checking content:', error));
    }

    // Function to load Update Account content into the dashboard section
    function loadUpdateAccountContent() {
        // Fetch the content of the Update Account page
        fetch('/Dashboard-TK/Update-Account.html')
            .then(response => response.text())
            .then(html => {
                // Replace the content of the dashboard section with the Update Account HTML
                dashboardContent.innerHTML = html;
                // Add event listener to the dashboard sidebar list item
                var dashboardLink = document.querySelector('.menu-links a[href="#dash-board"]');
                if (dashboardLink) {
                    dashboardLink.addEventListener('click', handleDashboardClick);
                } else {
                    console.error('Dashboard link not found.');
                }
            })
            .catch(error => console.error('Error loading Update Account content:', error));
    }

    // Function to handle clicking on the dashboard sidebar list item
    function handleDashboardClick(event) {
        event.preventDefault(); // Prevent default link behavior
        // Reset dashboard content to its original HTML
        dashboardContent.innerHTML = originalDashboardHTML;
    }

    // Add event listener to the "Add Employee" link
    if (addEmployeeLink && dashboardContent) {
        addEmployeeLink.addEventListener('click', handleAddEmployeeClick);
    } else {
        console.error('Add Employee link or dashboard content section not found.');
    }

    // Add event listener to the "For Checking" link
    if (forCheckingLink && dashboardContent) {
        forCheckingLink.addEventListener('click', handleForCheckingClick);
    } else {
        console.error('For Checking link or dashboard content section not found.');
    }

    // Add event listener to the "Update Account" link
    if (updateAccountLink && dashboardContent) {
        updateAccountLink.addEventListener('click', handleUpdateAccountClick);
    } else {
        console.error('Update Account link or dashboard content section not found.');
    }
});

//   //

//CARD AND VIEW DETAILS GO TO FOR CHECKING
document.addEventListener("DOMContentLoaded", function () {
    const dashboardContent = document.getElementById('dash-board');
    const originalDashboardHTML = dashboardContent.innerHTML;

    // Function to load For Checking content into the dashboard section
    function loadForCheckingContent() {
        // Fetch the content of the For Checking section
        fetch('/Dashboard-TK/For-Checking.html')
            .then(response => response.text())
            .then(html => {
                // Replace the content of the dashboard section with the For Checking HTML
                dashboardContent.innerHTML = html;
            })
            .catch(error => console.error('Error loading For Checking content:', error));
    }

    // Function to handle clicking on the card
    function handleCardClick(event) {
        if (event.target.classList.contains('card-for-checking') || event.target.closest('.card-for-checking')) {
            event.preventDefault(); // Prevent default link behavior
            loadForCheckingContent();
        }
    }

    // Function to handle clicking on the dashboard sidebar list item
    function handleDashboardClick(event) {
        if (event.target.closest('.menu-links a[href="/Dashboard-TK/For-Checking.html"]')) {
            event.preventDefault(); // Prevent default link behavior
            loadForCheckingContent();
        }
    }

    // Function to handle clicking on the dashboard sidebar list item
    function handleDashboardClick(event) {
        event.preventDefault(); // Prevent default link behavior
        if (event.target.closest('.menu-links a[href="/Dashboard-TK/For-Checking.html"]')) {
            loadForCheckingContent();
        }
    }

    // Function to reset dashboard content to its original HTML
    function resetDashboardContent() {
        dashboardContent.innerHTML = originalDashboardHTML;
    }

    // Event delegation for card click
    document.addEventListener('click', handleCardClick);

    // Event delegation for dashboard sidebar click
    document.addEventListener('click', handleDashboardClick);

    // Event delegation for dashboard link click
    document.addEventListener('click', function (event) {
        if (event.target.closest('.menu-links a[href="#dash-board"]')) {
            resetDashboardContent();
        }
    });
});

//


//Add Employee > Date's Placeholder
document.addEventListener("DOMContentLoaded", function () {
    const dateInputs = document.querySelectorAll('input[type="date"]');

    dateInputs.forEach(function (input) {
        input.addEventListener("focus", function () {
            input.nextElementSibling.style.opacity = "0";
            input.nextElementSibling.style.visibility = "hidden";
        });

        input.addEventListener("blur", function () {
            if (input.value === "") {
                input.nextElementSibling.style.opacity = "1";
                input.nextElementSibling.style.visibility = "visible";
            }
        });

        // Trigger blur event initially to show placeholder
        input.dispatchEvent(new Event("blur"));
    });
});
//

// Enable Drag Scrolling to Tables without Highlighting the table

//


//   //Task >  For Checking > View Button
// document.addEventListener("DOMContentLoaded", function () {
//     const forCheckingViewBtn = document.querySelectorAll(
//       ".for-checking .btn-view"
//     );

//     forCheckingViewBtn.forEach(function (link) {
//       link.addEventListener("click", function (event) {
//         event.preventDefault();
//         navigateToSection("#fc-view-emp-details");
//       });
//     });

//     function navigateToSection(sectionId) {
//       document.querySelectorAll("section").forEach(function (section) {
//         section.style.display = "none";
//       });

//       const targetSection = document.querySelector(sectionId);
//       if (targetSection) {
//         targetSection.style.display = "block";
//       }
//     }
//   });

//   //Task > For Checking> View Button > Back Button
//   document.addEventListener("DOMContentLoaded", function () {
//     const fcViewEmpDetailsBackBtn = document.querySelectorAll(
//       ".fc-view-emp-det-btn-back"
//     );

//     fcViewEmpDetailsBackBtn.forEach(function (link) {
//       link.addEventListener("click", function (event) {
//         event.preventDefault();
//         navigateToSection("#for-checking");
//       });
//     });

//     function navigateToSection(sectionId) {
//       document.querySelectorAll("section").forEach(function (section) {
//         section.style.display = "none";
//       });

//       const targetSection = document.querySelector(sectionId);
//       if (targetSection) {
//         targetSection.style.display = "block";
//       }
//     }
//   });
//   //





