﻿using System.Web;
using System.Web.Optimization;

namespace WebTemplate
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.5.1.min.js"
                ,"~/Scripts/bootstrap.min.js"
                , "~/Scripts/app/inspinia.js"
                , "~/Scripts/custom/currency.js"
                , "~/Scripts/custom/datetimepicker-custom.js"
                ,"~/Scripts/plugins/autoNumeric/autoNumeric.js"
                ,"~/Scripts/custom/custom-autonumeric.js"
                ,"~/Scripts/custom/jquery_global_custom.js"
                ,"~/Scripts/custom/calendar.min.js"
                ,"~/Scripts/custom/default-toastr-setting.js"
                ,"~/Scripts/custom/jquery.timeago.js"
                , "~/Scripts/plugins/maskMoney/jquery.maskMoney.js"
                ,"~/Scripts/plugins/pace/pace.min.js"
                ,"~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js"
                ,"~/Scripts/plugins/iCheck/icheck.min.js"
                ,"~/Scripts/plugins/fullcalendar/moment.min.js"
                ,"~/Scripts/plugins/fullcalendar/fullcalendar.min.js"
                ,"~/Scripts/plugins/datapicker/bootstrap-datepicker.js"
                ,"~/Scripts/plugins/switchery/switchery.js"
                ,"~/Scripts/plugins/chosen/chosen.jquery.js"
                , "~/Scripts/plugins/toastr/toastr.min.js"
                , "~/Scripts/plugins/daterangepicker/daterangepicker.js"
                , "~/Scripts/plugins/sweetalert/sweetalert.min.js"
                , "~/Scripts/plugins/footable/footable.all.min.js"
                , "~/Scripts/plugins/select2/select2.full.min.js"
                , "~/Scripts/plugins/ladda/spin.min.js"
                , "~/Scripts/plugins/pdfjs/pdf.js"
                , "~/Scripts/plugins/metisMenu/metisMenu.min.js"
                , "~/Scripts/plugins/yearpicker/yearpicker.js"

                , "~/Scripts/app/skin.config.min.js"
                ,"~/Scripts/jquery.unobtrusive-ajax.min.js"
                ,"~/Scripts/plugins/jquery-ui/jquery-ui.min.js"
                ,"~/Scripts/plugins/peity/jquery.peity.min.js"
                ,"~/Scripts/plugins/video/responsible-video.js"
                ,"~/Scripts/plugins/blueimp/jquery.blueimp-gallery.min.js"
                ,"~/Scripts/plugins/sparkline/jquery.sparkline.min.js"
                ,"~/Scripts/plugins/morris/raphael-2.1.0.min.js"
                ,"~/Scripts/plugins/morris/morris.js"
                ,"~/Scripts/plugins/flot/jquery.flot.js"
                ,"~/Scripts/plugins/flot/jquery.flot.tooltip.min.js"
                ,"~/Scripts/plugins/flot/jquery.flot.resize.js"
                ,"~/Scripts/plugins/flot/jquery.flot.pie.js"
                ,"~/Scripts/plugins/flot/jquery.flot.time.js"
                ,"~/Scripts/plugins/flot/jquery.flot.spline.js"
                ,"~/Scripts/plugins/rickshaw/vendor/d3.v3.js"
                ,"~/Scripts/plugins/rickshaw/rickshaw.min.js"
                ,"~/Scripts/plugins/chartjs/Chart.min.js"
                ,"~/Scripts/plugins/jeditable/jquery.jeditable.js"
                ,"~/Scripts/plugins/dataTables/datatables.min.js"
                ,"~/Scripts/plugins/jqGrid/i18n/grid.locale-en.js"
                ,"~/Scripts/plugins/jqGrid/jquery.jqGrid.min.js"
                ,"~/Scripts/plugins/codemirror/codemirror.js"
                ,"~/Scripts/plugins/codemirror/mode/javascript/javascript.js"
                ,"~/Scripts/plugins/nestable/jquery.nestable.js"
                ,"~/Scripts/plugins/validate/jquery.validate.min.js"
                ,"~/Scripts/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"
                ,"~/Scripts/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"
                ,"~/Scripts/plugins/ionRangeSlider/ion.rangeSlider.min.js"
                ,"~/Scripts/plugins/nouslider/jquery.nouislider.min.js"
                ,"~/Scripts/plugins/jasny/jasny-bootstrap.min.js"
                ,"~/Scripts/plugins/jsKnob/jquery.knob.js"
                ,"~/Scripts/plugins/steps/jquery.steps.min.js"
                ,"~/Scripts/plugins/dropzone/dropzone.js"
                ,"~/Scripts/plugins/summernote/summernote.min.js"
                ,"~/Scripts/plugins/colorpicker/bootstrap-colorpicker.min.js"
                ,"~/Scripts/plugins/cropper/cropper.min.js"
                //,"~/Scripts/plugins/angular/angular.js"
                ,"~/Scripts/plugins/angular/angular-2.js"
                ,"~/Scripts/plugins/html2canvas/html2canvas.js"
                ,"~/Content/plugins/cropper/cropper.min.css"
                ,"~/Scripts/plugins/jsTree/jstree.min.js"
                ,"~/Scripts/plugins/diff_match_patch/javascript/diff_match_patch.js"
                ,"~/Scripts/plugins/preetyTextDiff/jquery.pretty-text-diff.min.js"
                ,"~/Scripts/plugins/idle-timer/idle-timer.min.js"
                ,"~/Scripts/plugins/tinycon/tinycon.min.js"
                ,"~/Scripts/plugins/chartist/chartist.min.js"
                ,"~/Scripts/plugins/clockpicker/clockpicker.js"
                ,"~/Scripts/plugins/masonary/masonry.pkgd.min.js"
                ,"~/Scripts/plugins/slick/slick.min.js"
                ,"~/Scripts/plugins/ladda/ladda.min.js"
                ,"~/Scripts/plugins/ladda/ladda.jquery.min.js"
                ,"~/Scripts/plugins/dotdotdot/jquery.dotdotdot.min.js"
                ,"~/Scripts/plugins/touchspin/jquery.bootstrap-touchspin.min.js"
                ,"~/Scripts/plugins/bootstrapTour/bootstrap-tour.min.js"
                ,"~/Scripts/plugins/i18next/i18next.min.js"
                ,"~/Scripts/plugins/clipboard/clipboard.min.js"
                ,"~/Scripts/plugins/c3/c3.min.js"
                ,"~/Scripts/plugins/d3/d3.min.js"
                ,"~/Scripts/plugins/bootstrap-markdown/bootstrap-markdown.js"
                ,"~/Scripts/plugins/bootstrap-markdown/markdown.js"
                ,"~/Scripts/plugins/topojson/topojson.js"
                ,"~/Scripts/plugins/datamaps/datamaps.all.min.js"
                ,"~/Scripts/plugins/bootstrap-tagsinput/bootstrap-tagsinput.js"
                ,"~/Scripts/plugins/dualListbox/jquery.bootstrap-duallistbox.js"
                ,"~/Scripts/plugins/typehead/bootstrap3-typeahead.min.js"
                ,"~/Scripts/plugins/touchpunch/jquery.ui.touch-punch.min.js"
                ,"~/Scripts/plugins/wow/wow.min.js"
                ,"~/Scripts/plugins/pwstrength/pwstrength-bootstrap.min.js"
                ,"~/Scripts/plugins/pwstrength/zxcvbn.js"
                //,"~/Scripts/plugins/jQuery-webcam-master/jquery.webcam.js"
                ,"~/Scripts/plugins/jQuery-webcam-master/jquery.webcam.min.js"
                ,"~/Scripts/plugins/maskPlugin/jquery.mask.js"
                ,"~/Scripts/plugins/sweetalert2/src/sweetalert2.js"
                ));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/style.css"
                ,"~/Content/bootstrap.min.css"
                ,"~/fonts/font-awesome/css/font-awesome.min.css"
                , "~/Scripts/plugins/jquery-ui/jquery-ui.min.css"
                ,"~/Content/plugins/iCheck/custom.css"
                ,"~/Content/plugins/dataTables/datatables.min.css"
                ,"~/Content/plugins/fullcalendar/fullcalendar.css"
                ,"~/Content/plugins/datapicker/datepicker3.css"
                ,"~/Content/plugins/switchery/switchery.css"
                ,"~/Content/plugins/toastr/toastr.min.css"
                ,"~/Content/plugins/daterangepicker/daterangepicker-bs3.css"
                ,"~/Content/plugins/select2/select2.min.css"
                ,"~/Content/plugins/sweetalert/sweetalert.css"
                , "~/Content/plugins/jqGrid/ui.jqgrid.css"
                , "~/Content/plugins/yearpicker/yearpicker.css"

                , "~/Scripts/plugins/sweetalert2/src/sweetalert2.scss"
                ,"~/Scripts/custom/jquery.datetimepicker/jquery.datetimepicker.css"
                ,"~/Content/plugins/blueimp/css/blueimp-gallery.min.css"
                ,"~/Content/plugins/morris/morris-0.4.3.min.css"
                ,"~/Content/plugins/codemirror/codemirror.css"
                ,"~/Content/plugins/codemirror/ambiance.css"
                ,"~/Content/plugins/ionRangeSlider/ion.rangeSlider.css"
                ,"~/Content/plugins/ionRangeSlider/ion.rangeSlider.skinFlat.css"
                ,"~/Content/plugins/nouslider/jquery.nouislider.css"
                ,"~/Content/plugins/jasny/jasny-bootstrap.min.css"
                ,"~/Content/plugins/chosen/bootstrap-chosen.css"
                ,"~/Content/plugins/steps/jquery.steps.css"
                ,"~/Content/plugins/dropzone/basic.css"
                ,"~/Content/plugins/dropzone/dropzone.css"
                ,"~/Content/plugins/summernote/summernote.css"
                ,"~/Content/plugins/summernote/summernote-bs3.css"
                ,"~/Content/plugins/colorpicker/bootstrap-colorpicker.min.css"
                ,"~/Content/plugins/cropper/cropper.min.css"
                ,"~/Content/plugins/jsTree/style.css"
                ,"~/Content/plugins/chartist/chartist.min.css"
                ,"~/Content/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css"
                ,"~/Content/plugins/clockpicker/clockpicker.css"
                ,"~/Content/plugins/sweetalert/sweetalert.css"
                ,"~/Content/plugins/footable/footable.core.css"
                ,"~/Content/plugins/slick/slick.css"
                ,"~/Content/calendar.min.css"
                ,"~/Content/semantic.min.css"
                ,"~/Content/plugins/slick/slick-theme.css"
                ,"~/Content/plugins/ladda/ladda-themeless.min.css"
                ,"~/Content/plugins/touchspin/jquery.bootstrap-touchspin.min.css"
                ,"~/Content/plugins/bootstrapTour/bootstrap-tour.min.css"
                ,"~/Content/plugins/c3/c3.min.css"
                ,"~/Content/plugins/bootstrap-markdown/bootstrap-markdown.min.css"
                ,"~/Content/plugins/bootstrap-tagsinput/bootstrap-tagsinput.css"
                ,"~/Content/plugins/dualListbox/bootstrap-duallistbox.min.css"
                ,"~/Content/plugins/bootstrapSocial/bootstrap-social.css"
                ,"~/Content/plugins/textSpinners/spinners.css"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/semanticjs").Include(
            //    "~/Content/js/semantic.js",
            //    "~/Content/js/Chart.min.js",
            //    "~/Content/js/jquery-3.5.1.js",
            //    "~/Content/js/moment.min.js",
            //    "~/Content/js/calendar.min.js",
            //    "~/Content/js/jquery_global_custom.js"));

            //bundles.Add(new StyleBundle("~/bundles/semanticcss").Include(
            //    "~/Content/semantic.min.css",
            //    "~/Content/semantic.custom.css"));
        }
    }
}
