function IsAuth() {
    return localStorage.getItem("token") != null
}

function Logout() {
    localStorage.clear()
    window.location.href = "/auth/login"
}

function GetUserData() {
    // let user = 
}

const url = "http://localhost:5190/api"