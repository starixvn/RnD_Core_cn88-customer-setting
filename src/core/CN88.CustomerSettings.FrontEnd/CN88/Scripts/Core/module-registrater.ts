/// <reference path="../typings/angularjs/angular.d.ts" />

var app: ng.IModule = angular.module("customerSetting", ["ui.bootstrap"]);

app.config(["$compileProvider", ($compileProvider: any) => {
    $compileProvider.debugInfoEnabled(false);
    $compileProvider.commentDirectivesEnabled(false);
    $compileProvider.cssClassDirectivesEnabled(false);
}]);

namespace CustomerSetting {
    export class ModuleRegister {
        public static controller(name: string, target: Function) {
            app.controller(name, target);
        }

        public static service(name: string, target: Function) {
            app.service(name, target);
        }

        public static factory(name: string, target: Function) {
            app.factory(name, target);
        }

        public static filter(name: string, target: Function) {
            app.filter(name, target);
        }

        public static directive(name: string, target: any) {
            var directiveFactory: ng.IDirectiveFactory = function () {
                var args = Array.prototype.concat.apply([null], arguments);

                return new (Function.prototype.bind.apply(target, args));
            };

            directiveFactory.$inject = target.$inject;
            app.directive(name, directiveFactory);
        }

        public static value(name: string, target: Object) {
            app.value(name, target);
        }
    }
}