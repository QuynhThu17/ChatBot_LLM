window.scrollToElement = function (element) {
    if (element) {
        element.scrollIntoView({ behavior: "smooth" });
    }
};

window.preventEnterSubmit = function (element, dotnetHelper) {
    element.addEventListener("keydown", function (e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            dotnetHelper.invokeMethodAsync("OnEnterPressed");
        }
    });
};

// wwwroot/js/site.js
window.scrollToElement = (element) => {
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
};

window.focusElement = (element) => {
    if (element) {
        element.focus();
    }
};
