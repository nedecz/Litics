'use strict';
app.factory('authData', function () {

    let service = {
        authenticationData: {
            IsAuthenticated: false,
            username: "",
            accountname: "",
            expires_in: "",
            issued: "",
            expires: ""
        }
    };
    return service;

});