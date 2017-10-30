'use strict';
angular.module('updater')
    .service('patchDashboardService', patchDashboardService);

patchDashboardService.inject = ['$http', '$q', '__env'];

function patchDashboardService($http, $q, __env) {
    let apiBase = __env.apiUrl + '/updates';

    let service = {
        getTotalPatchResults: getTotalPatchResults,
        getGroupByLocationResults: getGroupByLocationResults,
        getGroupByProductResults: getGroupByProductResults,
        getGroupByOSResults: getGroupByOSResults,
        getGroupByOwnerResults: getGroupByOwnerResults,
        createChartObject: createChartObject
    };
    return service;

    function getTotalPatchResults() {
        let totalPatchResult = {
            headers: { total: "Total", patchPending: "Patch Pending", vulnerable: "Vulnerable" },
            values: [
                { total: 3000, patchPending: 550, vulnerable: 500 }
            ]
        };
        return totalPatchResult;
    }

    function getGroupByLocationResults() {
        let groupedPatchResult = {
            headers: { location: "Location", total: "Total", patchPending: "Patch Pending", vulnerable: "Vulnerable" },
            values: [
                { location: "DC1", total: 1000, patchPending: 250, vulnerable: 400 },
                { location: "DC2", total: 1000, patchPending: 100, vulnerable: 0 },
                { location: "DC3", total: 1000, patchPending: 200, vulnerable: 100 }
            ]
        };
        return groupedPatchResult;
    };

    function getGroupByProductResults() {
        let groupedPatchResult = {
            headers: { product: "Product", total: "Total", patchPending: "Patch Pending", vulnerable: "Vulnerable" },
            values: [
                { product: "Product-1", total: 500, patchPending: 50, vulnerable: 100 },
                { product: "Product-2", total: 300, patchPending: 30, vulnerable: 50 },
                { product: "Product-3", total: 200, patchPending: 20, vulnerable: 50 },
                { product: "Product-4", total: 500, patchPending: 150, vulnerable: 20 },
                { product: "Product-5", total: 500, patchPending: 150, vulnerable: 30 },
                { product: "Product-6", total: 1000, patchPending: 150, vulnerable: 250 }
            ]
        };
        return groupedPatchResult;
    };

    function getGroupByOSResults() {
        let groupedPatchResult = {
            headers: { os: "OS Type", total: "Total", patchPending: "Patch Pending", vulnerable: "Vulnerable" },
            values: [
                { os: "Windows", total: 1000, patchPending: 200, vulnerable: 400 },
                { os: "Linux", total: 2000, patchPending: 350, vulnerable: 100 }
            ]
        };
        return groupedPatchResult;
    };

    function getGroupByOwnerResults() {
        let groupedPatchResult = {
            headers: { owner: "Owner", total: "Total", patchPending: "Patch Pending", vulnerable: "Vulnerable" },
            values: [
                { owner: "Owner-1", total: 500, patchPending: 50, vulnerable: 100 },
                { owner: "Owner-2", total: 300, patchPending: 30, vulnerable: 50 },
                { owner: "Owner-3", total: 200, patchPending: 20, vulnerable: 50 },
                { owner: "Owner-4", total: 500, patchPending: 150, vulnerable: 20 },
                { owner: "Owner-5", total: 500, patchPending: 150, vulnerable: 30 },
                { owner: "Owner-6", total: 1000, patchPending: 150, vulnerable: 250 }
            ]
        };
        return groupedPatchResult;
    };

    function createChartObject(data) {
        let chartObject = new ChartObject({ legend: { display: true } });
        for (let k in data.headers) {
            if (typeof data.headers[k] !== 'function') {
                if (k === 'total' || k === 'patchPending' || k === 'vulnerable') {
                    chartObject.addLabelToData(data.headers[k]);
                }
            }
        }
        for (let index = 0; index < data.values.length; index++) {
            let element = data.values[index];
            let array = [];
            for (let k in element) {
                if (typeof element[k] !== 'function') {
                    if (k === 'total' || k === 'patchPending' || k === 'vulnerable') {
                        array.push(element[k]);
                    } else {
                        chartObject.addSeriesValue(element[k]);
                    }
                }
            }
            chartObject.addDataValue(array);
        }
        return chartObject;
    };
};