$(document).ready(function () {
    var dropdown = $('.js-example-basic-single');
    console.log(dropdown);
    console.log($.fn.select2);

    dropdown.select2({
        placeholder: 'Select an option',
        allowClear: true,
    });
});

function updateSelectedWeapon() {
    var selectedWeapon = $('#dropdown').val();
    $('#selectedWeaponDisplay').html('<h2>Selected Weapon: ' + selectedWeapon + '</h2>');

    // You can also use AJAX to update the server if needed
    // Example AJAX request:
    // $.post('/Index?handler=UpdateSelectedWeapon', { selectedWeapon: selectedWeapon });
}

$(document).ready(function () {
    var dropdown = $('.js-example-basic-single');
    console.log(dropdown);
    console.log($.fn.select2);

    dropdown.select2({
        placeholder: 'Select an option',
        allowClear: true,
    });
});