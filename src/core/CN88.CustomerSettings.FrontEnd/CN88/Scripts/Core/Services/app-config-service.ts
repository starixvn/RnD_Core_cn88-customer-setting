/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    export class AppConfigService {
        public static $inject: string[] = ["$window"];

        public get thousandSeparator(): string {
            return ",";
        }

        public get decimalSeparator(): string {
            return ".";
        }

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
            return this.applicationPath + "viewCN88CustomerSetting/querySetting?";
        }

        public getCreatePath(): string {
            return this.applicationPath + "createCN88Customer/create";
        }

        public getUpdatePath(): string {
            return this.applicationPath + "updateCN88CustomerSetting/update";
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

    app.run(["$http", "appConfigService", ($http: ng.IHttpService, appConfigService: AppConfigService) => {
        /* tslint:disable:no-string-literal */
        $http.defaults.headers.common["__RequestVerificationToken"] = appConfigService.requestVerificationToken();
        /* tslint:enable:no-string-literal */
    }]);
}