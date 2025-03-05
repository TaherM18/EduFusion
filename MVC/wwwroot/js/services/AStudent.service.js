let baseUrl = "http://localhost:5190/api";

$(document).ready(function () {
    initializeKendoComponents();
});

// Initialize Kendo Components
function initializeKendoComponents() {
    loadWindowAndNotification();
    loadStudentGrid();
}

// Load Kendo Window & Notification
function loadWindowAndNotification() {
    $("#myKModal").kendoWindow({
        width: "600px",
        title: "Student Form",
        visible: false,
        modal: true,
        actions: ["Close"]
    });

    $("#notification").kendoNotification({
        allowHideAfter: 1000,
        width: 300,
        position: { pinned: true, top: 30, right: 30 }
    });
}

// Load Kendo Grid
function loadStudentGrid() {
    $("#studentGrid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "http://localhost:5190/api" + "/student",
                    type: "GET",
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "studentID",
                    fields: {
                        studentID: { type: "number" },
                        standardID: { type: "number" },
                        rollNumber: { type: "string" },
                        guardianName: { type: "string" },
                        guardianContact: { type: "string" },
                        section: { type: "string" },
                        user: { defaultValue: {} },
                        standard: { defaultValue: {} }
                    }
                }
            },
            pageSize: 10
        },
        height: 400,
        pageable: true,
        sortable: true,
        filterable: true,
        columns: [
            { field: "studentID", title: "ID", width: "40px" },
            { field: "user.firstName", title: "First Name", template: "#= user.firstName || '' #", width: "100px" },
            { field: "user.lastName", title: "Last Name", template: "#= user.lastName || '' #", width: "100px" },
            { field: "user.birthDate", title: "Birth Date", template: "#= user.birthDate || '' #", width: "100px" },
            { field: "user.gender", title: "Gender", template: "#= user.gender || '' #", width: "80px" },
            { field: "user.image", title: "Image", template: "<img src='/profile_images/#= user.image ? user.image : 'placeholder.jpg' #' style='width:50px; height:50px;'/>", width: "80px" },
            { field: "user.email", title: "Email", template: "#= user.email || '' #", width: "120px" },
            { field: "user.contact", title: "Contact", template: "#= user.contact || '' #", width: "120px" },
            { field: "user.address", title: "Address", template: "#= user.address || '' #", width: "120px" },
            { field: "user.pincode", title: "Pin Code", template: "#= user.pincode || '' #", width: "80px" },
            { field: "standardID", title: "Standard", template: "#= standard ? standard.standardName : '' #", width: "100px" },
            { field: "rollNumber", title: "Roll No.", width: "80px" },
            { field: "guardianName", title: "Guardian", width: "150px" },
            { field: "guardianContact", title: "Guardian Contact", width: "150px" },
            { field: "section", title: "Section", width: "80px" },
            {
                title: "Actions",
                template: `
                    # if(isApproved) { #
                        <button class="k-button k-button-solid-warning m-1" onclick='UnApprove(#=studentID#)'>❌ Unapprove</button>
                    # } else { #
                        <button class="k-button k-button-solid-info m-1" onclick='Approve(#=studentID#)'>✅ Approve</button>
                    # } #
                    <button class='k-button k-button-solid-primary m-1' onclick='openEditForm(#=studentID#)'>✏️ Edit</button>
                    <button class='k-button k-button-solid-error m-1' onclick='deleteStudent(#=studentID#)'>🗑️ Delete</button>
                `,
                width: 160
            }
        ]
    });
}

async function Approve(id) {
    $.ajax({
        url: "http://localhost:5190/api" + `/student/approve/${id}`,
        method: "PUT",
        success: function (response) {
            showNotification("Approved Successfully", "success");
        },
        error: function (xhr) {
            showNotification(xhr.responseJSON.message, "error");
        }
    })
}

$("#btnExport").click(function () {
    // Get the Kendo UI Grid
    var grid = $("#studentGrid").data("kendoGrid");

    var data = grid.dataSource.view();
    var csvContent = "Student Id," +
        "First Name," +
        "Last Name," +
        "section," +
        "BirthDate," +
        "Gender," +
        "Image," +
        "Email," +
        "Address," +
        "Contact," +
        "Pincode," +
        "Standard," +
        "Guardian Name," +
        "Guardian Contact," +
        "Roll Number," +
        "Section" +
        "\n";

    data.forEach(function (row) {
        csvContent += row.studentID + "," +
            row.user.firstName + "," +
            row.user.lastName + "," +
            row.section + "," +
            row.user.birthDate + "," +
            row.user.gender + "," +
            row.user.image + "," +
            row.user.email + "," +
            "${row.user.address}" + "," +
            row.user.contact + "," +
            row.user.pincode + "," +
            row.standard.standardName + "," +
            row.guardianName + "," +
            row.guardianContact + "," +
            row.rollNumber + "," +
            row.section +
            "\n";
    });

    var blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    var link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "Student.csv";
    link.click();
});

async function UnApprove(id) {
    $.ajax({
        url: "http://localhost:5190/api" + `/student/unapprove/${id}`,
        method: "PUT",
        success: function (response) {
            showNotification("Approved Successfully", "success");
        },
        error: function (xhr) {
            showNotification(xhr.responseJSON.message, "error");
        }
    })
}


// Load Kendo Form
function loadStudentForm(studentData) {
    if (!studentData || Object.keys(studentData).length === 0) {
        console.error("Student data is empty:", studentData);
        showNotification("No student data found.", "error");
        return;
    }

    console.log("Loading form with data:", studentData);

    // Ensure the form is properly destroyed and re-initialized
    if ($("#studentForm").data("kendoForm")) {
        $("#studentForm").data("kendoForm").destroy();
        $("#studentForm").empty();
    }


    $("#studentForm").kendoForm({
        formData: studentData,
        items: [
            {
                field: "studentID",
                editor: function (container) {
                    $(container).append('<input type="hidden" name="studentID" readonly/>');
                },
                hidden: true,
                label: false,
            },
            { 
                field: "user", 
                defaultValue: {},
                editor: function (container) {
                    $(container).append('<input type="hidden" name="userObj" readonly/>');
                }
            },
            { field: "user.firstName", label: "First Name", validation: { required: true } },
            { field: "user.lastName", label: "Last Name", validation: { required: true } },
            { field: "user.birthDate", label: "Birth Date", editor: "DatePicker" },
            // Gender Radio Buttons
            {
                field: "user.gender",
                label: "Gender",
                editor: function (container, options) {
                    var genderValue = options.model.user?.gender || "";
                    var genderHtml = `
                    <label>
                        <input type="radio" name="gender" value="Male" ${genderValue === "Male" ? "checked" : ""}/> Male
                    </label>
                    <label>
                        <input type="radio" name="gender" value="Female" ${genderValue === "Female" ? "checked" : ""}/> Female
                    </label>
                    <label>
                        <input type="radio" name="gender" value="Other" ${genderValue === "Other" ? "checked" : ""}/> Other
                    </label>
                    `;
                    $(genderHtml).appendTo(container);
                }
            },
            // Image Upload Input
            {
                field: "user.image",
                label: "Image",
                editor: function (container) {
                    $(container).append('<input type="file" class="" name="ImageFile" accept="image/*"/>');
                }
            },

            { field: "user.email", label: "Email", validation: { required: true, email: true } },
            { field: "user.contact", label: "Contact", validation: { required: true } },
            { field: "user.address", label: "Address" },
            { field: "user.pincode", label: "Pin Code" },

            { field: "standardID", label: "Standard ID", validation: { required: true } },
            { field: "rollNumber", label: "Roll Number", validation: { required: true } },
            { field: "guardianName", label: "Guardian Name" },
            { field: "guardianContact", label: "Guardian Contact" },
            { field: "section", label: "Section" },
        ],
        buttonsTemplate: `
        <button type="submit" class="k-button k-button-lg k-button-solid-info">Save</button>
        <button type="button" class="k-button k-button-lg k-button-solid-base" onclick="closeModal()">Cancel</button>
        `,
        submit: function (e) {
            e.preventDefault();
            saveStudent(e.model);
        }
    });
}



// Open Modal for Adding New Student
function openAddForm() {
    loadStudentForm({
        studentID: 0,
        standardID: "",
        rollNumber: "",
        guardianName: "",
        guardianContact: "",
        section: ""
    });
    openModal();
}

// Open Modal for Editing Existing Student
function openEditForm(id) {
    $.ajax({
        url: `http://localhost:5190/api/student/${id}`,
        type: "GET",
        success: function (response) {
            loadStudentForm(response);
            openModal();
        },
        error: function () {
            showNotification("Failed to load student data.", "error");
        }
    });
}

// Open Modal
function openModal() {
    $("#myKModal").data("kendoWindow").center().open();
}

// Close Modal
function closeModal() {
    $("#myKModal").data("kendoWindow").close();
}

// Save Student (Create/Update)
function saveStudent(model) {
    console.log("saveStudent(model)",model);
    
    var formData = new FormData();

    // Append scalar fields
    formData.append("studentID", model.studentID || 0);
    formData.append("standardID", model.standardID);
    formData.append("rollNumber", model.rollNumber);
    formData.append("guardianName", model.guardianName);
    formData.append("guardianContact", model.guardianContact);
    formData.append("section", model.section);

    // Append user object fields
    // if (model.user) {
        formData.append("firstName", model.user.firstName);
        formData.append("lastName", model.user.lastName);
        formData.append("birthDate", model.user.birthDate);
        formData.append("gender", model.user.gender);
        formData.append("email", model.user.email);
        formData.append("contact", model.user.contact);
        formData.append("address", model.user.address);
        formData.append("pincode", model.user.pincode);
        formData.append("password", null);
    // }

    // Append image file if selected
    var imageFile = document.querySelector("input[name='ImageFile']").files[0];
    if (imageFile) {
        formData.append("ImageFile", imageFile);
    }

    // Send AJAX request
    $.ajax({
        url: model.studentID ? "http://localhost:5190/api/student/update" : "http://localhost:5190/api/student/register",
        type: model.studentID ? "PUT" : "POST",
        data: formData,
        contentType: false, // Important: Don't set Content-Type for FormData
        processData: false, // Important: Prevent jQuery from processing FormData
        success: function (response) {
            alert("Student saved successfully!");
            $("#studentModal").modal("hide");
            loadStudentGrid(); // Reload grid data
        },
        error: function (error) {
            alert("Error saving student!");
            console.log(error);
        }
    });
}


// Delete Student
function deleteStudent(id) {
    if (confirm("Are you sure you want to delete this student?")) {
        $.ajax({
            url: `http://localhost:5190/api/student/${id}`,
            type: "DELETE",
            success: function (response) {
                if (response.success) {
                    showNotification("Student deleted successfully!", "success");
                    reloadGrid();
                } else {
                    showNotification(response.message, "error");
                }
            },
            error: function () {
                showNotification("An error occurred. Please try again.", "error");
            }
        });
    }
}

// Show Kendo Notification
function showNotification(message, type) {
    $("#notification").data("kendoNotification").show(message, type);
}

// Reload Grid Data
function reloadGrid() {
    $("#studentGrid").data("kendoGrid").dataSource.read();
}