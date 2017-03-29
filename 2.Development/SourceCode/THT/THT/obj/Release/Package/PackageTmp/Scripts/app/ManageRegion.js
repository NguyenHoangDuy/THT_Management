
$(document).ready(function () {
    $("ul#menuLeft").find('#ul_root_4').addClass('open');
    $("ul#menuLeft").find('#ul_root_4').css('display', 'block');
    $("ul#menuLeft").find('#ul_root_4 ul#ul_item_1').css('display', 'block');
    $("#menu_ManageRegion").parent().addClass('active');
    $("#SearchProvinceID, #SearchStatus").select2();
    $("#s2id_SearchProvinceID, #s2id_SearchStatus").css('width', '240px');
    $("#txtID, #txtName").keypress(function (e) {
        if (e.keyCode == 13) {
            doSearch();
        }
    });
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

});

function RenderView() {
    debugger;
    var data = $("#grid").data("kendoGrid").dataSource.data();
    $.each(data, function (k, v) {
        var html = v.ProvinceID;
        if (html != "" && html != null) {
            $.each(listProvince, function (y, rule) {
                html = html.replace(rule.ProvinceID, '<span> ' + rule.TerritoryName + '</span>');
            });
            $('tr[data-uid="' + v.uid + '"] td:nth-child(4)').html(html);
        }
    })
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
        alertBox("Có lỗi!", " Chọn file để nhập Excel", false, 3000);
    }
}
function generateSelect2(id) {
    $("#" + id).select2();
    $("#s2id_" + id).css('width', '100%');
    $("#s2id_" + id).find('input').css('width', '100%');
}

function onDatabound() {
    resizeGrid();
    //RenderView();
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

    //$("#divLoading").hide();
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

function onRequestEnd(e) {
    if (e.type == "update" && !e.response.Errors) {
        alertBox("Thành công! ", "Cập nhật thành công", true, 3000);
        $("#grid").data("kendoGrid").dataSource.read();
    }
    if (e.type == "create" && !e.response.Errors) {
        alertBox("Thành công! ", "Lưu thành công", true, 3000);
        $("#grid").data("kendoGrid").dataSource.read();
    }
}

function error_handler(e) {
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
        //$("#grid").data("kendoGrid").dataSource.read();
    } 
}
function onRequestStart(e) {
    blockUI(false);
}

function blockUI(isMark) {
    if (isMark) {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false })).ajaxStop($.unblockUI);
    }
    else {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false, overlayCSS: { backgroundColor: 'transparent' } })).ajaxStop($.unblockUI);
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

function Cancel() {
    $('#grid').data('kendoGrid').cancelChanges();
}

function Insert() {
    $("#grid").data("kendoGrid").dataSource.insert();
}

function Update() {
    if ($('#grid').data('kendoGrid').dataSource.hasChanges() == true) {
        $('#grid').data('kendoGrid').saveChanges();
    }
}
function checkAll(e) {
    var x = $(e).prop('checked');
    $('#grid').find(".checkrowid").each(function () {
        $(this).prop('checked', x);
    });
}

function Delete() {
    var listrowid = "";
    $("#grid").find(".checkrowid").each(function () {
        if ($(this).prop('checked') == true) {
            listrowid += $(this).attr("id") + '@@@@';
        }
    });
    if (listrowid != null && listrowid != "") {
        var c = confirm("Bạn có chắc chắn muôn xóa.");
        if (c == true) {
            $.post(r + "/ManageRegion/Delete", { data: listrowid, }, function (data) {
                $.blockUI(true);
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
            return false;
        }
    }
    else{
        alertBox("Báo lỗi!", "Chọn dữ liệu để xóa.", false, 3000);
        return;
    }
}

function doSearch() {
    var grid = $("#grid").data("kendoGrid");
    var filter = { logic: "and", filters: [] };
    var Name = $("#txtID").val();
    if (Name) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "RegionID", operator: "eq", value: Name });
        filter.filters.push(filterOr);
    }
    var text = $("#txtName").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "RegionName", operator: "contains", value: text });
        filter.filters.push(filterOr);
    }
    var text = $("#SearchStatus").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        for (i = 0; i < text.length; i++) {
            filterOr.filters.push({ field: "Status", operator: "eq", value: text[i] });
        }
        filter.filters.push(filterOr);
    }
    grid.dataSource.filter(filter);
}


//$(document).ready(function () {
//   $("#grid").data("kendoGrid").bind("edit", function () { onEditMulSelect('ProvinceID', 'listProvince2') });
//});
// onEdit Multi Select 
function loadTextById(listOrgRule) {
    var result = listOrgRule;
    var data = $('#listProvince option');
    for (var i = 0, length = data.length; i < length; i++) {
        result = result.replace(data[i].value, data[i].label);
    }
    return result;
}
function onEditMulSelect(FieldName, DataName) {
    debugger;
    var charSplit = '; ';
    if ($('input[name=' + FieldName + ']').length > 0) {
        var html = ' <select name="select" id="MultiSelect" style="width: 250px;" multiple>';
        html += $('#' + DataName).html();
        html += '</select>';
        $('input[name="' + FieldName + '"]').parent().append(html);
        var OrgRule = $('input[name="' + FieldName + '"]').val().split(charSplit);
        for (var y = 0; y < OrgRule.length; y++) {
            $('#MultiSelect [value="' + OrgRule[y] + '"]').attr('selected', 'true');
        }
        $('#MultiSelect').width($('input[name=' + FieldName + ']').closest('td').width() - 20);
        //generateSelect2('MultiSelect');
        //$("#MultiSelect").select2();
        $("#MultiSelect").chosen();
        $('#MultiSelect_chosen').css('width', '645px');
        $('input[name="' + FieldName + '"]').hide();
        //$('#MultiSelect').focus();
        $('#MultiSelect_chosen input').focus();
        $('#MultiSelect').change(function () {
            var t = "";
            var data = $('#MultiSelect option:selected');
            for (var i = 0; i < data.length; i++) {
                t += data[i].value + charSplit;
            }
            $('input[name="' + FieldName + '"]').val(t);
            $('input[name="' + FieldName + '"]').trigger('change');
        });
    }
}
