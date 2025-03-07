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
        title: "Teacher Form",
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
    $("#teacherGrid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: baseUrl + "/teacher",
                    type: "GET",
                    dataType: "json"
                }
            },

            schema: {
                model: {
                    id: "teacherID",
                    fields: {
                        experienceYears: { type: "number" },
                        qualification: { type: "string" },
                        expertise: { type: "string" },
                        user: {
                            defaultValue: {},
                            fields: {
                                firstName: { type: "string" }
                                // lastName: { type: "string" },
                                // address: { type: "string" },
                                // gender: { type: "string" },
                                // email: { type: "string" },
                                // contact: { type: "string" },
                                // role: { type: "string" }
                            }
                        }
                    }
                }
            },
            pageSize: 10
        },
        toolbar: [
            "pdf",
            "excel",
            {
                template: '<button id="refreshGridBtn" class="k-button k-grid-toolbar-button p-1">' +
                    '<span class="k-icon k-i-refresh"></span> Refresh' +
                    '</button>'
            },
        ],
        dataBound: function () {
            // Ensure event binding only happens once
            $("#refreshGridBtn").off("click").on("click", function () {
                console.log("Refreshing Grid...");
                $("#teacherGrid").data("kendoGrid").dataSource.read();
            });
        },
        // height: 400,
        pageable: true,
        sortable: true,
        filterable: true,
        columns: [
            { field: "user.firstName", title: "First Name", template: "#= user && user.firstName || '' #", width: "100px" },
            { field: "user.lastName", title: "Last Name", template: "#= user && user.lastName || '' #", width: "100px" },
            { field: "user.address", title: "Address", template: "#= user && user.address || '' #", width: "100px" },
            { field: "user.gender", title: "Gender", template: "#= user && user.gender || '' #", width: "80px" },
            { field: "experienceYears", title: "Experience Years", template: "#= experienceYears || '' #", width: "120px" },
            { field: "expertise", title: "Expertise", template: "#= expertise.replace(/[{}]/g, '') || '' #", width: "120px" },
            { field: "qualification", title: "Qualification", template: "#= qualification || '' #", width: "120px" },

            {
                title: "Actions",
                template: `
                        # if(isApproved) { #
                            <button class="k-button k-button-solid-warning m-1" onclick='UnApproveT(#=teacherID#)'>❌ Unapprove</button>
                        # } else { #
                            <button class="k-button k-button-solid-info m-1" onclick='ApproveT(#=teacherID#)'>✅ Approve</button>
                        # } #
                        <button class='k-button k-button-solid-primary m-1' onclick='openEditForm(#=teacherID#)'>✏️ Edit</button>
                        <button class='k-button k-button-solid-error m-1' onclick='deleteStudent(#=teacherID#)'>🗑️ Delete</button>
                    `,
                width: 160
            }
        ]
    });
}

async function ApproveT(id) {
    $.ajax({
        url: baseUrl + `/teacher/approve/${id}`,
        method: "PUT",
        success: function (response) {
            showNotification("Approved Successfully", "success");
            $("#teacherGrid").data("kendoGrid").dataSource.read();
        },
        error: function (xhr) {
            showNotification(xhr.responseJSON.message, "error");
        }
    })
}

async function UnApproveT(id) {
    $.ajax({
        url: baseUrl + `/teacher/unapprove/${id}`,
        method: "PUT",
        success: function (response) {
            showNotification("Approved Successfully", "success");
            $("#teacherGrid").data("kendoGrid").dataSource.read();
        },
        error: function (xhr) {
            showNotification(xhr.responseJSON.message, "error");
        }
    })
}

// Delete Student
function deleteStudent(id) {
    if (confirm("Are you sure you want to delete this student?")) {
        $.ajax({
            url: `${baseUrl}/${id}`,
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