// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {

    let chartViewPortHeight;
    let chartViewPortwidth;

    const viewPortHeightConstant = 0.2;
    let viewPortwidthConstant = 0.2;

    jQuery(window).on('resize', _.debounce(calculateLayoutForCharts, 150));

    function calculateLayoutForCharts() {
        try {
            chartViewPortwidth = $(window).width() * viewPortwidthConstant;
            chartViewPortHeight = $(window).height() * viewPortHeightConstant;

            $('#ConsultantsComericalPerformanceChart').width = chartViewPortwidth + 'px';
            $('#ConsultantsComericalPerformanceChart').height = chartViewPortHeight + 'px';

        } catch (err) {
            console.error("Error calculateLayoutForCharts", err);
        }
    }

    


});