/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/services/app-config-service.ts" />
/// <reference path="../../../../core/services/request-service.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface ILoadSuggestedMainShareholder {
        (someFirstCharacters: string): ng.IPromise<string[]>;
    }

    interface IMainShareholderSuggestion extends ng.IScope {
        loadSuggestedMainShareholders: ILoadSuggestedMainShareholder;
    }

    class MainShareholderSuggestionDirective implements ng.IDirective {
        public static $inject: string[] = ["requestService", "appConfigService"];

        private static suggestingMainShareholderPath: string = "CreateCN88Customer/GetMainShareholderSuggestionNames";

        public replace: boolean = true;
        public require: string = "ngModel";
        public templateUrl: string = "main-shareholder-suggestion.htm";

        public link: ng.IDirectiveLinkFn = (scope: IMainShareholderSuggestion) => {
            scope.loadSuggestedMainShareholders = (someFirstCharacters: string) => {
                var requestPath: string = this.appConfigService.buildPath(MainShareholderSuggestionDirective.suggestingMainShareholderPath);

                return this.requestService.get(
                    requestPath,
                    { username: someFirstCharacters },
                    (response: ng.IHttpPromiseCallbackArg<any>) => response.data,
                    false);
            };
        };

        private requestService: RequestService;
        private appConfigService: AppConfigService;

        constructor(requestService: RequestService, appConfigService: AppConfigService) {
            this.requestService = requestService;
            this.appConfigService = appConfigService;
        }
    }

    ModuleRegister.directive("mainShareholderSuggestion", MainShareholderSuggestionDirective);
}