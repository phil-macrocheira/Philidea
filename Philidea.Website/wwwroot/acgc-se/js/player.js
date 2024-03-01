var gender;
var selectedItemID;
var selectedItemObj;
var inventoryItems = new Array(15).fill(0);
var inventoryItemConditions = new Array(15).fill(0);
var inventoryIconURL;
var PresentIconURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/Present.png";
var player;
var playerID;
var playerExists = [];

playerData = {
    name,
    gender,
    face,
    suntan,
    suntanDays,
    birthday,
    reset,
    resetCount,
    savings,
    debt,
    handheldItem,
    clothing,
    wallet,
    background,
    inventoryItems,
    inventoryItemConditions,
    itemSelectedID,
    itemSelectedObj,
    inventoryIconURL,
    presentChecked,
};
function uploadSavePlayer() {
    selectedItemID = "0x0000";
    selectedItemObj = findObjByID(0);
    inventoryIconURL = "";

    updatePlayerData(0, "_1"); // PLAYER 1
    updatePlayerData(1, "_2"); // PLAYER 2
    updatePlayerData(2, "_3"); // PLAYER 3
    updatePlayerData(3, "_4"); // PLAYER 4
}
function updatePlayerData(Player, PlayerID) {
    player = Player;
    playerID = PlayerID;
    playerExists[player] = getPlayerExists("player_exists");
    updateTabs();

    if (playerExists[player] == 0) {
        return;
    }

    document.getElementById("player_name" + playerID).value = getStringPlayer("player_name");
    document.getElementById("player_gender" + playerID).value = getGender("player_gender");
    document.getElementById("player_face" + playerID).value = getFace("player_face");
    document.getElementById("player_suntan" + playerID).value = getNumberPlayer("player_suntan");
    document.getElementById("player_suntan_days_remaining" + playerID).value = getNumberPlayer("player_suntan_days_remaining");
    updateTan(playerID);
    document.getElementById("player_birthday" + playerID).value = getBirthday("player_birthday");
    document.getElementById("player_reset_code" + playerID).value = getReset("player_reset_code");
    document.getElementById("player_reset_count" + playerID).value = getNumberPlayer("player_reset_count");
    document.getElementById("player_wallet" + playerID).value = getNumberPlayer("player_wallet");
    document.getElementById("player_savings" + playerID).value = getNumberPlayer("player_savings");
    document.getElementById("player_debt" + playerID).value = getNumberPlayer("player_debt");
    document.getElementById("player_equipped_item" + playerID).value = getIDPlayer("player_equipped_item");
    $('#player_equipped_item' + playerID).trigger('change');
    document.getElementById("player_clothing" + playerID).value = getIDPlayer("player_clothing");
    $('#player_clothing' + playerID).trigger('change');
    updateBackground();
    getInventoryConditions("player_inventory_items_conditions");
    getInventory("player_inventory_items");
    getInventoryBackground("player_inventory_background");
    updateHandheld(playerID);
    updateClothing(playerID);
}
function getPlayerExists(variable) {
    let exist = getNumberPlayer(variable);

    if (exist == 1) {
        document.getElementById("tab_player_btn" + playerID).classList.remove("hidden");
    }
    else {
        document.getElementById("tab_player_btn" + playerID).classList.add("hidden");
    }

    return exist;
}
function updateTabs() {
    if (playerExists[1] == 0) {
        document.getElementById("tab_player_1").classList.remove("inner-tabcontent");
        document.getElementById("tab_player_btn_1").classList.remove("active");
        document.getElementById("tab_player_btn_1").classList.add("hidden");
    }
    else {
        document.getElementById("tab_player_1").classList.add("inner-tabcontent");
        document.getElementById("tab_player_btn_1").classList.remove("hidden");
        document.getElementById("tab_player_btn_1").classList.add("active");
    }
}
function getSaveDataPlayer(variable) {
    let jsonData = getJsonData(variable);
    let index = jsonData.index + (player * 9280);
    size = jsonData.size;

    var saveData = new Uint8Array(size);
    for (let i = 0; i < size; i++) {
        saveData[i] = saveFile[index + i];
    }
    return saveData;
}
function updateItem(PlayerID) {
    playerID = PlayerID;
    player = parseInt(playerID.replace("_", ""), 10) - 1;
    selectedItemID = $('#itemDropdownInventory' + playerID).val();
    selectedItemObj = findObjByID(Hex2Dec(selectedItemID));

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "updateItem",
            player: player,
            selectedItemID: selectedItemID,
        },
        success: function (response) {
            $('#itemImage' + playerID).prop('src', response.itemIconURL);
            inventoryIconURL = response.inventoryIconURL;
            console.log('UPDATEITEM SUCCESS', response);
        },
        error: function (error) {
            console.error('UPDATEITEM ERROR', error);
        }
    });
}
function updateHandheld(PlayerID) {
    var playerID = PlayerID;
    var player = parseInt(playerID.replace("_", ""), 10) - 1;
    var selectedItemID = $('#player_equipped_item' + playerID).val();
    var selectedItemObj = findObjByID(Hex2Dec(selectedItemID));

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "updateHandheld",
            player: player,
            selectedItemID: selectedItemID,
        },
        success: function (response) {
            $('#handheldImage' + playerID).prop('src', response.handheldIconURL);
            console.log('UPDATEHANDHELD SUCCESS', response);
        },
        error: function (error) {
            console.error('UPDATEHANDHELD ERROR', error);
        }
    });
}
function updateClothing(PlayerID) {
    var playerID = PlayerID;
    var player = parseInt(playerID.replace("_", ""), 10) - 1;
    var selectedItemID = $('#player_clothing' + playerID).val();
    var selectedItemObj = findObjByID(Hex2Dec(selectedItemID));

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "updateClothing",
            player: player,
            selectedItemID: selectedItemID,
        },
        success: function (response) {
            $('#clothingImage' + playerID).prop('src', response.clothingIconURL);
            console.log('UPDATECLOTHING SUCCESS', response);
        },
        error: function (error) {
            console.error('UPDATECLOTHING ERROR', error);
        }
    });
}
function updateInventoryIcons() {
    for (let i = 0; i < 15; i++) {
        var obj = findObjByID(inventoryItems[i]);
        var slot = $('.inventory' + playerID + ' .inventory-slot' + playerID).eq(i);
        var imgElement = slot.find('img');

        if (imgElement.length === 0) {
            let newimgElement = $('<img>');
            slot.append(newimgElement);
            imgElement = slot.find('img');
        }
        if (obj && obj.IconURLUpscaled) {
            if (inventoryItemConditions[i] == 1) {
                imgElement.attr('src', PresentIconURL);
                imgElement.attr('bubbletext', 'Present (' + obj.Name + ')');
            }
            else {
                imgElement.attr('src', obj.IconURLUpscaled);
                imgElement.attr('bubbletext', obj.Name);
            }
        }
        else {
            imgElement.remove();
        }
    }
}
function updateBackground() {
    let clothing = document.getElementById("player_inventory_background" + playerID).value;
    let clothingObj = findObjByID(Hex2Dec(clothing));
    let backgroundURL = clothingObj.TextureURLUpscaled;

    let styleTag = document.createElement('style');
    styleTag.innerHTML = '.section-inventory' + playerID + '::after { background-image: url("' + backgroundURL + '") !important; }';
    document.head.appendChild(styleTag);
}
function getInventoryBackground(variable) {
    document.getElementById("player_inventory_background" + playerID).value = getID(variable);
    $('#player_inventory_background' + playerID).trigger('change');
}
function getStringPlayer(variable) {
    let saveData = getSaveDataPlayer(variable);
    let saveDataString = replaceChars(saveData);
    return saveDataString;
};
function getNumberPlayer(variable) {
    let saveData = getSaveDataPlayer(variable);
    var value = 0;
    for (var i = 0; i < size; i++) {
        value = (value << 8) + saveData[i];
    }
    return value;
}
function getIDPlayer(variable) {
    let saveData = getSaveDataPlayer(variable);
    let ID = (saveData[0] << 8) | saveData[1];
    return Dec2Hex(ID);
}
function getBirthday(variable) {
    let saveData = getSaveDataPlayer(variable);
    let day = saveData[1];
    if (saveData[0] == 0xFF && day == 0xFF) {
        return null;
    }
    let month = saveData[0] - 1;
    let year = new Date().getFullYear();
    let date = new Date(year, month, day);
    return date.toISOString().split('T')[0];
}
function getGender(variable) {
    gender = getNumberPlayer(variable);
    return gender;
}
function getFace(variable) {
    var face = getNumberPlayer(variable);

    if (gender == 0) {
        if (face >= 0x10 && face < 0x20) {
            document.getElementById("checkboxBeestung" + playerID).checked = true;
            face -= 0x10;
        }
        else if (face >= 0x20 && face < 0x30) {
            document.getElementById("checkboxGyroidFace" + playerID).checked = true;
            face -= 0x20;
        }
        else if (face >= 0x30) {
            document.getElementById("checkboxBeestung" + playerID).checked = true;
            document.getElementById("checkboxGyroidFace" + playerID).checked = true;
            face -= 0x30;
        }
        return face;
    }
    if (gender == 1) {
        if (face >= 0x08 && face < 0x18) {
            document.getElementById("checkboxBeestung" + playerID).checked = true;
            face -= 0x10;
        }
        else if (face >= 0x18 && face < 0x28) {
            document.getElementById("checkboxGyroidFace" + playerID).checked = true;
            face -= 0x20;
        }
        else if (face >= 0x28 && face < 0xF8) {
            document.getElementById("checkboxBeestung" + playerID).checked = true;
            document.getElementById("checkboxGyroidFace" + playerID).checked = true;
            face -= 0x30;
        }
        else if (face >= 0xF8) {
            return face - 0xF8;
        }
        else {
            return face + 8;
        }
    }
}
function updateTan(PlayerID) {
    playerID = PlayerID;
    let tan = document.getElementById("player_suntan" + playerID).value;
    let tanColor = document.getElementById("tan-color" + playerID);

    if (tan == 0) {
        tanColor.style.backgroundColor = "#FFBD94";
    }
    else if (tan == 1) {
        tanColor.style.backgroundColor = "#EFAD84";
    }
    else if (tan == 2) {
        tanColor.style.backgroundColor = "#E7A57B";
    }
    else if (tan == 3) {
        tanColor.style.backgroundColor = "#D6946B";
    }
    else if (tan == 4) {
        tanColor.style.backgroundColor = "#CE8C63";
    }
    else if (tan == 5) {
        tanColor.style.backgroundColor = "#C6845A";
    }
    else if (tan == 6) {
        tanColor.style.backgroundColor = "#BD7B52";
    }
    else if (tan == 7) {
        tanColor.style.backgroundColor = "#B5734A";
    }
    else if (tan == 8) {
        tanColor.style.backgroundColor = "#A56339";
    }
    else {
        tanColor.style.backgroundColor = "#FFBD94";
    }
}
function getReset(variable) {
    let saveData = getSaveDataPlayer(variable);
    if (saveData[0] > 0) {
        document.getElementById("checkboxReset" + playerID).checked = true;
    }
}
function getInventoryConditions(variable) {
    let saveData = getSaveDataPlayer(variable);
    for (var i = 0; i < 15; i++) {
        let byte = 3 - Math.floor(i / 4);
        let bitOffset = (i % 4) * 2;
        let bits = (saveData[byte] >> bitOffset & 0b11); // 0 = None, 1 = Present, 2 = Quest
        inventoryItemConditions[i] = bits;
    }
}
function getInventory(variable) {
    let saveData = getSaveDataPlayer(variable);
    let dataView = new DataView(saveData.buffer);
    for (var i = 0; i < 30; i += 2) {
        let item = dataView.getInt16(i, false); /* false for big endian */
        inventoryItems[i / 2] = item;
    }
    updateInventoryIcons();
}

// FOCUS SEARCH FIELD ON DROPDOWN MENUS
$(document).ready(function () {
    $('#itemDropdownInventory' + playerID).select2();
    $('#itemDropdownInventory' + playerID).on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_inventory_background' + playerID).select2();
    $('#player_inventory_background' + playerID).on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_equipped_item' + playerID).select2();
    $('#player_equipped_item' + playerID).on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_clothing' + playerID).select2();
    $('#player_clothing' + playerID).on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });
});