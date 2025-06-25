window.scrollToElement = function (elementId) {
    var elem = document.getElementById(elementId);
    if (elem) {
        elem.scrollIntoView({ behavior: 'smooth', block: 'end' });
    }
}

window.focusElement = function (elementId) {
    var elem = document.getElementById(elementId);
    if (elem) {
        elem.focus();
    }
}
