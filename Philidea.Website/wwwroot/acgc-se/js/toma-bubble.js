function hookTooltipListeners() {
    document.querySelectorAll('.inventory-slot').forEach(function (slot) {
        slot.addEventListener('mousemove', function (e) {
            var title = null;
            var imgElement = slot.getElementsByTagName('img')[0];
            if (imgElement && imgElement.hasAttribute('bubbletext')) {
                title = slot.getElementsByTagName('img')[0].getAttribute('bubbletext');
            }
            if (title !== null) {
                var tooltipText = document.querySelector('.ac-bubble-content p');
                tooltipText.innerText = title;
                var tooltip = document.querySelector('.ac-bubble');
                tooltip.style.display = 'block';
                tooltip.style.top = ((Math.round((e.clientY - 32) / 64) * 64) + 36) + 'px'; // if grid is moved, adjust e.clientY +- y
                tooltip.style.left = ((Math.round((e.clientX - 25 - 32) / 64) * 64) + 36) + 'px'; // if grid is moved, adjust e.clientX +- x
            }
        });
        slot.addEventListener('mouseout', function () {
            var tooltip = document.querySelector('.bubble-tooltip');
            if (tooltip) {
                tooltip.style.display = 'none';
            }
        });
    });
}
document.addEventListener("DOMContentLoaded", function () {
    hookTooltipListeners();
});