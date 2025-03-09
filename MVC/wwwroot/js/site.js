function IsAuth() {
    return localStorage.getItem("token") != null
}

function CheckLogin() {
    const urlPath = window.location.pathname.split("/"); // Get URL segments
    const firstSegment = urlPath[1]?.toLowerCase(); // First part of URL (e.g., "teacher")
    const secondSegment = urlPath[2]?.toLowerCase(); // Second part of URL (e.g., "register")

    // Allow "teacher/register" without redirection
    if (firstSegment === "teacher" && secondSegment === "register" || firstSegment === "home" || firstSegment === "") {
        console.log("Allowing teacher registration page");
        return;
    }

    let role = GetUserData().user ? GetUserData().user.role : GetUserData().role

    if (!IsAuth() || role.toLowerCase() !== firstSegment[0].toLowerCase()) {
        console.log("Logged out");
        console.log(role);
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

const baseUrl = "http://localhost:5190/api";
const url = "http://localhost:5190/api";

$(document).ready(function () {
    console.log("Document ready. Initializing notification...");

    if ($("#notification").length) {
        $("#notification").kendoNotification({
            autoHideAfter: 3000,
            width: 300,
            position: { pinned: true, top: 62, right: 30 }
        });
        console.log("Notification initialized.");
    } else {
        console.log("Error: #notification element not found.");
    }
});


function showNotification(message, type) {
    $("#notification").data("kendoNotification").show(message, type);
}