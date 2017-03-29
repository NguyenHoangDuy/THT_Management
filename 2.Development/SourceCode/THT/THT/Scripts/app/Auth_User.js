var keyAction;
var indexTabstripActive = -1;
var contentTab;
$(document).ready(function () {

    //active menu

    activeMenu(1, 2, 1, '2_2', 'menu_Auth_User');

    document.title = "Người dùng";

    //tabstrip
    $("#tabstrip").kendoTabStrip({
        animation: { open: { effects: "fadeIn" } },
        activate: onActivateTabstrip,
    });

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');
    $("#selectMerchant").select2();
    $("#s2id_selectMerchant").css('width', '240px');

    $("#Roles").select2();
    $("#s2id_Roles").css('width', '100%');
    //generateSelect2("Roles");

    //$("#listBrandID").select2();
    //$("#s2id_listBrandID").css('width', '100%');
    //generateSelect2("listBrandID");

    //popup
    $('#popupImport').kendoWindow({
        width: "600px",
        actions: ["Close"],
        title: "Nhập Excel",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupConfirm').kendoWindow({
        width: "410px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupActive').kendoWindow({
        width: "410px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupInActive').kendoWindow({
        width: "410px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupResetPassword').kendoWindow({
        width: "410px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
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
                if (data.errorfile != null && data.errorfile != "") {
                    console.log(data.errorfile);
                    $.SmartMessageBox({
                        title: "Lỗi",
                        content: "Có dòng lỗi, tải về để sửa lại.",
                        buttons: '[Hủy][Tải]'
                    }, function (ButtonPressed) {
                        if (ButtonPressed === "Tải") {

                            //window.location.href = r + "/Error/Download?file=" + data.errorfile;
                            //window.location.href = r + data.errorfile;
                            var locationFileName = data.errorfile;
                            var urlFolder = "ExcelImport\\Error\\";
                            var strParam = "urlFolder=" + urlFolder + "&file=" + locationFileName;
                            window.open(r + "/Home/Download?" + strParam, "_blank");
                        }
                        if (ButtonPressed === "Hủy") {
                            return;
                        }
                    });
                }
                else {
                    alertBox("Thành công", "", true, 3000);
                }
            }
            else {
                alertBox("Chưa nhập được<br>" + data.message, "", false, 3000);
                console.log(data.message);
            }
            $("#divLoading").hide();
        },
        complete: function (xhr) { }
    });

    //form trong popup
    $('#popup').kendoWindow({
        width: "410px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            UserID: {
                required: true,
                alphanumeric: true
            },
            DisplayName: {
                required: true
            },
            FullName: {
                required: true
            },
            Phone: {
                digits: true,
            },
            Email: {
                email: true,
            },
        },

        // Messages for form validation
        messages: {
            UserID: {
                required: "Thông tin bắt buộc"
            },
            DisplayName: {
                required: "Thông tin bắt buộc"
            },
            FullName: {
                required: "Thông tin bắt buộc"
            },
            Phone: {
                digits: "Số điện thoại là ký tự số",
            },
            Email: {
                email: "Email không hợp lệ",
            },
        },

        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },

        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                beforeSend: function () { $("#loading").removeClass('hide'); },

                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã người dùng: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.UserID + '</strong><input type="hidden" id="UserID" name="UserID" value="' + data.UserID + '" /></label> <div style="clear:both"></div></div>');
                            $("#RowCreatedAt").val(dateToString(data.RowCreatedAt));
                            $("#RowCreatedBy").val(data.RowCreatedBy);

                            keyAction = -1;
                        }
                        onSaveRole();
                        readHeaderInfo();

                        showTabstrip();
                        $("#grid").data("kendoGrid").dataSource.read();
                        //selectedTabstrip(1);
                        alertBox("Thành công! ", "Lưu thành công", true, 3000);
                    }
                    else {
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        console.log(data.message);
                    }
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }
    });

    //goi ham search khi nhan enter
    $("#txtUserID,#Roles_search,#Group_search").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });

    //bam + de them moi
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
                        var boolCreatedBy = $("#RowCreatedBy").val() != "system";
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0 && boolCreatedBy) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
                            onSaveRole();
                        }
                        else if (indexTabstripActive == 2 && $("#btnInsert").length > 0) {
                        }
                        break;
                }
            }
        }
    });

    $('#formPopup #Birthday').datepicker({
        dateFormat: 'dd/mm/yy',
        prevText: '<i class="fa fa-chevron-left"></i>',
        nextText: '<i class="fa fa-chevron-right"></i>',
    });
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtUserID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "UserID", operator: "contains", value: text });
        filterOr.filters.push({ field: "DisplayName", operator: "contains", value: text });
        filterOr.filters.push({ field: "FullName", operator: "contains", value: text });
        filterOr.filters.push({ field: "Phone", operator: "contains", value: text });
        filterOr.filters.push({ field: "Email", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#Group_search").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "Roles", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#Roles_search").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "AppliedFor", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }

    text = $("#selectIsActive_search").val();
    if (text) {
        filter.filters.push({ field: "IsActive", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}
//grid 
function onDatabound(e) {
    resizegridUser();
    var grid = $("#grid").data("kendoGrid");

    // ask the parameterMap to create the request object for you
    var requestObject = (new kendo.data.transports["aspnetmvc-server"]({ prefix: "" }))
    .options.parameterMap({
        page: grid.dataSource.page(),
        sort: grid.dataSource.sort(),
        filter: grid.dataSource.filter()
    });

    // Get the export link as jQuery object
    var $exportLink = grid.element.find('.export');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');
    if (href) {
        // Update the 'page' parameter with the grid's current page
        //href = href.replace(/page=([^&]*)/, 'page=' + requestObject.page || '~');

        // Update the 'sort' parameter with the grid's current sort descriptor
        href = href.replace(/sort=([^&]*)/, 'sort=' + requestObject.sort || '~');

        // Update the 'pageSize' parameter with the grid's current pageSize
        //href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + grid.dataSource._pageSize);

        //update filter descriptor with the filters applied

        href = href.replace(/filter=([^&]*)/, 'filter=' + (requestObject.filter || '~'));

        // Update the 'href' attribute
        $exportLink.attr('href', href);
    }
    changeStatusGrid('grid', 6);
    $("#divLoading").hide();
}

function onRequestStart(e) {
    $("#divLoading").show();
}

function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    $("#divLoading").hide();
}
//popup
function onOpenPopup(key, obj) {
    eventHotKey = true;
    $("#formPopup").find('section em').remove();
    $("#formPopup").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });
    if (key == 0) {     // Create
        keyAction = key;
        popup.title('Thêm');
        onRefreshPopup();
        //$("#formPopup").find('fieldset:eq(1) section:eq(0)').hide();
        $("#selectMerchant option:selected").removeAttr("selected");
        $("#selectMerchant").trigger('change');
        //$("#listBrandID").val('');
        //$("#listBrandID").trigger("change");
        setTimeout(function () {
            $("#UserID").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        showTabstrip();
        //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã người dùng: <strong style="color:red;margin-left: 20px;">' + id + '</strong></label><input type="hidden" id="UserID" name="UserID" value="' + id + '" />');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã người dùng: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="UserID" name="UserID" value="' + id + '" /></label> <div style="clear:both"></div></div>');
        //$("#formPopup").find('fieldset:eq(1) section:eq(0)').show();
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
        onBindDataToForm(dataItem);
        setTimeout(function () {
            $("#DisplayName").focus();
        }, 500);

        //$("#selectMerchant option:selected").removeAttr("selected");
        //$("#selectMerchant").trigger('change');
        //$.post(r + "/Auth_User/getMerchantbyID/", { userID: dataItem.UserID }, function (data) {
        //    if (data.success) {
        //        debugger;
        //        var value = data.data;
        //        $("#selectMerchant").val(value[0].MerchantID);
        //        $("#selectMerchant").trigger('change');


        //    }
        //});

        //$.post(r + "/Auth_User/getBrandIDbyUserID/", { userID: id }, function (data) {
        //    if (data.success) {
        //        //debugger;

        //        $("#listBrandID option:selected").removeAttr('selected');
        //        generateSelect2("listBrandID");
        //        var value = data.listBrandIDs;

        //        for (var i = 0; i < value.length; i++) {
        //            $("#listBrandID option[value='" + value[i].BrandID + "']").attr('selected', true);
        //        }
        //        $("#listBrandID").trigger("change");
        //    }
        //});

        //selectedTabstrip(0);
        $.post(r + "/Auth_User/GetUserByID", { userID: id }, function (data) {
            showLoading();

            if (data.success) {
                var value = data.data;
                var active = value.IsActive == true ? "True" : "False";
                $("#IsActive option[value='" + active + "']").attr('selected', true);
                $("#IsActive").select2();
                $("#IsActive").trigger('change');
                $("#s2id_IsActive").css('width', '240px');
                $("#RowCreatedAt").val(dateToString(value.RowCreatedAt));
                $("#RowCreatedBy").val(value.RowCreatedBy);
                debugger;
                if (value.RowCreatedBy == "system") {
                    $('#btnSubmit').css({ 'display': 'none' })
                }
                else {
                    $('#btnSubmit').css({ 'display': 'block' })
                }
                // Role of User
                $("#Roles option:selected").removeAttr('selected');
                for (var i = 0; i < data.groupuser.length; i++) {
                    $("#Roles option[value='" + data.groupuser[i].RoleID + "']").attr('selected', true);
                }
                $("#Roles").trigger("change");
                //generateSelect2("Roles");
                //$("#Roles").select2();
                //$("#s2id_Roles").css('width', '440px');


                readHeaderInfo();

            }
            else {
                alertBox("Báo lỗi", "", false, 3000);
                console.log(data.message);
            }
            hideLoading();
        });
    }
}

function readHeaderInfo() {

    contentTab = setContentTab(["UserID", "DisplayName", "FullName", "IsActive", 'Phone', 'Email'], "30");

}

function onRefreshPopup() {
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã người dùng (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="UserID" name="UserID" class="input-xs" placeholder="Mã người dùng" style="margin-right:85px" /></label><div style="clear:both"></div></div>');
    $("#DisplayName").val('');
    $("#FullName").val('');
    $("#IsActive option[value='True']").attr('selected', true);
    $("#IsActive").trigger('change');
    $("#Phone").val('');
    $("#Email").val('');
    $("#Note").val('');
    $("#RowCreatedAt").val('');
    $("#RowCreatedBy").val('');
    $('#btnSubmit').css({ 'display': 'block' });
    $("#Roles option:selected").removeAttr('selected');
    generateSelect2("Roles");
    hideTabStrip();
}

function openImport() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImport').data("kendoWindow").center().open();
    $(".k-window span.k-i-close").click(function () {
        $("#divMaskPopup").hide();
    });
}

function beginImport() {
    var value = $("input[name='FileUpload']").val();
    if (value != null && value != "") {
        $("#divLoading").show();
        $("#importform").submit();
    }
    else {
        alertBox("Báo lỗi", "Chọn file để nhập Excel", false, 3000);
    }
}

//tabstrip
function onActivateTabstrip(e) {
    var index = $(e.item).index();
    indexTabstripActive = index;

    if (index != 0) {
        var currentTab = e.contentElement.id;
        getContentTab(currentTab, contentTab);
    }

    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
    }, 1000);
}

function selectedTabstrip(index) {
    indexTabstripActive = index;
    $("#tabstrip").kendoTabStrip().data("kendoTabStrip").select(index);

    if (index != 0) {
        var currentTab = 'tabstrip-' + (index + 1);
        getContentTab(currentTab, contentTab);
    }

    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(' + index + ')').css('height', '100%');
    }, 500);
}

function showTabstrip() {
    $("#tabstrip").find('ul li:not(:first)').show();
}

function hideTabStrip() {
    $("#tabstrip").find('ul li:not(:first)').hide();
    setTimeout(function () {
        $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
    }, 1000);
    //selectedTabstrip(0);
}

//========================================== code khac neu co ====================================

//Cap nhat phan quyen
function onUpdatePermissionData() {
    $("#divLoading").show();
    $.post(r + "//ExecPermissionData", { userID: "" }, function (data) {
        if (data.success) {
            alertBox("Cập nhật thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
        $("#divLoading").hide();
    });
}

function onResetPassword2(obj) {
    debugger
    var userID = $(obj).data('id');
    $('#username').val(userID);
    if (userID) {
        //$.confirm({
        //    title: 'Xác nhận!',
        //    content: 'Bạn chắc chắn!',
        //    confirmButtonClass: 'btn btn-small btn-info',
        //    cancelButtonClass: 'btn btn-small btn-danger',
        //    confirmButton: 'Đồng ý',
        //    cancelButton: 'Bỏ qua',
        //    confirm: function () {
        //        $.post(r + "/Auth_User/ResetPassword", { data: userID }, function (data) {
        //            if (data.success) {
        //                alertBox("Thông báo! ", "Cập nhật mật khẩu thành công", true, 3000);
        //            }
        //            else {
        //                alertBox("Báo lỗi! ", data.message, false, 3000);
        //                $("#loading").addClass('hide');
        //                console.log(data.message);
        //            }
        //        });
        //    },
        //    cancel: function () {
        //        return;
        //    }
        //});
        $("#divMaskPopup").show();
        var popup = $('#popupConfirm').data("kendoWindow");
        popup.center().open();
        $(".k-window span.k-i-close").click(function () {
            eventHotKey = false;
            $("#divMaskPopup").hide();
        });
    }
}

function onResetPassword(obj) {
    var userID = $(obj).data('id');
    if (userID) {
        $.SmartMessageBox({
            title: "Khôi phục mật khẩu",
            content: "Khôi phục mật khẩu cho " + userID + " ?",
            buttons: '[Hủy][OK]'
        }, function (ButtonPressed) {
            if (ButtonPressed === "OK") {
                $("#divLoading").show();
                $(obj).attr('disabled', true);
                $.post(r + "/Auth_User/ResetPasswordUser", { userID: userID }, function (data) {
                    if (data.success) {
                        alertBox("Thông báo!", "Khôi phục thành công", true, 3000);
                    }
                    else {
                        alertBox("Báo lỗi", "Chưa khôi phục được", false, 3000);
                        console.log(data.message);
                    }
                    $(obj).attr('disabled', false);
                    $("#divLoading").hide();
                });
            }
            if (ButtonPressed === "Hủy") {
                return;
            }
        });
    }
    else {
        alertBox("Báo lỗi", "Chưa khôi phục được", false, 3000);
    }
}

//Role
function onSaveRole() {
    var text = $("#Roles").val();
    if (text == null || text.length == 0) {
        text = "";
    }
    var id = $("#UserID").val();
    $("#loadingSaveRole").removeClass('hide');
    $("#btnSaveRole").attr('disabled', true);
    $.post(r + "/Auth_User/AddUserToGroup", { id: id, data: text.toString() }, function (data) {
        //if (data.success) {
        //    //selectedTabstrip(2);
        //    generateSelect2("Roles");

        //    $("#grid").data("kendoGrid").dataSource.read();
        //    alertBox("Thành công! ", "Lưu thành công", true, 3000);
        //}
        //else {
        //    alertBox("Báo lỗi! ", data.message, false, 3000);
        //    console.log(data.message);
        //}
        //$("#loadingSaveRole").addClass('hide');
        //$("#btnSaveRole").attr('disabled', false);
    });
}
function resizegridUser() {
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
function checkAllHeader(e) {
    var x = $(e).prop('checked');
    $('#grid').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}
function UpdateStatus() {
    var listrowid = "";
    //var NumberID = $("#NumberID").val();
    var userid = $("#username").val();
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $.confirm({
            title: 'Xác nhận!',
            content: 'Bạn chắc chắn!',
            confirmButtonClass: 'btn btn-small btn-info',
            cancelButtonClass: 'btn btn-small btn-danger',
            confirmButton: 'Đồng ý',
            cancelButton: 'Bỏ qua',
            confirm: function () {
                $.post(r + "/Auth_User/ResetPassword", { data: userid }, function (data) {
                    if (data.success) {
                        alertBox("Thông báo! ", "Cập nhật mật khẩu thành công", true, 3000);
                    }
                    else {
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                        $("#loading").addClass('hide');
                        console.log(data.message);
                    }
                });
            },
            cancel: function () {
                return;
            }
        });
        //$("#divMaskPopup").show();
        //var popup = $('#popupConfirm').data("kendoWindow");
        //popup.center().open();
        //$(".k-window span.k-i-close").click(function () {
        //    eventHotKey = false;
        //    $("#divMaskPopup").hide();
        //});
    }
    else {
        alertBox("Báo lỗi!", "Vui lòng chọn người dùng.", false, 3000);
    }
}
function onUpdateStatus(key, obj) {
    debugger;
    var listrowid = "";
    var userid = $("#username").val();
    if (key == 0) {
        $('#popupConfirm').data("kendoWindow").close();
        $.post(r + "/Auth_User/ResetPassword", { data: userid }, function (data) {
            if (data.success) {
                alertBox("Thông báo! ", "Cập nhật mật khẩu thành công", true, 3000);
            }
            else {
                alertBox("Báo lỗi! ", data.message, false, 3000);
                $("#loading").addClass('hide');
                console.log(data.message);
            }
        });
    }else {
        $('#popupConfirm').data("kendoWindow").close();
    }
}

function showPopupEditInfor2(obj) {
    debugger
    idEditInfor = obj.id;
    idPopup = ".k-window";
    //$("#divMaskPopup").show();
    //code moi cho ham show poupup

    $.get(r + "/Auth_User/EditInfor", { userid: idEditInfor }, function (data) {
        if (data) {
            $("#popupEditInfor").parent().remove();
            $("#divEditinfor").empty().html(data);
            

            //==========================================================form popup edit infor====================================================
            $('#popupEditInfor').kendoWindow({
                width: "900px",
                actions: ["Close"],
                visible: false,
                resizable: false,
                close: function (e) {
                    $("#divMaskPopup").hide();
                },
            });
            var popup = $('#popupEditInfor').data("kendoWindow");
            //getListItem({ Level: 'Province' }, 'formPopupEditInfor #ProvinceID', 'Auth_User', 'GetProvinceDDL', '240px');
            //generateSelect2('s', 'formPopupEditInfor #ProvinceID', value.ProvinceID, '240px');
            popup.center().open();

        }

    });
}
    
    
function showEditForm(e) {
    debugger
    var UserID = $(e).data("id");
    showPopupEditInfor(UserID);
    $("#divMaskPopup").show();

}




