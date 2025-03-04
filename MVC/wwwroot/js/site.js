function IsAuth() {
    return localStorage.getItem("token") != null
}

function CheckLogin() {
    const url = window.location.href.split("/")[3];
    console.log( url[0]);
    if (!IsAuth() || GetUserData().user.role.toLowerCase() != url[0].toLowerCase()) {
        console.log("Logged out")
        window.location.href = "/auth/login"
    }
    console.log("Logged in")
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