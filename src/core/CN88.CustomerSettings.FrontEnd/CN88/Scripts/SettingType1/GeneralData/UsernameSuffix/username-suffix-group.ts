/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../core/models/result.ts" />
/// <reference path="../../../core/module-registrater.ts" />
/// <reference path="../../models/account-detail.ts" />

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
        private static validatingUsernamePath: string = "CreateCustomer/CheckExistedUsername";

        public replace: boolean = true;

        public templateUrl: string = "GeneralData/UsernameSuffix/username-suffix-group.htm";

        public controller: any = [
            "$scope",
            "requestService",
            "appConfigService",
            ($scope: IUsernameSuffixGroupScope, requestService: RequestService, appConfigService: AppConfigService) => {
                $scope.selectedSuffixes = $scope.accountDetail.usernameSuffixes.slice();
                $scope.accountDetail.username = $scope.accountDetail.uplineUsername + $scope.selectedSuffixes.join();

                $scope.validateUsername = (suffixIndex: number, selectedSuffix: string) => {
                    var validatingPath: string = appConfigService.buildPath(UsernameSuffixGroupDirective.validatingUsernamePath);

                    $scope.selectedSuffixes[suffixIndex] = selectedSuffix;
                    $scope.accountDetail.username = $scope.accountDetail.uplineUsername + $scope.selectedSuffixes.join("");

                    requestService.get(
                        validatingPath,
                        { username: $scope.accountDetail.username },
                        (response: ng.IHttpPromiseCallbackArg<any>) => {
                            var validatingResult: Result = new Result(response.data.code, response.data.message, response.data.msgType);

                            $scope.isValid = validatingResult.isSuccessful;
                            $scope.isChanged = true;
                        });
                };
            }];
    }

    ModuleRegister.directive("usernameSuffixGroup", UsernameSuffixGroupDirective);
}