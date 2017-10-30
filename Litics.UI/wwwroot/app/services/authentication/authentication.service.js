'use strict';
angular.module('updater')
    .service('authenticationService', authenticationService);

authenticationService.inject = ['$http', '$q', '$window', 'authData', '__env'];

function authenticationService($http, $q, $window, moment, authData, __env) {
    let apiBase = __env.apiUrl;

    let service = {
        tokenInfo: tokenInfo,
        validateToken: validateToken,
        setHeader: setHeader,
        getHeader: getHeader
    };
    return service;

    function tokenInfo() {
        let authInfo = {
            accessToken: sessionStorage.getItem("accessToken"),
            userName: sessionStorage.getItem("username"),
            accountname: sessionStorage.getItem("accountname"),
            expires_in: sessionStorage.getItem("expires_in"),
            issued: sessionStorage.getItem("issued"),
            expires: sessionStorage.getItem("expires")
        };
        return authInfo;
    }

    function setHeader(http) {
        console.log(http);
        delete http.defaults.headers.common['X-Requested-With'];
        if ((tokenInfo() != undefined) && (tokenInfo().accessToken != undefined) && (tokenInfo().accessToken != null) && (tokenInfo().accessToken != "")) {
            http.defaults.headers.common['Authorization'] = 'Bearer ' + tokenInfo().accessToken;
            http.defaults.headers.common['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
            authData.authenticationData.username = tokenInfo().username;
            authData.authenticationData.accountname = tokenInfo().accountname;
            authData.authenticationData.IsAuthenticated = true;
            authData.authenticationData.expires_in = tokenInfo().expires_in;
            authData.authenticationData.issued = tokenInfo().issued;
            authData.authenticationData.expires = tokenInfo().expires;
        }
    }

    function getHeader(){
        let header = { Authorization: 'Bearer '+tokenInfo().accessToken };
        console.log(header);
        return header;
    }
    function validateToken() {
        let authInfo = tokenInfo();
        if (authInfo !== 'undefined') {
            var tokenExpiredInUTC = moment.utc(authInfo.expires);
            var utcNow = moment.utc();
            return tokenExpiredInUTC.diff(utcNow) > 0;
        }
        return false;
    }
};