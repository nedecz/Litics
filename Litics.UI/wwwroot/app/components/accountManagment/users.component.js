function UsersCtrl(accountService) {
    let vm = this;

    vm.users = [];
    vm.roles = [];
    vm.selectedUser = null;
    vm.selectedRole = null;

    vm.isArray = angular.isArray;
    vm.getUsers = getUsers;
    vm.getRoles = getRoles;
    vm.selectUser = selectUser;
    vm.addUserToRole = addUserToRole;
    vm.deleteUserFromRole = deleteUserFromRole;
    vm.lockUser = lockUser;
    vm.unlockUser = unlockUser;
    vm.deleteUser = deleteUser;

    vm.$onInit = function () {
        vm.getUsers();
    };

    function selectUser(user) {
        vm.selectedUser = user;
    }

    function addUserToRole() {
        console.log("Add user to role...")
        accountService.addUserToRole(vm.selectedUser.UserId,vm.selectedRole).then(function (data) {
            console.log(data);
            getUsers();
        }, function (data) {
            console.log(data);
        });
    }
    
    function getUsers() {
        accountService.getUsers().then(function (data) {
            vm.users = data;
        }, function (data) {
            console.log(data);
        });
    }

    function getRoles() {
        accountService.getRoles().then(function (data) {
            vm.roles = data;
        }, function (data) {
            console.log(data);
        });
    }

    function deleteUserFromRole() {
        console.log("Delete user from role...");
        accountService.deleteUserFromRole(vm.selectedUser.UserId,vm.selectedRole).then(function (data) {
            console.log(data);
            getUsers();
        }, function (data) {
            console.log(data);
        });
    }

    function lockUser() {
        console.log("Lock user...")
        accountService.lockUser(vm.selectedUser.UserId).then(function (data) {
            console.log(data);
            getUsers();
        }, function (data) {
            console.log(data);
        });
    }

    function unlockUser() {
        console.log("Unlock user...")
        accountService.unlockUser(vm.selectedUser.UserId).then(function (data) {
            console.log(data);
            getUsers();
        }, function (data) {
            console.log(data);
        });
    }

    function deleteUser() {
        console.log("Delete user...")
        accountService.deleteUser(vm.selectedUser.UserId).then(function (data) {
            console.log(data);
            getUsers();
        }, function (data) {
            console.log(data);
        });
    }
    
}

angular.module('updater')
    .component('usersComponent', {
        templateUrl: 'app/views/accountManagment/users.html',
        controller: UsersCtrl
    });