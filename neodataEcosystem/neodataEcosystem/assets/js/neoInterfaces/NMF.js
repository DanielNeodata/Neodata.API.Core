var _NMF = {
    _log: true,
    _err_401: "Error de autenticación en NeoInterfaces.<br/>Contacte a soporte con el <b>código 401</b>",
    _err_403: "Error de transacción en NeoInterfaces.<br/>Contacte a soporte con el <b>código 403</b>",
    _txt_aceptar: "Aceptar",
    _external_id:null,
    onErrHandler: function(msg) {
        var _text = "";
        if (typeof msg === 'object') {
            if (msg.message != undefined) {
                _text = msg.message;
            } else {
                _text = JSON.stringify(msg);
            }
        } else {
            _text = msg;
        }
        _NMF.onModalAlert("System alert", _text, "warning");
    },
    onDestroyModal: function (_target) {
        $(_target).remove();
        $(".modal-backdrop").remove();
        $("body").removeClass("modal-open");
    },
    onModalAlert: function (_title, _body, _class, _callback) {
        if (_class == "") { _class = "info"; }
        _NMF.onDestroyModal("#alterModal");
        var _html = "<div class='modal fade' id='alterModal' role='dialog'>";
        _html += " <div class='modal-dialog modal-dialog-centered' role='document'>";
        _html += "  <div class='modal-content'>";
        _html += "    <div class='modal-header text-" + _class + "'>";
        _html += "      <h2 class='modal-title'>" + _title + "</h2>";
        _html += "    </div>";
        _html += "    <div class='modal-body'>";
        _html += _body;
        _html += "    </div>";
        _html += "    <div class='modal-footer font-weight-light'>";
        _html += "       <button type='button' class='btn-raised btn btn-cancel-alert btn-" + _class + " btn-sm'></span>" + _NMF._txt_aceptar + "</button>";
        _html += "    </div>";
        _html += "  </div>";
        _html += " </div>";
        _html += "</div>";
        $("body").append(_html);
        $("body").off("click", ".btn-cancel-alert").on("click", ".btn-cancel-alert", function () {
            _NMF.onDestroyModal("#alterModal");
            if ($.isFunction(_callback)) { _callback(); }

        });
		var myModal = new bootstrap.Modal(document.getElementById('alterModal'), { backdrop: true});
		myModal.show();
        return true;
    },
    onLog: function (_message) { if (_NMF._log) { console.log(_message); } },

    onAuthenticate: function (_credentials) {
        return new Promise(
            function (resolve, reject) {
                try {
                    var formData = new FormData();
                    formData.append("id", _credentials.id);
                    formData.append("username", _credentials.username);
                    formData.append("password", _credentials.password);
                    var ajaxRq = $.ajax(
                        {
                            type: "POST",
                            dataType: "json",
                            url: (_CONFIG.serverNeoAuthentication + "Authenticate"),
                            data: formData,
                            processData: false,
                            mimeType: "multiform/form-data",
                            contentType:false,
                            beforeSend: function () { },
                            complete: function () { },
                            error: function (xhr, ajaxOptions, thrownError) {
                                _NMF.onErrHandler(_NMF._err_401);
                                reject(thrownError);
                            },
                            success: function (datajson) {
                                var _response = null;
                                if (datajson.status == "ERROR") {
                                    _NMF.onErrHandler(_NMF._err_401);
                                } else {
                                    _response = {
                                        "id_application": datajson.records[0].id_application,
                                        "application": datajson.records[0].application,
                                        "id_user": datajson.records[0].id_user,
                                        "username": datajson.records[0].username,
                                        "token": datajson.tokenSingleUse
                                    }
                                }
                                resolve(_response);
                            }
                        });
                } catch (err) {
                    _NMF.onErrHandler(err.message);
                    reject(err);
                }
            });
    },
    onBuildPage: function (_id, _title) {
        _NMF._external_id = _id;
        _NMF.onAuthenticate(_CONFIG._jsonAuthenticationInterfaces)
            .then(function (_auth) {
                try {
                    if (_auth == null) { throw _NMF._err_401; }
                    $("body").hide();
                    var formData = new FormData();
                    formData.append("id", _NMF._external_id);
                    formData.append("token", _auth.token);
                    formData.append("id_application", _auth.id_application);
                    formData.append("id_user", _auth.id_user);
                    var ajaxRq = $.ajax(
                        {
                            type: "POST",
                            dataType: "json",
                            url: (_CONFIG.serverNeoInterfaces + "Search"),
                            data: formData,
                            processData: false,
                            mimeType: "multiform/form-data",
                            contentType: false,
                            beforeSend: function () { },
                            complete: function () { },
                            error: function (xhr, ajaxOptions, thrownError) {
                                _NMF.onErrHandler(_NMF._err_401);
                            },
                            success: function (datajson) {
                                $(document).attr("title", _title);
                                if (datajson.records[0].head != "") { $("head").html(datajson.records[0].head); }
                                $("body").html(datajson.records[0].body);
                                $("html").append(datajson.records[0].script);
                                $("body").fadeIn("slow");

                                var _raw = {
                                    "operation": "print",
                                    "form": datajson.records[0].description,
                                    "api": datajson.trace,
                                    "function": datajson.scope,
                                    "message": datajson.message
                                };
                                _NMF.onTransaction(_raw);
                            }
                        });
                } catch (err) {
                    _NMF.onErrHandler(err.message);
                    $("body").show();
                    reject(err);
                }
            });
    },
    onTransaction: function (_raw) {
        return new Promise(
            function (resolve, reject) {
                _NMF.onAuthenticate(_CONFIG._jsonAuthenticationTransactions)
                    .then(function (_auth) {
                        try {
                            if (_auth == null) { throw _NMF._err_401; }
                            var formData = new FormData();
                            formData.append("token", _auth.token);
                            formData.append("id_application", _auth.id_application);
                            formData.append("id_user", _auth.id_user);
                            formData.append("id_type_status", 1);
                            formData.append("raw_data", JSON.stringify(_raw));
                            formData.append("mime_type", "application/json");
                            formData.append("externalId", (_raw.operation + ":" + _NMF._external_id));

                            var ajaxRq = $.ajax(
                                {
                                    type: "POST",
                                    dataType: "json",
                                    url: (_CONFIG.serverNeoTransaction + "Create"),
                                    data: formData,
                                    processData: false,
                                    mimeType: "multiform/form-data",
                                    contentType: false,
                                    beforeSend: function () { },
                                    complete: function () { },
                                    error: function (xhr, ajaxOptions, thrownError) {
                                        _NMF.onErrHandler(_NMF._err_403);
                                        reject(null);
                                    },
                                    success: function (datajson) {
                                        resolve(null);
                                    }
                                });
                        } catch (err) {
                            _NMF.onErrHandler(err.message);
                            $("body").show();
                            reject(err);
                        }
                    });
            });
    },
};
