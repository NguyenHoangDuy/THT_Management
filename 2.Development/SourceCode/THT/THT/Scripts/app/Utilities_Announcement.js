var checkedIds = new Array();
var keyAction;
var indexTabstripActive = -1;
var currentAnnouncementID = "";
var editor;
var contentTab;

$(document).ready(function () {

    //active menu
    //resetMenu();
    activeMenu(1, 2, 2, '2_1', 'menu_Utilities_Announcement');

    loadToolbarStyle();

    //fillter & form popup
    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    $("#Status").select2();
    $("#s2id_Status").css('width', '240px');


    $("#Type").select2();
    $("#s2id_Type").css('width', '240px');
    //editor
    $(function () { createEditor('en', 'HTMLContentView') });
   
    //check box on grid
    $('#checkAll').bind('click', function () {
        
        if ($(this).is(':checked')) {
            $('.checkvalue').each(function () {
                if (!$(this).is(':checked')) {
                    $(this).trigger('click');
                }
            })
        }
        else {
            $('.checkvalue').each(function () {
                if ($(this).is(':checked')) {
                    $(this).trigger('click');
                }
            })
        }
    });

    $('#popupConfirmDelete').kendoWindow({
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
        visible: false,
        title: "Cảnh báo",
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupInActive').kendoWindow({
        width: "410px",
        actions: ["Close"],
        visible: false,
        title: "Cảnh báo",
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });
    $('#popupImport').kendoWindow({
        width: "600px",
        actions: [ "Close"],
        title: "Import",
        visible: false,
        resizeable: false,
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
                    alertBox("Thành công!", "Cập nhật thành công!", true, 3000);
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

    //popup
    $('#popup').kendoWindow({
        width: "900px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        //open: function (e) {
        //    this.wrapper.css({ top: $('#header').height() });
        //}
    });

    //form trong popup
    $("#formCreate").validate({
        // Rules for form validation
        rules: {
            AnnouncementID: {
                required: true,
                alphanumeric: true
            },
            Title: {
                required: true
            },
           
        },

        // Messages for form validation
        messages: {
            AnnouncementID: {
                required: "Thông tin bắt buộc"
            },
            Title: {
                required: "Thông tin bắt buộc"
            },
            
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            //error.insertAfter(element);
        },

        submitHandler: function (form) {
            $(form).ajaxSubmit({

                //clearForm: true,//To clear form after ajax submitting
                data: '{"HTMLContent":"' + CKEDITOR.instances['HTMLContentView'].getData() + '"}',
                beforeSend: function () {
                    $("#loading").removeClass('hide');
                    return;
                },
                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                            //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã thông báo: <strong style="color:red;margin-left: 20px;">' + data.
                            + '</strong></label><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="' + data.AnnouncementID + '" />';
                            $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float: left; width: 120px;">Mã thông báo: </label><label style="float: left;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.AnnouncementID + '</strong><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="' + data.AnnouncementID + '" /></label> <div style="clear:both"></div>');
                            $("#CreatedAt").val(dateToString(data.createdat));
                            $("#CreatedBy").val(data.createdby);
                            currentAnnouncementID = data.AnnouncementID;
                            keyAction = -1;
                        }
                        readHeaderInfo();
                        //showTabStrip();
                        $("#grid").data("kendoGrid").dataSource.read();
                        //selectedTabstrip(1);
                        alertBox("Thành công !", "Lưu thành công",true, 3000);
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
    $("#txtAnnouncementID,#AppliedFor_search").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
            return false;
        }
    });

    $("#selectIsActive_search").change(function () {
       // doSearch();
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
                        if (indexTabstripActive == -1) {
                            indexTabstripActive = 0;
                        }
                        if (indexTabstripActive == 0 && $("#btnInsert").length > 0) {
                            $("#formPopup").submit();
                        }
                        else if (indexTabstripActive == 1 && $("#btnInsert").length > 0) {
                        }
                        break;
                }
            }
        }
    });
    
});

//========================================== code co ban ====================================

//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtAnnouncementID").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        if ($.isNumeric(text)) {
            filterOr.filters.push({ field: "AnnouncementID", operator: "eq", value: text });
        }
        filterOr.filters.push({ field: "Title", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#AppliedFor_search").val();
    if (text) {
        filter.filters.push({ field: "AppliedFor", operator: "contains", value: text });
    }
    text = $("#selectIsActive_search").val();
    if (text) {

        filter.filters.push({ field: "Status", operator: "eq", value: text });
    }

    grid.dataSource.filter(filter);
}

//grid 
function onDatabound(e) {
    resizegrid_Announcement();
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

    changeStatusGrid('grid', 4);

    //load checkbox of selected rows
    
    for (var i = 0; i < checkedIds.length; i++) {
        
        $('#grid').find("input[class='checkvalue'][data-id='" + checkedIds[i] + "']").prop('checked', true);
    }
    //

    //$("#gridForm input[name='SalePriceOrther']")

    if (checkedIds.length > 0) {
        $("#btnDelete").css('opacity', '1');
    } else {
        $("#btnDelete").css('opacity', '0.3');
    }
}

function checkBox(obj) {
    
    if ($(obj).is(':checked')) {
        var index = checkedIds.indexOf($(obj).data('id'));
        if (index < 0) {
            //khong co moi pust vao, co roi thi bo qua
            checkedIds.push($(obj).data('id'));
        }
       
    }
    else {
        var index = checkedIds.indexOf($(obj).data('id'));
        if (index > -1) {
            checkedIds.splice(index, 1);
        }
    }
    if (checkedIds.length > 0) {
        $('#btnDelete').css('opacity', '1');
    } else {
        $('#btnDelete').css('opacity', '0.3');
    }
  
}


function onRequestStart(e) {
    $("#divLoading").show();
}

function onRequestEnd(e) {
    
    if (e.type == "update" || e.type == "create" ) {
        if (e.response.Errors == null) {
            
            alertBox("Thành công! ", "Lưu thành công", true, 3000);

        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }
    
    else if (e.type == "read") {
        
    }
    else {
        arrExport = new Array();
        for (var i = 0; i < e.sender._view.length; i++) {
            var value = e.sender._view[i];
            arrExport.push(value.AnnouncementID);
        }
    }
    $("#divLoading").hide();
}

//popup
function onOpenPopup(key, obj) {
    $("#formCreate").find('section em').remove();
    $("#formCreate").find('section label').removeClass('state-error').removeClass('state-success');
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    var popup = $('#popup').data("kendoWindow");
    popup.center().open();
    $(".k-window span.k-i-close").click(function () {
        eventHotKey = false;
        $("#divMaskPopup").hide();
    });

    if (key == 0) {     // Create
        popup.title('Thêm');
        $('#formCreate')[0].reset();
        $("#formCreate").attr("action", "/Utilities_Announcement/Create");
        //onRefreshPopup();
        setTimeout(function () {
            $("#Title").focus();
        }, 500);
    }
    else {      // Update
        $.post(r + "/Utilities_Announcement/GetById", { Id: obj }, function (data) {
            if (data.success) {
                var item = data.data;
                $('#AnnouncementID').val(item.AnnouncementID);
                $('#Title').val(item.Title);
                $("#Type option[value='" + item.Type + "']").attr('selected', true);
                $("#Type").select2();
                $("#s2id_Type").css('width', '100%');
                $("#Type").trigger('change');
                $("#StartDate").val(dateToString(item.StartDate));
                $("#EndDate").val(dateToString(item.EndDate));
                $('#Orders').val(item.Orders);
                $("#Status option[value='" + item.Status + "']").attr('selected', true);
                $("#Status").select2();
                $("#s2id_Status").css('width', '100%');
                $("#Status").trigger('change');
                $('#Description').val(item.Description);
                debugger
                CKEDITOR.instances['HTMLContentView'].setData(item.HTMLBody);
                $("#HTMLBody").val(item.HTMLBody);

                $("#formCreate").attr("action", "/Utilities_Announcement/Update");

            }
            else {

            }
        })


    }
}
function ResetForm() {
    $('#popup').data("kendoWindow").close();
    $("#divMaskPopup").hide();
}
function readHeaderInfo() {

    contentTab = setContentTab(["AnnouncementID", "Title"], "30");

}

function onRefreshPopup() {
   
    $('#formPopup')[0].reset();
    $("#formPopup").find('fieldset:eq(0) section:eq(0)').empty();
    //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label">Mã thông báo (*)<strong style="color:red;margin-left:10%"></strong></label><input type="hidden" id="AnnouncementID" name="AnnouncementID" value="0"/>');

    //$("#formPopup").find('fieldset:eq(0) section:eq(0)').empty().append('<label class="label" style="float:left">Mã thông báo (*)(*)</label><label class="input" style="float:right;width:240px;"><input type="text" id="AnnouncementID" name="AnnouncementID" class="input-xs" placeholder="Mã thông báo" style="margin-right:85px" /><b class="tooltip tooltip-top-right">Mã thông báo</b></label><div style="clear:both"></div>');
    //$("#HTMLContentView").data("kendoEditor").value("");
    CKEDITOR.instances['HTMLContentView'].setData("");
    $("#Status option[value='True']").attr('selected', true);
    $("#CreatedAt").val('');
    $("#CreatedBy").val('');
    $(".AvatarPathUser").attr("src", r + "/img/avatars/5.png");
    //hideTabStrip();
}


function deleteSelected() {
    
    //var data = checkedIds;
    if (checkedIds != "" && checkedIds != null) {
        $.SmartMessageBox({
            title: "",
            content: "Bạn có muốn xóa không?",
            buttons: '[Không][Có]'
        }, function (ButtonPressed) {
            if (ButtonPressed === "Có") {
                
                $.post(r + "/Utilities_Announcement/Deactive", { data: checkedIds.toString() }, function (data) {
                    if (data.success) {
                        
                        //$('#grid').attr('data-kendogrid-selected', '');
                        checkedIds = new Array();
                        //$.gritter.add({
                        //    // (string | mandatory) the heading of the notification
                        //    title: '',
                        //    // (string | mandatory) the text inside the notification
                        //    text: 'Xóa thành công',
                        //    class_name: 'gritter-success'
                        //});

                        alertBox("Thành công! ", "Xóa thành công", true, 3000);
                   
                        $("#grid").data("kendoGrid").dataSource.read();
                    }
                    else {
                        //$.gritter.add({
                        //    // (string | mandatory) the heading of the notification
                        //    title: '',
                        //    // (string | mandatory) the text inside the notification
                        //    text: data.message,
                        //    class_name: 'gritter-error'
                        //});

                        alertBox(data.message, "", false, 3000);

                    }
                });
            }
            if (ButtonPressed === "Không") {
                return;
            }

        });
    } else {
        return;
    }
}

//import
function openImport() {
    idPopup = ".k-window";
    $("#divMaskPopup").show();
    $('#popupImport').data("kendoWindow").open();
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


//========================================== code khac neu co ====================================

//Save Form
function onSaveForm() {
    debugger
    var editor_data = CKEDITOR.instances['HTMLContentView'].getData();
    var AnnouncementID = $("#AnnouncementID").val();
    var Title = $("#Title").val();
    var TextContent = $("#TextContent").val();
    var Status = $("#Status").val();
    var CreatedAt= $("#CreatedAt").val();
    var CreatedBy = $("#CreatedBy").val();

    var form = $('#formPopup');
    var action = form.attr("action");
    var serializedForm = form.serializeArray();

    var data = { AnnouncementID: AnnouncementID, Title: Title, HTMLContentView: editor_data, TextContent: TextContent, Status: Status, CreatedAt: CreatedAt };
    $("#loadingSaveForm").removeClass('hide');
    $("#btnSaveForm").attr('disabled', true);
    $.post(r + "/Utilities_Announcement/Create",
        { data: data }
        , function (data) {
        if (data.success) {
            alertBox("Thành công!","Lưu thành công", true, 3000);
            $("#grid").data("kendoGrid").dataSource.read();
        }
        else {
            alertBox("Báo lỗi", "Chưa lưu được", false, 3000);
            console.log(data.message);
        }
        $("#loadingSaveForm").addClass('hide');
        $("#btnSaveForm").attr('disabled', false);
    });
}

//ckeditor
function createEditor(languageCode, id) {
    var htmlContentDivHeight = parseInt($(window).height()) - $('#htmlContentInfor').height() - 460;
    var editor = CKEDITOR.replace(id, { language: languageCode, height: htmlContentDivHeight });
    CKEDITOR.instances['HTMLContentView'].on('change', function () {
        var editor_data = CKEDITOR.instances['HTMLContentView'].getData();
        $('#HTMLBody').val(editor_data);
    });
}

function getAnnouncementID() {

    return { AnnouncementID: currentAnnouncementID };
}
function resizegrid_Announcement() {
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
