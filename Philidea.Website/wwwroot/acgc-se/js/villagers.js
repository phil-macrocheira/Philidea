function updateVillagers() {
    var selectedVillagerIDs = $('#villagerDropdown').val() + "," + "0x4E";

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        url: '@Url.Page("Index/acgc-save-editor")',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "updateVillagers",
            selectedVillagerIDs: selectedVillagerIDs,
        },
        success: function (response) {
            document.getElementById("villagerImage").src = document.getElementById("villagerImage").src;
            console.log('UPDATEVILLAGERS SUCCESS', response);
        },
        error: function (error) {
            console.error('UPDATEVILLAGERS ERROR', error);
        }
    });
}

// FOCUS SEARCH FIELD ON DROPDOWN MENUS
$(document).ready(function () {
    $('#villagerDropdown').select2();
    $('#villagerDropdown').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });
});