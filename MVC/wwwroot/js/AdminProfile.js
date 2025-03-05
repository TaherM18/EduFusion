$(document).ready(function () {

    console.log("GetUserData():", GetUserData());
    console.log("UserID:", GetUserData()?.UserID);


    $.ajax({
        url: 'http://localhost:5190/api/admin/' + GetUserData().userID,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            console.log('API Response:', response);

            const admin = response;
            $('#adminId').text(admin.adminID);
            $('#editAdminId').val(admin.adminID);

            $('#adminFName-h').text(admin.adminFName);
            $('#adminFName-l').text(admin.adminFName);
            $('#editAdminFName').val(admin.adminFName);

            $('#adminLName-h').text(admin.user.firstName);
            $('#adminLName-l').text(admin.user.firstName);
            $('#editAdminLName').val(admin.user.firstName);

            $('#email').text(admin.user.email);
            $('#editEmail').val(admin.user.email);

            $('#contact').text(admin.user.contact.trim());
            $('#editContact').val(admin.user.contact.trim());

            $('#dateOfBirth').text(admin.user.birthDate.split(' ')[0]);
            $('#editDateOfBirth').val(admin.user.birthDate.split(' ')[0]);

            var dateOfBirth = admin.user.birthDate.split('T')[0];
            var [year, month, day] = dateOfBirth.split('-');

            var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var formattedDate = `${day}-${monthNames[parseInt(month, 10) - 1]}-${year}`;

            $('#dateOfBirth').text(formattedDate);
            $('#editDateOfBirth').val(formattedDate);

            $('#gender').text(admin.user.gender);
            $('#editGender').val(admin.user.gender);

            $('#address').text(admin.user.address);
            $('#editAddress').val(admin.user.address);

            $('#pinCode').text(admin.user.pincode);
            $('#editPinCode').val(admin.user.pincode);

        },
        error: function (xhr, status, error) {
            console.error('Error details:', xhr.responseText);
            alert('API request failed. Please try again later.');
        }
    });
});

function openEditDialog() {
    $('#editAdminDialog').fadeIn();
}

function closeEditDialog() {
    $('#editAdminDialog').fadeOut();
}

function saveProfile() {
    var formData = new FormData();

    formData.append('adminId', $('#editAdminId').val());
    formData.append('adminFName', $('#editAdminFName').val());
    formData.append('adminLName', $('#editAdminLName').val());
    formData.append('dateOfBirth', $('#editDateOfBirth').val());
    formData.append('gender', $('#editGender').val());
    formData.append('email', $('#editEmail').val());
    formData.append('contact', $('#editContact').val());
    formData.append('address', $('#editAddress').val());
    formData.append('pinCode', $('#editPinCode').val());

    $.ajax({
        url: 'http://localhost:5190/api/admin/',
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
                alert(response.message || 'Failed to update admin profile.');
            }
        },
        error: function (xhr) {
            console.error('Error details:', xhr.responseText);
            alert('API request failed. Please try again later.');
        }
    });
}
