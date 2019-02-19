/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/services/app-config-service.ts" />
/// <reference path="../../../../core/services/request-service.ts" />
/// <reference path="../../../../core/module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface ILoadSuggestedHeadlines {
        (someFirstCharacters: string): ng.IPromise<string[]>;
    }

    interface IHeadlinesSuggestionScope extends ng.IScope {
        loadSuggestedHeadlines: ILoadSuggestedHeadlines;
    }

    class HeadlineSuggestionDirective implements ng.IDirective {
        public static $inject: string[] = ["requestService", "appConfigService"];

        private static suggestingHeadlinesPath: string = "CreateCN88Customer/GetHeadlineSuggestionNames";

        public replace: boolean = true;
        public require: string = "ngModel";
        public templateUrl: string = "headline-suggestion.htm";

        public link: ng.IDirectiveLinkFn = (scope: IHeadlinesSuggestionScope) => {
            scope.loadSuggestedHeadlines = (someFirstCharacters: string) => {
                var requestPath: string = this.appConfigService.buildPath(HeadlineSuggestionDirective.suggestingHeadlinesPath);

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

    ModuleRegister.directive("headlineSuggestion", HeadlineSuggestionDirective);
}