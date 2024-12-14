window.onload = () => {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    $("#cart-count").html(cart.length);

    $("#cinfo").html("Informacja: Nasza biblioteka wykorzystuje do prawidłowego działania pliki cookies.");
    $("#cinfo").css("margin-top", "0px");

    $("#cookie").css("padding", "15px");
    $("#cookie").css("align-items", "center");

    if (location.href.includes("/books")) {
        const url = new URL(window.location.href);
        const segments = url.pathname.split('/');
        const id = Number.parseInt(segments[segments.length - 1]);

        const title = $("#title").html().trim();
        const author = $("#author").html().trim();

        addCartAction(id, title, author);

        try {
            loadAuthors();
        } catch (e) {
            ;
        }
    }

    if (location.href.includes("/cart"))
        showCart();
}

function addCartAction(id, title, name) {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const book = {
        id: id,
        title: title,
        author: name
    };

    const exists = cart.some(item => item.id === book.id && item.title === book.title && item.author === book.author);

    if (!exists) {
        $("#cart-action").html(`<button id="add-to-cart" class="btn btn-primary">Dodaj do koszyka rezerwacji</button>`);
        $("#add-to-cart").on("click", () => addToCart(id, title, name));
    } else {
        $("#cart-action").html(`<button id="remove-from-cart" class="btn btn-primary">Usuń z koszyka rezerwacji</button>`);
        $("#remove-from-cart").on("click", () => removeFromCart(id, title, name));
    }

}

function showCart() {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    let cartHTML = ``;

    for (let i = 0; i < cart.length; i++) {
        cartHTML += `
            <div class="row justify-content-center mb-3 align-items-center border p-3" id="book-` + cart[i].id + `">
                <div class="col-2 d-none d-md-inline">
                    <img src="/images/book.png" height="80" width="80">
                </div>
                <div class="col">
                    <div>
                        <h2 style="font-size: 17px; margin-bottom: 3px;">
                            <a href="/books/` + cart[i].id + `" class="fw-bold text-decoration-none">` + cart[i].title + `</a>
                        </h2>
                    </div>
                    <div class="mb-1">` + cart[i].author + `</div>
                </div>
                <div class="col text-end">
                    <button class="btn btn-danger" id="remove-button-` + i + `">Usuń</button>
                </div>
            </div>`;
    }

    $("#cart-content").html($("#cart-content").html() + cartHTML);

    buttonHTML = `
            <div class="row justify-content-center mt-4">
                <div class="col text-center">
                    <button class="btn btn-primary" id="reservation-button">Dokonaj rezerwacji</button>
                </div>
            </div>
        `;
    $("#cart-content").html($("#cart-content").html() + buttonHTML);
    $("#reservation-button").on("click", () => prepareReservation());

    if (cart.length == 0) {
        $("#reservation-button").attr("disabled", "");
    }

    for (let i = 0; i < cart.length; i++) {
        $("#remove-button-" + i).on("click", () => removeFromCart(cart[i].id, cart[i].title, cart[i].author));
    }
}

function addToCart(id, title, name) {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const book = {
        id: id,
        title: title,
        author: name
    };
    const exists = cart.some(item => item.id === book.id && item.title === book.title && item.author === book.author);

    if (exists)
        return;

    if (cart.length >= 3) {
        alert("Koszyk jest już pełny.");
        return;
    }

    cart.push(book);
    $("#cart-count").html(cart.length);
    localStorage.setItem('cart', JSON.stringify(cart));

    $("#cart-action").html(`<button id="remove-from-cart" class="btn btn-primary">Usuń z koszyka rezerwacji</button>`);
    $("#remove-from-cart").on("click", () => removeFromCart(id, title, name));
}

function removeFromCart(id, title, name) {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];

    const book = {
        id: id,
        title: title,
        author: name
    };

    const isEqual = (obj1, obj2) =>
        Object.keys(obj2).every(key => obj1[key] === obj2[key]);

    const updatedCart = cart.filter(obj => !isEqual(obj, book));
    localStorage.setItem('cart', JSON.stringify(updatedCart));
    $("#cart-count").html(updatedCart.length);

    if (location.href.includes("/books/")) {
        $("#cart-action").html(`<button id="add-to-cart" class="btn btn-primary">Dodaj do koszyka rezerwacji</button>`);
        $("#add-to-cart").on("click", () => addToCart(id, title, name));
    } else if (location.href.includes("/cart")) {
        $("#book-" + id).remove();
        if (updatedCart.length == 0) {
            $("#reservation-button").attr("disabled", "");
        }
    }
}

async function sendReservation(books) {
    const ids = [];
    for (let i = 0; i < books.length; i++) {
        ids.push(books[i].id);
    }

    const data = {
        bookIds: ids
    };

    $.ajax({
        url: '/reservations',
        method: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(data),
        success: function (response) {
            localStorage.removeItem('cart');
            $("#cart-count").html("0");

            let respHTML = `<div class="row justify-content-center mb-2">
                                <div class="col text-center">
                                    <div class="alert alert-success" role="alert">
                                      Rezerwacja pomyślnie dokonana.
                                    </div>
                                </div>
                            </div>
                            <div class="row justify-content-center mb-3">
                                <div class="col text-center">
                                    <h1 style="font-size: 18px; font-weight: bold;">Twoja rezerwacja:</h1>
                                </div>
                            </div>
                            `;

            respHTML += `
                    <div class="row justify-content-center align-items-center">
                        <div class="col">
                            <div class="accordion" id="reservation-accordion">
                                <div class="accordion-item">
                                    <h2 class="accordion-header">
                                        <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="true" aria-controls="collapseOne">
                                            <div>
                                                <div class="mb-1">Rezerwacja ID ` + response.loanId + `</div>
                                                <div class="mb-1">Data rezerwacji: ` + response.dateFrom + `</div>
                                                <div>Termin rezerwacji:
                                                    <span>` + response.dateTo + `</span>
                                                </div>
                                            </div>
                                        </button>
                                    </h2>
                                    <div id="collapseOne" class="accordion-collapse collapse show" data-bs-parent="#reservation-accordion">
                                        <div class="accordion-body">
                                            <ul>`;

            for (let i = 0; i < books.length; i++)
                respHTML += `<li>`+ books[i].author +` - ` + books[i].title + `</li>`;
            
            respHTML += `</ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
            `;

            $("#cart-content").html(respHTML);
            $("#reservation-feedback-div").removeClass("d-none");
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;

            const errorHTML = `
                <div class="alert alert-danger" role="alert">
                  ` + errorMessage + `
                </div>
            `;

            $("#reservation-feedback").html(errorHTML);
            $("#reservation-feedback-div").removeClass("d-none");
        }
    });
}

function prepareReservation() {
    const confirmation = confirm("Czy chcesz zarezerwować wybrane książki?");
    if (!confirmation)
        return;

    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const uniqueBooks = Array.from(
        new Map(cart.map(book => [book.id, book])).values()
    );

    if (uniqueBooks.length == 0 || uniqueBooks.length > 3)
        return;

    sendReservation(uniqueBooks);
}
