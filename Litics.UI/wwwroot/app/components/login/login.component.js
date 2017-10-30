function LoginCtrl(loginService, $state, $stateParams) {
    let vm = this;

    vm.loginData = {
        accountname: "",
        username: "",
        password: ""
    }

    vm.login = login;

    function login() {
        loginService.login(vm.loginData.username, vm.loginData.password, vm.loginData.accountname)
            .then(
            function (response) {
                console.log("It's Authenticated!");
                
            }, function (response) {
                console.log(response);
            });
    };
}

angular.module('updater')
    .component('loginComponent', {
        templateUrl: 'app/views/login/login.html',
        controller: LoginCtrl
    });