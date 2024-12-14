async function handleLogin(event) {
    event.preventDefault();

    const username = $("#login").val();
    const password = $("#password").val();

    const data = {
        Login: username,
        Password: password
    };

    $.ajax({
        url: '/users/login',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            location.href = "/users/data";
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            $("#errorMessage").html(errorMessage);
        }
    });
}

function clearError(event) {
    $("#errorMessage").html("");
}