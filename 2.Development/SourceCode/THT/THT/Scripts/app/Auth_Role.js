
var menuIDSelected = "";
var indexTabstripActive = -1;
var selectedMenu = "";
var contentTab;
$(document).ready(function () {
    //active menu

    activeMenu(1, 2, 1, '2_1', 'menu_Auth_Role');

    document.title = "Nhóm người dùng";
    //tabstrip
    //$("#tabstrip").kendoTabStrip({
    //    animation: { open: { effects: "fadeIn" } },
    //    activate: onActivateTabstrip,
    //});
    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');

    //generateSelect2("selectAction");
    //$("#selectAction").select2();
    //$("#s2id_selectAction").css('width', '240px');

    $("#selectUser").select2();
    $("#s2id_selectUser").css('width', '618px');

    $("#selectFunction").select2();
    $("#s2id_selectFunction").css('width', '618px');

    //popup
    $('#popup').kendoWindow({
        width: "780px",
        actions: ["Close"],
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

    $('#popupConfirmActive').kendoWindow({
        width: "410px",
        actions: ["Close"],
        title: "Cảnh báo",
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    //form trong popup
    $("#formPopup").validate({
        // Rules for form validation
        rules: {
            RoleName: {
                required: true
            }
        },

        // Messages for form validation
        messages: {
            RoleName: {
                required: "Thông tin bắt buộc"
            }
        },

        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },

        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                beforeSend: function () {
                    $("#loading").removeClass('hide');
                    $("#formPopup").find('button').attr('disabled', true);
                },
                success: function (data) {
                    if (data.success) {
                        if (data.insert) {
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="margin-top:-8px; margin-bottom: -6px;">Mã nhóm: <strong style="color:red;margin-left: 52px;">' + data.RoleID + '</strong></label><input type="hidden" id="RoleID" name="RoleID" value="' + data.RoleID + '"/>');
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
                            $("#RowCreatedAt").val(dateToString(data.RowCreatedAt));
                            $("#RowCreatedBy").val(data.RowCreatedBy);
                        }
                        $("#RoleID").val(data.RoleID);
                        readHeaderInfo();
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#formPopup").find('button').attr('disabled', false);
                        //showTabstrip();
                        SavePermission();
                    }
                    else {
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi", data.message, false, 3000);
                        console.log(data.message);
                    }
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }
    });

    //goi ham search khi nhan enter
    $("#txtRoleID").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
            return false;
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
                        //create at

                        var boolCreatedBy = $("#RowCreatedBy").val() != "system";
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0 && boolCreatedBy) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
                            onSaveUser();
                        }
                        break;
                }
            }
        }
    });

    //================================== Tab Phân quyền chức năng =====================================
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var text = $("#txtRoleID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "RoleID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "RoleName", operator: "contains", value: text });
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
    resizegridRole();
    changeStatusGrid('grid', 3);
    $("#divLoading").hide();
}

function onRequestStart(e) {
    $("#divLoading").show();
}
function resizegridRole() {
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

function UpdateStatus(key) {
    var listrowid = "";
    var NumberID = $("#NumberID").val();
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        $("#divMaskPopup").show();
        if (key == 0) {
            var popup = $('#popupConfirm').data("kendoWindow");
            popup.center().open();
        }
        else {
            var popup = $('#popupConfirmActive').data("kendoWindow");
            popup.center().open();
        }
        $(".k-window span.k-i-close").click(function () {
            eventHotKey = false;
            $("#divMaskPopup").hide();
        });
    }
    else {
        alertBox("Báo lỗi!", "Vui lòng nhóm quyền cần cập nhật.", false, 3000);
    }
}
function onUpdateStatus(key, obj) {
    debugger;
    var listrowid = "";
    var NumberID = $("#NumberID").val();
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        //var c = confirm("Bạn có chắc chắn muốn ngưng hoạt động?.");
        if (key == 0 || key == 1) {
            if (key == 0)
                $('#popupConfirm').data("kendoWindow").close();
            else
                $('#popupConfirmActive').data("kendoWindow").close();
            $.post(r + "/Auth_Role/UpdateStatus", { data: listrowid, status: key }, function (data) {
                if (data.success) {
                    alertBox("Thông báo! ", "Cập nhật trạng thái nhóm quyền thành công", true, 3000);
                    $('#checkboxcheckAll').prop('checked', false);
                    $("#grid").data("kendoGrid").dataSource.read();

                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                    $("#loading").addClass('hide');
                    console.log(data.message);
                }
            });
        }
        else
            if (key == 2) {
                $('#popupConfirm').data("kendoWindow").close();
                return false;
            }
            else {
                $('#popupConfirmActive').data("kendoWindow").close();
                return false;
            }
    }
    else {
        alertBox("", "Vui lòng chọn nhóm quyền cần cập nhật.", false, 3000);
    }
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
    $("#popup").parent().css('top', '20px');
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });

    if (key == 0) {     // Create
        popup.title('Tạo nhóm quyền');
        $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
        $("#RoleName").val('');
        $("#IsActive option:selected").removeAttr('selected');
        $("#IsActive").select2();
        $("#s2id_IsActive").css('width', '240px');
        $("#Note").val('');
        $("#RowCreatedAt").val('');
        $("#RowCreatedBy").val('');
        $("#divGridAction").empty;
        $('#btnSubmit').css({ 'display': 'block' });      
        $("#RoleID").val('');
        $("#selectFunction").val('');
        $("#selectFunction").trigger('change');     
        $("#selectUser option:selected").removeAttr('selected');

        showLoading();
        setTimeout(function () {
            $("#RoleID").focus();
        }, 500);
        loadgridpopup();
        hideLoading();
    }
    else if (key == 1 || key == 2) {      // 1 : Update, 2 : Sao chép           
        var id = $(obj).data('id');
        $("#selectAction option").removeAttr('selected');

        generateSelect2("selectAction");
        if (key == 1) {
            popup.title('Cập nhật quyền');
            $("#RoleID").val(id);
            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="margin-top:-8px;margin-bottom: -6px;">Mã nhóm: <strong style="color:red;margin-left: 52px;">' + id + '</strong></label><input type="hidden" id="RoleID" name="RoleID" value="' + id + '"/>');
            $("#formPopup").find('fieldset:eq(0) section:eq(0)').show();
            var currentRow = $(obj).closest("tr");
            var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
            onBindDataToForm(dataItem);
            showLoading();
            loadgridpopup();
            $("#selectUser").prop("disabled", true);
            $("#selectFunction").val('');
            $("#selectFunction").trigger('change');

            $.post(r + "/Auth_Role/GetMenuID", { id: id }, function (data) {
                if (data.success) {
                    $("#selectFunction option:selected").removeAttr('selected');
                    generateSelect2("selectFunction");
                    for (var i = 0; i < data.data.length; i++) {
                        $("#selectFunction option[value='" + data.data[i].MenuID + "']").attr('selected', true);
                    }
                    $("#selectFunction").trigger("change");
                }
            });

            $.post(r + "/Auth_Role/GetByID", { id: id }, function (data) {
                if (data.success) {
                    var value = data.data;
                    var active = value.IsActive == true ? "True" : "False";
                    $("#IsActive option[value='" + active + "']").attr('selected', true);
                    $("#IsActive").select2();
                    $("#s2id_IsActive").css('width', '240px');
                    $("#RowCreatedAt").val(dateToString(value.RowCreatedAt));
                    $("#RowCreatedBy").val(value.RowCreatedBy);
                    if (value.RowCreatedBy == "system") {
                        $('#btnSubmit').css({ 'display': 'none' })
                    }
                    else {
                        $('#btnSubmit').css({ 'display': 'block' })
                    }
                    // User of Role
                    $("#selectUser option:selected").removeAttr('selected');
                    for (var i = 0; i < data.listuser.length; i++) {
                        $("#selectUser option[value='" + data.listuser[i].UserID + "']").attr('selected', true);
                    }
                    $("#selectUser").trigger("change");
                    //generateSelect2("selectUser");
                    //readHeaderInfo();

                }
                else {
                    alertBox("Báo lỗi", "", false, 3000);
                    console.log(data.message);
                }
                hideLoading();
            });
        }
        else if (key == 2) {
            popup.title('Sao chép');
            $("#IsCopy").val('1');      // Value = 1 gửi lên Server để biết là sao chép
            $("#RoleID").val(id);
            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
            onRefreshPopup();
            $("#selectUser option:selected").removeAttr('selected');
            generateSelect2("selectUser");
            setTimeout(function () {
                $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
            }, 1000);
        }
    }
    setTimeout(function () {
        $("#RoleName").focus();
    }, 500);
}

function readHeaderInfo() {

    contentTab = setContentTab(["RoleID", "RoleName", "IsActive"], "30");

}

function onRefreshPopup() {
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
    $("#RoleName").val('');
    $("#IsActive option:selected").removeAttr('selected');
    $("#IsActive").select2();
    $("#s2id_IsActive").css('width', '240px');
    $("#Note").val('');
    $("#RowCreatedAt").val('');
    $("#RowCreatedBy").val('');
    $("#divGridAction").empty;
    $('#btnSubmit').css({ 'display': 'block' });
    generateSelect2("selectUser");
    hideTabStrip();
}
//function showTabstrip() {
//    $("#tabstrip").find('ul li:not(:first)').show();
//}

//function hideTabStrip() {
//    $("#tabstrip").find('ul li:not(:first)').hide();
//    setTimeout(function () {
//        $("#tabstrip").find('div.k-content:eq(0)').css('height', '100%');
//    }, 1000);
//}

//========================================== Tab Người dùng ==================================

function onSaveUser() {
    var text = $("#selectUser").val();
    if (text == null || text.length == 0) {
        text = "";
    }
    var id = $("#RoleID").val();
    if (typeof id == 'undefined') {
        alertBox("Báo lỗi", "Bạn chưa thêm nhóm quyền!!", false, 3000);
        return;
    }
    $("#loadingSaveUser").removeClass('hide');
    $("#btnSaveUser").attr('disabled', true);
    $.post(r + "/Auth_Role/AddUserToGroup", { id: id, data: text.toString() }, function (data) {
        if (data.success) {
            generateSelect2("selectUser");
            $("#grid").data("kendoGrid").dataSource.read();
            alertBox("Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", "Chưa lưu được", false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveUser").addClass('hide');
        $("#btnSaveUser").attr('disabled', false);
    });
}

//========================================== Tab Phân quyền chức năng ========================
function checkParentNode(nodes, checkedNodes) {
    if (typeof nodes != "undefined" && nodes.checked != true) {
        if (jQuery.inArray(nodes.id, checkedNodes) < 0)
            checkedNodes.unshift(nodes.id);
        if (typeof nodes.parent().parent() != "undefined") {
            checkParentNode(nodes.parent().parent(), checkedNodes);
        }
    }
}

// function that gathers IDs of checked nodes
function checkedNodeIds(nodes, checkedNodes) {
    for (var i = 0; i < nodes.length; i++) {
        if (nodes[i].checked) {
            if (typeof nodes[i].parent() != "undefined" && typeof nodes[i].parent().parent() != "undefined") {
                checkParentNode(nodes[i].parent().parent(), checkedNodes);
            }
            checkedNodes.push(nodes[i].id);
        }
        if (nodes[i].hasChildren) {
            checkedNodeIds(nodes[i].children.view(), checkedNodes);
        }
    }
}

// show checked node IDs on datasource change

function onLoadGridAction() {
    var obj = { "RoleID": $("#RoleID").val(), "MenuID": menuIDSelected };
    var detailTemplate = kendo.template($('#templateGridAction').html());
    $("#divGridAction").html(detailTemplate(obj));
    loadToolbarStyle();
}
function onChangeTree(e) {
    var selectedRows = this.select();
    var dataItem = this.dataItem(selectedRows[0]);
    if (dataItem.children.view().length > 0 || dataItem.id == "Home") {
        $("#divGridAction").empty();
        return;
    }
    menuIDSelected = dataItem.id;
    onLoadGridAction();
}

function onRequestStartGridAction(e) {
    $("#divLoading").show();
}

function onRequestEndGridAction(e) {
    if (e.type == "create" || e.type == "update" || e.type == "delete") {
        if (e.response.Errors == null) {
            onLoadGridAction();
            alertBox("Thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
}

function onDataboundGridAction(e) {
    $("#divLoading").hide();
}

function onEditGridAction(e) {
    if (e.model.isNew() == false) {
        $('input[name=Action]').parent().html(e.model.Action);
    }
}

function SavePermission() {
    var id = $("#RoleID").val();
    if (typeof id == 'undefined') {
        alertBox("Báo lỗi", "Chưa thêm nhóm quyền!!", false, 3000);
        return;
    }
    var listrowid = "";
    $("#gridpopup").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });

    debugger;
    var functionid = "";
    var text = $("#selectFunction").val();
    if (text != null) {
        for (i = 0; i < text.length - 1 ; i++) {
            functionid += functionid + "'" + text[i] + "',";
        }
        functionid += functionid + "'" + text[text.length - 1] + "'";
    }

    $.post(r + "/Auth_Role/SavePermission", { RoleID: $("#RoleID").val(), data: listrowid, FunctionID: functionid }, function (data) {
        if (data.success) {
            alertBox("Thành công!", "Lưu thành công", true, 3000);
        }
        else {
            alertBox("Báo lỗi", data.message, false, 3000);
            console.log(data.message);
        }
    });
}
function loadgridpopup() {
    var obj = {};
    //blockUIFromUser(true);
    var tempp = $('#formPopup #MenuName').val();
    obj = { "MenuName": tempp };
    var detailTemplate = kendo.template($('#templategrid').html());
    $("#templatecondittion").html(detailTemplate(obj));
    $("#templatecondittion").css({ 'clear': 'both' });
    $("#templatecondittion").css({ 'font-size': '11px' });
}
function onDataboundCondition(e) {
    var grid = $("#gridpopup").data("kendoGrid");
    var content = $("#gridpopup").find(".k-grid-content");
    content.height(265);
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
    $("#divLoading").hide();
    var x = $(e).prop('checked');
    $('#gridpopup').find(".checkrowid").each(function () {
        if ($(this).val() == 1)
            $(this).prop('checked', true);
        else {
            $(this).prop('checked', false);
        }
    });
}
function getRoleID() {
    if (typeof $("#RoleID").val() == 'undefined' || $("#RoleID").val() == null)
        return {
            roleID: ""
        }
    else {
        return {
            roleID: $("#RoleID").val()
        };
    }
}
function onRequestEndDetail(e) {
    if (e.type == "update" && !e.response.Errors) {
        alertBox("Thành công! ", "Cập nhật thành công", true, 3000);
        $("#gridpopup").data("kendoGrid").dataSource.read();
    }
    if (e.type == "create" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        $("#gridpopup").data("kendoGrid").dataSource.read();

    }
}
function error_handlerDetail(e) {
    if (e.errors) {
        var message = "";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alertBox("Báo lỗi! ", message, false, 3000);
    } else {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
    }
}
function checkAll() {
    var grid = $('#gridpopup').data('kendoGrid');
    grid.tbody.find(".checkAll").each(function () {
        debugger;
        var $this = $(this),
            ckbox,
            state;

        if ($this.hasClass("k-state-selected")) {
            debugger;
            ckbox = $this.find("td:last input");
            state = ckbox.prop("checked");
            ckbox.prop("checked", !state);
        }
    })
}

function ChangeFunction() {
    debugger;
    var grid = $("#gridpopup").data("kendoGrid");
    if (grid != null) {
        var filter = { logic: "and", filters: [] };

        var text = $("#selectFunction").val();
        if (text) {
            var filterOr = { logic: "or", filters: [] };
            for (i = 0; i < text.length; i++) {
                filterOr.filters.push({ field: "ParentMenuID", operator: "contains", value: text[i] });
            }
            filter.filters.push(filterOr);
        }
        grid.dataSource.filter(filter);
    }
    $('#checkboxcheckAllView').prop('checked', false);
    $('#checkboxcheckAllInsert').prop('checked', false);
    $('#checkboxcheckAllUpdate').prop('checked', false);
    $('#checkboxcheckAllDelete').prop('checked', false);
    $('#checkboxcheckAllExport').prop('checked', false);
    $('#checkboxcheckAllImport').prop('checked', false);
}

function checkAllView(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='_TTView']").each(function () {
        $(this).prop('checked', x);
    });
}

function checkAllInsert(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='_TTInsert']").each(function () {
        $(this).prop('checked', x);
    });
}

function checkAllUpdate(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='_TTUpdate']").each(function () {
        $(this).prop('checked', x);
    });
}


function checkAllDelete(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='_TTDelete']").each(function () {
        $(this).prop('checked', x);
    });
}


function checkAllExport(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='_TTExport']").each(function () {
        $(this).prop('checked', x);
    });
}


function checkAllImport(e) {
    var x = $(e).prop('checked');
    $('#gridpopup').find("[id*='Import']").each(function () {
        $(this).prop('checked', x);
    });
}
//========================================== code khac neu co ====================================

function changeAsset() {
}
