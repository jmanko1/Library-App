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

function finishLoan(loanId, elementId) {
    const confirmation = confirm("Czy chcesz zakończyć wypożyczenie?");

    if (!confirmation)
        return;

    $.ajax({
        url: '/loans/' + loanId + '/finish',
        method: 'PUT',
        contentType: 'application/json',
        success: function (response) {
            $("#loan-info_" + elementId).removeClass("d-none");
            $("#finish_button_" + elementId).remove();
        },
        error: function (xhr, status, error) {
            const responseJson = JSON.parse(xhr.responseText);
            const errorMessage = responseJson.message;
            alert(errorMessage);
        }
    });
}