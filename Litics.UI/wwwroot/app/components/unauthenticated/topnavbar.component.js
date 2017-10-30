function UnauthenticatedTopnavBarCtrl() {
    let vm = this;
}

angular.module('updater')
    .component('unauthenticatedtopnavbarComponent', {
        templateUrl: 'app/views/unauthenticated/topnavbar.html',
        controller: UnauthenticatedTopnavBarCtrl
    });