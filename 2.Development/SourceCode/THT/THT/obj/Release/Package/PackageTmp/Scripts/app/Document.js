var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;

$(document).ready(function () {

    $("ul#menuLeft").find('#ul_root_3').addClass('open');
    $("ul#menuLeft").find('#ul_root_3').css('display', 'block');
    $("#menu_Document").parent().addClass('active');

    resetMenu();
    $("#menu_Document").parent().addClass('active');

    //fillter & form popup
    $("#selectStatus_search").select2();
    $("#s2id_selectStatus_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');

    $("#txtSaved").select2();
    $("#s2id_txtSaved").css('width', '220px');

    $("#txtExpirated").select2();
    $("#s2id_txtExpirated").css('width', '220px');

    $("#CategoryID").select2();
    $("#s2id_CategoryID").css('width', '240px');

    $("#listCatagory").select2();
    $("#s2id_listCatagory").css('width', '220px');

    $("#SideboardID").select2();
    $("#s2id_SideboardID").css('width', '240px');

    $('#selectStatus_search,#listCatagory,#txtSaved,#txtExpirated,#ngaytao').bind('change', function () {
        doSearch();
    })

    $(".allownumericwithdecimal").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    })

    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    $("#txtDocumentID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });

    if ($("#btnInsert").length > 0) {
        $(document).keypress(function (e) {
            if (e.keyCode == 43) {  // 43 : +
                onOpenPopup(0, null);
            }
        });
    }
    //Ctr + S luu form
    $(document).bind('keydown', function (event) {
        if (eventHotKey) {
            if (event.ctrlKey || event.metaKey) {
                switch (String.fromCharCode(event.which).toLowerCase()) {
                    case 's':
                        event.preventDefault();
                        var boolCreatedBy = $("#CreatedBy").val() != "system";
                        $("#formPopup").submit();
                        break;
                }
            }
        }
    });

    $("#importform").ajaxForm({
        beforeSend: function () {
            $("#popupImport").data("kendoWindow").close();
        },
        uploadProgress: function (event, position, total, percentComplete) { },
        success: function (data) {
            if (data.success) {
                $("#grid").data("kendoGrid").dataSource.read();
                hideMaskPopup('#divMaskPopup');
                if (data.totalError > 0) {
                    console.log(data.errorfile);
                    $.SmartMessageBox({
                        title: "Lỗi",
                        content: "Có dòng lỗi, tải về để sửa lại.",
                        buttons: '[Hủy][Tải]'
                    }, function (ButtonPressed) {
                        if (ButtonPressed === "Tải") {
                            window.open(r + "/ExcelImport/" + data.link, "_blank");
                        }
                        if (ButtonPressed === "Hủy") {
                            return;
                        }
                    });
                }
                else {
                    alertBox("Thành công! ", "Cập nhập thành công " + data.total + " mẫu tin", true, 3000);
                }
            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                console.log(data.message);
            }
            $("#divLoading").hide();
            hideMaskPopup('#divMaskPopup');
        },
        complete: function (xhr) { }
    });
    //form trong popup
    $('#popup').kendoWindow({
        width: "400px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    $('#popupConfirm').kendoWindow({
        width: "300px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    $('#popupDelete').kendoWindow({
        width: "350px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            CategoryID: {
                required: true,
            },
            No: {
                required: true,
            },
            strDateSave: {
                required: true,
                // alphanumeric: true
            },
            strExpirationDate: {
                required: true,
                // alphanumeric: true
            },
            //FileUpload: {
            //    required: true,
            //    // alphanumeric: true
            //},
        },

        // Messages for form validation
        messages: {
            CategoryID: {
                required: "Thông tin bắt buộc"
            },
            strDateSave: {
                required: "Thông tin bắt buộc"
            },

            No: {
                required: "Thông tin bắt buộc",
            },
            strExpirationDate: {
                required: "Thông tin bắt buộc",
                // alphanumeric: true
            },
            //FileUpload: {
            //    required: "Thông tin bắt buộc",
            //    // alphanumeric: true
            //},
        },
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                beforeSend: function () { $("#loading").removeClass('hide'); },
                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {
                            // Create
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã hồ sơ (*) </label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.DocumentID + '</strong><input type="hidden" id="DocumentID" name="DocumentID" value="' + data.DocumentID + '" /></label> <div style="clear:both"></div></div>');
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        $("#grid").data("kendoGrid").dataSource.read();
                        alertBox("Thành công!", " Lưu thành công", true, 3000);
                        $("#loading").addClass('hide');
                    }
                    else {
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        $("#loading").addClass('hide');
                        console.log(data.message);
                    }
                }
            });
            return false;
        }
    });
});


//popup
function onOpenPopup(key, obj) {
    eventHotKey = true;
    $("#formPopup").find('section em').remove();
    $("#formPopup").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    //Tạo mới
    if (key == 0) {
        keyAction = key;
        popup.title('Thêm');
        LoadDataPopup();
    }
        // Cập nhậpS
    else {
        keyAction = -1;
        popup.title('Cập nhật thông tin cấu hình');
        Obj = obj;
        id = $(obj).data('id');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã hồ sơ (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="DocumentID" name="DocumentID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        LoadDataPopup();
    }
}

function onDownload(obj) {
    var currentRow = $(obj).closest("tr");
    var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
    console.log(dataItem.Path + dataItem.FileNameServer);
    window.open(dataItem.Path + dataItem.FileNameServer);
}

// Load lại data khi tạo mới hoặc cập nhật
function onBindDataToFormCompany(dataItem) {
    for (var propName in dataItem) {
        if (dataItem[propName] != null && dataItem[propName].constructor.toString().indexOf("Date") > -1) {
            var d = kendo.toString(kendo.parseDate(dataItem[propName]), 'dd/MM/yyyy')
            if (d != '01/01/1900') {
                $("#" + propName).val(d);
            }
        }
        else {
            $("#" + propName).val(dataItem[propName]);
        }
    }
}

function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtDocumentID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "No", operator: "contains", value: text });
        filterOr.filters.push({ field: "Description", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }

    var text = $("#listCatagory").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        for (i = 0; i < text.length; i++) {
            filterOr.filters.push({ field: "CategoryID", operator: "contains", value: text[i] });
        }
        filter.filters.push(filterOr);
    }

    var text = $("#txtSaved").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        for (i = 0; i < text.length; i++) {
            filterOr.filters.push({ field: "StatusSaved", operator: "contains", value: text[i] });
        }
        filter.filters.push(filterOr);
    }

    var text = $("#txtExpirated").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        for (i = 0; i < text.length; i++) {
            filterOr.filters.push({ field: "StatusExpirated", operator: "contains", value: text[i] });
        }
        filter.filters.push(filterOr);
    }


    var ngaytao = $('#ngaytao').val();
    if (ngaytao) {

        var from = ngaytao.split('-')[0].trim().split("/");
        var startdate = new Date(from[2], from[1] - 1, from[0]);

        var to = ngaytao.split('-')[1].trim().split("/");
        var todate = new Date(to[2], to[1] - 1, to[0]);

        todate.setDate(todate.getDate() + 1);
        filter.filters.push({ field: "CreatedAt", operator: "gte", value: kendo.toString(startdate, 'yyyy-MM-dd') });
        filter.filters.push({ field: "CreatedAt", operator: "lt", value: kendo.toString(todate, 'yyyy-MM-dd') });
    }

    grid.dataSource.filter(filter);
}

function checkAllHeader(e) {
    var x = $(e).prop('checked');
    $('#grid').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}

var valKey = -100;
var listrowid = "";

function UpdateStatus(key) {
    debugger;
    valKey = key;
    listrowid = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        var popup = $('#popupConfirm').data("kendoWindow");
        if (key == 1) {
            $("#txtLabel").text(" hoạt động");
        }
        else if (key == 0) {
            $("#txtLabel").text(" ngưng hoạt động");
        }
        popup.center().open();
        $(".k-window span.k-i-close").click(function () {
            eventHotKey = false;
            $("#divMaskPopup").hide();
        });
    }
    else {
        alertBox("Báo lỗi!", "Vui lòng chọn dữ liệu.", false, 3000);
    }
}

function onUpdateStatus(key, obj) {
    if (key == 0) {
        $('#popupConfirm').data("kendoWindow").close();
        valKey = -100;
        return;
    }

    if (listrowid != null && listrowid != "") {
        //var c = confirm("Bạn có chắc chắn muốn ngưng hoạt động?.");
        $.post(r + "/Document/UpdateStatus", { data: listrowid, status: valKey }, function (data) {
            if (data.success) {
                alertBox("Thông báo! ", "Cập nhật trạng thái cấu hình thành công", true, 3000);
                $('#checkboxcheckAll').prop('checked', false);
                $("#grid").data("kendoGrid").dataSource.read();

            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                $("#loading").addClass('hide');
                console.log(data.message);
            }
        });
        $('#popupConfirm').data("kendoWindow").close();
    }
    else {
        alertBox("", "Vui lòng chọn người dùng.", false, 3000);
        $('#popupConfirm').data("kendoWindow").close();
    }
}

function onDatabound(e) {
    resizeGrid();
    var grid = $("#grid").data("kendoGrid");
    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
    .options.parameterMap({
        page: grid.dataSource.page(),
        sort: grid.dataSource.sort(),
        filter: grid.dataSource.filter()
    });
    var $exportLink = grid.element.find('.export');
    var href = $exportLink.attr('href');
    if (href) {
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');
        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));
        $exportLink.attr('href', href);
    }
    changeStatusGrid('grid', 6);
    $("#divLoading").hide();
}

function resizeGrid() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#grid').offset().top);
    var gridElement = $("#grid"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 1);
}
function changeStatusGrid(idGrid) {
    var arrRow = $("#" + idGrid).find(".k-grid-content table tbody tr");
    for (var i = 0; i < arrRow.length; i++) {
        var arrCol = arrRow[i].cells;
        if (arrCol.length == 0) {
            continue;
        }
        changeIsActive(arrCol);
    }

    $("#divLoading").hide();
}

function changeIsActive(arrCol) {
    for (var j = 0; j < arrCol.length; j++) {
        var tr;
        var columnName = '';
        tr = arrCol[j];
        var attr = $(tr).attr('data-column');
        if (typeof attr === typeof undefined || attr === false) {
            continue;
        }
        if (attr == 'IsActive') {
            if (tr.textContent == "true") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Đang hoạt động</span>');
            }
            else if (tr.textContent == "false") {
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Ngưng hoạt động</span>');
            }
        }
    }
}
function dateToString(date) {

    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        return day + '/' + month + '/' + date.getFullYear();
    }
    else {
        return "";
    }
}

function alertBox(title, content, flag, timeout) {
    //var icon = flag ? "fa-thumbs-up" : "fa-thumbs-down";
    $.smallBox({
        title: title,
        content: content,
        color: flag ? "#e0efd8" : "#f2dedf",
        //iconSmall: "fa " + icon + " bounce animated",
        timeout: timeout
    });
}

function ResetForm() {
    LoadDataPopup();
}

function LoadDataPopup() {

    if (keyAction == 0) {
        $('#formPopup')[0].reset();
        $("#CreatedBy").val('');
        $("#CreatedAt").val('');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
        var todayDate = kendo.toString(kendo.parseDate(new Date()), 'dd/MM/yyyy');
        $("#DateSave").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });;
        $('#DateSave').val(todayDate);
        $('#DateSave').data('kendoDatePicker').enable(true);

        $("#CategoryID").val('');
        $('#CategoryID').trigger('change');

        $("#ExpirationDate").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });;
        $('#ExpirationDate').val(todayDate);
        $('#ExpirationDate').data('kendoDatePicker').enable(true);

        $("#FileUpload").val('');
        $("#FileUpload").text('');

        $("#Status option[value='True']").attr('selected', true);
        setTimeout(function () {
            $("#CategoryID").focus();
        }, 500);
    }
    else {
        var currentRow = $(Obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);

        onBindDataToFormCompany(dataItem);

        $('#CategoryID').val(dataItem.CategoryID);
        $('#CategoryID').trigger('change');

        $("#txtFormat").text(dataItem.Format);
        $('#Format').val(dataItem.Format);

        $('#SideboardID').val(dataItem.SideboardID);
        $('#SideboardID').trigger('change');

        var datesave = kendo.toString(dataItem.DateSave, 'dd/MM/yyyy');
        $("#DateSave").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });
        $('#DateSave').val(datesave);
        $('#DateSave').data('kendoDatePicker').enable(false);

        var expirationdate = kendo.toString(dataItem.ExpirationDate, 'dd/MM/yyyy');
        $("#ExpirationDate").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });
        $('#ExpirationDate').val(expirationdate);
        $('#ExpirationDate').data('kendoDatePicker').enable(true);

        if (dataItem.IsExpirated == 'HH02')
            $('#ExpirationDate').data('kendoDatePicker').enable(false);

        $("#FileUpload").val('');
        $("#FileUpload").text('');
        //$('#FileUpload').trigger('change');
        //$('#FileUpload').attr("value", dataItem.FileNameLocal);

        //var status = dataItem.Status == true ? "True" : "False";
        //$('#Status').val(status);
        //$('#Status').trigger('change');
        setTimeout(function () {
            $("#CategoryID").focus();
        }, 500);
    }
}

function KeyCodeDate(e) {
    if (window.event.keyCode > 31 || window.event.keyCode < 90 || window.event.keyCode == 8) {
        return false;
    }
}


function ConfirmDelele(key, obj) {
    if (listrowid != null && listrowid != "") {
        //var c = confirm("Bạn có chắc chắn muốn ngưng hoạt động?.");
        if (key == 0) {
            $('#popupDelete').data("kendoWindow").close();
            $.post(r + "/Document/Delete", { data: listrowid, }, function (data) {
                if (data.success) {
                    alertBox("Thành công!", " Xóa thành công", true, 3000);
                    $("#grid").data("kendoGrid").dataSource.read();
                    $('#checkboxcheckAll').prop('checked', false);
                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                    //$("#grid").data("kendoGrid").dataSource.read();
                }
            });
        }
        else {
            $('#popupDelete').data("kendoWindow").close();
            return false;
        }
    }
}

var listrowid = "";
function Delete() {
    listrowid = "";
    var flagfail = 0;
    var StatusSaved = "";
    var StatusExpirated = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            var currentRow = $(this).closest("tr");
            var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
            listrowid += $(this).attr("id") + '@@@@';
            StatusSaved = dataItem.StatusSaved;
            StatusExpirated = dataItem.StatusExpirated;

            if (StatusSaved != 'TTHL2' || StatusExpirated != 'TTHH2') {
                //alertBox("Báo lỗi!", "Văn bản " + dataItem.DocumentName + " chưa hết hạn lưu trữ và hiệu lực.", false, 3000);
                flagfail++; $(this).prop('checked', false);
            }
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        var popupDelete = $('#popupDelete').data("kendoWindow");
        if (flagfail != 0)
            $("#txtWarning").text("Lưu ý: có văn bản chưa hết hạn lưu trữ và hiệu lực.");
        popupDelete.center().open();
        $(".k-window span.k-i-close").click(function () {
            eventHotKey = false;
            $("#divMaskPopup").hide();
        });
    }
    else if (flagfail === 0) {
        alertBox("Báo lỗi! ", "Vui lòng chọn dữ liệu.", false, 3000);
        flagfail = 0;
        return;
    }
}

function ChangeCategory() {

    if ($("#CategoryID").val() != "" && $("#CategoryID").val() != null)
        if (keyAction == 0) {
            $.post(r + "/Document/GetCategory", { CategoryID: $("#CategoryID").val() }, function (data) {
                if (data.success) {

                    $("#Format").val(data.data.Format);
                    $("#TimeSave").val(data.data.TimeSave);
                    $("#IsSaved").val(data.data.IsSaved);
                    $("#IsExpirated").val(data.data.IsExpirated);

                    $("#txtFormat").text(data.data.Format);
                    $("#DocumentName").val(data.data.CategoryName);
                    $("#SideboardID").val(data.data.SideboardID);
                    $("#SideboardID").trigger('change');
                }
            });
        }
}
