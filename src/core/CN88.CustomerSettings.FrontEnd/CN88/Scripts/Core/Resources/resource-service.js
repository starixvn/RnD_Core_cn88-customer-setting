/*jshint -W101,-W106,-W044,-W098*/

(function () {
    var labels = {};

    labels["en-us"] = {
        "Cancel": "Cancel",
        "Save": "Save"
    };

    labels["zh-hans"] = {
        "Cancel": "取消",
        "Save": "保存"
    };

    labels["zh-tw"] = {
        "Cancel": "取消",
        "Save": "保存"
    };

    var messages = {};

    messages["en-us"] = {
        "NoChangeToUpdate": "There is no change to update.",
        "SystemError": "Oops... you are here because of a system error, please notify system administrator.",
        "YouAreNotAuthorized": "You are not authorized on this function!"
    };

    messages["zh-hans"] = {
        "NoChangeToUpdate": "没有任何的异动需要更新。",
        "SystemError": "不好意思…目前系统出错，请联系管理员！",
        "YouAreNotAuthorized": "您没有使用此功能的权限"
    };

    messages["zh-tw"] = {
        "NoChangeToUpdate": "沒有任何的異動需要更新。",
        "SystemError": "不好意思…目前系統故障，請聯絡系統管理員。",
        "YouAreNotAuthorized": "您沒有使用此功能的權限"
    };

    var resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("customerSettingResources", resources);
})();