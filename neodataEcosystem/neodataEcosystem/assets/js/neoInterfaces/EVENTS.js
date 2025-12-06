$("body").off("click", ".btnAccept").on("click", ".btnAccept", function () {
    var _data = _TOOLS.getFormValues(".dbase");
    _data.operation = "accept";
    var _geo = window.geoData;
    var _raw = {};
    $.extend(_raw, _data, _geo);
    _NMF.onTransaction(_raw).then(function () {
        _NMF.onModalAlert("Información", "Se han registrado los datos en forma correcta");
        $("input.dbase").val("");
        $("textarea.dbase").val("");
        $(".files-to-save").html("");
    });
});
$("body").off("click", ".btnGoToForm").on("click", ".btnGoToForm", function () {
    var _target = _TOOLS.b64_to_utf8($(this).attr("data-target"));
    var _url = _TOOLS.b64_to_utf8($(this).attr("data-route"));
    window.open(_url, _target);
});
$("body").off("change", ".file-input").on("change", ".file-input", function (e) {
    var filesCount = $(this)[0].files.length;
    var fileName = $(this).val().split('\\').pop();
    const fileReader = new FileReader();
    fileReader.readAsDataURL(e.target.files[0]);
    fileReader.onload = () => {
        var uuid = _TOOLS.UUID();
        var _id = ("file-" + uuid);
        var _html = "<li class='" + uuid + "'>";
        _html += "<a href='#' class='btnDeleteUpload' data-uuid='" + uuid + "'><span class='material-icons' style='color:red;'>delete</span></a>";
        _html += "<span style='font-size:0.7em;'> " + fileName +"</span>";
        _html += "<input id='" + _id + "' name='" + _id + "' type='hidden' class='dbase' value='" + fileReader.result + "'/>";
        _html += "</li>"
        $(".files-to-save").append(_html);
        $(".files-area").removeClass("d-none");
        $(".file-input").val("");
    };
    fileReader.onerror = (error) => { _NMF.onModalAlert("Error", "Se ha producido un error al adjuntar el archivo", "danger"); };
});
$("body").off("click", ".btnDeleteUpload").on("click", ".btnDeleteUpload", function () {
    if (!confirm("¿Confirmar eliminar el archivo adjunto?")) { return false; }
    $("." + $(this).attr("data-uuid")).remove();
});
