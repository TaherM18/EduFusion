document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.getElementById("themeToggle");
    const body = document.body;

    // Check local storage for theme preference
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-theme");
        themeToggle.textContent = "☀️ Light Mode";
    }

    // Toggle Theme
    themeToggle.addEventListener("click", function () {
        body.classList.toggle("dark-theme");

        // Update button text
        if (body.classList.contains("dark-theme")) {
            localStorage.setItem("theme", "dark");
            themeToggle.textContent = "☀️ Light Mode";
        } else {
            localStorage.setItem("theme", "light");
            themeToggle.textContent = "🌙 Dark Mode";
        }
    });
});
