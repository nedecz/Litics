'use strict';
angular.module('updater')
    .service('loginService', loginService);

loginService.inject = ['$http', '$q', 'authData', '__env'];

function loginService($http, $q, authData, __env) {
    let apiBase = __env.apiUrl;

    let service = {
        login: login,
        logout: logout
    };
    return service;


    function login(username, password, accountname) {
        let deferred = $q.defer();
        var authInfo = "grant_type=password&username=" + username + "&password=" + password + "&accountname=" + accountname;
        $http({
            url: apiBase + '/token',
            method: "POST",
            data: authInfo,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded', 'Access-Control-Allow-Origin': '*',
                'Access-Control-Allow-Methods': 'GET, POST'
            }
        }).then(
            function (resp, status, headers, config) {
                authData.authenticationData.IsAuthenticated = true;
                authData.authenticationData.username = resp.data.userName;
                authData.authenticationData.accountname = resp.data.accountname;
                authData.authenticationData.expires_in = resp.data.expires_in;
                authData.authenticationData.issued = resp.data['.issued'];
                authData.authenticationData.expires = resp.data['.expires'];
                sessionStorage.setItem("accessToken", resp.data.access_token);
                sessionStorage.setItem("username", resp.data.userName);
                sessionStorage.setItem("accountname", resp.data.accountname);
                sessionStorage.setItem("expires_in", resp.data.expires_in);
                sessionStorage.setItem("issued", resp.data['.issued']);
                sessionStorage.setItem("expires", resp.data['.expires']);
                deferred.resolve(resp);
            },
            function (resp, status, headers, config) {
                authData.authenticationData.IsAuthenticated = false;
                authData.authenticationData.userName = "";
                authData.authenticationData.accountname = "";
                authData.authenticationData.expires_in = "";
                authData.authenticationData.issued = "";
                authData.authenticationData.expires = "";
                deferred.reject(resp);
            });
        return deferred.promise;
    }

    function logout() {
        sessionStorage.removeItem("accessToken");
        sessionStorage.removeItem("username");
        sessionStorage.removeItem("accountname");
        sessionStorage.removeItem("expires_in");
        sessionStorage.removeItem("issued");
        sessionStorage.removeItem("expires");
        $http.defaults.headers.common['Authorization'] = '';
        authData.authenticationData.IsAuthenticated = false;
        authData.authenticationData.userName = "";
        authData.authenticationData.accountname = "";
        authData.authenticationData.expires_in = "";
        authData.authenticationData.issued = "";
        authData.authenticationData.expires = "";
    }
};