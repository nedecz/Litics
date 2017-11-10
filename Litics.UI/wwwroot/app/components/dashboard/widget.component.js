function WidgetCtrl($scope, $log) {
    let vm = this;
}
angular.module('updater')
    .component('widgetComponent', {
        controller: WidgetCtrl
    })
    .directive('compileDirective', function ($compile) {
        return {
            restrict: "E",
            replace: true,
            link: function (scope, element, attr) {
                scope.$watch(function () {
                    return attr.directive;
                }, function (val) {
                    element.html("");
                    if (val) {
                        console.log(val);
                        var directive = $compile(angular.element(val))(scope);
                        element.append(directive);
                    }
                });
            }
        };
    })
    .directive('baseWidget', function ($compile) {
        return {
            restrict: "E",
            templateUrl: 'app/views/dashboard/widget.html',
            replace: true,
            scope: {
                widget: "=",
                directive: "="
            },
        };
    })
    //Directives for example
    .directive('directiveOne', function ($compile) {
        return {
            replace: true,
            template: "<div>i'm directive one</div>"
        };
    })
    .directive('directiveTwo', function ($compile) {
        return {
            replace: true,
            scope: { val: "=" },
            template: "<div>i'm directive two with val={{val}}</div>"
        };
    })
    .directive('charts', function ($compile) {
        function linkFunc(scope, element, attrs) {
        }
        return {
            templateUrl: 'app/views/dashboard/widgets/linechart.html',
            scope: {
                widget: "=",
                config: "=",
                events: "="
            }
        };
    });