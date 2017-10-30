(function () {
    // Default environment variables
    var __env = {};

    // Import variables if present
    if (window) {
        Object.assign(__env, window.__env);
    }
})();

var app = angular.module('updater', ['ui.router','ui.router.state.events', 'ui.bootstrap', 'angularMoment', 'angular-nicescroll', 'angular.filter', 'chart.js', 'pasvaz.bindonce', 'checklist-model', 'datatables', 'datatables.bootstrap', 'datatables.buttons'])
    .constant('__env', __env)
    .config(['$stateProvider', '$locationProvider', '$urlRouterProvider', '__env', '$httpProvider',
        function ($stateProvider, $locationProvider, $urlRouterProvider, __env, $httpProvider) {

            $locationProvider.hashPrefix('');
            //route to auth/login if nothing else exists
            $urlRouterProvider.otherwise('/home')

            $stateProvider
                .state('home', {
                    url: "/home",
                    component: "unauthenticatedcommonComponent",
                    authenticate: false
                })
                .state('home.login ', {
                    url: "/login",
                    component: "loginComponent",
                    authenticate: false
                })
                .state('register', {
                    url: "/register",
                    component: "registerComponent",
                    authenticate: false
                })
                .state('app', {
                    url: "/app",
                    component: "commonComponent",
                    authenticate: false
                })
                .state('app.account', {
                    url: "/account",
                    component: "accountComponent",
                    authenticate: false
                })
                .state('app.account.application', {
                    url: "/application",
                    component: "applicationComponent",
                    authenticate: false
                })
                .state('app.account.users', {
                    url: "/users",
                    component: "usersComponent",
                    authenticate: true
                })
                .state('app.dashboard', {
                    url: "/dashboard",
                    component: 'dashboardComponent',
                    data: { pageTitle: 'Dashboard' },
                    authenticate: false
                });
        }])
    .run(['$rootScope', '$state', 'authenticationService', function ($rootScope, $state, authenticationService) {
        //check authentication at each state change      
        $rootScope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
            if (toState.authenticate && !authenticationService.validateToken()) {
                // User isn’t authenticated
                event.preventDefault();
                console.log("User is not authenticated, moving to login page");
                //go back to login page
                $state.go("home");
            } else {
                console.log("true");
            }
        });
    }]);
