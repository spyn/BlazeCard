window.blazeCard = window.blazeCard || {};

window.blazeCard.setDragData = (itemType) => {
    window.blazeCard._dragType = itemType;
};

window.blazeCard.initSortable = (containerId, dotnetRef) => {
    // Lightweight placeholder — Visual Mode uses button-add path; SortableJS can be wired later.
    const el = document.getElementById(containerId);
    if (!el) return;
    el.dataset.sortable = 'ready';
};

window.blazeCard.initDropZone = (zoneId, dotnetRef) => {
    const el = document.getElementById(zoneId);
    if (!el) return;
    el.addEventListener('dragover', (e) => e.preventDefault());
    el.addEventListener('drop', async (e) => {
        e.preventDefault();
        const type = window.blazeCard._dragType || e.dataTransfer.getData('text/plain');
        if (type && dotnetRef) {
            await dotnetRef.invokeMethodAsync('OnToolboxDrop', type);
        }
    });
};
