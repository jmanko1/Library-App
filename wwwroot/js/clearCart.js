window.onload = () => {
    localStorage.removeItem('cart');

    $("#cinfo").html("Informacja: Nasza biblioteka wykorzystuje do prawidłowego działania pliki cookies.");
    $("#cinfo").css("margin-top", "0px");

    $("#cookie").css("padding", "15px");
    $("#cookie").css("align-items", "center");
}
