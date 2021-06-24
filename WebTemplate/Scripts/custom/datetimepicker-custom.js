//(function ($) {

    $.fn.tpjdatetime = function (options) {

        var settings = $.extend({
            withdate: true,
            withtime: true,
            monthyearonly: false,
            startofthemonth: false,
            endofthemonth: false,
            startyear: 1970,
            addyear: 10
        }, options);

        $(this).attr('readonly', true).css('background-color', 'white').off('click').on('click', function () {
            var sender = $(this);
            var date = sender.val();
            date = date === "" ? $.now() : date;
            var id = sender.attr('id');
            var popupid = 'paul_custom_' + id;
            var position = sender.offset();
            var height = sender.outerHeight();
            var top = position.top + height;
            if ($(window).outerHeight() < top + 138)
                top = position.top - 138;
            var left = position.left;
            var d = new Date(date);
            var curr = new Date($.now());

            var month = d.getMonth() + 1;
            var day = d.getDate();
            var year = d.getFullYear();
            var days = new Date(year, month, 0).getDate();

            var hour = d.getHours() > 12 ? d.getHours() - 12 : (d.getHours() === 0 ? 12 : d.getHours());
            var ampm = d.getHours() >= 12 ? "PM" : "AM";
            var minute = d.getMinutes();

            var months = ["", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

            var monthopt = "";
            var dayopt = "";
            var yearopt = "";
            var houropt = "";
            var minuteopt = "";
            var ampmopt = "";

            var selected = "";

            var datetime = "";

            if (settings.withdate) {
                for (var i = 1; i <= 12; i++) {
                    selected = i === month ? "selected" : "";
                    monthopt += "<option value='" + i + "' " + selected + ">" + months[i] + "</option>";
                }
                if (!settings.monthyearonly) {
                    for (var l = 1; l <= days; l++) {
                        selected = l === day ? "selected" : "";
                        dayopt += "<option value='" + l + "' " + selected + ">" + padding(l) + "</option>";
                    }
                }
                else {
                    if (settings.startofthemonth)
                        day = 1;
                    else if (settings.endofthemonth) {
                        day = new Date(year, month, 0).getDate();
                    }
                }
                for (var j = settings.startyear; j <= curr.getFullYear() + settings.addyear; j++) {
                    selected = j === year ? "selected" : "";
                    yearopt += "<option value='" + j + "' " + selected + ">" + padding(j) + "</option>";
                }
                datetime = padding(month) + '/' + padding(day) + '/' + padding(year) + " ";
            }

            if (settings.withtime) {

                for (var m = 1; m <= 12; m++) {
                    selected = m === hour ? "selected" : "";
                    houropt += "<option value='" + m + "' " + selected + ">" + padding(m) + "</option>";
                }
                for (var k = 0; k < 60; k++) {
                    selected = k === minute ? "selected" : "";
                    minuteopt += "<option value='" + k + "' " + selected + ">" + padding(k) + "</option>";
                }

                selected = ampm === "AM" ? "selected" : "";
                ampmopt += "<option value='AM' " + selected + ">AM</option>";
                selected = ampm === "PM" ? "selected" : "";
                ampmopt += "<option value='PM' " + selected + ">PM</option>";
                datetime += padding(hour) + ':' + padding(minute) + ' ' + ampm;

            }
            sender.val(datetime.trim());
            var divstyle = "position: absolute;top: " + top + "px;left: " + left + "px;z-index: 99999;padding: 10px;background-color: #e7eaec;border: 2px solid #ddd;box-shadow: 3px 2px 6px 2px #ddd;";
            var html = "<div id='" + popupid + "' class='paul_custompopup' style='" + divstyle + "'>"
                + "          <div style='display: inline-grid;'>";
            if (settings.withdate) {
                html += "                          <div style='display: inline-flex; padding-bottom: 6px;'>"
                    + "                                          <div class='m-r-sm'><label class='control-label'>&nbsp;</label><br/>"
                    + "                                                          <i class='fa fa-calendar fa-2x'></i>"
                    + "                                          </div>"
                    + "                                          <div class='m-r-xs'>"
                    + "                                                          <label class='control-label' style='margin-bottom: 0;'>Month</label>"
                    + "                                                          <select id='paul_custom_month' class='form-control'>" + monthopt + "</select>"
                    + "                                          </div>";
                if (!settings.monthyearonly) {
                    html += "                                          <div class='m-r-xs'>"
                        + "                                                          <label class='control-label' style='margin-bottom: 0;'>Day</label>"
                        + "                                                          <select id='paul_custom_day' class='form-control'>" + dayopt + "</select>"
                        + "                                          </div>";
                }
                html += "                                          <div class='m-r-xs'>"
                    + "                                                          <label class='control-label' style='margin-bottom: 0;'>Year</label>"
                    + "                                                          <select id='paul_custom_year' class='form-control'>" + yearopt + "</select>"
                    + "                                          </div>"
                    + "                          </div>";
            }
            if (settings.withtime) {
                html += "                          <div style='display: inline-flex;'>"
                    + "                                          <div class='m-r-sm'><label class='control-label'>&nbsp;</label><br/>"
                    + "                                                          <i class='fa fa-clock-o fa-2x'></i>"
                    + "                                          </div>"
                    + "                                          <div class='m-r-xs'>"
                    + "                                                          <label class='control-label' style='margin-bottom: 0;'>Hours</label>"
                    + "                                                          <select id='paul_custom_hour' class='form-control'>" + houropt + "</select>"
                    + "                                          </div>"
                    + "                                          <div class='m-r-xs'>"
                    + "                                                          <label class='control-label' style='margin-bottom: 0;'>Minutes</label>"
                    + "                                                          <select id='paul_custom_minute' class='form-control'>" + minuteopt + "</select>"
                    + "                                          </div>"
                    + "                                          <div class='m-r-xs'>"
                    + "                                                          <label class='control-label' style='margin-bottom: 0;'>Period</label>"
                    + "                                                          <select id='paul_custom_ampm' class='form-control'>" + ampmopt + "</select>"
                    + "                                          </div>"
                    + "                          </div>";
            }
            html += "          </div>"
                + "</div>";




            if ($(".paul_custompopup").length > 0) {
                $(".paul_custompopup[id!='" + popupid + "']").remove();
            }
            $("body").append(html);
            if (settings.withdate) {
                $("#paul_custom_month").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "month", settings.withdate, settings.withtime, settings.monthyearonly, settings.startofthemonth, settings.endofthemonth);
                });
                $("#paul_custom_day").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "day", settings.withdate, settings.withtime);
                });
                $("#paul_custom_year").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "year", settings.withdate, settings.withtime);
                });
            }
            if (settings.withtime) {
                $("#paul_custom_hour").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "hour", settings.withdate, settings.withtime);
                });
                $("#paul_custom_minute").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "minute", settings.withdate, settings.withtime);
                });
                $("#paul_custom_ampm").off('change').on('change', function () {
                    dropdownfunction(sender, $(this), "ampm", settings.withdate, settings.withtime);
                });
            }
            $("body").off("mouseup").on("mouseup", function (e) {
                if (!([popupid, id, "paul_custom_month", "paul_custom_day", "paul_custom_year", "paul_custom_hour", "paul_custom_minute", "paul_custom_ampm"].includes(e.target.id))) {
                    $("#" + popupid).remove();
                }
                else
                    $(".paul_custompopup[id!='" + popupid + "']").remove();
            });
        });
    };

    function dropdownfunction(sender, dropdown, target, withdate, withtime, monthyearonly, startofthemonth, endofthemonth) {
        var date = sender.val();
        date = date === "" ? $.now() : date;
        var d = new Date(date);

        var month = target === "month" ? dropdown.val() : d.getMonth() + 1;
        var day = target === "day" ? dropdown.val() : d.getDate();
        var year = target === "year" ? dropdown.val() : d.getFullYear();
        var hour = target === "hour" ? dropdown.val() : (d.getHours() > 12 ? d.getHours() - 12 : (d.getHours() === 0 ? 12 : d.getHours()));
        var minute = target === "minute" ? dropdown.val() : d.getMinutes();
        var ampm = target === "ampm" ? dropdown.val() : (d.getHours() >= 12 ? "PM" : "AM");

        if (["month", "year"].includes(target)) {
            var days = new Date(year, month, 0).getDate();
            var dayopt = "";
            day = days < day ? 1 : day;
            for (var l = 1; l <= days; l++) {
                selected = l === day ? "selected" : "";
                dayopt += "<option value='" + l + "' " + selected + ">" + padding(l) + "</option>";
            }
            $("#paul_custom_day").html(dayopt);
        }
        var datetime = "";

        if (monthyearonly) {
            if (startofthemonth)
                day = 1;
            else if (endofthemonth) {
                day = new Date(year, month, 0).getDate();
            }
        }

        if (withdate) {
            datetime = padding(month) + '/' + padding(day) + '/' + padding(year) + " ";
        }
        if (withtime) {
            datetime += padding(hour) + ':' + padding(minute) + ' ' + ampm;
        }
        sender.val(datetime.trim());
    }
    function padding(num) {
        return (num < 10 ? '0' : '') + num;
    }
//})