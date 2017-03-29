/// <reference path="app.js" />
var currentTimeSystem;

$(document).ready(function () {
    $('.start_EndDate').daterangepicker({
        ranges: {
            'Hôm nay': [moment(), moment()],
            'Hôm qua': [moment().subtract('days', 1), moment().subtract('days', 1)],
            '7 ngày trước': [moment().subtract('days', 6), moment()],
            '30 ngày trước': [moment().subtract('days', 29), moment()],
            'Tháng này': [moment().startOf('month'), moment().endOf('month')],
            'Tháng trươc': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
        }, format: 'YYYY/MM/DD',
        locale: {
            applyLabel: 'Xác nhận',
            cancelLabel: 'Đóng lại',
            fromLabel: 'Từ',
            toLabel: 'Đến',
            customRangeLabel: 'Tuỳ chọn',
            daysOfWeek: ['T7', 'CN', 'T2', 'T3', 'T4', 'T5', 'T6'],
            monthNames: ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'],
            firstDay: 1,
        }
    });
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        prevText: '<i class="fa fa-chevron-left"></i>',
        nextText: '<i class="fa fa-chevron-right"></i>'
    });

    jQuery.validator.addMethod("alphanumeric", function (value, element) {
        return this.optional(element) || /^\w+$/i.test(value);
    }, "Không nhập ký tự đặc biệt");

    $(document).keypress(function (e) {

        if (e.keyCode == 13) {
            e.preventDefault();
            return false;
        }
    });

    getDateSystem();
});

function resizeGrid(number) {
    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = $("#grid").find(".k-grid-content");
    content.height(h - number);
}

function resizeOtherGrid(number, gridE) {

    var h_search = $(".divSearch").height();
    var h = parseInt($(window).height()) - h_search;
    var content = gridE.find(".k-grid-content");
    content.height(h - number);
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
//load dynamic list selection 

function getListItem(data, idElementSet, routerName, method, width) {

    $.ajax({
        url: r + "/" + routerName + "/" + method,
        type: "post",
        async: false,
        data: data,
        beforeSend: function () {
            showLoading();
        },
        success: function (data) {

            if (data.success) {

                $("#" + idElementSet).empty();
                var item = '<option></option>';
                for (var i = 0; i < data.data.length; i++) {
                    var value = data.data[i];
                    item += '<option value="' + value.ID + '">' + value.Name + '</option>';
                }
                $("#" + idElementSet).append(item);
                $("#" + idElementSet).select2();
                $("#s2id_" + idElementSet).css('width', width);
            }
            else {
                alertBox("Có lỗi", data.message, false, 3000);
                console.log(data.message);
            }
            hideLoading();
        }
    });
}

// chosen order value defalut dropdowlist

function changeChosenSelection(idItem, position) {

    //var teamp = $("#DistributorID").find(" option:eq(1)");
    var teamp = $("#" + idItem).find(" option:eq(0)").val();
    //var positionString="option:eq("+position+")";
    var teamp2 = $("#" + idItem).find("option:eq(" + position + ")").val();
    var teamp3 = $("#" + idItem).find("option:eq(" + position + ")").text();
    if (teamp2 == '') {
        return;
    }
    $("#" + idItem + " option[value='" + teamp2 + "']").attr("selected", "selected");
    $("#s2id_" + idItem).find(".select2-chosen").text(teamp3);
    $("#s2id_" + idItem).find(" a:eq(0)").removeClass().addClass("select2-choice");

}

// chuyen text true/fail thanh hoat dong/ ngung hoat dong tren luoi
function changeStatusGrid(idGrid) {
    var arrRow = $("#" + idGrid).find(".k-grid-content table tbody tr");
    for (var i = 0; i < arrRow.length; i++) {
        var arrCol = arrRow[i].cells;
        if (arrCol.length == 0) {
            continue;
        }
        //lưu ý nên để những đoạn code change chỉ cần thay đổi màu sắc lên trên những đoạn code thay đổi giá trị của cột
        //đoạn code bên dưới chỉ là đoạn code ví dụ đổi màu của cột dựa theo những thông số trên cột khác
        //changeNameByIsActive(arrCol);

        //'change' trong 1 column không liên quan những column khác
        changeIsActive(arrCol);
       

    }
    
    $("#divLoading").hide();
}

//vi du: chuyen mau ten day du dua theo trang thai isActive
function changeNameByIsActive(arrCol) {
    debugger;
    var tr_displayName = '';
    var tr_fullName = '';
    var tr_isActive = '';
    for (var j = 0; j < arrCol.length; j++) {
        var tr;
        var columnName = '';
        tr = arrCol[j];
        var attr = $(tr).attr('data-column');
        // For some browsers, `attr` is undefined; for others,
        // `attr` is false.  Check for both.
        if (typeof attr === typeof undefined || attr === false) {
            continue;
        }
        if (attr == 'DisplayName') {
            tr_displayName = tr;
        }
        if (attr == 'FullName') {
            tr_fullName = tr;
        }
        if (attr == 'IsActive') {
            tr_isActive = tr;
        }
    }
    if (tr_isActive.textContent == "true") {
        $(tr_displayName).css({ 'background-color': 'rgb(178,199,206)', 'color': 'rgb(156, 0, 6)' });
        $(tr_fullName).css({ 'background-color': 'rgb(178,199,206)', 'color': 'rgb(156, 0, 6)' });
        
    }
    else if (tr_isActive.textContent == "false") {
        $(tr_displayName).css({ 'background-color': 'rgb(255,199,206)', 'color': 'rgb(156, 0, 6)' });
        $(tr_fullName).css({ 'background-color': 'rgb(255,199,206)', 'color': 'rgb(156, 0, 6)' });
    }
}

//lưu ý nên để những đoạn code change chỉ cần thay đổi màu sắc lên trên những đoạn code thay đổi giá trị của cột
function changeIsActive(arrCol) {
    for (var j = 0; j < arrCol.length; j++) {
        var tr;
        var columnName = '';
        tr = arrCol[j];
        var attr = $(tr).attr('data-column');
        // For some browsers, `attr` is undefined; for others,
        // `attr` is false.  Check for both.
        if (typeof attr === typeof undefined || attr === false) {
            continue;
        }
        if (attr == 'IsActive') {
            if (tr.textContent == "true") {
                //$(tr).empty().append('<span class="label label-success" style="font-size:12px">Đang hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Đang hoạt động</span>');
            }
            else if (tr.textContent == "false") {
                //$(tr).empty().append('<span class="label label-danger" style="font-size:12px">Ngưng hoạt động</span>');
                $(tr).empty().append('<span class="label-success" style="font-size:12px">Ngưng hoạt động</span>');
            }
        }
    }
}

function loadToolbarStyle() {
    // Remove Icon
    $("div.k-grid-toolbar a span").remove();


    $("a.k-grid-cancel-changes").css({ 'background-color': '#D15b47', 'color': 'white' });
    $("a.k-grid-save-changes").hover(
      function () { $(this).css({ 'background-color': '#5b835b', 'color': 'white' }); },
       function () { $(this).css({ 'background-color': '#739e73', 'color': 'white' }); }
    )
    $("a.k-grid-save-changes").css({ 'background-color': '#428bca', 'color': 'white' });

    $("a.btn-custom1").hover(

     function () { $(this).css({ 'background-color': '#5b835b', 'color': 'white' }); },
      function () { $(this).css({ 'background-color': '#739e73', 'color': 'white' }); }
   )
    $("a.btn-custom1").css({ 'background-color': '#739e73', 'color': 'white' });

}

function onBindDataToForm(dataItem) {

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

//chuyen doi tu ngay thang nam thanh nam thang ngay
function convertDateFormart(strDate, type) {
    var dateConver = "";
    var res = strDate.split("/");

    if (res.length < 3) {
        return "";
    }
    //defalute tu ngay thang nam -> nam thang ngay
    if (type == 0) {
        dateConver = res[2] + "/" + res[1] + "/" + res[0];
    }
    return dateConver
}

function dateToString(date) {

    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        return date.getFullYear() + '/' + month + '/' + day + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
    }
    else {
        return "";
    }
}

function dateToShortDateString(date) {
    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        return date.getFullYear() + '/' + month + '/' + day;
    }
    else {
        return "";
    }
}

function dateToShortDateStringFormat(date, format) {
    if (date != null) {
        date = new Date(date.match(/\d+/)[0] * 1);
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var month = (date.getMonth() + 1) < 10 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1);
        if (format == "dd/MM/yyyy") {
            return day + '/' + month + '/' + date.getFullYear();
        }
        else if (format == "yyyy/MM/dd") {
            return date.getFullYear() + '/' + month + '/' + day;
        }
    }
    else {
        return "";
    }
}

function removeSignFromString(obj) {
    var str;
    if (eval(obj)) {
        str = eval(obj).value;
    }
    else {
        str = obj;
    }

    str = str.toLowerCase();
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    //str= str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'| |\"|\&|\#|\[|\]|~|$|_/g,"-");  
    // tìm và thay thế các kí tự đặc biệt trong chuỗi sang kí tự - /
    //str= str.replace(/-+-/g,"-"); //thay thế 2- thành 1-  
    str = str.replace(/^\-+|\-+$/g, "");
    //cắt bỏ ký tự - ở đầu và cuối chuỗi 
    //eval(obj).value = str.toUpperCase();
    return str;
}

function generateSelect2(id) {
    $("#" + id).select2();
    $("#s2id_" + id).css('width', '100%');
    $("#s2id_" + id).find('input').css('width', '100%');
}

function activeMenu(li, ulRoot, ulItem, ulItem2, idMenu) {
    $("ul#menuLeft").find('li:eq(' + li + ')').addClass('open');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot).css('display', 'block');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem).css('display', 'block');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem + ' ul#ul_item' + ulItem2).css('display', 'block');
    $("#" + idMenu).parent().addClass('active');

    $("ul#menuLeft").find('li:eq(' + li + ') > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' > li:eq(0) > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
    $("ul#menuLeft").find('li:eq(' + li + ') ul#ul_root_' + ulRoot + ' ul#ul_item_' + ulItem + ' > li:eq(0) > a > b > em').removeClass('fa-plus-square-o').addClass('fa-minus-square-o');
}

function showLoading() {
    $("#divLoading").show();
}

function hideLoading() {
    $("#divLoading").hide();
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function htmlEncode(value) {
    //create a in-memory div, set it's inner text(which jQuery automatically encodes)
    //then grab the encoded contents back out.  The div never exists on the page.
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

//========================================== LoadContentTab ===========================================

function setContentTab(listItem, widthcontent) {

    contentTab = "";
    for (var i = 0; i <= listItem.length - 1; i++) {

        var itemValue = "";
        var itemLabel = "";
        var itemType = $("#" + listItem[i]).is('select');

        //itemValue
        if ($("#" + listItem[i]).is('select')) {
            itemValue = $("#" + listItem[i] + " option:selected").text();
        }
        else if ($("#" + listItem[i]).is('label')) {
            itemValue = $("#" + listItem[i]).text();
        }
        else if ($("#" + listItem[i]).is('span')) {
            itemValue = $("#" + listItem[i]).text();
        }
        else {
            itemValue = $("#" + listItem[i]).val();
            if (itemValue == 'true') {
                itemValue = "Có";
            }
            else if (itemValue == 'false') {
                itemValue = "Không";
            }
        }

        //itemLabel
        if ($("#" + listItem[i]).is('label')) {

            itemLabel = $("#" + listItem[i]).parent().find("label:eq(0)").text();

        }
        else {

            itemLabel = $("#" + listItem[i]).parent().parent().find("label:eq(0)").text();
            //itemLabel = itemLabel.replace('(*)', '');
        }
        itemLabel = $.trim(itemLabel.replace('(*)', ''));

        itemLabel = $.trim(itemLabel.replace(':', ''));
        itemValue = $.trim(itemValue);
        contentTab += ''
                    + '<div style="width:' + widthcontent + '%;float:left">'
                        + '<label>'
                            + '<i class="fa fa-lg fa-caret-right" style="color:blue;margin-right:5px"></i>'
                            + '<span style="color:#808080; font-size: 12px;font-family: Tahoma, Verdana, Segoe, sans-serif;">' + itemLabel + ': </span>'
                        + '</label>'
                        + '<label>'
                            + '<span style=" font-size: 12px;font-weight: bold;font-family: Tahoma, Verdana, Segoe, sans-serif;">&nbsp;' + itemValue + '</span>'
                        + '</label>'
                    + '</div>';

    }
    return contentTab;
}

function getContentTab(currentTab, contentTab) {
    $("#" + currentTab).find(".ContentTab").remove();
    //pre contentTab;
    var contentFinal = "";

    contentFinal = '<div class="ContentTab" style="width:100%;float:left;clear:both;background-color:#FFF2CC"><div style="width:100%;float:left;clear:both;color:#478fca;font-size: 13px;font-weight: normal;"></div>';
    contentFinal += contentTab;
    //contentFinal += "</div>";
    $("#" + currentTab).prepend("" + contentFinal);
}

//========================================== CODE sua style cua kieu so ===========================================

function numberFormatCustom(obj) {
    //CODE sua style cua kieu so

    var className = obj.container[0].className;
    var isnumberFormar = false;
    if (className == "numberRule k-edit-cell" || className == "numberRule k-dirty-cell k-edit-cell") {
        isnumberFormar = true;
    }
    else if (className == "currencyRule k-edit-cell" || className == "currencyRule k-dirty-cell k-edit-cell") {
        isnumberFormar = true;
    }
    else if (className == "percentRule k-edit-cell" || className == "percentRule k-dirty-cell k-edit-cell") {
        isnumberFormar = true;
    }
    else if (className == "decimalRule k-edit-cell" || className == "decimalRule k-dirty-cell k-edit-cell") {
        isnumberFormar = true;
    }
    if (isnumberFormar == false) {
        return;
    }

    var tb;
    if (obj.container.find(".k-textbox").length > 0) {
        tb = obj.container.find(".k-textbox");
    }
    else if (obj.container.find(".k-numerictextbox span input:eq(1)").length > 0) {
        tb = obj.container.find(".k-numerictextbox span input:eq(1)");
        //remove span
        obj.container.find(".k-numerictextbox span span").remove();
        obj.container.find(".k-numerictextbox span").css({ "width": "100%" });
    }
    else if (obj.container.find(".text-box.single-line").length > 0) {
        tb = obj.container.find(".text-box.single-line");
        tb.removeClass("text-box single-line").addClass("k-textbox");
        tb.attr("type", "text");
        //kieu interger la kieu so luong

    }

    if (className == "numberRule k-edit-cell" || className == "numberRule k-dirty-cell k-edit-cell") {

        tb.autoNumeric('init', { aPad: false, vMax: '999999', mDec: 0 });
    }
    if (className == "currencyRule k-edit-cell" || className == "currencyRule k-dirty-cell k-edit-cell") {

        tb.autoNumeric('init', { aPad: false, vMax: '999999999999' });
    }
    if (className == "percentRule k-edit-cell" || className == "percentRule k-dirty-cell k-edit-cell") {
        tb.autoNumeric('init', { aPad: false, vMax: '100' });
    }
    if (className == "decimalRule k-edit-cell" || className == "decimalRule k-dirty-cell k-edit-cell") {
        tb.autoNumeric('init', { aPad: false, vMax: '999999', vMin: '0', mDec: 2 });
    }
    //tb.focus();
    if (tb != null) {
        tb.css({ "text-align": "right" });
        //number formart
        setTimeout(function () {
            tb.select();
        }, 300);
    }


}

//========================================== Lay ngay he thong ===========================================

function getDateSystem() {
    $.post(r + "/Home/GetSystemDate", {}, function (data) {
        if (data.success) {
            currentTimeSystem = data.data;
            return currentTimeSystem;
        }
        else {
            alertBox("Có lỗi", "", false, 3000);
            console.log(data.message);
        }
        hideLoading();
    });
}

//========================================== ngan viec nhan supmit nhieu lan ===========================================
function blockUIFromUser(isMark) {
    if (isMark) {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false })).ajaxStop($.unblockUI);
    }
    else {
        $(document).ajaxStart($.blockUI({ message: '<i class="fa fa-spinner fa-3x fa-lg fa-spin txt-color-blueDark"></i>', theme: false, overlayCSS: { backgroundColor: 'transparent' } })).ajaxStop($.unblockUI);
    }
}

