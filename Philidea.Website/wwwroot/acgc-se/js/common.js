var saveFile;
var json;
var size;

// FORCE NUMBER INPUTS TO MIN/MAX
document.addEventListener("DOMContentLoaded", function () {
    let numberInputs = document.querySelectorAll('input[type="number"]');
    numberInputs.forEach(function (input) {
        input.addEventListener("input", function () {
            let max = parseInt(input.getAttribute("max"), 10);
            let value = parseInt(input.value, 10);

            if (value < 0) {
                input.value = 0;
            } else if (value > max) {
                input.value = max;
            }
        });
    });
});

if (debugMode) {
    uploadSave();
}
function uploadSave() {
    var formData = new FormData();
    var file = document.getElementById("fileUpload").files[0];
    formData.append('fileUpload', file);

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            saveFile = Uint8Array.from(atob(response.save_file), c => c.charCodeAt(0));
            json = response.json;

            uploadSavePlayer();

            document.getElementById("town_name").value = getString("town_name");
            document.getElementById("bulletin_board_post_01").value = getString("bulletin_board_post_01");
            document.getElementById("bulletin_board_post_02").value = getString("bulletin_board_post_02");

            //console.log('Save File Uploaded', response);
        },
        error: function (error) {
            console.error('ERROR: Save File Upload Failed', error);
        }
    });
}
function findObjByID(ID) {
    let IDStr = Dec2Hex(ID);
    return itemData.find(function (item) {
        return item.ID === IDStr;
    });
}
function Hex2Dec(Hex) {
    return parseInt(Hex, 16);
}
function Dec2Hex(Dec) {
    return '0x' + Dec.toString(16).padStart(4, '0').toUpperCase();
}
function getJsonData(name) {
    let jsonDataObject = JSON.parse(json);
    let saveInfoElement = jsonDataObject[name];
    let offsetHex = saveInfoElement["Global Byte Offset"];
    let offset = Hex2Dec(offsetHex);
    let size = saveInfoElement["Byte Size"];
    return { index: offset, size: size };
}
function getSaveData(variable, offset) {
    if (offset === undefined) {
        return getSaveData(variable, 0);
    }
    let jsonData = getJsonData(variable);
    let index = jsonData.index + offset;
    size = jsonData.size;

    var saveData = new Uint8Array(size);
    for (let i = 0; i < size; i++) {
        saveData[i] = saveFile[index + i];
    }
    return saveData;
}
function getString(variable, offset) {
    if (offset === undefined) {
        return getString(variable, 0);
    }
    let saveData = getSaveData(variable,offset);
    let saveDataString = replaceChars(saveData);
    return saveDataString;
};
function getNumber(variable, offset) {
    if (offset === undefined) {
        return getNumber(variable, 0);
    }
    let saveData = getSaveData(variable,offset);
    var value = 0;
    for (var i = 0; i < size; i++) {
        value = (value << 8) + saveData[i];
    }
    return value;
}
function getID(variable, offset) {
    if (offset === undefined) {
        return getID(variable, 0);
    }
    let saveData = getSaveData(variable,offset);
    let ID = (saveData[0] << 8) | saveData[1];
    return Dec2Hex(ID);
}
function getYMD(variable) {
    let saveData = getSaveData(variable,offset);
    let year = saveData[0] << 8 | saveData[1];
    let month = saveData[2] - 1;
    let day = saveData[3];
    let date = new Date(year, month, day);
    return date.toISOString().split('T')[0];
}