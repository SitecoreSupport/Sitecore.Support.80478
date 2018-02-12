// Sitecore.Support.80478
(function ($) {
    $.noConflict();

    $(document).ready(function () {
        $("form[data-wffm]").each(function () { $(this).wffmForm(); });
    });
})(jQuery);