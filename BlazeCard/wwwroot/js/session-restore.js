window.blazeCard = window.blazeCard || {};

window.blazeCard.session = {
    save: (key, json) => localStorage.setItem(key, json),
    load: (key) => localStorage.getItem(key),
    clear: (key) => localStorage.removeItem(key)
};
