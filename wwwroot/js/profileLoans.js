function extendLoan(loanId, elementId) {
    const confirmation = confirm("Czy chcesz wydłużyć wypożyczenie?");
    if (!confirmation) 
        return;
    
    $.ajax({
        url: '/loans/' + loanId + '/extend',
        method: 'PUT',
        contentType: 'application/json',
        success: function (response) {
            const dateString = $("#date-to_" + elementId).html();
            let [day, month, year] = dateString.split(".");
            const dateTo = new Date(`${year}-${month}-${day}`);

            dateTo.setMonth(dateTo.getMonth() + 1);
            day = String(dateTo.getDate()).padStart(2, '0');
            month = String(dateTo.getMonth() + 1).padStart(2, '0');
            year = dateTo.getFullYear();

            const today = new Date();
            const timeDifference = dateTo - today;
            const daysLeft = Math.ceil(timeDifference / (1000 * 60 * 60 * 24));

            $("#date-to_" + elementId).html(day + "." + month + "." + year);
            $("#days_left_" + elementId).html("Pozostało dni: " + daysLeft);
            alert("Termin zwrotu wypożyczenia został wydłużony do " + day + "." + month + "." + year);
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}

function cancelReservation(reservationId, elementId) {
    const confirmation = confirm("Czy chcesz anulować rezerwację?");
    if (!confirmation)
        return;

    $.ajax({
        url: '/reservations/' + reservationId + '/cancelreservation',
        method: 'PUT',
        contentType: 'application/json',
        success: function (response) {
            $("#reservation-info_" + elementId).removeClass("d-none");
            $("#cancel_button_" + elementId).remove();
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}