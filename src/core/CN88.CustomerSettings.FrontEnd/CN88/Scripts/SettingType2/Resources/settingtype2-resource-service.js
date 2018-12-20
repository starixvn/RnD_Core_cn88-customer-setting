/*jshint -W101,-W106,-W044,-W098*/

(function () {
    "use strict";

    var labels = {};

    labels["en-us"] = {
        "AccountDetail": "Account Detail"
    };

    labels["zh-cn"] = {
    };

    labels["zh-tw"] = {
    };

    labels["zh-hans"] = {
    };

    labels["ko-kr"] = {
    };

    labels["ja-jp"] = {
    };

    labels["th-th"] = {
    };

    var messages = {};

    messages["en-us"] = {
    };

    messages["zh-cn"] = {
    };

    messages["zh-tw"] = {
    };

    messages["zh-hans"] = {
    };

    messages["ko-kr"] = {
    };

    messages["ja-jp"] = {
    };

    messages["th-th"] = {
    };

    var settingType2Resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("settingType2Resources", settingType2Resources);
})();