
$(document).ready(function () {
    //resetMenu();
    $("ul#menuLeft").find('li:eq(1)').addClass('open');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2').css('display', 'block');
    $("ul#menuLeft").find('li:eq(1) ul#ul_root_2 ul#ul_item_2').css('display', 'block');
 
    $("#menu_Utilities_Territory").parent().addClass('active');

    $("#txtTerritoryIDCountry").keypress(function (e) {

        if (e.keyCode == 13) {
            doSearchCountry();
            return false;
        }
    });
    $("#txtTerritoryIDRegion").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchRegion();
            return false;
        }
    });
    $("#txtTerritoryIDProvince").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchProvince();
            return false;
        }
    });
    $("#txtTerritoryIDDistrict").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchDistrict();
            return false;
        }
    });


    $("#txtTerritoryIDRegionParent").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchRegion();
            return false;
        }
    });
    $("#txtTerritoryIDProvinceParent").keypress(function (e) {
        if (e.keyCode == 13) {

            doSearchProvince();
            return false;
        }
    });
    $("#txtTerritoryIDDistrictParent").keypress(function (e) {

        if (e.keyCode == 13) {

            doSearchDistrict();
            return false;
        }
    });
});
//filter Country

function doSearchCountry() {
    var grid = $("#gridCountry").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDCountry").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    grid.dataSource.filter(filter);
}

function doSearchRegion() {
    
    var grid = $("#gridRegion").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDRegion").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDRegionParent").val();
    if (text) {
       
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}

function doSearchProvince() {
    
    var grid = $("#gridProvince").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDProvince").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDProvinceParent").val();
    if (text) {
      
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}

function doSearchDistrict() {
    
    var grid = $("#gridDistrict").data("kendoGrid");
    var filter = { logic: "and", filters: [] };

    var text = $("#txtTerritoryIDDistrict").val();
    if (text) {
        var filterOr = { logic: "or", filters: [] };
        filterOr.filters.push({ field: "TerritoryID", operator: "contains", value: text });
        filterOr.filters.push({ field: "TerritoryName", operator: "contains", value: text });

        filter.filters.push(filterOr);
    }
    text = $("#txtTerritoryIDDistrictParent").val();
    if (text) {
        filter.filters.push({ field: "ParentName", operator: "contains", value: text });
    }
    grid.dataSource.filter(filter);
}


//===========================================Databound ===========================================

function onDataboundCountry(e) {
   
    var grid = $("#gridCountry").data("kendoGrid");
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
}

function onDataboundRegion(e) {
   
    var grid = $("#gridRegion").data("kendoGrid");
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
}

function onDataboundProvince(e) {
  
    var grid = $("#gridProvince").data("kendoGrid");
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
}

function onDataboundDistrict(e) {
    resizegridDistrict();
    var grid = $("#gridDistrict").data("kendoGrid");
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
}


//===========================================Request===========================================

function onRequestStart(e) {
    blockUIFromUser(false);
    //$("#divLoading").show();
}

function onRequestEnd(e) {
    
    if (e.type == "update" || e.type == "create") {
        if (e.response.Errors == null) {
            alertBox("Thành công! ", "Lưu thành công", "", true, 3000);
        }
        else {
            alertBox("Báo lỗi", e.response.Errors.er.errors[0], false, 3000);
        }
    }

    else {
        arrExport = new Array();
        for (var i = 0; i < e.sender._view.length; i++) {
            var value = e.sender._view[i];
            arrExport.push(value.TerritoryID);
        }
    }
    
    //$("#divLoading").hide();
}

function resizegridCountry() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridCountry').offset().top);
    var gridElement = $("#gridCountry"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 250);
}
$('#btnQuocGia').click(function () {
    resizegridCountry();
});

function resizegridRegion() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridRegion').offset().top);
    var gridElement = $("#gridRegion"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 250);
}
$('#btnVung').click(function () {
    resizegridRegion();
});

function resizegridProvince() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridProvince').offset().top);
    var gridElement = $("#gridProvince"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 250);
}
$('#btnTinh').click(function () {
    resizegridProvince();
});
function resizegridDistrict() {
    var offsetbottom = parseInt($(window).height()) - parseInt($('#gridDistrict').offset().top);
    var gridElement = $("#gridDistrict"),
    dataArea = gridElement.find(".k-grid-content"),
    otherElements = gridElement.children().not(".k-grid-content"),
    otherElementsHeight = 0;
    otherElements.each(function () {
        otherElementsHeight += $(this).outerHeight();
    });
    dataArea.height(offsetbottom - otherElementsHeight - 1);
}
