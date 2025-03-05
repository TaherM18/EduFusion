function IsAuth() {
    return localStorage.getItem("token") != null
}

function CheckLogin() {
    const urlPath = window.location.pathname.split("/"); // Get URL segments
    const firstSegment = urlPath[1]?.toLowerCase(); // First part of URL (e.g., "teacher")
    const secondSegment = urlPath[2]?.toLowerCase(); // Second part of URL (e.g., "register")

    // Allow "teacher/register" without redirection
    if (firstSegment === "teacher" && secondSegment === "register" ||  firstSegment === "home" || firstSegment === "") {
        console.log("Allowing teacher registration page");
        return;
    }

    if (!IsAuth() || GetUserData().user.role.toLowerCase() !== firstSegment) {
        console.log("Logged out");
        window.location.href = "/auth/login";
    } else {
        console.log("Logged in");
    }
}


function Logout() {
    localStorage.clear()
    window.location.href = "/auth/login"
}

function GetUserData() {
    let user = JSON.parse(localStorage.getItem('user'))
    return user;
}

const baseUrl = "http://localhost:5190/api"
const url = "http://localhost:5190/api"