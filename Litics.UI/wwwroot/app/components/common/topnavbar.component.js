function TopnavbarCtrl(loginService) {
    let vm = this;

    vm.logout = logout;

    function logout(){
        console.log("Logout...");
        loginService.logout();
    }
}

angular.module('updater')
    .component('topnavbarComponent', {
        templateUrl: 'app/views/common/topnavbar.html',
        controller: TopnavbarCtrl
    });