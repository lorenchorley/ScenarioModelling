document.addEventListener('DOMContentLoaded', function (e1) {
    const vSep = document.querySelector('.vertical-separator');
    const left = document.getElementById('left');
    const topText = document.getElementById('topText');
    const bottomText = document.getElementById('bottomText');
    const hSep = document.querySelector('.horizontal-separator');

    let topRatio = 0.5; // Ratio to preserve relative height
    console.log('Registering mousedown to ' + vSep);

    vSep.addEventListener('mousedown', function (e2) {
        console.log('vSep mousedown triggered');

        document.body.style.cursor = 'col-resize';
        const startX = e.clientX;
        const startWidth = left.offsetWidth;

        function doDrag(e) {
            left.style.width = (startWidth + e.clientX - startX) + 'px';
        }

        function stopDrag() {
            document.removeEventListener('mousemove', doDrag);
            document.removeEventListener('mouseup', stopDrag);
            document.body.style.cursor = 'default';
        }

        document.addEventListener('mousemove', doDrag);
        document.addEventListener('mouseup', stopDrag);
    });

    console.log('Registering mousedown to ' + hSep.id);

    hSep.addEventListener('mousedown', function (e3) {
        console.log('hSep mousedown triggered');

        document.body.style.cursor = 'row-resize';
        const startY = e.clientY;
        const startTopHeight = topText.offsetHeight;
        const containerHeight = left.offsetHeight;

        function doDrag(e) {
            let newTopHeight = startTopHeight + (e.clientY - startY);
            let newBottomHeight = containerHeight - newTopHeight - hSep.offsetHeight;

            if (newTopHeight > 50 && newBottomHeight > 50) {
                topText.style.flex = 'none';
                bottomText.style.flex = 'none';
                topText.style.height = newTopHeight + 'px';
                bottomText.style.height = newBottomHeight + 'px';
                topRatio = newTopHeight / (newTopHeight + newBottomHeight); // UPDATED: Save ratio
            }
        }

        function stopDrag() {
            document.removeEventListener('mousemove', doDrag);
            document.removeEventListener('mouseup', stopDrag);
            document.body.style.cursor = 'default';
        }

        document.addEventListener('mousemove', doDrag);
        document.addEventListener('mouseup', stopDrag);
    });

    // UPDATED: Adjust heights on resize
    window.addEventListener('resize', function () {
        const containerHeight = left.offsetHeight;
        const sepHeight = hSep.offsetHeight;
        const usableHeight = containerHeight - sepHeight;

        const newTopHeight = usableHeight * topRatio;
        const newBottomHeight = usableHeight - newTopHeight;

        topText.style.flex = 'none';
        bottomText.style.flex = 'none';
        topText.style.height = newTopHeight + 'px';
        bottomText.style.height = newBottomHeight + 'px';
    });
});