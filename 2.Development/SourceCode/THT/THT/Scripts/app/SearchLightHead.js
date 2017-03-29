var numHeight = 230;
var keyAction;
var indexTabstripActive = -1;
var contentTab;

$(document).ready(function () {

    $("ul#menuLeft").find('#ul_root_3').addClass('open');
    $("ul#menuLeft").find('#ul_root_3').css('display', 'block');
    $("#menu_SearchLightHead").parent().addClass('active');

    resetMenu();
    $("#menu_SearchLightHead").parent().addClass('active');

    $("#formPopup").find('fieldset:eq(0)').hide();
    $('#dvRodSize').hide();

    var cboRodSize = $("#RodSize");
    var i = cboRodSize[0].selectedIndex;
    var valueText = $("#RodSize  option:selected").text();
    var id = $("#RodSize  option:selected").val();
    
    if (id == "" || locdau(valueText) == "khong")
        $('#txtValue').val("0");
    if (locdau(valueText) != "khong" && id != "") {
        $('#txtValue').val("");
        $('#dvRodSize').show();
    }

    $("#TKGroup").select2();
    $("#s2id_TKGroup").css('width', '250px');

    $("#Head").select2();
    $("#s2id_Head").css('width', '250px');

    $("#Length").select2();
    $("#s2id_Length").css('width', '250px');

    $("#Thickness").select2();
    $("#s2id_Thickness").css('width', '250px');

    $("#BottomDiameter").select2();
    $("#s2id_BottomDiameter").css('width', '250px');

    $("#TopDiameter").select2();
    $("#s2id_TopDiameter").css('width', '250px');

    $("#Size").select2();
    $("#s2id_Size").css('width', '250px');

    $("#Empire").select2();
    $("#s2id_Empire").css('width', '250px');

    $("#BoltCenter").select2();
    $("#s2id_BoltCenter").css('width', '250px');

    $("#BoltSize").select2();
    $("#s2id_BoltSize").css('width', '250px');

    $("#EpMo").select2();
    $("#s2id_EpMo").css('width', '250px');

    $("#HeadHeight").select2();
    $("#s2id_HeadHeight").css('width', '250px');

    $("#HeadSize").select2();
    $("#s2id_HeadSize").css('width', '250px');

    $("#TendonSize").select2();
    $("#s2id_TendonSize").css('width', '250px');

    $("#TendonNumber").select2();
    $("#s2id_TendonNumber").css('width', '250px');

    $("#TubeSize").select2();
    $("#s2id_TubeSize").css('width', '250px');

    $("#SilverLining").select2();
    $("#s2id_SilverLining").css('width', '250px');

    $("#RodSize").select2();
    $("#s2id_RodSize").css('width', '250px');

    $("#Hill").select2();
    $("#s2id_Hill").css('width', '250px');

    $("#Hinge").select2();
    $("#s2id_Hinge").css('width', '250px');

    $("#MuzzleHead").select2();
    $("#s2id_MuzzleHead").css('width', '250px');

    $("#PlinthType").select2();
    $("#s2id_PlinthType").css('width', '250px');


    $('#popupConfirm').kendoWindow({
        width: "500px",
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
            TKGroup: {
                required: true,
            },
            Head: {
                required: true,
            },
            Length: {
                required: true,
                // alphanumeric: true
            },
            Thickness: {
                required: true,
            },
            BottomDiameter: {
                required: true,
            },
            TopDiameter: {
                required: true,
            },
            Size: {
                required: true,
            },
            Empire: {
                required: true,
            },
            BoltCenter: {
                required: true,
            },
            BoltSize: {
                required: true,
            },
            EpMo: {
                required: true,
            },
            HeadHeight: {
                required: true,
            },
            HeadSize: {
                required: true,
            },
            TendonSize: {
                required: true,
            },
            TendonNumber: {
                required: true,
            },
            TubeSize: {
                required: true,
            },
            RodSize: {
                required: true,
            },
            Hill: {
                required: true,
            },
            Hinge: {
                required: true,
            },
            MuzzleHead: {
                required: true,
            },
            PlinthType: {
                required: true,
            },
            SilverLining: {
                required: true,
            },
        },

        // Messages for form validation required: "Thông tin bắt buộc",
        messages: {
            TKGroup: {
                required: "Thông tin bắt buộc",
            },
            Head: {
                required: "Thông tin bắt buộc",
            },
            Length: {
                required: "Thông tin bắt buộc",
                // alphanumeric: true
            },
            Thickness: {
                required: "Thông tin bắt buộc",
            },
            BottomDiameter: {
                required: "Thông tin bắt buộc",
            },
            TopDiameter: {
                required: "Thông tin bắt buộc",
            },
            Size: {
                required: "Thông tin bắt buộc",
            },
            Empire: {
                required: "Thông tin bắt buộc",
            },
            BoltCenter: {
                required: "Thông tin bắt buộc",
            },
            BoltSize: {
                required: "Thông tin bắt buộc",
            },
            EpMo: {
                required: "Thông tin bắt buộc",
            },
            HeadHeight: {
                required: "Thông tin bắt buộc",
            },
            HeadSize: {
                required: "Thông tin bắt buộc",
            },
            TendonSize: {
                required: "Thông tin bắt buộc",
            },
            TendonNumber: {
                required: "Thông tin bắt buộc",
            },
            TubeSize: {
                required: "Thông tin bắt buộc",
            },
            RodSize: {
                required: "Thông tin bắt buộc",
            },
            Hill: {
                required: "Thông tin bắt buộc",
            },
            Hinge: {
                required: "Thông tin bắt buộc",
            },
            MuzzleHead: {
                required: "Thông tin bắt buộc",
            },
            PlinthType: {
                required: "Thông tin bắt buộc",
            },
            SilverLining: {
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
                        $("#formPopup").find('fieldset:eq(0)').show();
                        $('#FinalCode').val(data.FinalCode);
                        $('#Description').val(data.Description);
                        $('#Parameters').val(data.Parameters);
                        $('#Weight').val(data.Weight);

                    }
                    else {
                        $("#divMaskPopup").show();
                        var popup = $('#popupConfirm').data("kendoWindow");
                        popup.center().open();
                        $(".k-window span.k-i-close").click(function () {
                            eventHotKey = false;
                            $("#divMaskPopup").hide();
                        });
                        //alertBox("Báo lỗi! ", data.message, false, 3000);
                        //$("#loading").addClass('hide');
                        //console.log(data.message);
                    }
                }
            });
            return false;
        }
    });
});


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



function Confirm(key, obj) {
        if (key == 1) {
            $('#popupConfirm').data("kendoWindow").close();
            $.post(r + "/SearchLightHead/LightHead_SendMail", function (data) {
                if (data.success) {
                    alertBox("Thành công!", " Gửi mail thành công", true, 3000);
                }
                else {
                    alertBox("Báo lỗi! ", data.message, false, 3000);
                }
            });
        }
        else {
            $('#popupConfirm').data("kendoWindow").close();
            return false;
        }
    
}