function DashboardCtrl() {
    let vm = this;
}

angular.module('updater')
    .component('dashboardComponent', {
        templateUrl: 'app/views/dashboard/dashboard.html',
        controller: DashboardCtrl
    });