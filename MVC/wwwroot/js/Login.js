$(document).ready(function () {
    const validationSuccess = $("#validation-success");
    $("#loginform").kendoForm({
        validatable: {
            validateOnBlur: true,
            validationSummary: false,
            errorTemplate: "<span class='k-form-error'>#:message#</span>"
        },
        items: [
            {
                field: "Email", label: "Email Address:", validation: {
                    required: {
                        message: "Please enter your email address."
                    }, email: { message: "Please enter a valid email address." }
                }
            },
            {
                field: "Password",
                label: "Password:",
                validation: { required: { message: "Please enter your password." } },
                editor: function (container, options) {
                    $('<input type="password" id="Password" name="' + options.field + '" title="Password" required = "required" autocomplete = "off" aria - labelledby="Password-form-label" data - bind="value: Password" aria - describedby="Password-form-hint" /> ')
                        .appendTo(container)
                        .kendoTextBox();
                }
            }],
        buttonsTemplate: '<button type="submit" class="k-button k-button-lg k-button-solid-success">Login</button><button class= "k-button-solid-base k-button-lg" > Clear</button> ',
        validateField: function (e) {
            validationSuccess.html("");
        },
        submit: function (e) {
            e.preventDefault();
            const LoginData = new FormData();
            LoginData.append("c_Email", e.model.Email);
            LoginData.append("c_Password", e.model.Password);

            $.ajax({
                url: "/Auth/LoginAPI",
                method: "post",
                contentType: false,
                processData: false,
                data: LoginData,
                success: function (result) {
                    console.log(result)
                    if (result.success == "true") {
                        window.location.href = "/Home/Contacts"
                    } else {
                        validationSuccess.html("<div class='k-messagebox k-messagebox-error'><p>" + result.message + "</p></div>");
                    }
                },
                error: function (xhr, status, error) {
                    var errors = JSON.parse(xhr.responseText);
                    validationSuccess.html("<div class='k-messagebox k - messagebox - error'><p>" + errors.message.c_Email + "</p><p>" + errors.message.c_Password + "</p></div>");
                }
            });
        },
        clear: function (ev) {
            validationSuccess.html("");
        }
    });
});