var _TOOLS = {
    UUID: function () {
        var s = [];
        var hexDigits = "0123456789abcdef";
        for (var i = 0; i < 36; i++) { s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1); }
        s[14] = "4";
        s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
        s[8] = s[13] = s[18] = s[23] = "-";
        var uuid = s.join("");
        return uuid;
    },
    todayYYYYMMDD: function (_separator) {
        var currentDate = new Date();
        var day = currentDate.getDate();
        var month = currentDate.getMonth() + 1;
        var year = currentDate.getFullYear();
        if (day < 10) { day = "0" + day; }
        if (month < 10) { month = "0" + month; }
        return (year + _separator + month + _separator + day);
    },
    getYear: function (_date, _separator) {
        _date = _date.split("T")[0];
        var _v = _date.split("T")[0].split(_separator);
        return _v[0];
    },
    getMonth: function (_date, _separator) {
        _date = _date.split("T")[0];
        var _v = _date.split("T")[0].split(_separator);
        return _v[1];
    },
    getDay: function (_date, _separator) {
        _date = _date.split("T")[0];
        var _v = _date.split("T")[0].split(_separator);
        return _v[2];
    },
    toDDMMYY: function (_date, _separator) {
        try {
            _date = _date.split("T")[0];
            var _v = _date.split("T")[0].split(_separator);
            return (_v[2] + "/" + _v[1] + "/" + _v[0]);
        } catch (err) {
            return "";
        }
    },
    getNow: function () {
        var currentDate = new Date();
        var second = currentDate.getSeconds();
        var minute = currentDate.getMinutes();
        var hour = currentDate.getHours();
        var day = currentDate.getDate();
        var month = currentDate.getMonth() + 1;
        var year = currentDate.getFullYear();
        if (day < 10) { day = "0" + day; }
        if (month < 10) { month = "0" + month; }
        if (hour < 10) { hour = "0" + hour; }
        if (minute < 10) { minute = "0" + minute; }
        if (second < 10) { second = "0" + second; }
        return day + "/" + month + "/" + year + " " + hour + ":" + minute + ":" + second;
    },
    getNowYYYMMDD: function () {
        var currentDate = new Date();
        var day = currentDate.getDate();
        var month = currentDate.getMonth() + 1;
        var year = currentDate.getFullYear();
        if (day < 10) { day = "0" + day; }
        if (month < 10) { month = "0" + month; }
        return year + "-" + month + "-" + day;
    },
    validate: function (_selector) {
        var _ret = true;
        $(_selector).each(function () { _ret = (_TOOLS.formatValidation($(this)) && _ret); });
        return _ret;
    },
    formatValidation: function (_obj) {
        var _ret = true;
        var _color = "transparent";
        var _color_alert = "rgba(251, 4, 11,0.25)";
        switch (_obj.prop("tagName")) {
            case "TEXTAREA":
            case "INPUT":
                switch (_obj.attr("type")) {
                    case "email":
                        if (!_TOOLS.isValidEmail(_obj.val())) {
                            _color = _color_alert;
                            _ret = false;
                            //_obj.val(_obj.val().toLowerCase());
                        }
                        break;
                    case "checkbox":
                        var _checked = _obj.is(":checked");
                        if (!_checked) {
                            _color = _color_alert;
                            _ret = false;
                            _obj.parent().css("color", _color);
                        } else {
                            _obj.parent().css("color", "transparent");
                        }
                        break;
                    default:
                        if (_obj.hasClass("data-list")) {
                            if (_obj.attr("data-selected-id") == "" || _obj.attr("data-selected-id") == undefined) { _color = _color_alert; _ret = false; }
                        } else {
                            if (_obj.val() == "") { _color = _color_alert; _ret = false; }
                        }
                        break;
                }
                break;
            case "SELECT":
                if (_obj.val() == "-1" || _obj.val() == undefined || _obj.val() == null) { _color = _color_alert; _ret = false; }
                break;
        }
        _obj.css("background-color", _color);
        return _ret;
    },
    getFormValues: function (_selector) {
        var _jsonSave = {};
        $(_selector).each(function () {
            var property = $(this).attr('name');
            var value = "";
            switch (true) {
                case $(this).hasClass("combo"):
                    if ($(this).length == 0) { value = ""; } else { value = $(this).val(); }
                    if (value == null || value == "-1" || value == "0" || value == "") { value = "-1"; }
                    break;
                case $(this).hasClass("check"):
                    if ($(this).prop("checked")) { value = $(this).val(); } else { value = ""; }
                    break;
                default:
                    value = $(this).val();
                    break;
            }
            _jsonSave[property] = value;
        });
        return _jsonSave;
    },
    isElementVisible: function (el) {
        if (typeof jQuery !== 'undefined' && el instanceof jQuery) { el = el[0]; }
        var rect = el.getBoundingClientRect();
        var windowHeight = (window.innerHeight || document.documentElement.clientHeight);
        var windowWidth = (window.innerWidth || document.documentElement.clientWidth);
        var vertInView = (rect.top <= windowHeight) && ((rect.top + rect.height) >= 0);
        var horInView = (rect.left <= windowWidth) && ((rect.left + rect.width) >= 0);
        return (vertInView && horInView);
    },
    isValidEmail: function (email) {
        var em = /^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i;
        return em.test(email);
    },
    utf8_to_str: function (a) {
        for (var i = 0, s = ''; i < a.length; i++) {
            var h = a[i].toString(16)
            if (h.length < 2) h = '0' + h
            s += '%' + h
        }
        return decodeURI(s);
    },
    utf8_to_b64: function (str) { return window.btoa(unescape(encodeURIComponent(str))); },
    b64_to_utf8: function (str) { str = str.replace(/\s/g, ''); return decodeURIComponent(escape(window.atob(str))); },
    b64_to_Blob: function (content, contentType) {
        contentType = contentType || '';
        const sliceSize = 512;
        const byteCharacters = window.atob(content);
        const byteArrays = [];
        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);
            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) { byteNumbers[i] = slice.charCodeAt(i); }
            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }
        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    },
    toFullscreen: function (_id) {
        const element = document.getElementById(_id);
        if (screenfull.isEnabled) { screenfull.request(element); }
    },
    onAllUrlParams: function (url) {
        var queryString = url ? url.split('?')[1] : window.location.search.slice(1);
        var obj = {};
        if (queryString) {
            queryString = queryString.split('#')[0];
            var arr = queryString.split('&');
            for (var i = 0; i < arr.length; i++) {
                var a = arr[i].split('=');
                var paramName = a[0];
                var paramValue = typeof (a[1]) === 'undefined' ? true : a[1];
                paramName = paramName.toLowerCase();
                if (typeof paramValue === 'string') paramValue = paramValue.toLowerCase();
                if (paramName.match(/\[(\d+)?\]$/)) {
                    var key = paramName.replace(/\[(\d+)?\]/, '');
                    if (!obj[key]) obj[key] = [];
                    if (paramName.match(/\[\d+\]$/)) {
                        var index = /\[(\d+)\]/.exec(paramName)[1];
                        obj[key][index] = paramValue;
                    } else {
                        obj[key].push(paramValue);
                    }
                } else {
                    if (!obj[paramName]) {
                        obj[paramName] = paramValue;
                    } else if (obj[paramName] && typeof obj[paramName] === 'string') {
                        obj[paramName] = [obj[paramName]];
                        obj[paramName].push(paramValue);
                    } else {
                        obj[paramName].push(paramValue);
                    }
                }
            }
        }
        return obj;
    },
    onSetCookie: function (cname, cvalue, exdays) {
        const d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        let expires = ("expires=" + d.toUTCString());
        document.cookie = (cname + "=" + cvalue + ";" + expires + ";path=/");
    },
    onGetCookie: function (cname) {
        let name = cname + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') { c = c.substring(1); }
            if (c.indexOf(name) == 0) { return c.substring(name.length, c.length); }
        }
        return "";
    },
    prettyPrint: function (obj, _target) {
        var pretty = JSON.stringify(obj, undefined, 4);
        $(_target).html("<pre>" + pretty + "</pre>");
    },
    capitalize: function (str) {
        const lower = str.toLowerCase();
        return str.charAt(0).toUpperCase() + lower.slice(1);
    },
};
