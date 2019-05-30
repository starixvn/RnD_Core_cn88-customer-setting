/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />
/// <reference path="parameters-service.ts" />

namespace CustomerSetting {
    export class ScrollService {
        public static $inject: string[] = ["$window", "parametersService"];

        private $window: ng.IWindowService;
        private parametersService: ParametersService;

        constructor($window: ng.IWindowService, parametersService: ParametersService) {
            this.$window = $window;
            this.parametersService = parametersService;
        }

        public scrollTo(x: number, y: number): void {
            if (this.parametersService.isMobileView
                && this.isIOS()
                && this.$window.top) {
                this.$window.top.scrollTo(x, y);
            }

            this.$window.scrollTo(x, y);
        }

        private isIOS(): boolean {
            return !!navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform);
        }
    }

    ModuleRegister.service("scrollService", ScrollService);
}