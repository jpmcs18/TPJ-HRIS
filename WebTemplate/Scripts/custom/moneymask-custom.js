(function ($) {
    $.fn.tpjmoneymask = function () {
        $(this).off('keydown').on('keydown', function (e) {
            var regex = new RegExp('[0-9]');
            if (regex.test(String.fromCharCode(e.which)) || [8, 9, 13, 27, 35, 36, 37, 39, 190].includes(e.keyCode) || (e.keyCode === 65 && e.ctrlKey === true)) {
                console.log("asd");
                return true;
            }
            else {
                console.log("asdas");
                return false;
            }
        });
    };

}(jQuery));