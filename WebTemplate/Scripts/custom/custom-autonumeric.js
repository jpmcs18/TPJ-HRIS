function AddAutoNumeric(selector) {
    $(selector).autoNumeric();
}

function AddAutoNumericWithMinMax(selector, min, max) {
    min = min != null
        ? min
        : -10000000000000;
    max = max != null
        ? max
        : 10000000000000;

    $(selector).autoNumeric('init', { vMin: min, vMax: max });
}

function HasAutoNumeric(element) {
    try {
        GetAutoNumericValue(element);
        return true;
    }
    catch(err) {
        return false;
    }
}

function GetAutoNumericValue(selector) {
    return $(selector).autoNumeric('get');
}

function SetAutoNumericValue(selector, value) {
    return $(selector).autoNumeric('set', value);
}

function DestroyAutoNumeric(selector) {
    return $(selector).autoNumeric('destroy');
}

function InitializeAutoNumeric(selector) {
    $(selector).autoNumeric('destroy');
    $(selector).autoNumeric();
}