let vrequired = true;

$(document).ready(function () {
  $("<style>", {
    type: "text/css",
    html: `
      .registration-container {
        max-width: 1200px;
        margin: 0 auto;
        padding: 20px;
        background-color: #f9f9f9;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      }
      
      .k-wizard-steps {
        background-color: #ffffff;
        border-radius: 6px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
      }
      
      .k-wizard-content {
        padding: 24px;
        background-color: #ffffff;
        border-radius: 6px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
      }
      
        .k-wizard-buttons .k-button {
        font-size: 16px;
        font-weight: 500;
        padding: 10px 24px;
        height: auto;
        min-width: 140px;
        border-radius: 4px;
        transition: all 0.2s ease;
      } 
      
      .k-wizard-buttons {
        padding: 16px 0;
        margin-top: 16px;
      }
      
      .k-form-hint {
        color: #666;
        font-style: italic;
        margin-top: 6px;
        display: block;
      } 
      
      #captchaContainer {
        background-color: #f5f5f5;
        padding: 12px;
        border-radius: 4px;
        margin-bottom: 12px;
      }
      
      #CaptchaText {
        font-family: 'Courier New', monospace;
        font-weight: bold;
        font-size: 18px;
        letter-spacing: 2px;
        color: #333;
        padding: 8px;
        display: block;
        margin-bottom: 10px;
        background: linear-gradient(to right, #f5f5f5, #e0e0e0);
        text-align: center;
      }
      
      #refreshCaptcha {
        margin-bottom: 10px;
      }
    `
  }).appendTo("head");

  // Wrap wizard in a container div
  $("#registrationWizard").wrap('<div class="registration-container"></div>');

  // Initialize Kendo Wizard
  var wizard = $("#registrationWizard")
    .kendoWizard({
      steps: [
        {
          title: "Personal Details",
          form: {
            layout: "grid",
            grid: {
              cols: 2,
              gutter: 20,
            },
            items: [
              {
                field: "FirstName",
                label: "First Name",
                id: "firstname",
                validation: { required: vrequired },
              },
              {
                field: "LastName",
                label: "Last Name",
                id: "lastname",
                validation: { required: vrequired },
              },
              {
                field: "Email",
                label: "Email",
                id: "email",
                validation: { required: vrequired, email: true },
              },
              {
                field: "Password",
                label: "Password",
                id: "password",
                validation: {
                  required: vrequired,
                },
                editor: function (container, options) {
                  $("<input id='password' name='Password' type='password' required/>")
                    .appendTo(container)
                    .kendoTextBox()
                    .attr("type", "password")
                    .attr("autocomplete", "new-password");
                },
              },
              {
                field: "ConfirmPassword",
                label: "Confirm Password",
                id: "confirmpassword",
                validation: { required: vrequired },
                editor: function (container, options) {
                  $("<input id='confirmpassword' name='ConfirmPassword' type='password' required/>")
                    .appendTo(container)
                    .kendoTextBox()
                    .attr("type", "password")
                    .attr("autocomplete", "new-password");
                },
              },
              {
                field: "Gender",
                label: "Gender",
                editor: "DropDownList",
                id: "gender",
                editorOptions: {
                  dataSource: ["Male", "Female", "Other"],
                  optionLabel: "Select Gender",
                },
                validation: { required: vrequired },
              },
              {
                field: "Address",
                label: "Address",
                id: "address",
                validation: { required: vrequired },
              },
              {
                field: "PhoneNumber",
                label: "Phone Number",
                id: "phone",
                validation: { required: vrequired },
                editor: function (container, options) {
                  $("<input id='phone' name='PhoneNumber' required/>")
                    .appendTo(container)
                    .kendoMaskedTextBox({ mask: "(000) 000-0000" });
                },
              },
              {
                field: "BirthDate",
                label: "Date of Birth",
                id: "dob",
                editor: "DatePicker",
                validation: { required: vrequired },
              },
              {
                field: "ProfilePicture",
                label: "Profile Picture:",
                editor: function (container, options) {
                  $("<input name='ProfilePicture' type='file'>")
                    .appendTo(container)
                    .kendoUpload({
                      multiple: false,
                      files: [],
                      validation: {
                        allowedExtensions: [".jpg", ".jpeg", ".png"],
                        maxFileSize: 4194304 // 4MB
                      },
                      localization: {
                        select: "Choose profile image"
                      }
                    });
                },
              },
            ],
          }
        },
        {
          title: "Qualifications",
          enabled: true,
          form: {
            items: [
              {
                field: "Qualification",
                label: "Qualification",
                editor: "DropDownList",
                editorOptions: {
                  dataSource: ["B.Ed", "M.Ed", "Ph.D", "Other"],
                  optionLabel: "Select Qualification",
                },
              },
              {
                field: "Experience",
                label: "Experience (Years)",
                id: "experience",
                editor: "NumericTextBox",
                editorOptions: {
                  min: 0,
                  max: 50,
                  format: "# years",
                  change: function () {
                    $("#experienceSlider")
                      .data("kendoSlider")
                      .value(this.value());
                  },
                },
              },
              {
                field: "ExperienceSlider",
                label: "Experience Level",
                id: "experienceSlider",
                editor: "Slider",
                editorOptions: {
                  min: 0,
                  max: 50,
                  smallStep: 1,
                  largeStep: 5,
                  tickPlacement: "none",
                  change: function () {
                    $("#experience")
                      .data("kendoNumericTextBox")
                      .value(this.value());
                  },
                },
              },
              {
                field: "Subjects",
                label: "Subjects Expertise",
                validation: { required: vrequired },
                editor: "MultiSelect",
                editorOptions: {
                  dataSource: [
                    "Math",
                    "Science",
                    "English",
                    "History",
                    "Geography",
                  ],
                  placeholder: "Select subjects you specialize in",
                  tagMode: "single"
                },
              },
            ],
          }
        },
        {
          title: "Verification",
          enabled: true,
          form: {
            items: [

              {
                field: "CaptchaInput",
                label: "Enter CAPTCHA",
                editor: function (container, options) {
                  $(`<div id="captchaContainer">
                    <span id="CaptchaText" class="k-textbox k-display-block" 
                      oncontextmenu="return false;" 
                      onselectstart="return false;" 
                      ondragstart="return false;" 
                      oncopy="return false;" 
                      oncut="return false;" 
                      style="user-select: none; -webkit-user-select: none; -ms-user-select: none;">
                    </span>
                    <button type="button" id="refreshCaptcha" class="k-button k-primary">Refresh CAPTCHA</button>
                  </div>
                  <input type='text' id='CaptchaInput' class='k-textbox' required placeholder="Enter the code shown above">
                  <span class="k-form-hint">Please enter the characters you see in the image above</span>`).appendTo(container);
                },
              },
            ],
          },
        },
      ],
      done: function (e) {
        submitAllForms(e);
      },
      messages: {
        next: "Continue →",
        previous: "← Back",
        done: "Submit Registration"
      },
      contentPosition: "bottom"
    })
    .data("kendoWizard");

  // Add password match validation
  $("#registrationWizard").kendoValidator({
    rules: {
      passwordMatch: function (input) {
        if (input.is("[name=ConfirmPassword]")) {
          return input.val() === $("input[name=Password]").val();
        }
        return true;
      }
    },
    messages: {
      passwordMatch: "Passwords do not match!"
    }
  });

  generateCaptcha();

  $("#refreshCaptcha").click(function () {
    generateCaptcha();
  });

  function generateCaptcha() {
    let captcha = Math.random().toString(36).substr(2, 6).toUpperCase();
    $("#CaptchaText").text("Captcha: " + captcha);
    $("#CaptchaInput").val(""); // Clear input field
    $("#CaptchaInput").data("generatedCaptcha", captcha);
  }

  function submitAllForms(e) {
    let personal = e.forms[0]._model;
    let qualification = e.forms[1]._model;

    let enteredCaptcha = $("#CaptchaInput").val();
    let generatedCaptcha = $("#CaptchaInput").data("generatedCaptcha");

    if (enteredCaptcha != generatedCaptcha) {
      alert("CAPTCHA validation failed! Please try again.");
      generateCaptcha();
      return;
    }

    let subjects = Object.values(qualification.Subjects);
    for (let i = 1; i <= 4; i++) {
      subjects.pop();
    }

    let formData = new FormData();

    formData.append("user.FirstName", personal.FirstName);
    formData.append("user.LastName", personal.LastName);
    formData.append("user.Email", personal.Email);
    formData.append("user.Password", personal.Password);
    formData.append("user.Gender", personal.Gender);
    formData.append("user.Address", personal.Address);
    formData.append("user.Contact", personal.PhoneNumber);
    formData.append("user.BirthDate", $("input[name='BirthDate']").val());

    var fileInput = $("input[name='ProfilePicture']")[0].files[0];

    formData.append("user.ImageFile", fileInput);
    formData.append("user.Image", fileInput.name);


    formData.append("Qualification", qualification.Qualification);
    formData.append("ExperienceYears", qualification.ExperienceSlider);
    formData.append("Expertise", subjects.join(","));
    console.log("formData:\n", formData);

    $("body").append('<div class="k-loading-mask" style="width:100%;height:100%;top:0;left:0;position:fixed;background-color:rgba(255,255,255,0.7);z-index:9999;"><span class="k-loading-text">Processing registration...</span><div class="k-loading-image"></div><div class="k-loading-color"></div></div>');


    $.ajax({
      url: "http://localhost:5190/api/teacher/register",
      type: "POST",
      contentType: false,
      processData: false,
      data: formData,
      success: function (response) {
        $(".k-loading-mask").remove();

        // Show success message with Kendo Dialog
        $("<div>").kendoDialog({
          width: "400px",
          title: "Registration Successful",
          closable: true,
          modal: true,
          content: "<p>Your registration has been completed successfully!</p><p>You can now login using your email and password.</p>",
          actions: [
            {
              text: "Continue to Login", primary: true, cssClass: "green-button", action: function () {
                // Redirect to login page
                window.location.href = "/auth/login";
              }
            }
          ],
          open: function () {
            $(".k-dialog-titlebar").css("background-color", "#28a745").css("color", "white");
            $(".green-button").css({
              "background-color": "#28a745", // Green button
              "border-color": "#28a745",
              "color": "white"
            });
          }
        }).data("kendoDialog").open();


      },
      error: function (xhr) {
        $(".k-loading-mask").remove();

        // Show error message with Kendo Dialog
        $("<div>").kendoDialog({
          width: "400px",
          title: "Registration Error",
          closable: true,
          modal: true,
          content: "<p>There was an error processing your registration:</p><p>" + xhr.responseText + "</p>",
          actions: [
            { text: "Try Again", primary: true }
          ]
        }).data("kendoDialog").open();

      },
    });
  }
});