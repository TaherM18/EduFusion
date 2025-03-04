
$(document).ready(function () {
    
    $.ajax({
        url: 'http://localhost:5190/api/student/45',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log('API Response:', response);

                const student = response;
                $('#studentId').text(student.studentID);
                $('#editStudentId').val(student.studentID);

                $('#studentFName-h').text(student.studentFName);
                $('#studentFName-l').text(student.studentFName);
                $('#editStudentFName').val(student.studentFName);

                $('#studentLName-h').text(student.user.firstName);
                $('#studentLName-l').text(student.user.firstName);
                $('#editStudentLName').val(student.user.firstName);

                $('#rollNumber').text(student.rollNumber);
                $('#editRollNumber').val(student.rollNumber);

                $('#guardianName').text(student.guardianName);
                $('#editGuardianName').val(student.guardianName);

                $('#guardianContact').text(student.guardianContact.trim());
                $('#editGuardianContact').val(student.guardianContact.trim());
                
                $('#section').text(student.section);
                $('#editSection').val(student.section); 

                $('#editStandardid').val(student.standard.standardID);

                $('#standardName').text(student.standard.standardName);
                $('#editStandardName').val(student.standard.standardName);

                $('#dateOfBirth').text(student.user.birthDate.split(' ')[0]);
                $('#editDateOfBirth').val(student.user.birthDate.split(' ')[0]);

                // Format dateOfBirth as 19-Mar-1982
                var dateOfBirth = student.user.birthDate.split('T')[0]; // Get '1982-03-19'
                var [year, month, day] = dateOfBirth.split('-');

                var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                var formattedDate = `${day}-${monthNames[parseInt(month, 10) - 1]}-${year}`;

                $('#dateOfBirth').text(formattedDate);
                $('#editDateOfBirth').val(formattedDate);

                $('#gender').text(student.user.gender);
                $('#editGender').val(student.user.gender);

                $('#email').text(student.user.email);
                $('#editEmail').val(student.user.email);

                $('#contact').text(student.user.contact.trim());
                $('#editContact').val(student.user.contact.trim());

                $('#address').text(student.user.address);
                $('#editAddress').val(student.user.address);

                $('#pinCode').text(student.user.pincode);
                $('#editPinCode').val(student.user.pincode);
           
        },
        error: function (xhr, status, error) {
            console.error('Error details:', xhr.responseText);
            alert('API request failed. Please try again later.');
        }
    });
});

function openEditDialog() {
    $('#editStudentDialog').fadeIn();
}

function closeEditDialog() {
    $('#editStudentDialog').fadeOut();
}
function saveProfile() {
    var formData = new FormData();

    formData.append('studentId', $('#editStudentId').val());
    formData.append('studentFName', $('#editStudentFName').val());
    formData.append('studentLName', $('#editStudentLName').val());
    formData.append('dateOfBirth', $('#editDateOfBirth').val());
    formData.append('guardianName', $('#editGuardianName').val());
    formData.append('guardianContact', $('#editGuardianContact').val());
    formData.append('RollNumber', $('#editRollNumber').val());
    formData.append('StandardName', $('#editStandardName').val());
    formData.append('section', $('#editSection').val());
    formData.append('gender', $('#editGender').val());
    formData.append('email', $('#editEmail').val());
    formData.append('contact', $('#editContact').val());
    formData.append('address', $('#editAddress').val());
    formData.append('pinCode', $('#editPinCode').val());

    $.ajax({
        url: 'http://localhost:5190/api/student/',
        type: 'PUT',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                alert('Profile updated successfully!');
                closeEditDialog();
                location.reload();
            } else {
                alert(response.message || 'Failed to update student profile.');
            }
        },
        error: function (xhr) {
            console.error('Error details:', xhr.responseText);
            alert('API request failed. Please try again later.');
        }
    });
}