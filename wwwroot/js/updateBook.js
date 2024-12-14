async function loadAuthors() {
    const authorSelect = document.getElementById("newAuthor");

    if (authorSelect == null)
        return;

    const authorId = authorSelect.firstElementChild.value;

    const authorsResponse = await fetch("/authors");
    const authors = await authorsResponse.json();

    authors.sort((a, b) => {
        if (a.firstName < b.firstName) return -1; // a jest przed b
        if (a.firstName > b.firstName) return 1;  // a jest za b
        return 0; // a i b są równe
    });

    authors.forEach(author => {
        if (author.authorId != authorId) {
            const option = document.createElement("option");
            option.value = author.authorId;
            option.textContent = `${author.firstName} ${author.lastName}`;
            authorSelect.appendChild(option);
        }
    });
}

async function updateBook(event) {
    event.preventDefault();

    const authorId = Number.parseInt($("#newAuthor").val());

    if (authorId == NaN) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Wybierz prawidłowego autora.");
        return;
    }

    const author = $('option[value="' + authorId + '"]').html();
    const title = $("#newTitle").val();
    const quantity = Number.parseInt($("#newQuantity").val());

    if (quantity == NaN) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj prawidłową liczbę wszystkich egzemplarzy.");
        return;
    }

    if (quantity < Number.parseInt($("#unavailable-copies").html())) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Liczba wszystkich egzemplarzy nie może być mniejsza niż liczba wypożyczonych lub zarezerwowanych egzemplarzy.");
        return;
    }

    if (!isTitleValid(title)) {
        $("#feedback").css("color", "red");
        $("#feedback").html("Podaj prawidłowy tytuł.");
        return;
    }

    const url = new URL(window.location.href);
    const segments = url.pathname.split('/');
    const id = Number.parseInt(segments[segments.length - 1]);

    const data = {
        Title: title,
        Quantity: quantity,
        AuthorId: authorId
    };

    $.ajax({
        url: '/books/' + id,
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            $("#title").html(title);
            $("#author").html(author);
            $("#title-span").html(title);
            $("#author-span").html(author);
            $("#all-copies").html(quantity);
            $("#available-copies").html(quantity - Number.parseInt($("#unavailable-copies").html()));

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

function clearFeedback() {
    $("#feedback").html("");
}