function addCSSLink() {
    const link = document.createElement('link');
    link.rel = 'stylesheet';
    link.href = '/css/settings.css';
    document.head.appendChild(link);
}

document.addEventListener('DOMContentLoaded', addCSSLink);

async function changeEmail(event) {
    event.preventDefault();

    const password = $("#email-password").val();
    const email = $("#newemail").val();

    const data = {
        NewEmail: email,
        Password: password
    };

    $.ajax({
        url: '/users/changeemail',
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#email-password").val("");
            $("#newemail").val("");

            $("#change-email-feedback").css("color", "green");
            $("#change-email-feedback").html("Adres email został pomyślnie zmieniony");

            $("#email").html(email);
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            $("#change-email-feedback").css("color", "red");
            $("#change-email-feedback").html(errorMessage);
        }
    });
}

async function changePassword(event) {
    event.preventDefault();

    const oldPasword = $("#password_old").val();
    const newPassword = $("#password1").val();
    const confirmPassword = $("#password2").val();

    if (newPassword.length < 8) {
        $("#change-password-feedback").css("color", "red");
        $("#change-password-feedback").html("Hasło musi mieć co najmniej 8 znaków.");
        return;
    }

    if (newPassword != confirmPassword) {
        $("#change-password-feedback").css("color", "red");
        $("#change-password-feedback").html("Hasła nie są identyczne.");
        return;
    }

    const data = {
        OldPassword: oldPasword,
        NewPassword: newPassword,
        ConfirmNewPassword: confirmPassword
    };

    $.ajax({
        url: '/users/changepassword',
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#password_old").val("");
            $("#password1").val("");
            $("#password2").val("");

            $("#change-password-feedback").css("color", "green");
            $("#change-password-feedback").html("Hasło zostało pomyślnie zmienione");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            $("#change-password-feedback").css("color", "red");
            $("#change-password-feedback").html(errorMessage);
        }
    });
}

function clearEmailFeedback(event) {
    $("#change-email-feedback").html("");
}

function clearPasswordFeedback(event) {
    $("#change-password-feedback").html("");
}