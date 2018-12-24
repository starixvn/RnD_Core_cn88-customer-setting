/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface IBlockUiDirectiveScope extends ng.IScope {
        isLoading: boolean;
    }

    class BlockUiDirective implements ng.IDirective {
        public replace: boolean = true;

        public templateUrl: string = "Directives/BlockUI/block-ui.htm";

        public controller: any = ["$scope", "blockUiService", ($scope: IBlockUiDirectiveScope, blockUiService: BlockUiService) => {
            blockUiService.setup($scope);
        }];
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