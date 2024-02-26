function hookTooltipListeners() {
    document.querySelectorAll('.inventory-slot').forEach(function (slot) {
        slot.addEventListener('mousemove', function (e) {
            var title = null;
            var imgElements = slot.getElementsByTagName('img');
            var imgElement = imgElements[0];
            if (imgElement && imgElement.hasAttribute('bubbletext')) {
                title = slot.getElementsByTagName('img')[0].getAttribute('bubbletext');
            } else {
                return;
            }
            if (title !== null) {
                var tooltipText = document.querySelector('.ac-bubble-content p');
                tooltipText.innerText = title;
                var tooltip = document.querySelector('.ac-bubble');
                tooltip.style.display = 'block';
                var magicYnumber = document.body.getElementsByClassName("inventory")[0].getBoundingClientRect().top;
                var magicXnumber = document.body.getElementsByClassName("inventory")[0].getBoundingClientRect().left;
                var cellXSize = document.body.getElementsByClassName("inventory")[0].getBoundingClientRect().right - document.body.getElementsByClassName("inventory")[0].getBoundingClientRect().left;
                cellXSize = cellXSize / 5;
                tooltip.style.top = magicYnumber - (cellXSize / 2)  + ((Math.round((e.clientY - magicYnumber + (cellXSize / 2)) / cellXSize) * cellXSize )) + 'px'; // if grid is moved, adjust e.clientY +- y
                tooltip.style.left = magicXnumber + ((Math.round((e.clientX - magicXnumber - (cellXSize / 2)) / cellXSize) * cellXSize) + (cellXSize / 2)) + 'px'; // if grid is moved, adjust e.clientX +- x
            }
        });
        slot.addEventListener('mouseout', function () {
            var tooltip = document.querySelector('.ac-bubble');
            if (tooltip) {
                tooltip.style.display = 'none';
            }
        });
    });
}
document.addEventListener("DOMContentLoaded", function () {
    hookTooltipListeners();
});