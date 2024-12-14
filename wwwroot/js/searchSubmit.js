async function searchSubmit(event) {
    event.preventDefault();

    const pattern = $("#search-pattern").val();
    if (pattern != "")
        location.href = '/search/' + pattern;
}