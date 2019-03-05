/// <reference path="../../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../../../core/module-registrater.ts" />
/// <reference path="../../../models/account-detail.ts" />
/// <reference path="username-suffix-group.ts" />

namespace CustomerSetting {
    export interface IUsernameSuffixScope extends ng.IScope {
        suffixIndex: number;
        suffix: string;
        range: string[];
        validateUsername: IValidateUsername;
        validateSuffix: () => void;
    }

    class UsernameSuffixDirective implements ng.IDirective {
        public replace: boolean = true;

        public get scope(): any {
            return {
                suffixIndex: "=",
                suffix: "=?",
                validateUsername: "="
            };
        }

        public templateUrl: string = "username-suffix.htm";

        public link: ng.IDirectiveLinkFn = (scope: IUsernameSuffixScope) => {
            scope.range = this.generateSuffixRange();
            scope.suffix = scope.suffix || scope.range[0];
            scope.validateSuffix = () => {
                scope.validateUsername(scope.suffixIndex, scope.suffix);
            };
        };

        private generateSuffixRange(): string[] {
            var numeberRange: string[] = this.generateCharacterRange("0", "9");
            var characterRange: string[] = this.generateCharacterRange("A", "Z");
            var availableRange: string[] = numeberRange.concat(characterRange);

            return availableRange;
        }

        private generateCharacterRange(start: string, stop: string): string[] {
            var range: string[] = [];
            var begin: number = start.charCodeAt(0);
            var end: number = stop.charCodeAt(0);

            for (var index = begin; index <= end; index++) {
                range.push(String.fromCharCode(index));
            }

            return range;
        }
    }

    ModuleRegister.directive("usernameSuffix", UsernameSuffixDirective);
}