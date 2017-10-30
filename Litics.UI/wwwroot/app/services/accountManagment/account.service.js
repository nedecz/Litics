'use strict';
angular.module('updater')
    .service('accountService', accountService);

accountService.inject = ['$http', '$q', 'authenticationService', '__env'];

function accountService($http, $q, authenticationService, __env) {
    let apiBase = __env.apiUrl + "/Account/";

    let service = {
        addUserToRole: addUserToRole,
        changePassword: changePassword,
        createApp: createApp,
        createEsIndex: createEsIndex,
        getApp: getApp,
        getRoles: getRoles,
        deleteApp: deleteApp,
        deleteEsIndex: deleteEsIndex,
        deleteUserFromRole: deleteUserFromRole,
        deleteUser: deleteUser,
        getUsers: getUsers,
        lockUser: lockUser,
        unlockUser: unlockUser,
        modifyApp: modifyApp,
        setPassword: setPassword
    };
    return service;

    function addUserToRole(userId, role) {
        let data = {
            UserId: userId,
            Role: role
        };
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "AddUserToRole",
            method: "POST",
            data: data
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function changePassword() {

    }

    function createApp() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "CreateApp",
            method: "POST",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function createEsIndex() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "CreateEsIndex",
            method: "POST",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function getApp() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "GetApp",
            method: "GET",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function getRoles() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "GetRoles",
            method: "GET",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function deleteApp() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "DeleteApp",
            method: "DELETE",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function deleteEsIndex() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "DeleteEsIndex",
            method: "DELETE",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function deleteUserFromRole(userId, role) {
        let data = {
            UserId: userId,
            Role: role
        };
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "DeleteUserFromRole",
            method: "POST",
            data: data
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function getUsers() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "GetUsers",
            method: "GET",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function modifyApp() {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "ModifyApp",
            method: "POST",
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function lockUser(userId) {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "LockUser?userId=" + userId,
            method: "POST"
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function unlockUser(userId) {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "UnlockUser?userId=" + userId,
            method: "POST"
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }
    function deleteUser(userId) {
        let deferred = $q.defer();
        authenticationService.setHeader($http);
        $http({
            url: apiBase + "DeleteUser?userId=" + userId,
            method: "POST"
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }

    function setPassword() {

    }

    function register(data) {
        console.log(data);
        let deferred = $q.defer();
        $http({
            url: apiBase + "/Register",
            method: "POST",
            data: data,
        }).then(function (resp, status, headers, config) {
            deferred.resolve(resp.data);
        }, function (resp, status, headers, config) {
            deferred.reject(resp);
        });
        return deferred.promise;
    }
};