$(document).ready(function () {

    $("ul#menuLeft").find('#ul_root_2').addClass('open');
    $("ul#menuLeft").find('#ul_root_2').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_2 ul#ul_item_7').css('display', 'block');
    $("#menu_Utilities_Email").parent().addClass('active');

    $("#selectIsActive_search").select2();
    $("#s2id_selectIsActive_search").css('width', '100%');

    //$("#ListMailTos").select2();
    //$("#s2id_ListMailTos").css('width', '240px');
    //$("#ListMailCCs").select2();
    //$("#s2id_ListMailCCs").css('width', '240px');
    $(function () { createEditor('en', 'HTMLContentView') });
    //form trong popup
    $('#popup').kendoWindow({
        width: "820px",
        actions: ["Close"],
        visible: false,
        resizable: false,
        close: function (e) {
            $("#divMaskPopup").hide();
        }
    });

    $("#formPopupMail").validate({
        // Rules for form validation
        rules: {
            UserMail: {
                required: true,
            },
            PasswordMail: {
                required: true
            },
            Port: {
                required: true,
                digits: true,
            },
            Host: {
                required: true,
            },

        },
        // Messages for form validation
        messages: {
            UserMail: {
                required: "Thông tin bắt buộc",
            },
            PasswordMail: {
                required: "Thông tin bắt buộc",
            },
            Port: {
                required: "Thông tin bắt buộc",
                digits: "Chỉ nhật ký tự số",
            },
            Host: {
                required: "Thông tin bắt buộc",
            }
        },
        // Do not change code below
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        },
        submitHandler: function (form) {
            $(form).ajaxSubmit({
                //clearForm: true,//To clear form after ajax submitting
                data: '{"HTMlBody":"' + CKEDITOR.instances['HTMLContentView'].getData() + '"}',
                beforeSend: function () {
                    $("#loading").removeClass('hide');
                },
                success: function (data) {
                    if (data.success) {
                        if (keyAction == 0) {   // Create
                          //  $("#formPopupMail").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã cấu hình: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + data.ID + '</strong><input type="hidden" id="ID" name="ID" value="' + data.ID + '" /></label> <div style="clear:both"></div></div>');
                            $("#CreatedAt").val(dateToString(data.CreatedAt));
                            $("#CreatedBy").val(data.CreatedBy);
                            keyAction = -1;
                        }
                        $("#grid").data("kendoGrid").dataSource.read();
                        alertBox("Thành công! ", "Lưu thành công", true, 3000);
                    }
                    else {
                        $("#loading").addClass('hide');
                        alertBox("Báo lỗi! ", data.message, false, 3000);
                    }
                    $("#loading").addClass('hide');
                }
            });
            return false;
        }
    });
});
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
//========================================== code co ban ====================================
//search
function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    //var text = $("#txtUserID").val();
    //if (text) {
    //    var filterOr = { logic: "or", filters: [] };
    //    //  filterOr.filters.push({ field: "UserID", operator: "contains", value: text });
    //    //  filterOr.filters.push({ field: "DisplayName", operator: "contains", value: text });
    //    //  filterOr.filters.push({ field: "FullName", operator: "contains", value: text });
    //    filter.filters.push(filterOr);
    //}
    grid.dataSource.filter(filter);
}
//grid 
function onDatabound(e) {
    resizegridMail();
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
    $("#divLoading").hide();
}

function onRequestStart(e) {
    $("#divLoading").show();
}
function onRequestEnd(e) {
    if (e.type == "update" || e.type == "create" || e.type == "delete") {
        if (e.response.Errors == null) {
            alertBox("Thành công!", "Lưu thành công", true, 3000);
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
    $("#formPopupMail").find('section em').remove();
    $("#formPopupMail").find('section label').removeClass('state-error').removeClass('state-success');
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
        setTimeout(function () {
            $("#UserMail").focus();
        }, 500);
    }
    else {      // Update
        keyAction = -1;
        popup.title('Chỉnh sửa');
        var id = $(obj).data('id');
        $("#formPopupMail").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float: right;">Mã cấu hình: </label></div><div class="divfile"><label style="float: right;text-align: left;width: 240px;height: 19px;padding-top: 2px;"><strong style="color:red;">' + id + '</strong><input type="hidden" id="ID" name="ID" value="' + id + '" /></label> <div style="clear:both"></div></div>');
        $("#formPopupMail").find('fieldset:eq(1) section:eq(0)').show();
        var currentRow = $(obj).closest("tr");
        var dataItem = $("#grid").data("kendoGrid").dataItem(currentRow);
     
        onBindDataToFormMail(dataItem);
        CKEDITOR.instances['HTMLContentView'].setData(htmlDecode(dataItem.HTMlBody));
        $("#HTMlBody").val(dataItem.HTMlBody);

        setTimeout(function () {
            $("#UserMail").focus();
        }, 500);
    }
}
//ckeditor
function createEditor(languageCode, id) {
    var htmlContentDivHeight = parseInt($(window).height()) - $('#div1').height() - 530;
    var editor = CKEDITOR.replace(id, { language: languageCode, height: htmlContentDivHeight });
    CKEDITOR.instances['HTMLContentView'].on('change', function () {
        var editor_data = CKEDITOR.instances['HTMLContentView'].getData();
        $('#HTMlBody').val(htmlEncode(editor_data));
    });
}

function onBindDataToFormMail(dataItem) {
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
function onRefreshPopup() {
    CKEDITOR.instances['HTMLContentView'].setData(htmlDecode(""));
    $("#formPopupMail").find('fieldset:eq(0) section:eq(0)').empty().append('<div class="divlabel"><label class="label" style="float:right">Mã cấu hình (*)</label></div><div class="divfile"><label class="input" style="float:right;width:240px;"><input type="text" id="ID" name="ID" class="input-xs" style="margin-right:85px" /></label><div style="clear:both"></div></div>');
    $("#UserMail").val('');
    $("#PasswordMail").val('');
    $("#Port").val('');
    $("#Host").val('');
}
//========================================== code khac neu co ====================================
function resizegridMail() {
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
function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}
