var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var DemoFrom;
(function (DemoFrom) {
    "use strict";
    var app = angular.module("actorForm", []);
    var DemoFormController = /** @class */ (function () {
        function DemoFormController($http, $location, $window, postingToIframeService) {
            this.$http = $http;
            this.$location = $location;
            this.$window = $window;
            /* tslint:disable:no-string-literal */
            this.applicationPath = this.$window["applicationPath"];
            /* tslint:enable:no-string-literal */
            if (this.applicationPath && this.applicationPath[this.applicationPath.length - 1] !== "/") {
                this.applicationPath += "/";
            }
            this.isCreateNew = true;
            this.postingToIframeService = postingToIframeService;
            this.systemTypes = SampleDataFactory.getSystemTypes();
            this.actorRoles = Actor.getAvailableRoles();
            this.languages = SampleDataFactory.getLanuages();
            this.changeActorRole();
        }
        DemoFormController.prototype.switchForm = function (state) {
            this.preventSavingSession = state;
            this.preventShowingData = !state;
        };
        DemoFormController.prototype.setCreateNew = function (isCreateNew) {
            this.preventSavingSession = state;
            this.preventShowingData = !state;
        };
        DemoFormController.prototype.changeActorRole = function () {
            var selectedRole = angular.isDefined(this.actor) ? this.actor.role : Role.company;
            var selectedSystemId = angular.isDefined(this.actor) ? this.actor.system : this.systemTypes[0].key;
            this.actor = SampleDataFactory.getActor(selectedRole, selectedSystemId);
            this.target = undefined;
            this.targetRoles = Target.getAvailableRoles(selectedRole);
            this.isMultipleUpdates = false;
            this.getTarget(selectedRole);
            this.switchForm(false);
        };
        DemoFormController.prototype.getTarget = function () {
            var selectedRole = angular.isDefined(this.target) ? this.target.role : this.targetRoles[0].key;
            this.uplineRoles = undefined;

            //if (this.isMultipleUpdates) {
            //    this.target = SampleDataFactory.getMultipleTarget(selectedRole, uplineRole);
            //}
            //else {
                this.target = SampleDataFactory.getSingleTarget(selectedRole);
            //}
        };
        DemoFormController.prototype.saveSession = function () {
            var _this = this;
            this.$http.post(this.resolveUrl("home/saveSession"), this.actor).then(function () {
                _this.switchForm(true);
            });
        };
        DemoFormController.prototype.showSetting = function (isCreateNew) {
            var updateUrl = this.resolveUrl("CN88CustomerSetting/CreateCustomer/Create");
            var uplineId = this.target.uplineId;
            var role = this.target.role;
            //$.ajax({
            //    method: "GET",
            //    url: this.resolveUrl("CN88CustomerSetting/CreateCustomer/CheckExistedUsername"),
            //    data: {
            //        username: "SupertestCN88Ricky1d"
            //    }
            //}).done(function (result) { console.log(result); });

            //$.ajax({
            //    method: "GET",
            //    url: this.resolveUrl("CN88CustomerSetting/CreateCN88Customer/GetHeadlineCurrency"),
            //        data: {
            //            username: "MSHCN88"
            //    }
            //}).done(function (result) { console.log(result); });

            //$.ajax({
            //    method: "POST",
            //    url: this.resolveUrl("CN88CustomerSetting/ViewCustomerSetting/QuerySetting"),
            //    data: {
            //        productId: this.target.productId,
            //        customerId: this.target.customerId,
            //        isCreateNew: isCreateNew,
            //        customerLevel: this.target.role,
            //        uplineId: uplineId
            //    }
            //})
            //    .done(function (rawSetting) {
            //        //console.log(rawSetting);
            //        var setting = JSON.parse(rawSetting);
            //        var newUsername = "SupertestCN88Ricky1e";
            //        if (role === 3) {
            //            newUsername = "SupertestCN88Ricky1c001";
            //        } else if (role === 2) {
            //            newUsername = "SupertestCN88Ricky1c001001";
            //        }
            //        else if (role === 1) {
            //            newUsername = "SupertestCN88Ricky1c001001002";
            //        }
            //        setting.settingTypes[0].settingItems[0].username = newUsername;
            //        $.ajax({
            //            method: "POST",
            //            url: updateUrl,
            //            data: {
            //                setting: JSON.stringify(setting),
            //                uplineId: uplineId
            //            }
            //        })
            //            .done(function (msg) {
            //                console.log(msg);
            //            });
            //    });


            var params = {
                language: this.actor.language,
                productId: this.target.productId,
                customerLevel: this.target.role,
                uplineId: this.target.uplineId,
                path: isCreateNew
                    ? this.resolveUrl("CN88CustomerSetting/ViewCN88CustomerSetting/Create")
                    : this.resolveUrl("CN88CustomerSetting/ViewCN88CustomerSetting"),
                frameName: "setting-page"
            };
            if (this.isMultipleUpdates) {
                params.isMultiple = true;
                params.customerIds = this.target.customerIds;
                params.customerNames = this.target.customerNames;
            }
            else {
                params.isMultiple = false;
                params.customerId = this.target.customerId;
            }
            this.postingToIframeService.submitToIframe(params);

        };
        DemoFormController.prototype.viewMatrix = function () {
            var productId = this.target.productId;
            var customerName = "";
            if (this.isMultipleUpdates) {
                customerName = this.target.firstCustomerName;
            }
            else {
                customerName = this.target.customerName;
            }
            this.$window.open(this.resolveUrl("Matrix/ViewCustomerSettingMatrix?showFilter=true&productId=" + productId + "&settingTypeId=1&customerName=" + customerName), "_bank");
        };
        DemoFormController.prototype.resolveUrl = function (requestUrl) {
            return this.applicationPath + requestUrl;
        };
        DemoFormController.$inject = ["$http", "$location", "$window", "postingToIframeService"];
        return DemoFormController;
    }());
    app.controller("DemoFormController", DemoFormController);
    var PostingToIframeDirective = /** @class */ (function () {
        function PostingToIframeDirective(postingToIframeService) {
            var _this = this;
            this.replace = true;
            this.templateUrl = applicationPath + "scripts/demoform/posting-to-iframe.htm";
            this.link = function (scope, element) {
                var form = element[0];
                _this.postingToIframeService.setup(scope);
                scope.submitToIframe = function () {
                    form.submit();
                    return false;
                };
            };
            this.postingToIframeService = postingToIframeService;
        }
        return PostingToIframeDirective;
    }());
    app.directive("postingToIframe", ["postingToIframeService", function () {
        var args = Array.prototype.concat.apply([null], arguments);
        return new (Function.prototype.bind.apply(PostingToIframeDirective, args));
    }]);
    var PostingToIframeService = /** @class */ (function () {
        function PostingToIframeService($timeout) {
            this.$timeout = $timeout;
        }
        PostingToIframeService.prototype.setup = function (scope) {
            this.scope = scope;
        };
        PostingToIframeService.prototype.submitToIframe = function (params) {
            var _this = this;
            Object.keys(params).forEach(function (prop) {
                _this.scope[prop] = params[prop];
            });
            this.$timeout(function () {
                _this.scope.submitToIframe();


            });


        };
        PostingToIframeService.$inject = ["$timeout"];
        return PostingToIframeService;
    }());
    app.service("postingToIframeService", PostingToIframeService);
    var SampleDataFactory = /** @class */ (function () {
        function SampleDataFactory() {
        }
        SampleDataFactory.getSystemTypes = function () {
            return new Array(new KeyValuePair(1, "NAP"), new KeyValuePair(2, "Alpha"), new KeyValuePair(3, "Internal Alpha"), new KeyValuePair(4, "External NAP"), new KeyValuePair(5, "External NAP Bet Setting"));
        };
        SampleDataFactory.getLanuages = function () {
            return new Array(
                new KeyValuePair("en-US", "English (en-US)"),
                new KeyValuePair("zh-TW", "繁體中文 (zh-TW)"),
                new KeyValuePair("zh-Hans", "简体中文 (zh-Hans)"),
                new KeyValuePair("ja-JP", "日本語 (ja-JP)"),
                new KeyValuePair("th-TH", "ภาษาไทย (th-TH)"),
                new KeyValuePair("ko-KR", "한국어 (ko-KR)"),
                new KeyValuePair("vi-VN", "Tiếng Việt (vi-VN)"));
        };
        SampleDataFactory.getActor = function (selectedRole, systemId) {
            var actorId = 0;
            var name = "";
            var subAccId = 0;
            switch (selectedRole) {
                case Role.company:
                    actorId = 8;
                    name = "sa";
                    subAccId = 111;
                    break;
                case Role.super:
                    actorId = 33619385;
                    name = "SuperTestCNE";
                    subAccId = 3;
                    break;
                case Role.master:
                    actorId = 33619391;
                    name = "SuperTestCNE0001";
                    subAccId = 728497;
                    break;
                case Role.agent:
                    actorId = 33619392;
                    name = "SuperTestCNE00010001";
                    subAccId = 728498;
                    break;
            }
            return this.createActor(actorId, name, selectedRole, subAccId, systemId);
        };
        SampleDataFactory.getSingleTarget = function (role) {
            var id = 0;
            var name = "";
            var uplineId = 0;
            switch (role) {
                case Role.super:
                    id = 33619385;
                    name = "SuperTestCNE";
                    break;
                case Role.master:
                    id = 33619391;
                    name = "SuperTestCNE0001";
                    uplineId = 33619385;
                    break;
                case Role.agent:
                    id = 33619392;
                    name = "SuperTestCNE00010001";
                    uplineId = 33619391;
                    break;
                case Role.member:
                    id = 33619393;
                    name = "supertestdemo334340000";
                    uplineId = 33619392;
                    break;
            }
            return this.createSingleTarget(id, name, role, uplineId);
        };
        SampleDataFactory.getMultipleTarget = function (role, uplineRole) {
            var ids = "";
            var names = "";
            switch (role) {
                case Role.super:
                    ids = "1176070,576313,3230178";
                    names = "SuperTest1,SuperTest2,SuperTest12";
                    break;
                case Role.master:
                    ids = "1176083,14167350,14373487";
                    names = "master1,mastertest01,masterkz";
                    break;
                case Role.agent:
                    ids = "1176086,14373905,14374140";
                    names = "agent01,agent03,agentlcs000";
                    break;
                case Role.member:
                    ids = "7890346,8252951,8252986";
                    names = "AGENT01000,AGENT01001,AGENT01002";
                    break;
            }
            return this.createMultipleTargetInfo(ids, names, role, uplineRole);
        };
        SampleDataFactory.createActor = function (id, name, role, subAccId, systemId) {
            return new Actor(id, name, role, subAccId, this.defaultLanguage, systemId);
        };
        SampleDataFactory.createSingleTarget = function (id, name, role, uplineId) {
            return new SingleTarget(id, name, role, uplineId, this.defaultProductId);
        };
        SampleDataFactory.createMultipleTargetInfo = function (ids, names, role, uplineRole) {
            return new MultipleTargets(ids, names, role, uplineRole, this.defaultProductId);
        };
        SampleDataFactory.defaultProductId = 1;
        SampleDataFactory.defaultLanguage = "en-US";
        return SampleDataFactory;
    }());
    var Role;
    (function (Role) {
        Role[Role["company"] = 5] = "company";
        Role[Role["super"] = 4] = "super";
        Role[Role["master"] = 3] = "master";
        Role[Role["agent"] = 2] = "agent";
        Role[Role["member"] = 1] = "member";
    })(Role || (Role = {}));
    var KeyValuePair = /** @class */ (function () {
        function KeyValuePair(key, value) {
            this.key = key;
            this.value = value;
        }
        return KeyValuePair;
    }());
    var Actor = /** @class */ (function () {
        function Actor(actorId, actorName, role, subAccountId, language, system) {
            this.actorId = actorId;
            this.actorName = actorName;
            this.role = role;
            this.subAccountId = subAccountId;
            this.language = language;
            this.system = system;
        }
        Actor.getAvailableRoles = function () {
            var roles = new Array();
            for (var item in Role) {
                if (item) {
                    var roleId = parseInt(item, 10);
                    if (!isNaN(roleId) && roleId !== Role.member) {
                        roles.push(new KeyValuePair(roleId, Role[roleId]));
                    }
                }
            }
            return roles.reverse();
        };
        return Actor;
    }());
    var Target = /** @class */ (function () {
        function Target(productId, role) {
            this.productId = productId;
            this.role = role;
        }
        Target.getAvailableRoles = function (uplineRole) {
            var roles = new Array();
            for (var item in Role) {
                if (item) {
                    var roleId = parseInt(item, 10);
                    if (roleId < uplineRole) {
                        roles.push(new KeyValuePair(roleId, Role[roleId]));
                    }
                }
            }
            return roles.reverse();
        };
        Target.getUplineRoles = function (downlineRole) {
            var roles = new Array();
            for (var item in Role) {
                if (item) {
                    var roleId = parseInt(item, 10);
                    if (downlineRole < roleId && roleId < Role.company) {
                        roles.push(new KeyValuePair(roleId, Role[roleId]));
                    }
                }
            }
            return roles.reverse();
        };
        return Target;
    }());
    var SingleTarget = /** @class */ (function (_super) {
        __extends(SingleTarget, _super);
        function SingleTarget(customerId, customerName, role, uplineId, productId) {
            var _this = _super.call(this, productId, role) || this;
            _this.customerId = customerId;
            _this.customerName = customerName;
            _this.uplineId = uplineId;
            return _this;
        }
        return SingleTarget;
    }(Target));
    var MultipleTargets = /** @class */ (function (_super) {
        __extends(MultipleTargets, _super);
        function MultipleTargets(customerIds, customerNames, role, uplineRole, productId) {
            var _this = _super.call(this, productId, role, uplineRole) || this;
            _this.customerIds = customerIds;
            _this.customerNames = customerNames;
            return _this;
        }
        Object.defineProperty(MultipleTargets.prototype, "firstCustomerName", {
            get: function () {
                if (this.customerNames) {
                    var names = this.customerNames.split(",");
                    if (names.length > 0) {
                        return names[0];
                    }
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        return MultipleTargets;
    }(Target));
})(DemoFrom || (DemoFrom = {}));
/* tslint:disable */
var showCN88CancelButton = true;
function cn88SuccessfulUpdatingCallback(isSuccessful) {
    console.log("Be called after updating sucessfully!");
}
function cn88CancelCallback() {
    console.log("Cancel the form!");
}
/* tslint:enable */
//# sourceMappingURL=demo-form.js.map