/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/models/result.ts" />
/// <reference path="../../../../core/services/app-config-service.ts" />
/// <reference path="../../../../core/services/request-service.ts" />
/// <reference path="../../../../core/module-registrater.ts" />
/// <reference path="../../../models/account-detail.ts" />

module CustomerSetting {
    "use strict";

    export interface IValidateUsername {
        (suffixIndex: number, selectedSuffix: string): void;
    }

    export interface IUsernameSuffixGroupScope extends ng.IScope {
        accountDetail: AccountDetail;
        validateUsername: IValidateUsername;
        isValid: boolean;
        isChanged: boolean;
        selectedSuffixes: string[];
    }

    class UsernameSuffixGroupDirective implements ng.IDirective {
        public static $inject: string[] = ["requestService", "appConfigService"];

        private static validatingUsernamePath: string = "CreateCustomer/CheckExistedUsername";

        public replace: boolean = true;

        public templateUrl: string = "SubSections/GeneralData/UsernameSuffix/username-suffix-group.htm";

        public link: ng.IDirectiveLinkFn = (scope: IUsernameSuffixGroupScope) => {
            scope.selectedSuffixes = scope.accountDetail.usernameSuffixes.slice();
            scope.accountDetail.username = scope.accountDetail.uplineUsername + scope.selectedSuffixes.join("");

            scope.validateUsername = (suffixIndex: number, selectedSuffix: string) => {
                var validatingPath: string = this.appConfigService.buildPath(UsernameSuffixGroupDirective.validatingUsernamePath);

                scope.selectedSuffixes[suffixIndex] = selectedSuffix;
                scope.accountDetail.username = scope.accountDetail.uplineUsername + scope.selectedSuffixes.join("");

                this.requestService.get(
                    validatingPath,
                    { username: scope.accountDetail.username },
                    (response: ng.IHttpPromiseCallbackArg<any>) => {
                        var validatingResult: Result = new Result(response.data.code, response.data.message, response.data.msgType);

                        scope.isValid = validatingResult.isSuccessful;
                        scope.isChanged = true;
                    });
            };
        };

        private requestService: RequestService;
        private appConfigService: AppConfigService;

        constructor(requestService: RequestService, appConfigService: AppConfigService) {
            this.requestService = requestService;
            this.appConfigService = appConfigService;
        }
    }

    ModuleRegister.directive("usernameSuffixGroup", UsernameSuffixGroupDirective);
}