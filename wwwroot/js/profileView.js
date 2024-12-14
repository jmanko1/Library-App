function changeView(n) {
    switch (n) {
        case 1:
            $("#loansSpan").addClass("active");
            $("#reservationsSpan").removeClass("active");
            $("#userdataSpan").removeClass("active");

            $("#loans").removeClass("d-none");
            $("#reservations").addClass("d-none");
            $("#userdata").addClass("d-none");

            break;
        case 2:
            $("#loansSpan").removeClass("active");
            $("#reservationsSpan").addClass("active");
            $("#userdataSpan").removeClass("active");

            $("#loans").addClass("d-none");
            $("#reservations").removeClass("d-none");
            $("#userdata").addClass("d-none");

            break;
        default:
            $("#loansSpan").removeClass("active");
            $("#reservationsSpan").removeClass("active");
            $("#userdataSpan").addClass("active");

            $("#loans").addClass("d-none");
            $("#reservations").addClass("d-none");
            $("#userdata").removeClass("d-none");
    }
}

function showActiveLoans() {
    $("#dropdown-loans-content").html("Aktywne wypożyczenia");
    $(".actualLoanClass").removeClass("d-none");
    $(".finishedLoanClass").addClass("d-none");
}

function showFinishedLoans() {
    $("#dropdown-loans-content").html("Zakończone wypożyczenia");
    $(".actualLoanClass").addClass("d-none");
    $(".finishedLoanClass").removeClass("d-none");
}

function showActiveReservations() {
    $("#dropdown-reservations-content").html("Aktywne rezerwacje");
    $(".actualReservationClass").removeClass("d-none");
    $(".cancelledReservationClass").addClass("d-none");
}

function showCancelledReservations() {
    $("#dropdown-reservations-content").html("Anulowane rezerwacje");
    $(".actualReservationClass").addClass("d-none");
    $(".cancelledReservationClass").removeClass("d-none");
}