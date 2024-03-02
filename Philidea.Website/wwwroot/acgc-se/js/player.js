var playerDataArray = [];
var playerNum;
var playerOffset;
var itemSelectedID = "0x0000";
var itemSelectedURL = "acgc-se/ui-icons/None.png";
var inventoryIconSelectedURL = "";
var PresentIconURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/Present.png";

playerData = {
    exists: 1,
    name: "",
    gender: 0,
    face: 0,
    suntan: 0,
    suntanDays: 0,
    birthday: new Date(),
    reset: 0,
    resetCount: 0,
    savings: 0,
    debt: 0,
    handheldItemID: "0x0000",
    handheldItemURL: "acgc-se/ui-icons/None.png",
    clothingID: "0x2400",
    clothingURL: "acgc-se/ui-icons/None.png",
    wallet: 0,
    backgroundID: "0x24E2",
    backgroundURL: "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Textures_Upscaled/24E2_big_dot_shirt.png",
    inventoryItems: new Array(15).fill(0),
    inventoryItemConditions: new Array(15).fill(0),
};
for (let i = 0; i < 4; i++) {
    let newPlayerData = { ...playerData, inventoryItems: [...playerData.inventoryItems], inventoryItemConditions: [...playerData.inventoryItemConditions] };
    playerDataArray.push(newPlayerData);
}
function uploadSavePlayer() {
    createPlayerData(0); // PLAYER 1
    createPlayerData(1); // PLAYER 2
    createPlayerData(2); // PLAYER 3
    createPlayerData(3); // PLAYER 4
    updatePlayer1Tab();
    setupPlayerTab(0); // OPEN PLAYER 1 TAB
}
function createPlayerData(playerNumArg) {
    playerNum = playerNumArg;
    playerOffset = playerNum * 9280;
    let exists = getPlayerExists();
    playerDataArray[playerNum].exists = exists;
    if (exists == 0) { return; }

    let face = getNumber("player_face", playerOffset);
    let gender = getGender();

    document.getElementById("player_name").value = getString("player_name",playerOffset);
    document.getElementById("player_gender").value = getGender();
    document.getElementById("player_face").value = getFace(face,gender);
    document.getElementById("player_suntan").value = getNumber("player_suntan",playerOffset);
    document.getElementById("player_suntan_days_remaining").value = getNumber("player_suntan_days_remaining",playerOffset);
    document.getElementById("player_birthday").value = getBirthday();
    document.getElementById("player_reset_code").value = getReset();
    document.getElementById("player_reset_count").value = getNumber("player_reset_count",playerOffset);
    
    document.getElementById("player_savings").value = getNumber("player_savings",playerOffset);
    document.getElementById("player_debt").value = getNumber("player_debt", playerOffset);

    document.getElementById("player_equipped_item").value = getID("player_equipped_item",playerOffset);
    document.getElementById("player_clothing").value = getID("player_clothing",playerOffset);

    document.getElementById("player_wallet").value = getNumber("player_wallet", playerOffset);
    getInventoryBackground("player_inventory_background");
    getInventoryConditions("player_inventory_items_conditions");
    getInventory("player_inventory_items");

    updatePlayer(playerNum);
}
function getPlayerExists() {
    let exists = getNumber("player_exists", playerOffset);

    let playerNumStr = String(playerNum);
    unhideTab(exists,playerNumStr);
    return exists;
}
function unhideTab(exists, playerNumStr) {
    if (exists == 1)
        document.getElementById("tab_player_btn_" + playerNumStr).classList.remove("hidden");
    else
        document.getElementById("tab_player_btn_" + playerNumStr).classList.add("hidden");
}
function getGender() {
    let gender = getNumber("player_gender", playerOffset);
    playerDataArray[playerNum].gender = gender;
    return gender;
}
function getFace(face, gender) {
    let beestung = document.getElementById("checkboxBeestung");
    let gyroidFace = document.getElementById("checkboxGyroidFace");

    if (gender == 0) {
        if (face >= 0x10 && face < 0x20) {
            beestung.checked = true;
            gyroidFace.checked = false;
            face -= 0x10;
        }
        else if (face >= 0x20 && face < 0x30) {
            beestung.checked = false;
            gyroidFace.checked = true;
            face -= 0x20;
        }
        else if (face >= 0x30) {
            beestung.checked = true;
            gyroidFace.checked = true;
            face -= 0x30;
        }
        else {
            beestung.checked = false;
            gyroidFace.checked = false;
        }
    }
    else if (gender == 1) {
        if (face >= 0x08 && face < 0x18) {
            beestung.checked = true;
            gyroidFace.checked = false;
            face -= 0x08;
        }
        else if (face >= 0x18 && face < 0x28) {
            beestung.checked = false;
            gyroidFace.checked = true;
            face -= 0x18;
        }
        else if (face >= 0x28 && face < 0xF8) {
            beestung.checked = true;
            gyroidFace.checked = true;
            face -= 0x28;
        }
        else if (face >= 0xF8) {
            beestung.checked = false;
            gyroidFace.checked = false;
            face -= 0xF8;
        }
        else {
            beestung.checked = false;
            gyroidFace.checked = false;
            face += 8;
        }
    }
    return face;
}
function getBirthday() {
    let saveData = getSaveData("player_birthday",playerOffset);
    let day = saveData[1];
    if (saveData[0] == 0xFF && day == 0xFF) {
        return null;
    }
    let month = saveData[0] - 1;
    let year = new Date().getFullYear();
    let date = new Date(year, month, day);
    return date.toISOString().split('T')[0];
}
function getReset() {
    let saveData = getSaveData("player_reset_code", playerOffset);
    let reset = saveData[0];
    if (reset > 0) {
        document.getElementById("checkboxReset").checked = true;
    }
    return reset;
}
function getInventoryBackground(variable) {
    document.getElementById("player_inventory_background").value = getID(variable, playerOffset);
}
function getInventoryConditions(variable) {
    let saveData = getSaveData(variable, playerOffset);
    for (let i = 0; i < 15; i++) {
        let byte = 3 - Math.floor(i / 4);
        let bitOffset = (i % 4) * 2;
        let bits = (saveData[byte] >> bitOffset & 0b11); // 0 = None, 1 = Present, 2 = Quest
        playerDataArray[playerNum].inventoryItemConditions[i] = bits;
    }
}
function getInventory(variable) {
    let saveData = getSaveData(variable, playerOffset);
    let dataView = new DataView(saveData.buffer);
    let currentPlayerInventory = playerDataArray[playerNum].inventoryItems;

    for (let i = 0; i < 30; i += 2) {
        let item = dataView.getInt16(i, false); /* false for big endian */
        currentPlayerInventory[i / 2] = item;
    }
}
function updatePlayer(playerNumArg) {
    currentPlayerNum = playerNumArg;

    updateName();
    updateGender();
    updateFace();
    updateTan();
    updateTanDays();
    updateBirthday();
    updateReset();
    updateResetCount();
    updateSavings();
    updateDebt();
    updateWallet();

    $('#player_equipped_item').trigger('change'); // calls updateHandheld()
    $('#player_clothing').trigger('change'); // calls updateClothing()
    $('#player_inventory_background').trigger('change'); // calls updateBackground()
}
function updateName() {
    playerDataArray[currentPlayerNum].name = document.getElementById("player_name").value;
    //console.log("Player " + (currentPlayerNum+1) + " Name Updated");
}
function updateGender() {
    playerDataArray[currentPlayerNum].gender = document.getElementById("player_gender").value;
    //console.log("Player " + (currentPlayerNum+1) + " Gender Updated");
}
function updateFace() {
    let face = parseInt(document.getElementById("player_face").value);
    let gender = document.getElementById("player_gender").value;
    let beestung = document.getElementById("checkboxBeestung").checked;
    let gyroidFace = document.getElementById("checkboxGyroidFace").checked;

    if (gender == 0) {
        if (beestung)
            face += 0x10;
        if (gyroidFace)
            face += 0x20;
    }
    else if (gender == 1) {
        if (face >= 0x08) {
            face -= 0x08;
            if (beestung)
                face += 0x10;
            if (gyroidFace)
                face += 0x20;
        }
        else if (face < 0x08) {
            if (beestung && gyroidFace)
                face += 0x28;
            else if (gyroidFace)
                face += 0x18;
            else if (beestung)
                face += 0x08;
            else
                face += 0xF8;
        }
    }
    playerDataArray[currentPlayerNum].face = face;
    //console.log("Player " + (currentPlayerNum + 1) + " Face Updated");
}
function updateTan() {
    playerDataArray[currentPlayerNum].suntan = document.getElementById("player_suntan").value;
    updateTanColor();
    //console.log("Player " + (currentPlayerNum+1) + " Tan Updated");
}
function updateTanColor() {
    let tan = document.getElementById("player_suntan").value;
    let tanColor = document.getElementById("tan-color");

    if (tan == 0)
        tanColor.style.backgroundColor = "#FFBD94";
    else if (tan == 1)
        tanColor.style.backgroundColor = "#EFAD84";
    else if (tan == 2)
        tanColor.style.backgroundColor = "#E7A57B";
    else if (tan == 3)
        tanColor.style.backgroundColor = "#D6946B";
    else if (tan == 4)
        tanColor.style.backgroundColor = "#CE8C63";
    else if (tan == 5)
        tanColor.style.backgroundColor = "#C6845A";
    else if (tan == 6)
        tanColor.style.backgroundColor = "#BD7B52";
    else if (tan == 7)
        tanColor.style.backgroundColor = "#B5734A";
    else if (tan == 8)
        tanColor.style.backgroundColor = "#A56339";
    else
        tanColor.style.backgroundColor = "#FFBD94";
}
function updateTanDays() {
    playerDataArray[currentPlayerNum].suntanDays = document.getElementById("player_suntan_days_remaining").value;
    //console.log("Player " + (currentPlayerNum+1) + " Tan Days Updated");
}
function updateBirthday() {
    playerDataArray[currentPlayerNum].birthday = document.getElementById("player_birthday").value;
    //console.log("Player " + (currentPlayerNum+1) + " Birthday Updated");
}
function updateReset() {
    playerDataArray[currentPlayerNum].reset = document.getElementById("player_reset_code").value;
    //console.log("Player " + (currentPlayerNum+1) + " Reset Updated");
}
function updateResetCount() {
    playerDataArray[currentPlayerNum].resetCount = document.getElementById("player_reset_count").value;
    //console.log("Player " + (currentPlayerNum+1) + " Reset Count Updated");
}
function updateSavings() {
    playerDataArray[currentPlayerNum].savings = document.getElementById("player_savings").value;
    //console.log("Player " + (currentPlayerNum+1) + " Savings Updated");
}
function updateDebt() {
    playerDataArray[currentPlayerNum].debt = document.getElementById("player_debt").value;
    //console.log("Player " + (currentPlayerNum+1) + " Debt Updated");
}
function updateWallet() {
    playerDataArray[currentPlayerNum].wallet = document.getElementById("player_wallet").value;
    //console.log("Player " + (currentPlayerNum+1) + " Wallet Updated");
}
function updateInventoryIcons() {
    for (let i = 0; i < 15; i++) {
        var obj = findObjByID(playerDataArray[currentPlayerNum].inventoryItems[i]);
        var slot = $('.inventory' + ' .inventory-slot').eq(i);
        var imgElement = slot.find('img');

        if (imgElement.length === 0) {
            let newimgElement = $('<img>');
            slot.append(newimgElement);
            imgElement = slot.find('img');
        }
        if (obj && obj.IconURLUpscaled) {
            if (playerDataArray[currentPlayerNum].inventoryItemConditions[i] == 1) {
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
function updateHandheld() {
    let selectedItemID = $('#player_equipped_item').val();
    playerDataArray[currentPlayerNum].handheldItemID = selectedItemID;

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "getItemURLs",
            selectedItemID: selectedItemID,
        },
        success: function (response) {
            playerDataArray[currentPlayerNum].handheldItemURL = response.itemImageURL;
            $('#handheldImage').prop('src', response.itemImageURL);
            //console.log("Player " + (currentPlayerNum+1) + " Handheld Item Updated", response);
        },
        error: function (error) {
            console.log("ERROR: Player " + (currentPlayerNum+1) + " Handheld Item Update FAILED", response);
        }
    });
}
function updateClothing() {
    let selectedItemID = $('#player_clothing').val();
    playerDataArray[currentPlayerNum].clothingID = selectedItemID;

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "getItemURLs",
            selectedItemID: selectedItemID,
        },
        success: function (response) {
            playerDataArray[currentPlayerNum].clothingURL = response.itemImageURL;
            $('#clothingImage').prop('src', response.itemImageURL);
            //console.log("Player " + (currentPlayerNum+1) + " Clothing Updated", response);
        },
        error: function (error) {
            console.log("ERROR: Player " + (currentPlayerNum+1) + " Clothing Update FAILED", response);
        }
    });
}
function updateItem() {
    itemSelectedID = $('#itemDropdownInventory').val();

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "getItemURLs",
            selectedItemID: itemSelectedID,
        },
        success: function (response) {
            inventoryIconSelectedURL = response.inventoryIconURL;
            itemSelectedURL = response.itemImageURL;
            $('#itemImage').prop('src', response.itemImageURL);
            //console.log("Player " + (currentPlayerNum+1) + " Item Selected Updated", response);
        },
        error: function (error) {
            console.log("ERROR: Player " + (currentPlayerNum+1) + " Item Selected Update FAILED", response);
        }
    });
}
function updateBackground() {
    let clothing = document.getElementById("player_inventory_background").value;
    let clothingObj = findObjByID(Hex2Dec(clothing));
    let backgroundURL = clothingObj.TextureURLUpscaled;

    playerDataArray[currentPlayerNum].backgroundID = clothing;
    playerDataArray[currentPlayerNum].backgroundURL = backgroundURL;

    let styleTag = document.createElement('style');
    styleTag.innerHTML = '.section-inventory::after { background-image: url("' + backgroundURL + '") !important; }';
    document.head.appendChild(styleTag);

    //console.log("Player " + (currentPlayerNum+1) + " Background Updated");
}
function updatePlayer1Tab() {
    var tabElement = document.getElementById('inner-tab-border');
    document.getElementById('tab_player_btn_0').classList.add('active');

    if (playerDataArray[1].exists == 0) {
        document.getElementById("tab_player_btn_0").classList.add("hidden");
        tabElement.classList.remove('tabcontent-inner');
    }
    else {
        document.getElementById("tab_player_btn_0").classList.remove("hidden");
        tabElement.classList.add('tabcontent-inner');
    }
}
function setupPlayerTab(playerNumArg) {
    currentPlayerNum = playerNumArg;
    let face = playerDataArray[currentPlayerNum].face;
    let gender = playerDataArray[currentPlayerNum].gender;

    document.getElementById("player_name").value = playerDataArray[currentPlayerNum].name;
    document.getElementById("player_gender").value = gender;
    document.getElementById("player_face").value = getFace(face, gender);
    document.getElementById("player_suntan").value = playerDataArray[currentPlayerNum].suntan;
    updateTanColor();
    document.getElementById("player_suntan_days_remaining").value = playerDataArray[currentPlayerNum].suntanDays;
    document.getElementById("player_birthday").value = playerDataArray[currentPlayerNum].birthday;
    document.getElementById("player_reset_code").value = playerDataArray[currentPlayerNum].reset;
    document.getElementById("player_reset_count").value = playerDataArray[currentPlayerNum].resetCount;
    document.getElementById("player_savings").value = playerDataArray[currentPlayerNum].savings;
    document.getElementById("player_debt").value = playerDataArray[currentPlayerNum].debt;
    document.getElementById("player_wallet").value = playerDataArray[currentPlayerNum].wallet;

    document.getElementById("player_equipped_item").value = playerDataArray[currentPlayerNum].handheldItemID;
    document.getElementById("player_clothing").value = playerDataArray[currentPlayerNum].clothingID;
    document.getElementById("player_inventory_background").value = playerDataArray[currentPlayerNum].backgroundID;

    // Not ideal to re-contact server but it works
    $('#player_equipped_item').trigger('change');
    $('#player_clothing').trigger('change');
    $('#player_inventory_background').trigger('change');

    updateInventoryIcons();
}
// UPDATE INVENTORY SLOT WHEN CLICKED
$('.inventory-slot').click(function () {
    let itemSelectedIDNum = Hex2Dec(itemSelectedID);
    let selectedItemName = findObjByID(itemSelectedIDNum).Name;
    let slot = $(this).index();
    playerDataArray[currentPlayerNum].inventoryItems[slot] = itemSelectedIDNum;
    var $this = $(this);

    if (inventoryIconSelectedURL == "") {
        playerDataArray[currentPlayerNum].inventoryItems[slot] = 0;
        $this.html('<img src="">');
    }
    else if (document.getElementById("checkboxPresent").checked) {
        playerDataArray[currentPlayerNum].inventoryItemConditions[slot] = 1;
        $this.html('<img src="' + PresentIconURL + '" bubbletext="' + 'Present (' + selectedItemName + ')' + '">');
    }
    else {
        $this.html('<img src="' + inventoryIconSelectedURL + '" bubbletext="' + selectedItemName + '">');
    }
});
// FOCUS SEARCH FIELD ON DROPDOWN MENUS
$(document).ready(function () {
    $('#itemDropdownInventory').select2();
    $('#itemDropdownInventory').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_inventory_background').select2();
    $('#player_inventory_background').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_equipped_item').select2();
    $('#player_equipped_item').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });

    $('#player_clothing').select2();
    $('#player_clothing').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });
});