function patchDashboardCtrl($location, $state, $stateParams, patchDashboardService, DTOptionsBuilder) {
    let vm = this;
    vm.chartObject = {}
    vm.totalPatchResult = [];
    vm.groupedLocationPatchResult = [];
    vm.groupedProductPatchResult = [];
    vm.groupedOsPatchResult = [];
    vm.groupedOwnerPatchResult = [];
    vm.groupedBy = null;
    vm.dtOptions = DTOptionsBuilder.newOptions().withPaginationType('full_numbers').withBootstrap()
        .withDOM('fBrtipl').withOption('bFilter', false)
        .withButtons([
            'copy',
            'print',
            'csv',
        ]);

    //Functions
    vm.$onInit = function () {
        vm.totalPatchResult = getTotalPatchResults();
    };
    vm.clearGroupResults = clearGroupResults;
    vm.getGroupByLocationResults = getGroupByLocationResults;
    vm.getGroupByProductResults = getGroupByProductResults;
    vm.getGroupByOSResults = getGroupByOSResults;
    vm.getGroupByOwnerResults = getGroupByOwnerResults;

    function clearGroupResults() {
        vm.groupedLocationPatchResult = [];
        vm.groupedProductPatchResult = [];
        vm.groupedOsPatchResult = [];
        vm.groupedOwnerPatchResult = [];
        vm.groupedBy = null;
    };

    function getTotalPatchResults() {
        //TODO proper api call with promise
        let totalPatchResult = patchDashboardService.getTotalPatchResults();
        return totalPatchResult;
    };

    function getGroupByLocationResults() {
        //TODO proper api call with promise
        let groupedPatchResult = patchDashboardService.getGroupByLocationResults();
        vm.groupedLocationPatchResult = groupedPatchResult;
        vm.groupedBy = "Location (DC)";
        vm.chartObject = patchDashboardService.createChartObject(groupedPatchResult);
    };
    function getGroupByProductResults() {
        //TODO proper api call with promise
        let groupedPatchResult = patchDashboardService.getGroupByProductResults();
        vm.groupedProductPatchResult = groupedPatchResult;
        vm.groupedBy = "Product";
        vm.chartObject = patchDashboardService.createChartObject(groupedPatchResult);
    };
    function getGroupByOSResults() {
        //TODO proper api call with promise
        let groupedPatchResult = patchDashboardService.getGroupByOSResults();
        vm.groupedOsPatchResult = groupedPatchResult;
        vm.groupedBy = "OS";
        vm.chartObject = patchDashboardService.createChartObject(groupedPatchResult);
    };
    function getGroupByOwnerResults() {
        //TODO proper api call with promise
        let groupedPatchResult = patchDashboardService.getGroupByOwnerResults();
        vm.groupedOwnerPatchResult = groupedPatchResult;
        vm.groupedBy = "Owner";
        vm.chartObject = patchDashboardService.createChartObject(groupedPatchResult);
    };
};
angular
    .module('updater')
    .component('patchDashboard', {
        templateUrl: 'app/views/patchdashboard.html',
        controller: patchDashboardCtrl
    });