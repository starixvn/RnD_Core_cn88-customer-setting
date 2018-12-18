/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="blockui/block-ui.ts" />
/// <reference path="module-registrater.ts" />
/// <reference path="resources/resource-translation.ts" />

module CustomerSetting {
    "use strict";

    interface IServerRequestCallback {
        (response: ng.IHttpPromiseCallbackArg<any>): void;
    }

    export class RequestService {
        public static $inject: string[] = ["$http", "$window", "resourceService", "blockUiService"];

        private $http: ng.IHttpService;
        private $window: ng.IWindowService;
        private resourceService: ResourceService;
        private blockUiService: BlockUiService;
        private sharedScope: any;

        constructor(
            $http: ng.IHttpService,
            $window: ng.IWindowService,
            resourceService: ResourceService,
            blockUiService: BlockUiService) {
            this.$http = $http;
            this.$window = $window;
            this.resourceService = resourceService;
            this.blockUiService = blockUiService;
        }

        public registerSharedScope(sharedScope: any): void {
            this.sharedScope = sharedScope;
        }

        public request(path: string, parameters: any, callback: IServerRequestCallback) {
            this.blockUiService.blockUi();

            this.$http
                .post(path, parameters)
                .then((response: ng.IHttpPromiseCallbackArg<any>) => {
                    if (typeof response.data === "string") {
                        this.$window.document.write(response.data);
                    } else if (response.data.code === -302 && response.data.redirect) {
                        this.$window.location.href = this.$window.location.origin + response.data.redirect;
                    } else {
                        callback(response);
                    }
                }, (response: ng.IHttpPromiseCallbackArg<any>) => {
                    var error: Result;

                    if (response) {
                        if (response.status === 401) {
                            error = Result.notAuthorizedError;
                            error.message = this.resourceService.getMessage(error.message);
                        } else if (response.data) {
                            error = new Result(response.status, response.data.message, MessageType.error);
                        } else {
                            error = Result.systemError;
                            error.message = this.resourceService.getMessage(error.message);
                        }
                    }

                    this.sharedScope.requestMsgResult = error;
                })
                .finally(() => {
                    this.blockUiService.unblockUi();
                });
        }
    }

    ModuleRegister.service("requestService", RequestService);
}