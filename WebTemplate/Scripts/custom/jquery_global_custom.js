

function ShowBasicModal(header, msg, btnText) {
    var btnColor = header == 'Success' ? 'green' : 'red';
    $('.ui.basic.modal .content').prepend(`<div class="ui ${btnColor} icon header"></div>`);
    $('.ui.basic.modal .header').html(header);
    $('.ui.basic.modal .content p').html(msg);
    $('.ui.basic.modal .actions').html(`<div class="ui ${btnColor} ok inverted button">${btnText}</div>`);
    $('.ui.basic.modal').modal('show');
}

function Message(type, message, isMsgList) {
    var msg = type.toLowerCase() == 'error' && message == undefined ? 'Something went wrong.' : message;
    if (isMsgList) {
        msg = '';
        SetErrorFields(message);
        for (var i = 0; i < message.length; i++) {
            if (message[i].Error_Message.length > 0)
                msg += '<li>' + message[i].Error_Message + '</li>'
        }
        msg = '<ul class="list">' + msg + '</ul>';
    }
    var div = $('.ui.modal').hasClass('visible') ? '#ModalMessage' : '#Message';
    $(div + ' .ui.message').remove();
    $(div).html('<div class="ui message hidden"><i class="close icon"></i><div class="header"></div><div class="message"></div></div>');
    $(div + ' .ui.message').removeClass('success negative hidden').addClass(type.toLowerCase());
    $(div + ' .ui.message').find('.header').html(type);
    $(div + ' .ui.message').find('.message').html(msg);
    if (msg.length > 0)
        $(div + ' .ui.message').show();
    $("html, body").animate({ scrollTop: 0 }, 300);

    $('.ui.message .close').on('click', function () {
        $(this).closest('.message').transition('fade');
    });
}

function SetErrorFields(model) {
    for (var i = 0; i < model.length; i++) {
        $('[name="' + model[i].Object_Name + '"]').parent('div.ui').addClass('error');
    }
}

function FixedHeaderFooterTable() {
    var $body = $(".table-container-body"),
        $header = $(".table-container-header"),
        $footer = $(".table-container-footer");

    // Get ScrollBar width(From: http://bootstrap-table.wenzhixin.net.cn/)
    var scrollBarWidth = (function () {
        var inner = $('<p/>').addClass('fixed-table-scroll-inner'),
            outer = $('<div/>').addClass('fixed-table-scroll-outer'),
            w1, w2;
        outer.append(inner);
        $('body').append(outer);
        w1 = inner[0].offsetWidth;
        outer.css('overflow', 'scroll');
        w2 = inner[0].offsetWidth;
        if (w1 === w2) {
            w2 = outer[0].clientWidth;
        }
        outer.remove();
        return w1 - w2;
    })();

    // Scroll horizontal
    $body.on('scroll', function () {
        $header.scrollLeft($(this).scrollLeft());
        $footer.scrollLeft($(this).scrollLeft());
    });

    // Redraw Header/Footer
    var redraw = function () {
        var tds = $body.find("> table > tbody > tr:first-child > td");
        tds.each(function (i) {
            var width = $(this).innerWidth(),
                lastPadding = (tds.length - 1 == i ? scrollBarWidth : 0);
            lastHeader = $header.find("th:eq(" + i + ")").innerWidth(width + lastPadding);
            lastFooter = $footer.find("th:eq(" + i + ")").innerWidth(width + lastPadding);
        });
    };

    // Selection
    $body.find("> table > tbody > tr > td").click(function (e) {
        $body.find("> table > tbody > tr").removeClass("info");
        $(e.target).parent().addClass('info');
    });

    // Listen to Resize Window
    $(window).resize(redraw);
    redraw();

    // Resize Body
    var UpdateScrollableTableHeightOnPageResize = function () {
        var pagefontsize = parseFloat($("body").css("font-size"));
        var windowHeight = $(window).height() / pagefontsize;
        var notbodyheight = $('#TopNavigation').height() + $('#Breadcrumb').height() + $('#Footer').height() + $('.ui.search').parent('div').height();
        var bodyheight = windowHeight - notbodyheight;
        var tominus = $('.table-full-page').length > 0 ? notbodyheight : bodyheight;
        var pageWrapperheight = tominus / pagefontsize * 3.4;
        var tableBodyHeight = windowHeight - pageWrapperheight;
        var table = $('.table-full-page').length > 0 ? '.table-full-page ' : '';
        if (!table == false)
            $(table + '.table-container-body').css('height', tableBodyHeight + 'em');
    }

    // Listen to Resize Window
    $(window).resize(UpdateScrollableTableHeightOnPageResize);
    UpdateScrollableTableHeightOnPageResize();
}

function SearchTable() {
    var value = $(this).val().toLowerCase();

    if (value.length > 0) {
        $('#TableDimmer .loader').html("Looking for '" + value + "' in table...");
        $('#TableDimmer').addClass('active');
    }

    $('.table-container .table-container-body tbody tr').filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
    });

    var resultsCount = $('.table-container .table-container-body tbody tr:not(:hidden)').length;
    var resultsValue = $('#ResultsLabel').attr('value');
    var resultsText = resultsValue + ' records found';

    if (value.length > 0) {
        $('#TableDimmer').removeClass('active');
        resultsText = resultsCount + ' of ' + resultsText;
    }
    $('#ResultsLabel').html(resultsText);
}

function DeleteRowTable() {
    var resultsValue = parseInt($('#ResultsLabel').attr('value')) - 1;
    var resultsText = resultsValue + ' records found';
    $('#ResultsLabel').html(resultsText);
}

$(document).ready(function () {
    //$('#SearchTableString').off('keyup').on('keyup', SearchTable);
    //$('.ui.selection.fluid').dropdown({
    //    fullTextSearch: true,
    //    match: 'text',
    //    clearable: true
    //});
    ////$('.ui.accordion').accordion();
    //
    //$('#rightDropdownMenu').dropdown({
    //    action: 'hide'
    //});
    //
    //$('#Notifications').off('click').on('click', function (e) {
    //    $("#RightSideBar").sidebar('toggle');
    //});
    //
    //$(".burgerMenu").off('click').on("click", function () {
    //    $("#LeftSideBar").sidebar('toggle');
    //});

    FixedHeaderFooterTable();
});