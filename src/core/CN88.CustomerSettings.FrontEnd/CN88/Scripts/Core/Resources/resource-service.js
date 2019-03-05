/*jshint -W101,-W106,-W044,-W098*/

(function () {
    var labels = {};

    labels["en-us"] = {
        "Cancel": "Cancel",
        "Save": "Save"
    };

    labels["zh-cn"] = {
    };

    labels["zh-tw"] = {
    };

    labels["zh-hans"] = {
    };

    labels["th-th"] = {
    };

    var messages = {};

    messages["en-us"] = {
        "NoChangeToUpdate": "There is no change to update.",
        "SystemError": "Oops... you are here because of a system error, please notify system administrator.",
        "YouAreNotAuthorized": "You are not authorized on this function!"
    };

    messages["zh-cn"] = {
        "NoChangeToUpdate": "没有任何的变化更新。",
        "SystemError": "哎哟..因为系统故障所以您会身在此处，请向管理层联系吧",
        "YouAreNotAuthorized": "您没有使用此功能的权限"
    };

    messages["zh-tw"] = {
        "NoChangeToUpdate": "沒有任何的變化更新。",
        "SystemError": "哎喲..因為系統故障所以您會身在此處，請向管理層聯繫吧",
        "YouAreNotAuthorized": "您沒有使用此功能的權限"
    };

    messages["zh-hans"] = {
        "NoChangeToUpdate": "没有任何的变化更新。",
        "SystemError": "哎哟..因为系统故障所以您会身在此处，请向管理层联系吧",
        "YouAreNotAuthorized": "您没有使用此功能的权限"
    };

    messages["th-th"] = {
        "NoChangeToUpdate": "ไม่มีการเปลี่ยนแปลงการอัพเดท",
        "SystemError": "อุ้ย...คุณอยู่ที่นี่เนื่องจากระบบผิดพลาด กรุณาแจ้งผู้ให้บริการ",
        "YouAreNotAuthorized": "คุณไม่มีอำนาจในการเข้าใช้งานฟังก์ชันนี้"
    };

    var resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("customerSettingResources", resources);
})();
