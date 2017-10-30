function CommonCtrl() {
    let vm = this;
}

angular.module('updater')
    .component('commonComponent', {
        templateUrl: 'app/views/common/content.html',
        controller: CommonCtrl
    });