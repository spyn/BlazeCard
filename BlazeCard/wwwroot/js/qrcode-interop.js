window.blazeCard = window.blazeCard || {};

// Minimal QR via Google Charts API fallback replaced with SVG placeholder pattern for offline.
// Draws a deterministic pseudo-QR grid from message hash so preview works offline.
window.blazeCard.renderQr = (elementId, message) => {
    const el = document.getElementById(elementId);
    if (!el) return;
    const size = 21;
    const cell = 4;
    const canvas = document.createElement('canvas');
    canvas.width = size * cell;
    canvas.height = size * cell;
    const ctx = canvas.getContext('2d');
    ctx.fillStyle = '#fff';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = '#000';
    let h = 0;
    for (let i = 0; i < (message || '').length; i++) h = ((h << 5) - h + message.charCodeAt(i)) | 0;
    const drawFinder = (ox, oy) => {
        for (let y = 0; y < 7; y++)
            for (let x = 0; x < 7; x++) {
                const border = x === 0 || y === 0 || x === 6 || y === 6;
                const center = x >= 2 && x <= 4 && y >= 2 && y <= 4;
                if (border || center) ctx.fillRect((ox + x) * cell, (oy + y) * cell, cell, cell);
            }
    };
    drawFinder(0, 0);
    drawFinder(size - 7, 0);
    drawFinder(0, size - 7);
    for (let y = 0; y < size; y++) {
        for (let x = 0; x < size; x++) {
            if ((x < 8 && y < 8) || (x > size - 9 && y < 8) || (x < 8 && y > size - 9)) continue;
            const bit = ((h + x * 31 + y * 17) >>> 0) % 3 === 0;
            if (bit) ctx.fillRect(x * cell, y * cell, cell, cell);
        }
    }
    el.innerHTML = '';
    el.appendChild(canvas);
};
