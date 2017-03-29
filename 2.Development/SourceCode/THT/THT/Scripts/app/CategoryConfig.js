var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;

$(document).ready(function () {

    $("ul#menuLeft").find('#ul_root_3').addClass('open');
    $("ul#menuLeft").find('#ul_root_3').css('display', 'block');
    $("#menu_CategoryConfig").parent().addClass('active');

    resetMenu();
    $("#menu_CategoryConfig").parent().addClass('active');

    //fillter & form popup
    $("#SideboardID").select2();
    $("#s2id_SideboardID").css('width', '240px');

    $("#IsSaved").select2();
    $("#s2id_IsSaved").css('width', '240px');

    $("#listGroupDocument").select2();
    $("#s2id_listGroupDocument").css('width', '240px');

    

    $("#IsExpirated").select2();
    $("#s2id_IsExpirated").css('width', '240px');

    $(".allownumericwithdecimal").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
        if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    })

    $('#selectStatus_search').bind('change', function () {
        doSearch();
    })

    $(window).resize(function () {
        resizeGrid(numHeight);
    });

    $("#txtCategoryID").keypress(function (e) {
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
        width: "300px",
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

            CategoryName: {
                required: true,
            },
            IsSaved: {
                required: true,
            },
            IsExpirated: {
                required: true,
                // alphanumeric: true
            },
            SideboardID: {
                required: true,
                //alphanumeric: true
            },
            Format: {
                required: true,
                //alphanumeric: true
            },
            TimeSave: {
                required: true,
                number: true
                //alphanumeric: true
            },
            listGroupDocument: {
                required: true,
            },
        },

        // Messages for form validation
        messages: {

            CategoryName: {
                required: "Thông tin bắt buộc"
            },
            IsSaved: {
                required: "Thông tin bắt buộc"
            },
            IsExpirated: {
                required: "Thông tin bắt buộc"
                // alphanumeric: true
            },
            SideboardID: {
                required: "Thông tin bắt buộc"
                //alphanumeric: true
            },
            Format: {
                required: "Thông tin bắt buộc"
                //alphanumeric: true
            },
            TimeSave: {
                required: "Thông tin bắt buộc",
                number: "Nhập số"
            },
            listGroupDocument: {
                required: "Thông tin bắt buộc",
            },
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
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã văn bản (*) </label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.CategoryID + '</strong><input type="hidden" id="CategoryID" name="CategoryID" value="' + data.CategoryID + '" /></label> <div style="clear:both"></div></div>');
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
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div style="float: left; width: 120px; margin-left:0;"><label class="label" style="float: right;">Mã văn bản (*)</label></div><div style="float: left; width: 240px; margin-right: 0; margin-left: 10px;"> <label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="CategoryID" name="CategoryID" value="' + id + '"/></label> <div style="clear:both"></div></di>');
        LoadDataPopup();
    }
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
    var text = $("#txtCategoryID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "CategoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DocumentName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }

    text = $("#selectStatus_search").val();
    if (text) {
        filter.filters.push({ field: "Status", operator: "eq", value: text });
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
        $.post(r + "/CategoryConfig/UpdateStatus", { data: listrowid, status: valKey }, function (data) {
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

        $("#CategoryID").val('');
        $('#CategoryID').trigger('change');

        $("#SideboardID").val('');
        $('#SideboardID').trigger('change');

        $("#IsSaved").val('');
        $('#IsSaved').trigger('change');

        $("#listGroupDocument").val('');
        $('#listGroupDocument').trigger('change');

        $("#IsExpirated").val('');
        $('#IsExpirated').trigger('change');

        $("#Format").val('');
        $("#TimeSave").val('');

        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
        var todayDate = kendo.toString(kendo.parseDate(new Date()), 'dd/MM/yyyy');
        $("#ExpirationDate").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });;
        $('#ExpirationDate').val(todayDate);


        $("#Status option[value='True']").attr('selected', true);
        setTimeout(function () {
            $("#CategoryName").focus();
        }, 500);
    }
    else {
        var currentRow = $(Obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);

        onBindDataToFormCompany(dataItem);

        $.post(r + "/CategoryConfig/GetGroupDocumentByCategory", { CategoryID: id }, function (data) {
            if (data.success) {
                debugger;
                $("#listGroupDocument option:selected").removeAttr('selected');
                $("#listGroupDocument").trigger("change");
                //generateSelect2("ma_vung_hd");
                for (var i = 0; i < data.data.length; i++) {
                    $("#listGroupDocument option[value='" + data.data[i].GroupID + "']").prop('selected', true);
                }
                setTimeout(function () {
                }, 100);
                $("#listGroupDocument").trigger("change");
            }
        });


        $("#SideboardID").val(dataItem.SideboardID);
        $('#SideboardID').trigger('change');

        $("#IsSaved").val(dataItem.IsSaved);
        $('#IsSaved').trigger('change');

        $("#IsExpirated").val(dataItem.IsExpirated);
        $('#IsExpirated').trigger('change');

        $("#Format").val(dataItem.Format);
        $("#TimeSave").val(dataItem.TimeSave);

        var todayDate = kendo.toString(dataItem.ExpirationDate, 'dd/MM/yyyy');
        $("#ExpirationDate").kendoDatePicker({
            format: 'dd/MM/yyyy'
        });;
        $('#ExpirationDate').val(todayDate);

        var status = dataItem.Status == true ? "True" : "False";
        $('#Status').val(status);
        $('#Status').trigger('change');
        setTimeout(function () {
            $("#DCName").focus();
        }, 500);
    }
}

function KeyCodeDate(e) {
    if (window.event.keyCode > 31 || window.event.keyCode < 90 || window.event.keyCode == 8) {
        return false;
    }
}


function ConfirmDelele(key, obj) {
    debugger;
    var listrowid = "";

    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        //var c = confirm("Bạn có chắc chắn muốn ngưng hoạt động?.");
        if (key == 0) {
            $('#popupDelete').data("kendoWindow").close();
            $.post(r + "/CategoryConfig/Delete", { data: listrowid, }, function (data) {
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
    else {
        alertBox("", "Vui lòng chọn danh mục cần xóa.", false, 3000);
    }
}


function Delete() {
    var listrowid = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        debugger
        $("#divMaskPopup").show();
        var popupDelete = $('#popupDelete').data("kendoWindow");
        popupDelete.center().open();
        $(".k-window span.k-i-close").click(function () {
            eventHotKey = false;
            $("#divMaskPopup").hide();
        });
    }
    else {
        alertBox("Báo lỗi!", "Chọn dữ liệu để xóa.", false, 3000);
        return;
    }
}

;