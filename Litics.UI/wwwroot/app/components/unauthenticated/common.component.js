function UnauthenticatedCommonCtrl() {
    let vm = this;
}

angular.module('updater')
    .component('unauthenticatedcommonComponent', {
        templateUrl: 'app/views/unauthenticated/content.html',
        controller: UnauthenticatedCommonCtrl
    });