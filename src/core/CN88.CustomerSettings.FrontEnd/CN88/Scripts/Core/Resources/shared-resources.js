/*jshint -W101,-W106,-W044,-W098*/

(function () {
    "use strict";

    var labels = {};

    labels["en-us"] = {
        "Agent": "Agent",
        "Agents": "Agents",
        "CopyTooltip": "Click to duplicate the values",
        "CustomerSetting": "Customer Setting",
        "Fax": "Fax",
        "FirstName": "First Name",
        "GeneralInformation": "General Information",
        "LastName": "Last Name",
        "Live": "Live",
        "Master": "Master",
        "Masters": "Masters",
        "Member": "Member",
        "Members": "Members",
        "MobilePhone": "Mobile Phone",
        "Nickname": "Nickname",
        "No": "No",
        "Phone": "Phone",
        "Super": "Super",
        "Update": "Update",
        "UserName": "Username",
        "UserNames": "Usernames",
        "Yes": "Yes"
    };

    labels["zh-cn"] = {
        "Agent": "代理",
        "Agents": "代理",
        "CopyTooltip": "点击以复制数值",
        "CustomerSetting": "会员设定",
        "Fax": "传真",
        "FirstName": "名",
        "GeneralInformation": "基本信息",
        "LastName": "姓",
        "Live": "现场",
        "Master": "总代理",
        "Masters": "总代理",
        "Member": "会员",
        "Members": "会员",
        "MobilePhone": "手机",
        "Nickname": "昵称",
        "No": "无",
        "Phone": "电话",
        "Super": "超级总代理",
        "Update": "更改",
        "UserName": "用户名",
        "UserNames": "用户名",
        "Yes": "是"
    };

    labels["zh-tw"] = {
        "Agent": "代理",
        "Agents": "代理",
        "CopyTooltip": "點擊以復制數值",
        "CustomerSetting": "會員設定",
        "Fax": "傳真",
        "FirstName": "名",
        "GeneralInformation": "基本信息",
        "LastName": "姓",
        "Live": "現場",
        "Master": "總代理",
        "Masters": "總代理",
        "Member": "會員",
        "Members": "會員",
        "MobilePhone": "手機",
        "Nickname": "暱稱",
        "No": "無",
        "Phone": "電話",
        "Super": "超級總代理",
        "Update": "更改",
        "UserName": "用戶名",
        "UserNames": "用戶名",
        "Yes": "是"
    };

    labels["ko-kr"] = {
        "Agents": "에이전트",
        "CustomerSetting": "Customer Setting",
        "Fax": "팩스",
        "FirstName": "이름",
        "GeneralInformation": "일반 정보",
        "LastName": "성",
        "Live": "라이브",
        "Masters": "마스터",
        "Members": "회원",
        "MobilePhone": "핸드폰",
        "Nickname": "닉네임",
        "No": "아니오",
        "Phone": "전화",
        "Update": "업데이트",
        "UserName": "사용자 이름",
        "Yes": "Yes"
    };

    labels["ja-jp"] = {
        "Fax": "ファックス",
        "FirstName": "名",
        "GeneralInformation": "General Information",
        "LastName": "氏",
        "Live": "ライブ",
        "MobilePhone": "携帯電話番号",
        "Nickname": "ニックネーム",
        "No": "No",
        "Phone": "電話番号",
        "Update": "更新",
        "UserName": "ユーザー名",
        "Yes": "Yes"
    };

    labels["th-th"] = {
        "Agent": "เอเย่นต์",
        "Agents": "เอเจนท์",
        "CopyTooltip": "คลิ๊กค่าที่จะซ้ำกัน",
        "CustomerSetting": "ตั้งค่าสมาชิก",
        "Fax": "แฟกซ์",
        "FirstName": "ชื่อ",
        "GeneralInformation": "ข้อมูลทั่วไป",
        "LastName": "นามสกุล",
        "Live": "สด",
        "Master": "มาสเตอร์",
        "Masters": "มาสเตอร์ต่าง ๆ",
        "Member": "สมาชิก",
        "Members": "เมมเบอร์ต่าง ๆ",
        "MobilePhone": "โทรศัพท์มือถือ",
        "Nickname": "ชื่อเล่น",
        "No": "ไม่",
        "Phone": "โทรศัพท์",
        "Super": "ซุปเปอร์",
        "Update": "ล่าสุด",
        "UserName": "ชื่อผู้ใช้ระบบ",
        "UserNames": "ยูเซอร์เนม",
        "Yes": "ตกลง"
    };

    var messages = {};

    messages["en-us"] = {
        "AccessDeniedMsg": "Sorry, You don&apos;t have permission to access this page.",
        "AccountWasDisabled": "This account was disabled.",
        "CopiedSuccessfully": "All fields in green are copied.",
        "CopyingFailed": "Unable to copy to the fields in red.",
        "CustomersAreNotSameLine": "Customers are not in the same line!",
        "DataIsNotValid": "Data is not valid.",
        "DuplicateValuesAlertMsg": "Out of range for highlighted values.",
        "Msg_90001": "Update process is unsuccessful !",
        "NoChangeToUpdate": "There is no change to update.",
        "ProductNameMissing": "Product Id {0} does not have name in {1}",
        "SettingTypeError": "Unable to load the setting type due to a technical problem. Please try again later or notify system administrator!",
        "SettingTypeHasNoSettingItem": "Setting Type {0} does not have any setting item.",
        "SportTypeOrBetTypeIsInvalid": "Sport Type or Bet Type is invalid!",
        "SubAccountIdShouldBeZeroWhenSystemIsNotAgency": "Sub Account Id Should Be Zero When System Is Not Agency.",
        "SystemError": "Oops... you are here because of a system error, please notify system administrator.",
        "UplineLevelCannotBeNullIncaseOfEdittingMember": "Upline Level cannot be null incase of editting member.",
        "UplineLevelIsNotValid": "Upline Level is not valid.",
        "UplineLevelShouldBeNull": "Upline Level should be null.",
        "YouAreNotAuthorized": "You are not authorized on this function!"
    };

    messages["zh-cn"] = {
        "AccessDeniedMsg": "对不起,您不被允许进入此网页.",
        "AccountWasDisabled": "这个帐号禁用",
        "CopiedSuccessfully": "绿色的所有字段已被复制。",
        "CopyingFailed": "红色字段无法复制。",
        "CustomersAreNotSameLine": "会员不同线！",
        "DuplicateValuesAlertMsg": "突显值已超出范围。",
        "Msg_90001": "更新程序失败！",
        "NoChangeToUpdate": "没有任何的变化更新。",
        "SettingTypeError": "由于技术故障导致加载设置类型失败。请稍后再试或联系系统管理员！",
        "SystemError": "哎哟..因为系统故障所以您会身在此处，请向管理层联系吧",
        "YouAreNotAuthorized": "您没有使用此功能的权限"
    };

    messages["zh-tw"] = {
        "AccessDeniedMsg": "對不起, 您不被允許進入此網頁.",
        "AccountWasDisabled": "這個帳號禁用",
        "CopiedSuccessfully": "綠色的所有字段已被複製。",
        "CopyingFailed": "紅色字段無法複製。",
        "CustomersAreNotSameLine": "會員不同線！",
        "DuplicateValuesAlertMsg": "突顯值已超出範圍。",
        "Msg_90001": "更新程序失敗！",
        "NoChangeToUpdate": "沒有任何的變化更新。",
        "SettingTypeError": "由於技術故障導致加載設置類型失敗。請稍後再試或聯系系統管理員！",
        "SystemError": "哎喲..因為系統故障所以您會身在此處，請向管理層聯繫吧",
        "YouAreNotAuthorized": "您沒有使用此功能的權限"
    };

    messages["ko-kr"] = {
        "AccessDeniedMsg": "죄송합니다. 이페이지를 엑서스 할 수 있는 권한이 없습니다.",
        "AccountWasDisabled": "이 계정은 실행하실수없는 계정입니다.",
        "SystemError": "죄송합니다. 시스템에러가 발생했습니다. 관리자에게 문의 바랍니다.",
        "YouAreNotAuthorized": "해당 기능에 대한 권한이 없습니다."
    };

    messages["ja-jp"] = {
        "AccessDeniedMsg": "Sorry, You don&apos;t have permission to access this page.",
        "AccountWasDisabled": "This account was disabled.",
        "SystemError": "Oops... you are here because of a system error, please notify the administrators"
    };

    messages["th-th"] = {
        "AccessDeniedMsg": "ขออภัยคุณไม่ได้รับอนุญาติให้เข้าหน้านี้",
        "AccountWasDisabled": "บัญชีนี้ได้ถูกปิดไปแล้ว",
        "CopiedSuccessfully": "ช่องสีเขียวทั้งหมดจะถูกคัดลอก",
        "CopyingFailed": "ไม่สามารถคัดลอกไปยังช่องสีแดง",
        "CustomersAreNotSameLine": "ลูกเค้าไม่ได้อยู่ในสายเดียวกัน",
        "DuplicateValuesAlertMsg": "ตัวเลขที่ถูกไฮไลท์ไม่ได้อยู่ในขอบเขต",
        "Msg_90001": "กระบวนการอัพเดทสำเร็จแล้ว",
        "NoChangeToUpdate": "ไม่มีการเปลี่ยนแปลงการอัพเดท",
        "SettingTypeError": "ไม่สามารถโหลดการตั้งค่าต่าง ๆ เนื่องจากปัญหาทางด้านเทคนิค รบกวนลองใหม่อีกครั้งหรือจนกว่าจะได้รับคำประกาศจากทางผู้ดูแลระบบ",
        "SystemError": "อุ้ย...คุณอยู่ที่นี่เนื่องจากระบบผิดพลาด กรุณาแจ้งผู้ให้บริการ",
        "YouAreNotAuthorized": "คุณไม่มีอำนาจในการเข้าใช้งานฟังก์ชันนี้"
    };

    var resources = {
        "labels": labels,
        "messages": messages
    };

    angular.module("customerSetting").value("customerSettingResources", resources);
})();
