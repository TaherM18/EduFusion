$(document).ready(function () {
    const staticPath = "../../profile_images/";
    const validationSuccess = $("#validation-success");
    $("#registerform").kendoForm({
        validatable: {
            validateOnBlur: true,
            validationSummary: false,
            errorTemplate: "<span class='k-form-error'>#:message#</span>"
        },
        items: [
            { field: "Name", label: "Name:", validation: { required: { message: "Please enter your name." } } },
            {
                field: "Email", label: "Email:", validation: {
                    required: { message: "Please enter your email." }, email: {
                        message:
                            "Please enter a valid email address"
                    }
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
            },
            {
                field: "ConfirmPassword",
                label: "Confirm Password:",
                validation: { required: { message: "Please enter your password." }, },
                editor: function (container, options) {
                    $('<input type="password" id="ConfirmPassword" name="' + options.field + '" title="ConfirmPassword" required = "required" autocomplete = "off" aria - labelledby="Password-form-label" data - bind="value: ConfirmPassword" aria - describedby="Password-form-hint" /> ')
                        .appendTo(container)
                        .kendoTextBox();
                }
            },
            { field: "Address", label: "Address:" },
            { field: "Mobile", label: "Mobile:" },
            {
                field: "Gender",
                label: "Gender:",
                validation: { required: { message: "Please select your gender." } },
                editor: function (container, options) {
                    const genderOptions = [
                        { value: "Male", label: "Male" },
                        { value: "Female", label: "Female" },
                        { value: "Other", label: "Other" }
                    ];
                    $('<div class="k-radio-group"></div>')
                        .appendTo(container);
                    genderOptions.forEach(option => {
                        $('<label class="k-radio-label"></label>')
                            .append(
                                $('<input type="radio" style="margin:5px" name="' + options.field + '" value="' + option.value + '" required />')
                            )
                            .append(option.label)
                            .appendTo(container.find(".k-radio-group"));
                    });
                }
            },
            {
                field: "image",
                label: "Image:",
                editor: function (container, options) {
                    $('<input type="file" id="image" name="' + options.field + '" />')
                        .appendTo(container)
                        .kendoUpload({
                            multiple: false
                        });
                }
            },
        ],
        buttonsTemplate: '<button type="submit" class="k-button k-button-lg k-button-solid-success">Register</button> <button class="k - button - solid - base k- button - lg ">Clear</button>',
        validateField: function (e) {
            validationSuccess.html("");
        },
        submit: function (e) {
            e.preventDefault();
            const RegisterData = new FormData();
            RegisterData.append("Username", e.model.Name);
            RegisterData.append("Email", e.model.Email);
            RegisterData.append("Password", e.model.Password);
            RegisterData.append("ConfirmPassword", e.model.ConfirmPassword);
            RegisterData.append("Address", e.model.Address);
            RegisterData.append("Mobile", e.model.Mobile);
            RegisterData.append("Gender", e.model.Gender);
            var imageFile = $("input[name=image]")[0].files[0];
            if (imageFile) {
                RegisterData.append("ProfilePicture", imageFile); // Append the file
            }

            $.ajax({
                url: "/Auth/RegisterAPI",
                method: "POST",
                contentType: false,
                processData: false,
                data: RegisterData,
                success: function (result) {
                    if (result.success == "true") {
                        validationSuccess.html("<div class='k-messagebox k-messagebox-error'><p>" + result.message + " </p></div>");
                        window.location.href = "/Auth/Login"
                    } else {
                        validationSuccess.html("<div class='k-messagebox k-messagebox-error'><p>" + result.message + "</p></div>");
                    }
                },
                error: function (xhr, status, error) {
                    var errors = JSON.parse(xhr.responseText);
                    validationSuccess.html("<div class='k-messagebox k-messagebox-error'><p>" + errors.c_Password ?? ""
                        + "</p><p>" + errors.c_ConfirmPassword ?? "" + "</p></div>");
                }
            });
            //validationSuccess.html("<div class='k-messagebox k-messagebox-success'>Form data is valid!</div>");
        },
        clear: function (ev) {
            validationSuccess.html("");
        }
    });
});           