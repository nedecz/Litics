'use strict';
angular.module('updater')
    .service('registerService', registerService);

registerService.inject = ['$http', '$q', 'authData', '__env'];

function registerService($http, $q, authData, __env) {
    let apiBase = __env.apiUrl;

    let service = {
        register : register
    };
    return service;

    function register(data) {
        console.log(data);
        let deferred = $q.defer();
        $http({
            url: apiBase + "/Account/Register",
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