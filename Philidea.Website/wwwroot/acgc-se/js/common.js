var saveFile;
var json;
var size;
var filename;

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
    uploadSave(debugMode);
}
function uploadSave(debug = false) {
    var formData = new FormData();
    var file = document.getElementById("fileUpload").files[0];
    formData.append('fileUpload', file);
    if (!debug) {
        filename = file.name;
    }

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

            document.getElementById("downloadButton").style.display = "inline-block";

            //console.log('Save File Uploaded', response);
        },
        error: function (error) {
            console.error('ERROR: Save File Upload Failed', error);
        }
    });
}
function downloadEditedSave() {
    writeSave();

    const link = document.createElement('a');
    link.href = `/acgc-se/acgc-save-editor?handler=Download&fileName=${filename}`;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function writeSave() {
    writePlayerData();
    
    const formData = new FormData();
    formData.append('fileName', filename);
    formData.append('fileContent', new Blob([saveFile]));

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: '/acgc-se/acgc-save-editor?handler=CreateSaveFile',
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            // success
        },
        error: function (error) {
            console.error('ERROR: Save file upload to server failed', error);
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
function getJsonData(name, localName = "") {
    let jsonDataObject = JSON.parse(json);
    let saveInfoElement = jsonDataObject[name];
    let offsetHex = saveInfoElement["Global Byte Offset"];
    let offset = Hex2Dec(offsetHex);
    let size = saveInfoElement["Byte Size"];

    if (localName != "") {
        saveInfoElement = jsonDataObject[localName];
        offsetHex = saveInfoElement["Local Byte Offset"];
        offset += Hex2Dec(offsetHex);
        size = saveInfoElement["Byte Size"];
    }

    return { index: offset, size: size };
}
function getSaveData(variable, offset, localVariable = "") {
    if (offset === undefined) {
        return getSaveData(variable, 0);
    }
    let jsonData = getJsonData(variable,local);
    let index = jsonData.index + offset;
    size = jsonData.size;

    var saveData = new Uint8Array(size);
    for (let i = 0; i < size; i++) {
        saveData[i] = saveFile[index + i];
    }
    return saveData;
}
function getString(variable, offset, localVariable="") {
    if (offset === undefined) {
        return getString(variable, 0);
    }
    let saveData = getSaveData(variable,offset,local);
    let saveDataString = replaceChars(saveData);
    return saveDataString;
};
function setString(variable, offset, value, localVariable = "") {
    let jsonData = getJsonData(variable, local);
    let index = jsonData.index + offset;
    let size = jsonData.size;

    for (let i = 0; i < size; i++) {
        saveFile[index + i] = value.charCodeAt(i) || 0x20; // Set character code or 0x20 if value is shorter than size
    }
}
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
function setNumber(variable, offset, value, localVariable = "") {
    let jsonData = getJsonData(variable, local);
    let index = jsonData.index + offset;
    let size = jsonData.size;

    for (let i = size - 1; i >= 0; i--) {
        saveFile[index + i] = value & 0xFF;
        value >>= 8;
    }
}
function getID(variable, offset) {
    if (offset === undefined) {
        return getID(variable, 0);
    }
    let saveData = getSaveData(variable,offset);
    let ID = (saveData[0] << 8) | saveData[1];
    return Dec2Hex(ID);
}
function setID(variable, offset, value, localVariable = "") {
    let jsonData = getJsonData(variable, local);
    let index = jsonData.index + offset;

    let ID = parseInt(value, 16);
    saveFile[index] = (ID >> 8) & 0xFF; // High byte
    saveFile[index + 1] = ID & 0xFF; // Low byte
}
function setIDbyte(variable, offset, value, localVariable = "") {
    let jsonData = getJsonData(variable, local);
    let index = jsonData.index + offset;

    let ID = parseInt(value, 16);
    saveFile[index] = ID & 0xFF;
}
function getYMD(variable) {
    let saveData = getSaveData(variable,offset);
    let year = saveData[0] << 8 | saveData[1];
    let month = saveData[2] - 1;
    let day = saveData[3];
    let date = new Date(year, month, day);
    return date.toISOString().split('T')[0];
}
function setYMD(variable, offset, dateString, localVariable = "") {
    let jsonData = getJsonData(variable, local);
    let index = jsonData.index + offset;

    let date = new Date(dateString);
    let year = date.getFullYear();
    let month = date.getMonth() + 1;
    let day = date.getDate();

    saveFile[index] = (year >> 8) & 0xFF; // High byte of year
    saveFile[index + 1] = year & 0xFF; // Low byte of year
    saveFile[index + 2] = month;
    saveFile[index + 3] = day;
}