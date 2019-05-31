/*jshint -W101,-W106,-W044,-W098*/

(function () {
    var labels = {};

    labels["en-us"] = {
        "NewSuper": "New SMA",
        "NewMaster": "New Master Agent",
        "NewAgent": "New Agent",
        "NewMember": "New Member",
        "EditSuper": "Edit SMA",
        "EditMaster": "Edit Master Agent",
        "EditAgent": "Edit Agent",
        "EditMember": "Edit Member",
        "MySettings": "My Settings",
        "Cancel": "Cancel",
        "Save": "Save",
        "SaveAndNew": "Save & New"
    };

    labels["zh-hans"] = {
        "NewSuper": "新增股东",
        "NewMaster": "新增总代理",
        "NewAgent": "新增代理",
        "NewMember": "新增会员",
        "EditSuper": "编辑股东",
        "EditMaster": "编辑总代理",
        "EditAgent": "编辑代理商",
        "EditMember": "编辑会员",
        "MySettings": "我的设定",
        "Cancel": "取消",
        "Save": "保存",
        "SaveAndNew": "保存与新增"
    };

    labels["zh-tw"] = {
        "NewSuper": "新增股東",
        "NewMaster": "新增總代理",
        "NewAgent": "新增代理",
        "NewMember": "新增會員",
        "EditSuper": "編輯股東",
        "EditMaster": "編輯總代理",
        "EditAgent": "編輯代理商",
        "EditMember": "編輯會員",
        "MySettings": "我的設定",
        "Cancel": "取消",
        "Save": "保存",
        "SaveAndNew": "保存與新增"
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