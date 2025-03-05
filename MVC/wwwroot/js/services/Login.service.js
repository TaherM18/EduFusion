$(document).ready(function () {
    // Add loading indicator element
    $('body').append('<div id="loadingOverlay" class="loading-overlay"><div class="spinner"></div></div>');

    // Initialize form with improved styling
    $("#formContainer").kendoForm({
        formData: {
            email: "",
            password: ""
        },
        layout: "grid",
        grid: {
            cols: 1,
            gutter: 20
        },
        items: [
            {
                field: "email",
                label: "Email Address",
                editor: "TextBox",
                editorOptions: {
                    placeholder: "Enter your email",
                    size: "large"
                },
                validation: {
                    required: {
                        message: "Email is required"
                    },
                    email: {
                        message: "Please enter a valid email address"
                    }
                }
            },
            {
                field: "password",
                label: "Password",
                editor: function (c, o) {
                    $(`<input type="password" id="password" class="k-input-inner" name="${o.field}" />`).appendTo(c);
                },
                editorOptions: {
                    type: "password",
                    placeholder: "Enter your password",
                    size: "large"
                },
                validation: {
                    required: {
                        message: "Password is required"
                    }
                }
            }
        ],
        buttonsTemplate: `<button class='k-btn' type='reset'>Clear</button>`
    });

    // Add animation to the form
    $(".login-container").css("opacity", 0);
    setTimeout(function () {
        $(".login-container").animate({ opacity: 1 }, 500);
    }, 200);

    // Handle form submission with improved UX
    $("#loginForm").submit(function (event) {
        event.preventDefault();

        var form = $("#formContainer").data("kendoForm");

        if (form.validate()) {
            // Show loading overlay
            $("#loadingOverlay").fadeIn(200);

            // Get form values            
            var loginData = new FormData();
            loginData.append("Email", $("#email").val());
            loginData.append("Password", $("#password").val());

            $.ajax({
                url: url + "/User/Login",
                type: "POST",
                data: loginData,
                processData: false,
                contentType: false,
                success: function (response) {
                    console.log(response)

                    // Hide loading overlay
                    $("#loadingOverlay").fadeOut(200);

                    if (response.success) {
                        console.log("Here")

                        // Save token
                        localStorage.setItem("token", response.token);
                        localStorage.setItem("user", JSON.stringify(response.userData));

                        // Show success message
                        showNotification("Login successful", "success");

                        // Redirect with animation
                        setTimeout(function () {
                            $(".login-container").animate({ opacity: 0 }, 300, function () {
                                let role = GetUserData().user ? GetUserData().user.role : GetUserData().role
                                if (role == "A") {
                                    window.location.href = "/admin/index";
                                }
                                else if (role == "S") {
                                    window.location.href = "/student/index";
                                }
                                else if (role == "T") {
                                    window.location.href = "/teacher/dashboard";
                                }
                                else {
                                    window.location.href = "/home/index";
                                } 
                            });
                        }, 1000);
                    } else {
                        // Show error message
                        $("#errorMessage").text(response.message).slideDown(300);
                        shakeForm();
                    }
                },
                error: function (xhr) {
                    // Hide loading overlay
                    $("#loadingOverlay").fadeOut(200);

                    // Show appropriate error message
                    let errorMsg = "Invalid credentials!";
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMsg = xhr.responseJSON.message;
                    }

                    $("#errorMessage").text(errorMsg).slideDown(300);
                    shakeForm();
                }
            });
        } else {
            // Animate the form to indicate validation error
            shakeForm();
        }
    });

    // Function to shake the form on error
    function shakeForm() {
        $(".card").css("transition", "none");
        $(".card").animate({ marginLeft: "-10px" }, 100)
            .animate({ marginLeft: "10px" }, 100)
            .animate({ marginLeft: "-10px" }, 100)
            .animate({ marginLeft: "10px" }, 100)
            .animate({ marginLeft: "0px" }, 100, function () {
                $(".card").css("transition", "all 0.3s ease");
            });
    }

    // Function to show notifications
    function showNotification(message, type) {
        // Check if kendo notification is available
        if ($.fn.kendoNotification) {
            $("<div></div>").kendoNotification({
                position: {
                    pinned: false,
                    top: 30,
                    right: 30
                },
                autoHideAfter: 3000,
                stacking: "down",
                templates: [{
                    type: "success",
                    template: "<div class='notification success'><span class='icon'>✓</span><span>#= message #</span></div>"
                }, {
                    type: "error",
                    template: "<div class='notification error'><span class='icon'>✗</span><span>#= message #</span></div>"
                }]
            }).data("kendoNotification").show({ message: message }, type);
        } else {
            // Fallback if kendo notification is not available
            alert(message);
        }
    }
});