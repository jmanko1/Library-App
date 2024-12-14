async function removeBook() {
    const confirmation = confirm("Czy chcesz usunąć ten tytuł?");

    if (!confirmation) {
        return;
    }

    const url = new URL(window.location.href);
    const segments = url.pathname.split('/');
    const id = Number.parseInt(segments[segments.length - 1]);

    $.ajax({
        url: '/books/' + id,
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