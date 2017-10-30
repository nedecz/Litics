function AccountCtrl() {
    let vm = this;
}

angular.module('updater')
    .component('accountComponent', {
        templateUrl: 'app/views/accountManagment/account.html',
        controller: AccountCtrl
    });