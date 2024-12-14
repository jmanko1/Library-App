document.addEventListener('DOMContentLoaded', () => {
    const filterInput = document.getElementById('filter-input');

    filterInput.addEventListener('input', () => {
        const filterValue = filterInput.value.toLowerCase();
        const rows = document.querySelectorAll('.container .row.justify-content-center');

        rows.forEach(row => {
            const nameElement = row.querySelector('.accordion-header .mb-1:first-child');
            if (nameElement) {
                const name = nameElement.textContent.toLowerCase();
                if (name.includes(filterValue)) {
                    row.style.display = '';
                } else {
                    row.style.display = 'none';
                }
            }
        });
    });
});

function cancelExpiredReservations() {
    const confirmation = confirm("Czy chcesz anulować przestarzałe rezerwacje?");
    if (!confirmation)
        return;

    $.ajax({
        url: '/reservations/cancelexpiredreservations',
        method: 'PUT',
        contentType: 'application/json',
        success: function (response) {
            const message = response.message;
            const ids = response.ids;

            if (ids.length != 0) {
                for (i = 0; i < ids.length; i++) {
                    $("#reservation-cancel-info_" + ids[i]).removeClass("d-none");
                }
            }

            alert(message);
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}

function startLoan(reservationId) {
    var confirmation = confirm("Czy chcesz rozpocząć wypożyczenie?");

    if (!confirmation)
        return;

    $.ajax({
        url: '/reservations/' + reservationId + '/start',
        method: 'PUT',
        contentType: 'application/json',
        success: function (response) {
            $("#reservation-start-info_" + reservationId).removeClass("d-none");
            $("#start_button_" + reservationId).remove();
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}