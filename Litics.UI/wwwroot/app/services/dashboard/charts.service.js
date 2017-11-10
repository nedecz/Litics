'use strict';
angular.module('updater')
    .service('chartsService', chartsService);

chartsService.inject = ['$http', '$q', '__env'];

function chartsService($http, $q, __env) {

    let service = {
        lineChartOptions: lineChartOptions,
        multiBarChartOptions: multiBarChartOptions,
        stackedAreaChartOptions: stackedAreaChartOptions,

    };
    return service;

    function lineChartOptions(xLabel, yLabel) {
        return {
            chart: {
                type: 'lineChart',
                margin: {
                    top: 40,
                    right: 20,
                    bottom: 40,
                    left: 55
                },
                x: function (d) { return d.x; },
                y: function (d) { return d.y; },
                useInteractiveGuideline: true,
                xAxis: {
                    axisLabel: xLabel,
                    axisLabelDistance: -5
                },
                yAxis: {
                    axisLabel: yLabel,
                    tickFormat: function (d) {
                        return d3.format('.02f')(d);
                    },
                    axisLabelDistance: -10
                },
                showLegend: false
            }
        };
    }
    function stackedAreaChartOptions(xLabel, yLabel) {
        return {
            chart: {
                type: 'stackedAreaChart',
                margin: {
                    top: 20,
                    right: 20,
                    bottom: 30,
                    left: 40
                },
                useVoronoi: false,
                clipEdge: true,
                duration: 100,
                useInteractiveGuideline: true,
                xAxis: {
                    showMaxMin: false
                },
                yAxis: {},
                zoom: {
                    enabled: true,
                    scaleExtent: [
                        1,
                        10
                    ],
                    useFixedDomain: false,
                    useNiceScale: false,
                    horizontalOff: false,
                    verticalOff: true,
                    unzoomEventType: "dblclick.zoom"
                }
            }
        }
    }
    function multiBarChartOptions(xLabel, yLabel) {
        return {
            chart: {
                type: "multiBarChart",
                height: 450,
                margin: {
                    top: 20,
                    right: 20,
                    bottom: 45,
                    left: 45
                },
                clipEdge: true,
                duration: 500,
                stacked: true,
                xAxis: {
                    axisLabel: xLabel,
                    showMaxMin: false
                },
                yAxis: {
                    axisLabel: yLabel,
                    axisLabelDistance: -20
                }
            }
        }
    }
    function multiBarChartOptions(xLabel, yLabel) {
        return {
            chart: {
                type: "pieChart",
                height: 500,
                showLabels: true,
                duration: 500,
                labelThreshold: 0.01,
                labelSunbeamLayout: true,
                legend: {
                    margin: {
                        top: 5,
                        right: 35,
                        bottom: 5,
                        left: 0
                    }
                }
            }
        }
    }

};