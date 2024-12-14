async function searchAuthor(event) {
    event.preventDefault();

    const pattern = $("#author-pattern").val();
    if (pattern == "" || pattern == null) {
        return;
    }

    const authorsResponse = await fetch("/authors/search/" + pattern);
    const authors = await authorsResponse.json();

    authors.sort((a, b) => {
        if (a.firstName < b.firstName) return -1; // a jest przed b
        if (a.firstName > b.firstName) return 1;  // a jest za b
        return 0; // a i b są równe
    });

    $(".author").remove();

    const authorsContainer = document.getElementById("authors-container");
    authors.forEach(author => {
        const authorRow = document.createElement("div");
        authorRow.classList.add("row", "author", "justify-content-center", "mb-2", "border", "p-3");

        const authorContent = `
            <div class="col">
                <div class="row align-items-center">
                    <div class="col">
                        <h2 style="font-size: 17px;">${author.firstName} ${author.lastName}</h2>
                    </div>
                    <div class="col text-end">
                        <a href="/authors/${author.authorId}">
                            <button class="btn btn-primary">Edytuj</button>
                        </a>
                    </div>
                </div>
            </div>
        `;

        authorRow.innerHTML = authorContent;
        authorsContainer.appendChild(authorRow);
    });

}
