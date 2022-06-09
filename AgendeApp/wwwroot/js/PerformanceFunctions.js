$(document).ready(function () {

    function hideAllPanels() {
        $('#divBarChart').hide();
        $('#divRelatorio').hide();
        $('#divPieChart').hide();
    }

    function activatePanel(panelName) {
        try {
            switch (panelName) {
                case "divBarChart":
                    $('#divBarChart').show();
                    $('#divRelatorio').hide();
                    $('#divPieChart').hide();

                    isRelatorioVisible = false;
                    break;
                case "divRelatorio":
                    $('#divBarChart').hide();
                    $('#divRelatorio').show();
                    $('#divPieChart').hide();
                    break;
                case "divPieChart":
                    $('#divBarChart').hide();
                    $('#divRelatorio').hide();
                    $('#divPieChart').show();
                    break;
                default:
                    console.warn("Warn activatePanel. panelName unknown", panelName);
            }

        } catch (err) {
            console.error("Error activatePanel", err);
        }
    }

    // -------------------------------Shared---------------------------------- //

    const randomColor = () => {
        const newColor = Math.floor(Math.random() * 16777215).toString(16);
        return "#" + newColor;
    }

    let initYear = null;
    let initMonth = null;
    let endYear = null;
    let endMonth = null;

    function collectDateRange() {
        try {
            initYear = $('#initYear').val();
            initMonth = $('#initMonth').val();
            endYear = $('#endYear').val();
            endMonth = $('#endMonth').val();
        } catch (err) {
            console.error("Error collectDateRange", err);
        }
    }

    let consultantsForPerformance = null;

    function collectConsultantsForPerformance() {
        try {
            let consultantsObject = new Object();
            consultantsObject = $('#availableConsultants').val();
            consultantsForPerformance = JSON.stringify(consultantsObject);

        } catch (err) {
            console.error("Error collectConsultantsForPerformance", err);
        }
    }

    // -------------------------------Bar Chart---------------------------------- //

    let consultantsPerformanceData = null;
    let consultantsPerformanceError = null;
    async function getConsultantsPerformanceAsync(consultants, initYear, initMonth, endYear, endMonth) {
        try {

            let apiClient = axios.create();

            let axiosConfig = {
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            };

            let params = new FormData();
            params.append("consultants", consultants);
            params.append("initYear", initYear);
            params.append("initMonth", initMonth);
            params.append("endYear", endYear);
            params.append("endMonth", endMonth);

            const apiResult = await apiClient.post(
                "/Performance/GetConsultantsProfitsAsync",
                params,
                axiosConfig
            );

            consultantsPerformanceData = apiResult.data;
            apiClient = null;

        } catch (err) {
            console.error("Error getConsultantsPerformanceAsync", err);

            consultantsPerformanceData = err.response.data;

            // Return error from Axios
            if (consultantsPerformanceData == undefined || consultantsPerformanceData == "") {
                consultantsPerformanceError = err; //axios got an error
            }
        }
    }

    let barChart = null;
    let barChartDatasets = [];
    let xAxis = [];

    function prepareXAxis() {
        try {
            xAxis = [];
            let customLabel = [];
            for (let i = initYear; i <= endYear; i++) {

                if (endYear - initYear == 0) {
                    for (let j = initMonth; j <= endMonth; j++) {
                        customLabel = [];
                        customLabel.push('' + i);
                        customLabel.push('' + j);
                        xAxis.push(customLabel);
                    }
                    continue;
                }

                if (i == initYear) {
                    for (let j = initMonth; j <= 12; j++) {
                        customLabel = [];
                        customLabel.push('' + i);
                        customLabel.push('' + j);
                        xAxis.push(customLabel);
                    }
                    continue;
                }

                if (i == endYear) {
                    for (let j = 1; j <= endMonth; j++) {
                        customLabel = [];
                        customLabel.push('' + i);
                        customLabel.push('' + j);
                        xAxis.push(customLabel);
                    }
                    continue;
                }

                for (let j = 1; j <= 12; j++) {
                    customLabel = [];
                    customLabel.push('' + i);
                    customLabel.push('' + j);
                    xAxis.push(customLabel);
                }
            }

        } catch (err) {
            console.error("Error prepareXAxis", err);
        }
    }

    function prepareBarChartDatasets() {
        try {

            barChartDatasets = [];

            let data = consultantsPerformanceData.data;

            let averagePeriods = 0;

            for (let i = 0; i < data.consultants.length; i++) {

                let item = data.consultants[i];

                let tempDS = {
                    type: 'bar',
                    label: item.name,
                    data: [],
                    backgroundColor: [
                        randomColor(),
                    ],
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label || '';

                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(context.parsed.y);
                                }
                                return label;
                            }
                        }
                    }
                };

                averagePeriods = item.profits.length;

                let profits = [];
                for (let pi = 0; pi < item.profits.length; pi++) {
                    profits.push(item.profits[pi].receitaLiquida);
                }

                tempDS.data = profits;

                barChartDatasets.push(tempDS)
            }

            // average dataset

            if (averagePeriods > 0) {
                let tempAverageDS = {
                    type: 'line',
                    label: 'Average',
                    data: [],
                    backgroundColor: [
                        randomColor(),
                    ],
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label || '';

                                if (label) {
                                    label += ': ';
                                }
                                if (context.parsed.y !== null) {
                                    label += new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(context.parsed.y);
                                }
                                return label;
                            }
                        }
                    }
                };

                for (let i = 0; i < averagePeriods; i++) {
                    tempAverageDS.data.push(data.averageFixedCost);                   
                }

                barChartDatasets.push(tempAverageDS);

            }            

        } catch (err) {
            console.error("Error prepareBarChartDatasets", err);
        }
    }

    async function generatePerformanceBarChartAsync() {
        try {

            consultantsForPerformance = null;

            collectConsultantsForPerformance();

            let areConsultantsSelected = consultantsForPerformance == undefined || consultantsForPerformance == null || consultantsForPerformance == '[]';

            if (areConsultantsSelected) {
                alert("Select a consultant at least");
                return;
            }

            collectDateRange();

            await getConsultantsPerformanceAsync(consultantsForPerformance, initYear, initMonth, endYear, endMonth);

            if (consultantsPerformanceError != null) {
                alert("Error getting the data from the server");
                return;
            }

            prepareBarChartDatasets();

            prepareXAxis()

            let barChartControl = document.getElementById("ConsultantsComericalPerformanceChart").getContext('2d');

            if (barChart != null) {
                barChart.destroy();
            }

            barChart = null;

            let chartOptions = {};

            barChart = new Chart(barChartControl, {
                type: 'bar',
                data: {
                    labels: xAxis,
                    datasets: barChartDatasets,
                    options: chartOptions,
                }
            });

        }
        catch (err) {
            console.error("Error generatePerformanceBarChartAsync", err);
        }
    }

    // -----------------------------Pie Chart------------------------------------ //

    let consultantsPerformanceProfitsByRangeData = null;
    let consultantsPerformanceProfitsByRangeError = null;

    let pieChart = null;
    let pieChartLabels = [];
    let pieChartDataSet = [];
    let pieBackgroundColors = [];

    function preparePieChartDatasets() {
        try {

            pieChartDataSet = [];
            let pieChartData = [];
            pieChartLabels = [];
            pieBackgroundColors = [];

            let data = consultantsPerformanceProfitsByRangeData.data;

            let tempDS = {
                label: "Participao na Receita Liquida",
                data: [],
                backgroundColor: [],
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';

                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                label += new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(context.parsed);
                            }
                            return label;
                        }
                    }
                }
            };

            for (let i = 0; i < data.length; i++) {
                let item = data[i];
                pieChartLabels.push(item.name);
                //pieChartData.push(item.receitaLiquida);
                tempDS.data.push(item.receitaLiquida);
                tempDS.backgroundColor.push(randomColor());
            }
            
            //tempDS.data = pieChartData;
            pieChartDataSet.push(tempDS);


        } catch (err) {
            console.error("Error preparePieChartDatasets", err);
        }
    }

    async function getConsultantsProfitsByRangeAsync(consultants, initYear, initMonth, endYear, endMonth) {

        consultantsPerformanceProfitsByRangeData == null;
        consultantsPerformanceProfitsByRangeError = null;

        try {

            let apiClient = axios.create();

            let axiosConfig = {
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            };

            let params = new FormData();
            params.append("consultants", consultants);
            params.append("initYear", initYear);
            params.append("initMonth", initMonth);
            params.append("endYear", endYear);
            params.append("endMonth", endMonth);

            const apiResult = await apiClient.post(
                "/Performance/GetConsultantsProfitsByRangeAsync",
                params,
                axiosConfig
            );

            consultantsPerformanceProfitsByRangeData = apiResult.data;
            apiClient = null;

        } catch (err) {
            console.error("Error GetConsultantsProfitsByRangeAsync", err);

            consultantsPerformanceProfitsByRangeData = err.response.data;

            // Return error from Axios
            if (consultantsPerformanceProfitsByRangeData == undefined || consultantsPerformanceProfitsByRangeData ==  null || consultantsPerformanceProfitsByRangeData == "") {
                consultantsPerformanceProfitsByRangeError = err; //axios got an error
            }
        }
    }

    async function generatePerformancePieChartAsync() {
        try {

            consultantsForPerformance = null;

            collectConsultantsForPerformance();

            let areConsultantsSelected = consultantsForPerformance == undefined || consultantsForPerformance == null || consultantsForPerformance == '[]';

            if (areConsultantsSelected) {
                alert("Select a consultant at least");
                return;
            }

            collectDateRange();

            await getConsultantsProfitsByRangeAsync(consultantsForPerformance, initYear, initMonth, endYear, endMonth);

            if (consultantsPerformanceProfitsByRangeError != null) {
                alert("Error getting the data from the server");
                return;
            }

            preparePieChartDatasets();

            let pieChartControl = document.getElementById("ConsultantsComericalPerformancePieChart").getContext('2d');

            if (pieChart != null) {
                pieChart.destroy();
            }

            pieChart = null;

            let chartOptions = {};

            const pieData = {
                labels: pieChartLabels,
                datasets: pieChartDataSet
            };

            pieChart = new Chart(pieChartControl, {
                type: 'pie',
                data: pieData,
            });

        }
        catch (err) {
            console.error("Error generatePerformancePieChartAsync", err);
        }
    }

    // -----------------------------Relatorio------------------------------------ //

    let consultantsPerformanceLucro = null;
    let consultantsPerformanceLucroError = null;

    let RelatorioTableConfig = null;

    async function getConsultantsPerformanceLucroAsync(consultants, initYear, initMonth, endYear, endMonth) {
        try {

            let apiClient = axios.create();

            let axiosConfig = {
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json",
                },
            };

            let params = new FormData();
            params.append("consultants", consultants);
            params.append("initYear", initYear);
            params.append("initMonth", initMonth);
            params.append("endYear", endYear);
            params.append("endMonth", endMonth);

            const apiResult = await apiClient.post(
                "/Performance/GetConsultantsLucroAsync",
                params,
                axiosConfig
            );

            consultantsPerformanceLucro = apiResult.data;
            apiClient = null;

            

        } catch (err) {
            console.error("Error getConsultantsPerformanceAsync", err);

            consultantsPerformanceLucro = err.response.data;

            // Return error from Axios
            if (consultantsPerformanceLucro == undefined || consultantsPerformanceLucro == "") {
                consultantsPerformanceLucroError = err; //axios got an error
            }
        }
    }

    async function generateRelatorioAsync() {
        try {
            consultantsForPerformance = null;

            collectConsultantsForPerformance();

            let areConsultantsSelected = consultantsForPerformance == undefined || consultantsForPerformance == null || consultantsForPerformance == '[]';

            if (areConsultantsSelected) {
                alert("Select a consultant at least");
                return;
            }

            collectDateRange();

            await getConsultantsPerformanceLucroAsync(consultantsForPerformance, initYear, initMonth, endYear, endMonth);

            if (consultantsPerformanceLucro.result == true) {

                RelatorioTableConfig = {
                    destroy: true,
                    data: consultantsPerformanceLucro.data,
                    columns: [
                        { "data": "userId", "title":"UserID" },
                        { "data": "periodo", "title": "Periodo" },
                        { "data": "receitaLiquida", "title": "Receita Liquida", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
                        { "data": "brutsalario", "title": "Custo Fixo", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
                        { "data": "consultantComission", "title": "Comissão", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
                        { "data": "lucro", "title": "Lucro", render: $.fn.dataTable.render.number('.', ',', 2, 'R$ ') },
                    ],
                    pagingType: 'full_numbers',
                    paging: false,
                };

                $('#RelatorioTable').DataTable(RelatorioTableConfig);
            }


        } catch (err) {
            console.error("Error generateRelatorioAsync", err);
        }
        
    }

    // ----------------------------------------------------------------- //

    let optionsVisible = 0;

    $('#availableConsultants').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        numberDisplayed : 10,

        onChange: function (option, checked) {
            try {
                //optionsVisible = ('#availableConsultants option').length;
                //console.log("optionsVisible", optionsVisible);
                //$('#availableConsultants').multiselect('refresh');

                $("ulSelectedConsultants").empty();

                let selectedItems = $("#availableConsultants").val();

                for (let i = 0; i < selectedItems.length; i++) {

                    const value = selectedItems[i];
                    const txt = $("#availableConsultants option[value='" + value + "']").text();

                    $("ulSelectedConsultants").append("<li>" + txt + "</li>");
                }
                

            } catch (err) {
                console.error("Error #availableConsultants onChange", err);
            }
        },
    });

    // ----------------------------------------------------------------- //

    $("#processGrafico").click(async function () {
        try {
            hideAllPanels();
            await generatePerformanceBarChartAsync();
            activatePanel('divBarChart');
        } catch (err) {
            console.error("Error processGrafico Click", err)
        }
    });

    $("#processRelatorio").click(async function () {

        try {
            hideAllPanels();
            activatePanel('divRelatorio');
            generateRelatorioAsync();
        } catch (err) {
            console.error("Error processGrafico Click", err)
        }

    });

    $("#processPie").click(async function () {
        try {
            hideAllPanels();
            generatePerformancePieChartAsync();
            activatePanel('divPieChart');
        } catch (err) {
            console.error("Error processGrafico Click", err)
        }
    });


});