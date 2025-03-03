function IsAuth() {
    return localStorage.getItem("token") != null
}

const url = "http://localhost:5190/api"