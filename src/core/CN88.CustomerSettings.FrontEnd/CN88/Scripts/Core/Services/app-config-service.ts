/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class AppConfigService {
        public static $inject: string[] = ["$window"];

        private applicationPath: string;
        private antiForgeryToken: string;

        private $window: ng.IWindowService;

        constructor($window: ng.IWindowService) {
            var appConfig: any;

            this.$window = $window;

            /* tslint:disable:no-string-literal */
            appConfig = this.$window["appConfig"] || {};
            /* tslint:enable:no-string-literal */

            if (appConfig) {
                this.applicationPath = appConfig.applicationPath;
                this.antiForgeryToken = appConfig.antiForgeryToken;

                if (this.applicationPath && this.applicationPath[this.applicationPath.length - 1] !== "/") {
                    this.applicationPath += "/";
                }
            }
        }

        public getQueryPath(): string {
            return this.applicationPath + "viewCustomerSetting/querySetting?";
        }

        public getUpdatePath(): string {
            return this.applicationPath + "updateCustomerSetting/updateSetting";
        }

        public requestVerificationToken(): string {
            return this.antiForgeryToken;
        }
    }

    ModuleRegister.service("appConfigService", AppConfigService);
}