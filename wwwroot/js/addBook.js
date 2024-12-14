async function loadAuthors() {
    const authorSelect = document.getElementById("author");

    if (authorSelect == null)
        return;

    const authorsResponse = await fetch("/authors");
    const authors = await authorsResponse.json();

    authors.sort((a, b) => {
        if (a.firstName < b.firstName) return -1; // a jest przed b
        if (a.firstName > b.firstName) return 1;  // a jest za b
        return 0; // a i b są równe
    });

    authors.forEach(author => {
        const option = document.createElement("option");
        option.value = author.authorId;
        option.textContent = `${author.firstName} ${author.lastName}`;
        authorSelect.appendChild(option);
    });
}

async function addBook(event) {
    event.preventDefault();

    const title = $("#title").val();
    const authorId = Number.parseInt($("#author").val());

    if (authorId == NaN) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Wybierz prawidłowego autora.");
        return;
    }

    const quantity = Number.parseInt($("#quantity").val());

    if (quantity == NaN) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj prawidłową liczbę wszystkich egzemplarzy.");
        return;
    }

    if (quantity < 0) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Liczba wszystkich egzemplarzy nie może być mniejsza od zera.");
        return;
    }

    if (!isTitleValid(title)) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj prawidłowy tytuł.");
        return;
    }

    const data = {
        Title: title,
        Quantity: quantity,
        AuthorId: authorId
    };

    $.ajax({
        url: '/books',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#feedback").css("color", "green");
            $("#feedback").html("Książka została dodana.");

            $("#title").val("");
            $("#quantity").val("");
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

function isTitleValid(title) {
    if (typeof title !== "string" || title.trim().length === 0) {
        return false;
    }

    if (title.length < 3 || title.length > 100) {
        return false;
    }

    const forbiddenChars = /[^a-zA-Z0-9 .,!?'"żźćńółęąśŻŹĆĄŚĘŁÓŃ]/;
    if (forbiddenChars.test(title)) {
        return false;
    }

    if (title.includes("  ")) {
        return false;
    }

    return true;
}