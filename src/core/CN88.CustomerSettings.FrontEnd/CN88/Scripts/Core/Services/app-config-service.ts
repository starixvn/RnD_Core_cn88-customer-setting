/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class AppConfigService {
        public static $inject: string[] = ["$window"];

        private applicationPath: string;
        private antiForgeryToken: string;
        private editable: boolean;

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
                this.editable = appConfig.editable;

                if (this.applicationPath && this.applicationPath[this.applicationPath.length - 1] !== "/") {
                    this.applicationPath += "/";
                }
            }
        }

        public getQueryPath(): string {
            return this.applicationPath + "viewCustomerSetting/querySetting?";
        }

        public getCreatePath(): string {
            return this.applicationPath + "createCustomer/create";
        }

        public getUpdatePath(): string {
            return this.applicationPath + "updateCustomerSetting/update";
        }

        public buildPath(relativePath: string): string {
            return this.applicationPath + relativePath;
        }

        public requestVerificationToken(): string {
            return this.antiForgeryToken;
        }

        public canUpdate(): boolean {
            return this.editable;
        }
    }

    ModuleRegister.service("appConfigService", AppConfigService);
}