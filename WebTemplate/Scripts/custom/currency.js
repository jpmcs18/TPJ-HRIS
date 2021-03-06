////var currencyInput = document.querySelector('input[type="currency"]')
////var currency = 'PHP' // https://www.currency-iso.org/dam/downloads/lists/list_one.xml

////// format inital value
////onBlur({ target: currencyInput })

////// bind event listeners
////currencyInput.addEventListener('focus', onFocus)
////currencyInput.addEventListener('blur', onBlur)


////function localStringToNumber(s) {
////    return Number(String(s).replace(/[^0-9.-]+/g, ""))
////}

////function onFocus(e) {
////    var value = e.target.value;
////    e.target.value = value ? localStringToNumber(value) : ''
////}

////function onBlur(e) {
////    var value = e.target.value

////    var options = {
////        maximumFractionDigits: 2,
////        currency: currency,
////        style: "currency",
////        currencyDisplay: "symbol"
////    }

////    e.target.value = (value || value === 0)
////        ? localStringToNumber(value).toLocaleString(undefined, options)
////        : ''
////}

// https://codepen.io/akalkhair/pen/dyPaozZ
// Jquery Dependency
function InputCurrency() {
    $("input[type='currency']").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });
}

function formatNumber(n) {
    // format number 1000000 to 1,234,567
    return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
}

function formatCurrency(input, blur) {
    // appends $ to value, validates decimal side
    // and puts cursor back in right position.

    // get input value
    var input_val = input.val() || '0.00';

    // don't validate empty input
    if (input_val === "") { return; }

    // original length
    var original_len = input_val.length;

    // initial caret position 
    var caret_pos = input.prop("selectionStart");

    // check for decimal
    if (input_val.indexOf(".") >= 0) {

        // get position of first decimal
        // this prevents multiple decimals from
        // being entered
        var decimal_pos = input_val.indexOf(".");

        // split number by decimal point
        var left_side = input_val.substring(0, decimal_pos);
        var right_side = input_val.substring(decimal_pos);

        // add commas to left side of number
        left_side = formatNumber(left_side);

        // validate right side
        right_side = formatNumber(right_side);

        // On blur make sure 2 numbers after decimal
        if (blur === "blur") {
            right_side += "00";
        }

        // Limit decimal to only 2 digits
        right_side = right_side.substring(0, 2);

        // join number by .
        input_val = "" + left_side + "." + right_side; //currency in blank string removed

    } else {
        // no decimal entered
        // add commas to number
        // remove all non-digits
        input_val = formatNumber(input_val);
        input_val = "" + input_val; //currency in blank string removed

        // final formatting
        if (blur === "blur") {
            input_val += ".00";
        }
    }

    // send updated string to input
    input.val(input_val);

    // put caret back in the right position
    var updated_len = input_val.length;
    caret_pos = updated_len - original_len + caret_pos;
    input[0].setSelectionRange(caret_pos, caret_pos);
}