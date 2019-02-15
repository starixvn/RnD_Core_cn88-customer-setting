/// <reference path="../../typings/angularjs/angular.d.ts" />
/// <reference path="../directives/blockui/block-ui.ts" />
/// <reference path="../models/result.ts" />
/// <reference path="../resources/resource-service.ts" />
/// <reference path="../module-registrater.ts" />

module CustomerSetting {
    "use strict";

    interface IServerRequestCallback {
        (response: ng.IHttpPromiseCallbackArg<any>): any;
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

        public get(path: string, parameters: any, callback: IServerRequestCallback, blockUI: boolean = true): ng.IPromise<any> {
            if (blockUI) {
                this.blockUiService.blockUi();
            }

            return this.$http
                .get(path, { params: parameters })
                .then(
                    (response: ng.IHttpPromiseCallbackArg<any>) => this.handleSuccessfulResponse(response, callback),
                    (response: ng.IHttpPromiseCallbackArg<any>) => this.handleErrorResponse(response))
                .finally(() => {
                    if (blockUI) {
                        this.blockUiService.unblockUi();
                    }
                });
        }

        public post(path: string, parameters: any, callback: IServerRequestCallback, blockUI: boolean = true): ng.IPromise<any> {
            if (blockUI) {
                this.blockUiService.blockUi();
            }

            return this.$http
                .post(path, parameters)
                .then(
                    (response: ng.IHttpPromiseCallbackArg<any>) => this.handleSuccessfulResponse(response, callback),
                    (response: ng.IHttpPromiseCallbackArg<any>) => this.handleErrorResponse(response))
                .finally(() => {
                    if (blockUI) {
                        this.blockUiService.unblockUi();
                    }
                });
        }

        private handleSuccessfulResponse(response: ng.IHttpPromiseCallbackArg<any>, callback: IServerRequestCallback): any {
            if (typeof response.data === "string") {
                this.$window.document.write(response.data);
            } else if (response.data.code === -302 && response.data.redirect) {
                this.$window.location.href = this.$window.location.origin + response.data.redirect;
            } else {
                return callback(response);
            }
        }

        private handleErrorResponse(response: ng.IHttpPromiseCallbackArg<any>): any {
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
        }
    }

    ModuleRegister.service("requestService", RequestService);
}