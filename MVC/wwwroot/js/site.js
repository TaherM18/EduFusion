function IsAuth() {
    return localStorage.getItem("token") != null
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