async function updateAuthor(event) {
    event.preventDefault();

    const firstName = $("#newFirstName").val();
    if (firstName.trim().length == 0) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj imię autora");
    }

    const lastName = $("#newLastName").val();
    if (lastName.trim().length == 0) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj nazwisko autora");
    }

    const url = new URL(window.location.href);
    const segments = url.pathname.split('/');
    const id = Number.parseInt(segments[segments.length - 1]);

    const data = {
        FirstName: firstName,
        LastName: lastName
    };

    $.ajax({
        url: '/authors/' + id,
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#firstName").html(firstName);
            $("#lastName").html(lastName);
            $("#fullName").html(firstName + " " + lastName);

            $("#feedback").css("color", "green");
            $("#feedback").html("Dane zostały zaktualizowane.");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            $("#feedback").css("color", "red");
            $("#feedback").html(errorMessage);
        }
    });
}

async function removeAuthor() {
    const confirmation = confirm("Czy chcesz usunąć tego autora?");

    if (!confirmation) {
        return;
    }

    const url = new URL(window.location.href);
    const segments = url.pathname.split('/');
    const id = Number.parseInt(segments[segments.length - 1]);

    $.ajax({
        url: '/authors/' + id,
        method: 'DELETE',
        dataType: 'json',
        success: function (resp) {
            $("#deleted-info").removeClass("d-none");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}

function clearFeedback() {
    $("#feedback").html("");
}
