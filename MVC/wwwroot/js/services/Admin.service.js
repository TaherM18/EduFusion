const student = sessionStorage.getItem("user");
const standardId = GetUserData().standardID || 4;
const standardName = student ? student.standard.standardName : "4th";

// Function to fetch exams dynamically with delay
function fetchExams() {
    setTimeout(function () {
        $.ajax({
            url: "http://localhost:5190/api/exam/standard/1", // API to fetch exams
            type: "GET",
            dataType: "json",
            success: function (data) {
                console.log("Exams Data:", data); // Debugging
                if (data && data.length > 0) {
                    var exams = data.map(function (exam) {
                        return {
                            subject: exam.examName, // Adjust based on API response
                            date: exam.examDate,
                            time: exam.startTime,
                            venue: exam.classModel ? exam.classModel.className : "N/A"
                        };
                    });

                    examListView.setDataSource(new kendo.data.DataSource({ data: exams }));
                } else {
                    examListView.setDataSource(new kendo.data.DataSource({ data: [] }));
                }
            },
            error: function () {
                console.error("Failed to fetch exam data.");
            }
        });
    }, 1000); // 1000ms (1 second) delay before fetching exams
}

// Calculate and display countdown for each exam
function updateExamCountdowns() {
    $(".exam-countdown").each(function () {
        var examDate = new Date($(this).data("exam-date"));
        var now = new Date();
        var diffTime = examDate - now;
        var diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));

        if (diffDays > 0) {
            $(this).text(diffDays + " days remaining");
        } else {
            $(this).text("Today!").css("color", "#e74c3c").css("font-weight", "bold");
        }
    });
}

// Kendo Splitter for syllabus Summary
// Try to access the syllabus data safely
function fetchPerformanceData(standardId) {
    console.log("Fetching data using AJAX for standardID:", standardId);

    return $.ajax({
        url: `http://localhost:5190/api/subject-tracking/standard/${standardId}`,
        method: "GET",
        dataType: "json",
    }).then(function (data) {
        console.log("Fetched Data:", data);

        if (!Array.isArray(data)) {
            console.error("Invalid data format received from API");
            return [];
        }

        return data.map(item => ({
            category: item.subject?.subjectName || "Unknown Subject",
            value: item.percentage ?? 0
        }));
    }).catch(function (xhr, status, error) {
        console.error("AJAX Error:", status, error);
        return [];
    });
}

function initializeCharts(standardId) {
    fetchPerformanceData(standardId).done(function (performanceData) {
        if (!performanceData || performanceData.length === 0) {
            console.warn("No performance data available.");
            $("#performanceChart").html("<p style='color: red;'>No data available.</p>");
            return;
        }

        // **Destroy existing chart before creating a new one**
        const existingChart = $("#performanceChart").data("kendoChart");
        if (existingChart) {
            console.log("Destroying existing chart...");
            existingChart.destroy();
        }

        console.log("Creating new Kendo Chart...");
        $("#performanceChart").kendoChart({
            title: { text: "Subjects", font: "16px 'Segoe UI', sans-serif", color: "#333" },
            legend: { position: "bottom" },
            seriesDefaults: { type: "column", stack: false },
            series: [{
                name: "Current Grade",
                data: performanceData.map(p => p.value),
                color: "#4b6cb7",
                labels: { visible: true, template: "#= value #%" }
            }],
            valueAxis: { max: 100, title: { text: "Coverage (%)" } },
            categoryAxis: { categories: performanceData.map(p => p.category) },
            tooltip: { visible: true, template: "#= category #: #= value #%" },
            chartArea: { background: "white" },
            theme: "flat"
        });

        // **Auto-refresh every 30 seconds**
        setTimeout(() => initializeCharts(standardId), 30 * 1000);
    });
}


// Calculate and display countdown for each exam
function updateExamCountdowns() {
    $(".exam-countdown").each(function () {
        var examDate = new Date($(this).data("exam-date"));
        var now = new Date();
        var diffTime = examDate - now;
        var diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));

        if (diffDays > 0) {
            $(this).text(diffDays + " days remaining");
        } else {
            $(this).text("Today!").css("color", "#e74c3c").css("font-weight", "bold");
        }
    });
}

function animateElements() {
    $('.stat-card').each(function (index) {
        var $this = $(this);
        setTimeout(function () {
            $this.animate({
                opacity: 1,
                transform: 'translateY(0)'
            }, 300);
        }, index * 100);
    });

    $('.k-card').each(function (index) {
        var $this = $(this);
        setTimeout(function () {
            $this.animate({
                opacity: 1,
                transform: 'translateY(0)'
            }, 300);
        }, 400 + index * 150);
    });
}

$(document).ready(function () {

    $("#stdValue").text(standardName);

    // **Trigger Initialization on Page Load**
    initializeCharts(standardId);

    // Fetch subjects dynamically from API with a slight delay
    setTimeout(function () {
        $.ajax({
            url: `http://localhost:5190/api/standard/${standardId}`,
            type: "GET",
            success: function (data) {
                console.log("Subjects Data:", data); // Debugging
                if (data.subjects && data.subjects.length > 0) {
                    $("#subjectsListView").kendoListView({
                        dataSource: new kendo.data.DataSource({
                            data: data.subjects
                        }),
                        template: "<div class='subject-item'>#: subjectName #</div>"
                    });
                } else {
                    $("#subjectsListView").html("<p>No subjects found for this standard.</p>");
                }
            },
            error: function () {
                console.error("Error fetching subjects.");
                $("#subjectsListView").html("<p>Error fetching subjects.</p>");
            }
        });
    }, 500); // 500ms delay before fetching subjects

    // Kendo ListView for Upcoming Exams
    var examListView = $("#examListView").kendoListView({
        dataSource: new kendo.data.DataSource({
            data: [] // Initially empty, will be updated dynamically
        }),
        template: kendo.template($("#examTemplate").html())
    }).data("kendoListView");

    // Call the function to fetch exams
    fetchExams();

    // Kendo ComboBox for Class Selection
    $("#subjectsDropdown").kendoMultiSelect({
        placeholder: "Select Subjects",
        autoClose: false,
        tagMode: "single",
        animation: {
            open: {
                effects: "fadeIn",
                duration: 300
            }
        },
        dataSource: [] // Initially empty, will be populated dynamically
    });

    // Standard Dropdown
    $("#standardDropdown").kendoDropDownList({
        optionLabel: "Select Standard",
        dataTextField: "StandardName",
        dataValueField: "StandardID",
        dataSource: {
            transport: {
                read: {
                    url: "http://localhost:5190/api/student/40", // Your API for fetching standards
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var standardID = this.value();
            if (standardID) {
                fetchSubjects(standardID);
                fetchExams(standardID);
            }
        }
    });

    updateExamCountdowns();
    setInterval(updateExamCountdowns, 3600000); // Update every hour

    // Kendo ProgressBar for payment status
    $("#paymentProgress").kendoProgressBar({
        min: 0,
        max: 100,
        value: 65,
        type: "value",
        animation: {
            duration: 600
        }
    });

    // Kendo ActionSheet for Payments with improved styling
    $("#openActionSheet").click(function () {
        $("#paymentActionSheet").data("kendoActionSheet").open();
    });

    $("#paymentActionSheet").kendoActionSheet({
        items: [
            { text: "Pay Fees Online", icon: "dollar", cssClass: "action-pay", click: function () { alert("Redirecting to payment gateway..."); } },
            { text: "Download Fee Receipt", icon: "download", cssClass: "action-download", click: function () { alert("Generating receipt..."); } },
            { text: "View Payment History", icon: "history", cssClass: "action-history", click: function () { alert("Loading payment history..."); } },
            { text: "Fee Structure", icon: "file-txt", cssClass: "action-structure", click: function () { alert("Loading fee structure..."); } },
            { text: "Cancel", icon: "close", cssClass: "action-close", click: function () { } }
        ]
    });

    // Add animation effects for a more dynamic dashboard
    $('.stat-card, .k-card').css('opacity', '0').css('transform', 'translateY(20px)');

    setTimeout(animateElements, 100);
});