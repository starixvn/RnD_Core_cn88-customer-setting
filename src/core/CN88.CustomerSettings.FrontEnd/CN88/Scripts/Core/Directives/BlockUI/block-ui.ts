/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface IBlockUiDirectiveScope extends ng.IScope {
        isLoading: boolean;
    }

    class BlockUiDirective implements ng.IDirective {
        public static $inject: string[] = ["blockUiService"];

        public replace: boolean = true;

        public templateUrl: string = "block-ui.htm";

        public link: ng.IDirectiveLinkFn = (scope: IBlockUiDirectiveScope) => {
            this.blockUiService.setup(scope);
        };

        private blockUiService: BlockUiService;

        constructor(blockUiService: BlockUiService) {
            this.blockUiService = blockUiService;
        }
    }

    ModuleRegister.directive("blockUi", BlockUiDirective);

    export class BlockUiService {
        private scope: IBlockUiDirectiveScope;

        public setup(scope: IBlockUiDirectiveScope) {
            this.scope = scope;
        }

        public blockUi(): void {
            this.scope.isLoading = true;
        }

        public unblockUi(): void {
            this.scope.isLoading = false;
        }
    }

    ModuleRegister.service("blockUiService", BlockUiService);
}