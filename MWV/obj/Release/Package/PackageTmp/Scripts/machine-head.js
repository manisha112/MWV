//$('.switch-wrapper').on('click', 'a[href="#switchbtnOn"]', function () {
//    alert("mani");
//    
//    //var planNo = $(this).attr("data-id");

//    //$.ajax(
//    //          {
//    //              cache: false,
//    //              type: "GET",
//    //              url: "/MachineHead/SaveProductionRunActualTime/" + planNo,
//    //              contentType: "application/text; charset=utf-8",
//    //              success: function (data) {

//    //              },
//    //              error: function (xhr, ajaxOptions, thrownError) {

//    //              }
//    //          });
//});

$('#machineControlPanel').on('click', 'a[href="#machineOff"]', function () {
    //$('#machineStopRemark').find('option:first').attr('selected', 'selected')
    $('#machineStopRemark').val('');
    $('#offStopTimeDay').find('option:first').attr('selected', 'selected')
    $('#offStopTimeMonth').find('option:first').attr('selected', 'selected')
    $('#offStopTimeYear').find('option:first').attr('selected', 'selected')
    $('#offStopTimeHour').find('option:first').attr('selected', 'selected')
    $('#offStartTimeDay').find('option:first').attr('selected', 'selected')
    $('#offStartTimeMonth').find('option:first').attr('selected', 'selected')
    $('#offStartTimeYear').find('option:first').attr('selected', 'selected')
    $('#offStartTimeHour').find('option:first').attr('selected', 'selected')
    $('#offStartTimeHourAmPm').find('option:first').attr('selected', 'selected')
    //  hide other panels
    if ($(this).hasClass('off')) {
        $('#machineOffPanel').show();
        $('html,body').animate({
            scrollTop: $('#machineOffPanel').position().top - 20
        }, 400);
    }


});
$('#machineControlPanel').on('click', 'a[href="#machineOn"]', function () {
    //$('#machineStartRemark').find('option:first').attr('selected', 'selected')
    $('#machineStartRemark').val('');
    $('#onStartTimeDay').find('option:first').attr('selected', 'selected')
    $('#onStartTimeMonth').find('option:first').attr('selected', 'selected')
    $('#onStartTimeYear').find('option:first').attr('selected', 'selected')
    $('#onStartTimeHour').find('option:first').attr('selected', 'selected')
    $('#onStartTimeHourAmPm').find('option:first').attr('selected', 'selected')
    $('#offStartTimeMonth').find('option:first').attr('selected', 'selected')
    $('#offStartTimeYear').find('option:first').attr('selected', 'selected')
    $('#offStartTimeHour').find('option:first').attr('selected', 'selected')
    $('#offStartTimeHourAmPm').find('option:first').attr('selected', 'selected')

    if ($(this).hasClass('off')) {
        $('#machineOnPanel').show();

        $('html,body').animate({
            scrollTop: $('#machineOnPanel').position().top - 20
        }, 400);
    }


});
$('.machine_confirm_panel .remark')
.on('focus', function () {
    if ($(this).val() === 'Type Remark') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Remark');
    }
});
$('#machineOffPanel').on('click', 'a[href="#closeMachineOffPanel"]', function () {
    $('#StopMachinceFromDate').hide();
    $('#StopMachinceTodate').hide();
    $('#machineOffPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('#machineOffPanel').on('click', 'a[href="#stopMachine"]', function () {
    debugger;
    var validateStartTme = $('#offStopTimeDay').val() + "/" + $('#offStopTimeMonth').val() + "/" + $('#offStopTimeYear').val() + " " + $('#offStopTimeHour').val() + " " +$('#offStopTimeHourAmPm').val();
    var stratTime = $('#offStartTimeDay').val() + "/" + $('#offStartTimeMonth').val() + "/" + $('#offStartTimeYear').val() + " " + $('#offStartTimeHour').val() + " " + $('#offStartTimeHourAmPm').val();
    var StopResult = CompareDate(validateStartTme, stratTime);
    var StartResult = CompareDate(stratTime);



    var fromdateTimeValidate = $('#offStopTimeDay').val() + "/" + $('#offStopTimeMonth option:selected').text() + "/" + $('#offStopTimeYear').val();
    var todateTimeValidate = $('#offStartTimeDay').val() + "/" + $('#offStartTimeMonth option:selected').text() + "/" + $('#offStartTimeYear').val();

    var fromdateTimeChk = $('#offStopTimeYear').val() + "/" + $('#offStopTimeMonth').val() + "/" + $('#offStopTimeDay').val() + " " + $('#offStopTimeHour').val();
    var todateTimeChk = $('#offStartTimeYear').val() + "/" + $('#offStartTimeMonth').val() + "/" + $('#offStartTimeDay').val() + " " + $('#offStartTimeHour').val();
    var fromdateTime = ValidateDate(fromdateTimeValidate);
    var todateTime = ValidateDate(todateTimeValidate);
   
    var dateCompare = CompareDate(fromdateTimeChk, todateTimeChk);
    $('#StopMachinceFromDate').hide();
    $('#StopMachinceTodate').hide();
    if (fromdateTime == false && todateTime == false) {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceTodate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please select a Valid stop date !");
        $('#StopMachinceTodate').html('Please select a Valid start date !');
    }
    else if ($('#offStartTimeDay').val() == "DD" || $('#offStartTimeMonth').val() == "MM" || $('#offStartTimeYear').val() == "YYYY") {
        $('#StopMachinceTodate').show();
        $('#StopMachinceTodate').html("<p class='error-msg'>Please select a Valid Start date !");
        if ($('#offStopTimeDay').val() == "DD" || $('#offStopTimeMonth').val() == "MM" || $('#offStopTimeYear').val() == "YYYY") {
            //$('#StopMachinceFromDate').show();
            $('#StopMachinceTodate').show();
            $('#StopMachinceTodate').html('Please select a Valid stop date !');

        }
    } else if ($('#offStopTimeDay').val() == null && $('#offStopTimeMonth').val() == null && $('#offStopTimeYear').val() == null && $('#offStartTimeDay').val() == null && $('#offStartTimeMonth').val() == null && $('#offStartTimeYear').val() == null) {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceTodate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please select a Valid stop date !");
        $('#StopMachinceTodate').html('Please select a Valid start date !');
    }
    else if ($('#offStopTimeDay').val() == null && $('#offStopTimeMonth').val() == null && $('#offStopTimeYear').val() == null) {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please select a Valid stop date !");
    }
    else if ($('#offStopTimeDay').val() == null || $('#offStopTimeMonth').val() == null || $('#offStopTimeYear').val() == null) {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please select a Valid stop date !");
    }
    else if ($('#offStartTimeDay').val() == null || $('#offStartTimeMonth').val() == null || $('#offStartTimeYear').val() == null) {
        $('#StopMachinceTodate').show();
        $('#StopMachinceTodate').html("<p class='error-msg'>Please select a Valid Start date !");
    }
    else if (fromdateTime == false) {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please select a Valid stop date !");
    }
    else if (todateTime == false) {
        $('#StopMachinceTodate').show();
        $('#StopMachinceTodate').html("<p class='error-msg'>Please select a Valid start date !");
    } else if (dateCompare == false) {

        $('#StopMachinceFromDate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Stop date should be greater than or equal to Start date !");
    }
    else if ($('#machineStopRemark').val().trim() == "") {
        $('#StopMachinceFromDate').show();
        $('#StopMachinceFromDate').html("<p class='error-msg'>Please Enter Remark !");

    }

    else {
        $('#StopMachinceTodate').hide();
        $('#StopMachinceFromDate').hide();
        var selectedOrder =
       {
           offStopDateTime: $('#offStopTimeDay').val() + "/" + $('#offStopTimeMonth').val() + "/" + $('#offStopTimeYear').val() + " " + $('#offStopTimeHour').val(),
           offStopTimeHourAmPm: $('#offStopTimeHourAmPm').val(),
           offStartDateTime: $('#offStartTimeDay').val() + "/" + $('#offStartTimeMonth').val() + "/" + $('#offStartTimeYear').val() + " " + $('#offStartTimeHour').val(),
           offStartTimeHourAmPm: $('#offStartTimeHourAmPm').val(),
           SelectedRemark: $("#machineStopRemark").val(),
       }
        //  alert($('#offStopTimeDay').val() + "-" + $('#offStopTimeMonth').val() + "-" + $('#offStopTimeYear').val() + " " + $('#offStopTimeHour').val() + ":" + "00:00:");
        $.ajax({
            url: "/MachineHead/SubmitMachineOffDetails/",
            data: selectedOrder,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                        .success(function (result) {

                        })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })

        $('#machineOffPanel').hide();
        $('#machineControlPanel a:first-child').removeClass('off').addClass('on');
        $('#machineControlPanel a:last-child').removeClass('on').addClass('off');
        $('html,body').animate({
            scrollTop: 0
        }, 400);

    }
});
$('#machineOnPanel').on('click', 'a[href="#closeMachineOnPanel"]', function () {
    $('#startMachinceDate').hide();
    $('#machineOnPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('#machineOnPanel').on('click', 'a[href="#startMachine"]', function () {
    
    var fromdateTimeValidate = $('#onStartTimeDay').val() + "/" + $('#onStartTimeMonth option:selected').text() + "/" + $('#onStartTimeYear').val();
    var fromdateTime = ValidateDate(fromdateTimeValidate);

    $('#startMachinceDate').hide();


    if ($('#onStartTimeDay').val() == "DD" || $('#onStartTimeMonth').val() == "MM" || $('#onStartTimeYear').val() == "YYYY") {
        $('#startMachinceDate').show();
        $('#startMachinceDate').html("<p class='error-msg'>Please select a Valid start date !");
    }
    else if ($('#onStartTimeDay').val() == null || $('#onStartTimeMonth').val() == null || $('#onStartTimeYear').val() == null) {
        $('#startMachinceDate').show();
        $('#startMachinceDate').html("<p class='error-msg'>Please select a Valid start date !");
    }
    else if (fromdateTime == false) {
        $('#startMachinceDate').show();
        $('#startMachinceDate').html("<p class='error-msg'>Please select a Valid start date !");
    }

    else if ($('#machineStartRemark').val().trim() == "") {
        $('#startMachinceDate').show();
        $('#startMachinceDate').html("<p class='error-msg'>Please Enter Remark !");

    }

    else {
        $('#startMachinceDate').hide();


        var selectedOrder =
   {
       onStartDateTime: $('#onStartTimeDay').val() + "/" + $('#onStartTimeMonth').val() + "/" + $('#onStartTimeYear').val() + " " + $('#onStartTimeHour').val(),
       onStartTimeHourAmPm: $('#onStartTimeHourAmPm').val(),
       SelectedRemark: $("#machineStartRemark").val(),
   }
        $.ajax({
            url: "/MachineHead/SubmitMachineOnDetails/",
            data: selectedOrder,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                        .success(function (result) {

                        })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })

        $('#machineOnPanel').hide();
        $('#machineControlPanel a:first-child').removeClass('on').addClass('off');
        $('#machineControlPanel a:last-child').removeClass('off').addClass('on');

        $('html,body').animate({
            scrollTop: 0
        }, 400);

    }
});

$('#offStopTimeDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#offStopTimeMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#offStopTimeYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#offStartTimeDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#offStartTimeMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#offStartTimeYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#onStartTimeDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#onStartTimeMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#onStartTimeYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('.production_plan').on('click', 'a[href="#seeDetails"]', function () {
    var $detailsPanel = $('#planDetails-' + $(this).attr('id'));
    var id = $(this).attr('data-id');
    var selectedDate =
  {
      selectedDateTime: $(this).attr('data-id'),
  }
    $.ajax({
        url: "/MachineHead/GetProPlanDetailsBydates/",
        data: selectedDate,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                        if ($detailsPanel.is(':hidden')) {
                            $('.production_plan_details').hide();
                            $detailsPanel.show();
                            $detailsPanel.html(result);
                            $('html,body').animate({
                                scrollTop: $detailsPanel.position().top
                            }, 400);
                        }

                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })

});
$('.production_plan').on('click', 'a[href="#closePlanDetails"]', function () {
    $(this).closest('.production_plan_details').hide();


});
$('.production_plan').on('change', 'select', function () {
    $(this).next('.take_action_panel').show();
});
$('.take_action_panel').on('click', 'a[href="#confirmAssignment"]', function () {
    $(this).closest('.take_action_panel').hide();


});
$('.take_action_panel').on('click', 'a[href="#closeTakeAction"]', function () {

    $(this).closest('.take_action_panel').hide();


});



$(function () {
    $(document).on('click', 'a', function (evt) {
        var href = $(this).attr('href');

        if (href.indexOf(document.domain) > -1 || href.indexOf(':') === -1) {
            history.pushState({}, '', '');
        }

        return false;
    });
});

//Function Compare Date
function CompareDate(fromdtChk, todtChk) {

    // if (Date.parse(fromdate) > Date.parse(todate)) {
    if (Date.parse(fromdtChk) > Date.parse(todtChk)) {
        return false;
    }
    else {
        return true;
    }
}

function CompareTime(fromdtChk, todtChk,starttime,endtime) {
    if (Date.parse(fromdtChk) == Date.parse(todtChk)) {


        return false;
    }

}

 
 

function CheckRequestDeliveryDate(fromdtChk) {

    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    var FinalDate = yyyy + "/" + mm + "/" + dd

    if (Date.parse(fromdtChk) < Date.parse(FinalDate)) {
        return false;
    }
    else {

        return true;
    }


}

$(document).ready(function () {

    $.ajax({
        cache: false,
        type: "POST",
        url: "/Agent/GetCurrentyear/",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var dafulttext = "YYYY";
            //For Order SearchoffStartTimeYear
            $("#offStopTimeYear").empty();
            $("#offStopTimeYear").append($('<option>' + dafulttext + '</option>'));
            $("#offStopTimeYear").append($('<option value=' + data[0] + '>' + data[0] + '</option>'));
            $("#offStopTimeYear").append($('<option value=' + data[1] + '>' + data[1] + '</option>'));

        


        }

    });

});

$(document).ready(function () {

    $('#welcomePanel').removeClass('six_icons').addClass('four_icons');
    $('#welcomePanel').find('.tickets').remove();
    $('#welcomePanel').find('.schedule').remove();
});
//Change Password Close
$('#tempbodySection').on('click', 'a[href="#close-panel"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetMHDashboard();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetMHDashboard();
});

//$('#tempbodySection').on('click', 'a[href="#close-panel"]', function () {
//    $("#tempbodySection").find('#ChangePasswordinner').hide();
//    $("#tempbodySection").find("#changePasswordSuccess").hide();
//    GetMHDashboard();
//});
$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetMHDashboard();
});

$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
    event.stopImmediatePropagation();
    
    GetMHDashboard();
});

function GetMHDashboard() {
    $.ajax(
                 {
                     cache: false,
                     type: "POST",
                     url: "/MachineHead/BackToHomeMH/",
                     contentType: "application/json; charset=utf-8",
                     success: function (data) {
                         
                         $('#tempbodySection').show();
                         $('#tempbodySection').html(data);

                     },
                     error: function (xhr, ajaxOptions, thrownError) {

                     }
                 });



}