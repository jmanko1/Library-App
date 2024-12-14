async function handleRegistration(event) {
    event.preventDefault();

    const username = $("#login").val();
    if (username.length < 4) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Nazwa użytkownika musi mieć co najmniej 4 znaki.");
        return;
    }

    const email = $("#email").val();
    const password = $("#password").val();
    if (password.length < 8) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Hasło musi mieć co najmniej 8 znaków.");
        return;
    }

    const confirmPassword = $("#confirmPassword").val();
    if (confirmPassword != password) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Hasła nie są identyczne.");
        return;
    }

    const firstname = $("#firstname").val();
    const lastname = $("#lastname").val();
    const street = $("#street").val();
    const city = $("#city").val();

    const data = {
        Login: username,
        Email: email,
        Password: password,
        ConfirmPassword: confirmPassword,
        FirstName: firstname,
        LastName: lastname,
        Street: street,
        City: city
    };

    $.ajax({
        url: '/users/register',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#login").val("");
            $("#email").val("");
            $("#password").val("");
            $("#confirmPassword").val("");
            $("#firstname").val("");
            $("#lastname").val("");
            $("#street").val("");
            $("#city").val("");

            $("#feedback").css("color", "green");
            $("#feedback").html("Rejestracja przebiegła pomyślnie. Możesz się teraz zalogować.");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            $("#feedback").css("color", "red");
            $("#feedback").html(errorMessage);
        }
    });
}

function clearFeedback(event) {
    $("#feedback").html("");
}