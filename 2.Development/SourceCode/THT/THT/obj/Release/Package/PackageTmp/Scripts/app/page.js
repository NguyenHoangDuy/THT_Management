var idPopup;
var eventHotKey = false;

//$(document).ready(function () {
//    //trang chu
//    $("#menu_Home").click(function () {        
//        resetMenu();
//        $(document).unbind("keypress");
//        $(document).unbind("keydown");
//        eventHotKey = false;
//        onLoadPage(r + "/Home/Partial");
//        hideLoading();
//    });
//    //phan quyen
//    $("#menu_Auth_Role").click(function () {
//        onLoadPage(r + "/Auth_Role/PartialRole");
//    });
//    //nguoi dung
//    $("#menu_Auth_User").click(function () {
//        onLoadPage(r + "/Auth_User/PartialUser");
//    });
//    //thong bao
//    $("#menu_Utilities_Announcement").click(function () {
//        onLoadPage(r + "/Utilities_Announcement/TreeView");
//    });  
//    //Lịch Nghỉ
//    $("#menu_Utilities_Holiday").click(function () {
//        onLoadPage(r + "/Utilities_Holiday/TreeView");
//    });
//    //Lý do
//    $("#menu_Utilities_Reason").click(function () {
//        onLoadPage(r + "/Utilities_Reason/PartialReason");
//    });
//    // Phân cấp Vùng miền
//    $("#menu_Utilities_Territory").click(function () {
//        onLoadPage(r + "/Utilities_Territory/PartialOthersTerritory");
//    });    
//    //Kho ấn phẩm
//    $("#menu_AD_ItemList").click(function () {
//        onLoadPage(r + "/AD_ItemList/PartialRole");
//    });
//    //Kho và đơn vị tính
//    $("#menu_AD_WHAndUnit").click(function () {
//        onLoadPage(r + "/AD_WHAndUnit/PartialRole");
//    });
//    //Tạo đơn đặt đặt hàng NCC
//    $("#menu_AD_MCCreateOrder").click(function () {
//        onLoadPage(r + "/AD_MCCreateOrder/PartialMCCreateOrder");
//    });
//    // Xử lý đơn đặt hàng
//    $("#menu_AD_DecaOrderProcessing").click(function () {
//        onLoadPage(r + "/AD_DecaOrderProcessing/PartialOrderProcessing");
//    });
//    $("#menu_AD_MCViewOrder").click(function () {
//        onLoadPage(r + "/AD_MCViewOrder/PartialAD_MCViewOrder");
//    });
//    //Mua hàng
//    $('#menu_AD_CreateOrder').click(function () {
//        onLoadPage(r + "/AD_CreateOrder/PartialDetail");
//    });
//    $('#menu_AD_Order').click(function () {
//        onLoadPage(r + "/AD_Order/PartialRole");
//    });
//    $('#menu_AD_Purchase').click(function () {
//        onLoadPage(r + "/AD_Purchase/PartialRole");
//    });
//    //Giao hang
//    $('#menu_AD_PickingList').click(function () {
//        onLoadPage(r + "/AD_PickingList/PartialPickingList");
//    });
//    //Báo cáo nhập xuất tồn kho bao bì túi hộp
//    $("#menu_AD_Rpt_StockInOut").click(function () {
//        onLoadPage(r + "/AD_Rpt_StockInOut/PartialStockInOutReport");
//    });
//    //Báo cáo theo dõi xuất bao bì túi hộp theo NCC
//    $("#menu_AD_Rpt_SalesByMerchant").click(function () {
//        onLoadPage(r + "/AD_Rpt_SalesByMerchant/PartialReportSalesByMerchant");
//    });
//    // Nhập liệu đơn hàng
//    $("#menu_CM_ImportData").click(function () {
//        onLoadPage(r + "/CM_ImportData/PartialOrder");
//        //window.open(r + "/CM_ImportData/PartialOrder", '_blank');
//    });
//    $("#menu_CM_CrossCheckOCM").click(function () {
//        onLoadPage(r + "/CM_CrossCheckOCM/PartialCheckOCM");
//    });
//    //Xác nhận tiền đẫ thu
//    $("#menu_CM_ConfirmCollectionAmt").click(function () {
//        onLoadPage(r + "/CM_ConfirmCollectionAmt/PartialCM_Confirm");
//    });
//    $("#menu_CM_CrossCheckMerchant").click(function () {
//        onLoadPage(r + "/CM_CrossCheckMerchant/PartialCheckMerchant");
//    });
//    // Xác nhận trả tiền nhà cung cấp
//    $("#menu_CM_ConfirmPayMerchant").click(function () {
//        onLoadPage(r + "/CM_ConfirmPayMerchant/PartialMerchant_Confirm");
//    });
//    //Đơn vị vận chuyển
//    $("#menu_LG_Transporter").click(function () {
//        onLoadPage(r + "/LG_Transporter/PartialTransporter");
//    });
//    $("#menu_LG_DLFee").click(function () {
//        onLoadPage(r + "/LG_DLFee/PartialDeliveryFee");
//    });
//    $("#menu_LG_Support_DLFee").click(function () {
//        onLoadPage(r + "/LG_Support_DLFee/PartialDeliveryDiscount");
//    });
//    $("#menu_LG_Promotion").click(function () {
//        onLoadPage(r + "/LG_Promotion/PartialDeliveryPromotion");
//    });
//    $("#menu_LG_Rule").click(function () {
//        onLoadPage(r + "/LG_Rule/PartialLG_Rule");
//    });
//    $("#menu_AD_Printer").click(function () {
//        onLoadPage(r + "/AD_Printer/PartialPrinter");
//    });
//    $("#menu_LG_Contract").click(function () {
//        onLoadPage(r + "/LG_Contract/PartialContract");
//    });   
//    $("#menu_LG_Fomular").click(function () {
//        onLoadPage(r + "/LG_Fomular/PartialLG_Fomular");
//    });
//    $("#menu_LG_Rpt_Promotion").click(function () {
//        onLoadPage(r + "/LG_Rpt_Promotion/PartialLG_Rpt_Promotion");
//    });
   
//    $("#menu_CM_MerchantInfo").click(function () {
//        onLoadPage(r + "/CM_MerchantInfo/PartialCM_MerchantInfo");
//    });
//    $("#menu_AR_CreateAR").click(function () {
//        onLoadPage(r + "/AR_CreateAR/PartialAR_CreateAR");
//    });
//    $("#menu_LG_Rpt_DLFee").click(function () {
//        onLoadPage(r + "/LG_Rpt_DLFee/PartialLG_Rpt_DLFee");
//    });
//    $("#menu_AP_CreateAP").click(function () {
//        onLoadPage(r + "/AP_CreateAP/PartialRole");
//    });

//    $("#menu_AP_ConfirmPayTransporter").click(function () {
//        onLoadPage(r + "/AP_ConfirmPayTransporter/PartialAP_ConfirmPayTransporter");
//    });
//    $("#menu_CM_HistoryImportData").click(function () {
//        onLoadPage(r + "/CM_HistoryImportData/PartialCM_HistoryImportData");
//    });
//    $("#menu_AR_ConfirmReceiverAmt").click(function () {
//        onLoadPage(r + "/AR_ConfirmReceiverAmt/PartialAR_ConfirmMerchant");
//    });
//    $("#menu_AccRpt_Dashboard").click(function () {
//        onLoadPage(r + "/AccRpt_Dashboard/PartialAccRpt_Dashboard");
//    });
//    $("#menu_AccRpt_AdjustDebt").click(function () {
//        onLoadPage(r + "/AccRpt_AdjustDebt/PartialAccRpt_AdjustDebt");
//    });

//        var url = localStorage['urlpage'] || '';
//        if (url) {
//            onLoadPage(url);
//        }
//        else {
//            onLoadPage(r + "/Home/Partial");
//        }
//});

    function onLoadPage(url) {
        $(document).unbind("keypress");
        $(document).unbind("keydown");
        eventHotKey = false;
        if (url != "" && url != null) {
            $.ajax({
                url: url,
                type: "get",
                beforeSend: function () { $("#divLoading").show(); },
                success: function (data) {
                
                    $("div.k-window").remove();
                    $("div#content").html(data);
                    localStorage['urlpage'] = url;
                    $("#divLoading").hide();
                },
           
                error: function (e) { }            
            });
        }
    }

    function resetMenu() {
        $("#menuLeft").find('li').removeClass('active').removeClass('open');
        $("#menuLeft").find('ul').css('display', 'none');
        $("#menuLeft").find('em').removeClass('fa-minus-square-o').addClass('fa-plus-square-o');
    }
    //function resetMenuHomePage() {
    //    $("#menuLeft").find('li').removeClass('active').removeClass('open');
    //    $("#menuLeft").find('ul').css('display', 'none');
    //    $("#menuLeft").find('em').removeClass('fa-minus-square-o').addClass('fa-plus-square-o');
    //}
    function hideMaskPopup(idMask) {
        eventHotKey = false;
        $(idMask).hide();
        $(idPopup).fadeOut();
    }