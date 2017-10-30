function ApplicationCtrl(accountService) {
    let vm = this;

    vm.application = null;
    vm.createAppClicked = false;
    vm.modifyAppClicked = false;
    vm.deleteAppClicked = false;

    vm.createApp = createApp;
    vm.createEsIndex = createEsIndex;
    vm.getApp = getApp;
    vm.deleteApp = deleteApp;
    vm.deleteEsIndex = deleteEsIndex;
    vm.modifyApp = modifyApp;

    vm.$onInit = function () {
        vm.getApp();
    };

    function createApp() {
        accountService.createApp().then(function (data) {
            vm.application = {};
            vm.application.AppId = data.AppId;
            vm.application.ApiKey = data.ApiKey;
            vm.createAppClicked = true;
            vm.modifyAppClicked = false;
        }, function (data) {
            console.log(data);
        });
    }

    function createEsIndex() {

    }

    function getApp() {
        accountService.getApp().then(function (data) {
            vm.application = {};
            vm.application.AppId = data.AppId;
        }, function (data) {
            console.log(data);
        });
    }

    function deleteApp() {
        accountService.deleteApp().then(function (data) {
            vm.application = null;
            console.log(data);
        }, function (data) {
            console.log(data);
        });
    }

    function deleteEsIndex() {

    }

    function modifyApp() {
        accountService.modifyApp().then(function (data) {
            vm.application.AppId = data.AppId;
            vm.application.ApiKey = data.ApiKey;
            vm.modifyAppClicked = true;
            vm.createAppClicked = false;
        }, function (data) {
            console.log(data);
        });
    }
}

angular.module('updater')
    .component('applicationComponent', {
        templateUrl: 'app/views/accountManagment/application.html',
        controller: ApplicationCtrl
    });