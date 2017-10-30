function RegisterCtrl(registerService) {
    let vm = this;
    vm.register = register;

    function register(data) {
        console.log(data);
        registerService.register(data).then(
            function (data) {
                console.log(data);
            }, function (data) {
                console.log(data)
             }
        );
    }
}

angular.module('updater')
    .component('registerComponent', {
        templateUrl: 'app/views/register/register.html',
        controller: RegisterCtrl
    });