async function addAuthor(event) {
    event.preventDefault();

    const firstName = $("#firstName").val();
    const lastName = $("#lastName").val();

    if (firstName.trim().length == 0) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj imię autora");
        return;
    }

    if (lastName.trim().length == 0) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj nazwisko autora");
        return;
    }

    const data = {
        FirstName: firstName,
        LastName: lastName
    };

    $.ajax({
        url: '/authors',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#feedback").css("color", "green");
            $("#feedback").html("Autor został dodany.");

            $("#firstName").val("");
            $("#lastName").val("");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            $("#feedback").css("color", "red");
            $("#feedback").html(errorMessage);
        }
    });
}

function clearFeedback() {
    $("#feedback").html("");
}