var letterItemSelectedID = "0x0000";
var letterItemSelectedURL = "acgc-se/ui-icons/None.png";
var letterIconSelectedURL = "";
var LetterUnreadURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_in.png";
var LetterReadURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_in_open.png";
var LetterReadPresentURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_in_open_present.png";
var LetterUnreadPresentURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_in_present.png";
var LetterOutgoingURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_out_open.png";
var LetterOutgoingPresentURL = "https://raw.githubusercontent.com/TOTKSheet/ACGCImages/main/Items/Icon_Upscaled/letter_out_open_present.png";
function getLetters(variable) {
    for (let i = 0; i < 10; i++) {
        let offset = playerOffset + (i * 0x12A);

        let type = getNumber(variable, offset, "letter_type");

        let recipient = getString(variable, offset, "letter_recipient");
        playerDataArray[playerNum].lettersRecipient[i] = recipient;
        let senderRaw = getString(variable, offset, "letter_sender");
        let sender = getSender(senderRaw,type);
        playerDataArray[playerNum].lettersSender[i] = sender;

        let recipientType = getNumber(variable, offset, "letter_recipient_type");
        let senderType = getNumber(variable, offset, "letter_sender_type");
        let letterExists = getLetterExists(recipientType, senderType);
        playerDataArray[playerNum].lettersExist[i] = letterExists;

        let present = getID(variable, offset, "letter_present");
        playerDataArray[playerNum].lettersPresent[i] = present;
        let statusNum = getNumber(variable, offset, "letter_status");
        let [read, outgoing] = getLetterStatus(statusNum);
        playerDataArray[playerNum].lettersRead[i] = read;
        playerDataArray[playerNum].lettersOutgoing[i] = outgoing;
        playerDataArray[playerNum].lettersIconURL[i] = GetLetterIconURL(present,read,outgoing);

        if (letterExists) {
            console.log(`${sender}: ${type}`);
        }

        let name_index = getNumber(variable, offset, "letter_name_index");
        playerDataArray[playerNum].lettersStationery[i] = getIDbyte(variable, offset, "letter_stationery");
        let headerRaw = getString(variable, offset, "letter_header");
        let header = getHeader(headerRaw, name_index, recipient, type);
        updateName();
        let body = getString(variable, offset, "letter_body");
        let footer = getString(variable, offset, "letter_footer");
        playerDataArray[playerNum].lettersText[i] = `${header}\n\n${body}\n\n${footer}`;

        let senderClass = "sender";
        let recipientClass = "recipient";
        let senderArticle = "";
        let recipientArticle = "";

        if (sender.includes("Museum")) {
            senderArticle = "the ";
            senderClass = "museum";
        }
        if (recipient.includes("Museum")) {
            recipientArticle = "the ";
            recipientClass = "museum";
        }

        let senderHTML = `<span class="${senderClass}">${sender}</span>`;
        let recipientHTML = `<span class="${recipientClass}">${recipient}</span>`;

        let name = `Letter to\n${recipientArticle}${recipientHTML}\nfrom ${senderArticle}${senderHTML}`;
        playerDataArray[playerNum].lettersBubbleText[i] = name;
    }
}
function getLetterExists(recipientType, senderType) {
    if (recipientType == 0xFF && senderType == 0xFF) {
        return false;
    }
    return true;
}
function getLetterStatus(statusNum) {
    let read = false;
    let outgoing = false;

    if (statusNum == 2 || statusNum == 4) {
        read = true;
    }
    if (statusNum == 1) {
        outgoing = true;
    }

    return [read, outgoing];
}
function getHeader(headerRaw, name_index, recipient, type) {
    if (type == 2 || type == 3) {
        return headerRaw;
    }
    return headerRaw.slice(0, name_index) + recipient + headerRaw.slice(name_index);
}
function getSender(senderRaw, type) {
    if (type == 2 || type == 3) {
        return "Tom Nook";
    }
    if (type == 6) {
        return "HRA";
    }
    return senderRaw;
}
function GetLetterIconURL(present, read, outgoing) {
    let hasPresent = false;
    if (present != "0x0000") {
        hasPresent = true;
    }

    if (outgoing) {
        if (hasPresent) {
            return LetterOutgoingPresentURL;
        }
        else {
            return LetterOutgoingURL;
        }
    }
    else {
        if (read) {
            if (hasPresent) {
                return LetterReadPresentURL;
            }
            else {
                return LetterReadURL;
            }
        }
        else {
            if (hasPresent) {
                return LetterUnreadPresentURL;
            }
            else {
                return LetterUnreadURL;
            }
        }
    }
}
function updateLetterIcons() {
    for (let i = 0; i < 10; i++) {
        var exists = playerDataArray[currentPlayerNum].lettersExist[i];
        var bubbleText = playerDataArray[currentPlayerNum].lettersBubbleText[i];
        var letterIconURL = playerDataArray[currentPlayerNum].lettersIconURL[i];
        var slot = $('.letter-slot').eq(i);
        var imgElement = slot.find('img');

        if (imgElement.length === 0) {
            let newimgElement = $('<img>');
            slot.append(newimgElement);
            imgElement = slot.find('img');
        }
        if (exists) {
            imgElement.attr('src', letterIconURL);
            slot.find('.bubble').html(bubbleText);
            slot.addClass('letter-slot-item');
        }
        else {
            imgElement.remove();
            slot.find('.bubble').html('');
            slot.removeClass('letter-slot-item');
        }
    }
}
function updateItemLetter() {
    letterItemSelectedID = $('#itemDropdownLetter').val();

    $.ajax({
        headers: {
            RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        type: 'POST',
        url: uploadURL,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            func: "getItemURLs",
            selectedItemID: letterItemSelectedID,
        },
        success: function (response) {
            letterIconSelectedURL = response.inventoryIconURL;
            letterItemSelectedURL = response.itemImageURL;
            $('#itemLetterImage').prop('src', response.itemImageURL);
            //console.log("Player " + (playerNum+1) + " Item Selected Updated", response);
        },
        error: function (error) {
            console.log("ERROR: Player " + (playerNum + 1) + " Letter Item Selected Update FAILED", response);
        }
    });
}
// FOCUS SEARCH FIELD ON DROPDOWN MENUS
$(document).ready(function () {
    $('#itemDropdownLetter').select2();
    $('#itemDropdownLetter').on('select2:open', function () {
        window.setTimeout(function () {
            document.querySelector('.select2-search__field').focus();
        }, 0);
    });
});