
$('.vehicle_schedule').on('click', 'a[href="#seeScheduleDetails"]', function () {
    $('#dispatchCompleted').hide();
    // $('#seeScheduleDetails').find('option:first').attr('selected', 'selected')
    var $detailsPanel = $('#vehicleDetailsMst-' + $(this).attr('id'));
    var $detailsPanel2 = $('#vehicleDetailsChild-' + $(this).attr('id'));
    var id = $(this).attr('data-id');

    var text = $(this).attr('id');
    var textvalue = {
        DaySelectedValue: text,
    };

    $.ajax({
        url: "/Dispatch/TodaysVehiclesDDLbyStatus/",
        data: textvalue,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                   .success(function (result) {
                       if ($detailsPanel.is(':hidden')) {
                           $('.vehicle_schedule_details').hide();
                           if (textvalue == "Today") {
                               $('#vehicleDetails-Today').show();
                           } else
                               if (textvalue == "Tomorrow") {
                                   $('#vehicleDetails-Tomorrow').show();
                               } else
                                   if (textvalue == "NextDay") {
                                       $('#vehicleDetails-NextDay').show();
                                   }

                           $detailsPanel.show();
                           //  $detailsPanel2.show();
                           $detailsPanel.html(result);
                           //$('html,body').animate({
                           //    scrollTop: $detailsPanel.position().top
                           //}, 400);
                       }

                   })
                  .error(function (xhr, ajaxOptions, thrownError) {
                  })
    $.ajaxSetup({ cache: false });
    if (text == "Today") {
        //   quickViewRecentOrders
        var datatosend =
        {
            VehicleScheduleDate: $(this).attr('data-id'),
            SelectedValue: "Today",
        }

        $.ajax({
            url: "/Dispatch/VehicleScheduleDetailsToday/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                        .success(function (result) {

                            $('.vehicle_schedule_details').hide();
                            $('#vehicleDetails-Today').show();
                            //  $detailsPanel.show();
                            $('#vehicleDetailsMst-Today').show();
                            $('#vehicleDetailsChild-Today').html(result);
                            //$detailsPanel2.html(result);
                            //$detailsPanel.html(result);
                            $('html,body').animate({
                                scrollTop: $('#vehicleDetails-Today').position().top
                            }, 400);

                        })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })
    } else
        if (text == "Tomorrow") {

            var datatosend =
      {
          VehicleScheduleDate: $(this).attr('data-id'),
          SelectedValue: "Tomorrow",
      }
            $.ajax({
                url: "/Dispatch/VehicleScheduleDetailsByTomorrow/",
                data: datatosend,
                contentType: "application/html; charset=utf-8",
                type: "GET",
                dataType: 'html',
                context: document.body,
            })
                            .success(function (result) {
                                $('.vehicle_schedule_details').hide();
                                $('#vehicleDetails-Tomorrow').show();
                                // $detailsPanel.show();
                                //alert(result)
                                $('#vehicleDetailsMst-Tomorrow').show();
                                $('#vehicleDetailsChild-Tomorrow').html(result);
                                //  $detailsPanel2.html(result);
                                //$detailsPanel.html(result);
                                $('html,body').animate({
                                    scrollTop: $('#vehicleDetails-Tomorrow').position().top
                                }, 400);
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })
        } else
            if (text == "NextDay") {
                var datatosend =
                      {
                          VehicleScheduleDate: $(this).attr('data-id'),
                          SelectedValue: "NextDay",
                      }
                $.ajax({
                    url: "/Dispatch/VehicleScheduleDetailsByNextDay/",
                    data: datatosend,
                    contentType: "application/html; charset=utf-8",
                    type: "GET",
                    dataType: 'html',
                    context: document.body,
                })
                                .success(function (result) {
                                    $('.vehicle_schedule_details').hide();
                                    $('#vehicleDetails-NextDay').show();
                                    $('#vehicleDetailsMst-NextDay').show();
                                    //alert(result)
                                    $('#vehicleDetailsChild-NextDay').html(result);
                                    //  $detailsPanel2.html(result);
                                    //$detailsPanel.html(result);
                                  
                                    $('html,body').animate({
                                        scrollTop: $('#vehicleDetails-NextDay').position().top
                                    }, 400);
                                })
                               .error(function (xhr, ajaxOptions, thrownError) {
                               })
            }

});
$('.vehicle_schedule').on('click', 'a[href="#closeScheduleDetails"]', function () {
    //$('#vehiclesSearchResults').hide();
    //alert("dsd");
    $(this).closest('.vehicle_schedule_details').hide();


});
$('.vehicle_schedule').on('click', 'a[href="#closeDispatchDetails"]', function () {
    $(this).closest('.dispatch_details').hide();
    $(this).closest('.dispatch_details').prev('.vehicle_schedule_details').show();
    //$(this).closest('#vehicleDetails-Today').hide();
    $('#vehicleDetails-Today').hide();
});
// on the partial view _VehicleDispatchDetails
//$('.vehicle_schedule').on('click', 'a[href="#dispatchComplete"]', function () {
//    $(this).next('.dispatch_complete_attention').show();

//    $('html,body').animate({
//        scrollTop: $(window).scrollTop() + 175
//    });

//    
//});
//$('.dispatch_complete_attention').on('click', 'a[href="#closeAttention"]', function () {
//    $(this).closest('.take_action_panel').hide();

//    
//});
//$('.dispatch_complete_attention').on('click', 'a[href="#confirmComplete"]', function () {
//    var $detailsPanel = $(this).closest('.dispatch_details');

//    $(this).closest('.take_action_panel').hide();
//    $detailsPanel.hide();
//    $detailsPanel.prev('.vehicle_schedule_details').show();

//    $('html,body').animate({
//        scrollTop: $detailsPanel.position().top
//    }, 400);

//    
//});
//this change event i used in VechileScheduleDetais partial view
//$('.vehicle_schedule').on('change', 'select', function () {
//    var currentVal = $(this).val();

//    if (currentVal === 'show-arrived') {
//        $(this).next('.vehicle_results').find('li.arrived').show();
//        $(this).next('.vehicle_results').find('li.not_arrived').hide();
//        $(this).next('.vehicle_results').show();
//    } else if (currentVal === 'show-departed') {
//        $(this).next('.vehicle_results').find('li.not_departed').hide();
//        $(this).next('.vehicle_results').show();
//    } else if (currentVal === 'all-vehicles' || currentVal === 'show-in-queue') {
//        $(this).next('.vehicle_results').find('li').show();
//        $(this).next('.vehicle_results').show();
//    }
//});

$('.take_action_panel').on('click', 'a[href="#confirmAssignment"]', function () {
   
    $(this).closest('.take_action_panel').hide();
});
$('.take_action_panel').on('click', 'a[href="#closeTakeAction"]', function () {
    $(this).closest('.take_action_panel').hide();
});
$('#searchVehicles').on('change', 'select.search_by', function () {

    if ($(this).val() === 'agent-name') {
        ShowDateTime();
       // clrDates();
        $('#ErrorMsgInvalidToDate').html('');
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ProductionPlannerErrors').html('');
        $("#vehiclesSearchResults").hide();
        $("#vehiclessearchresultsMst").hide();
        $('#searchByProductCode').hide();
        $('#searchByVehicleNo').hide();
        $('#searchByName').show();
    } else if ($(this).val() === 'product-code') {
        ShowDateTime();
       // clrDates();
        $('#ErrorMsgInvalidToDate').html('');
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ProductionPlannerErrors').html('');
        $("#vehiclesSearchResults").hide();
        $("#vehiclessearchresultsMst").hide();
        $('#searchByProductCode').show();
        $('#searchByVehicleNo').hide();
        $('#searchByName').hide();
    } else if ($(this).val() === 'vehicle-no') {
        RedreshDateTime();
      //  clrDates();
        $('#ErrorMsgInvalidToDate').html('');
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ProductionPlannerErrors').html('');
        $("#vehiclesSearchResults").hide();
        $("#vehiclessearchresultsMst").hide();
        $('#searchByProductCode').hide();
        $('#searchByVehicleNo').show();
        $('#searchByName').hide();
    }
});
function RedreshDateTime() {
    $('#searchVehiclesFromDay').hide();
    $('#searchVehiclesFromMonth').hide();
    $('#searchVehiclesFromYear').hide();
    $('#searchVehiclesToDay').hide();
    $('#searchVehiclesToMonth').hide();
    $('#searchVehiclesToYear').hide();
    $('.form-label').hide();
}
function ShowDateTime() {
    $('#searchVehiclesFromDay').show();
    $('#searchVehiclesFromMonth').show();
    $('#searchVehiclesFromYear').show();
    $('#searchVehiclesToDay').show();
    $('#searchVehiclesToMonth').show();
    $('#searchVehiclesToYear').show();
    $('.form-label').show();
}
$('#searchVehicles').on('click', 'a[href="#searchVehicles"]', function () {

    // alert("search vehicles is clicked");
    // make an ajax call to get the search result
    var fromdt = $('#searchVehiclesFromDay').val() + "/" + $('#searchVehiclesFromMonth').val() + "/" + $('#searchVehiclesFromYear').val();
    var todt = $('#searchVehiclesToDay').val() + "/" + $('#searchVehiclesToMonth').val() + "/" + $('#searchVehiclesToYear').val();
    //if ($('#searchVehiclesFromDay').val() == "DD" || $('#searchVehiclesFromMonth').val() == "MM" || $('#searchVehiclesFromYear').val() == "YYYY") {
    var fromDate = ValidateDate(fromdt);
    var toDate = ValidateDate(todt);
    function CallbleAjax() {
        var searchBy_String = $('#searchVehicles select.search_by').val();
        // alert(searchBy_String);
        var search_String;
        if (searchBy_String == 'agent-name') {
            search_String = $('#AgentList').val();

        }
        if (searchBy_String == 'product-code') {
            search_String = $('#ProductCodeList').val();

        }
        if (searchBy_String == 'vehicle-no') {
            search_String = $('#searchByVehicleNo .text-input').val();

        }

        //alert(search_String);

        //alert(from_Date, to_Date);
        var datatosend = {
            searchByString: searchBy_String,
            searchString: search_String,
            fromDate: $('#searchVehiclesFromDay').val() + "-" + $('#searchVehiclesFromMonth').val() + "-" + $('#searchVehiclesFromYear').val(),
            toDate: $('#searchVehiclesToDay').val() + "-" + $('#searchVehiclesToMonth').val() + "-" + $('#searchVehiclesToYear').val(),
        }
        var selectedText = {
            selectedTextValue: "Search",
            searchByString: $('#searchVehicles select.search_by').val(),
        }
        $.ajax({
            url: "Dispatch/TodaysVehiclesDDLbyStatus/",
            data: selectedText,
            type: "POST",
            dataType: 'html',
            context: document.body,
        })
                .success(function (result) {
                    //alert("display in partial view");
                    $('#vehiclesSearchResults').show();
                    $('#vehiclessearchresultsMst').show();
                    $('#vehiclessearchresultsMst').html(result);
                })
               .error(function (xhr, ajaxOptions, thrownError) {
               })

        $.ajaxSetup({ cache: false });
        $.ajax({
            url: "Dispatch/VehicleSearchResults/",
            data: datatosend,
            type: "POST",
            dataType: 'html',
            context: document.body,
        })
                         .success(function (result) {

                             //  $('#vehiclesSearchResults').show();
                             $('#vehiclessearchresultsChild').show();
                             $('#vehiclessearchresultsChild').html(result);
                             $('#vehiclesSearchResults').show();

                             $('html,body').animate({
                                 scrollTop: $('#vehiclesSearchResults').position().top - 30
                             }, 400);
                         })
                        .error(function (xhr, ajaxOptions, thrownError) {
                        })

       
    }

    var searchBy_String = $('#searchVehicles select.search_by').val();

    if (searchBy_String == "agent-name")
        $('#ErrorMsgInvalidToDate').html('');
    $('#ErrorMsgInvalidFromDate').html('');
    $('#ProductionPlannerErrors').html('');
    $("#vehiclesSearchResults").hide();
    $("#vehiclessearchresultsMst").hide();
    if ($("#AgentList option:selected").text() == "Select Agent Name ") {
        $('#ProductionPlannerErrors').html('');
        $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Select Agent Name !</p>");
        $("#vehiclesSearchResults").hide();
        $("#vehiclessearchresultsMst").hide();
    }

    else {
        $('#ErrorMsgInvalidToDate').html('');
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ProductionPlannerErrors').html('');

        if ($('#searchVehiclesFromDay').val() == "DD" && $('#searchVehiclesFromMonth').val() == "MM" && $('#searchVehiclesFromYear').val() == "YYYY"
&& $('#searchVehiclesToDay').val() == "DD" && $('#searchVehiclesToMonth').val() == "MM" && $('#searchVehiclesToYear').val() == "YYYY") {

            $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
            $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
            $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();

        } else if ($('#searchVehiclesFromDay').val() == "DD" || $('#searchVehiclesFromMonth').val() == "MM" || $('#searchVehiclesFromYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
            $("#vehiclesSearchResults").hide();
            $('#ErrorMsgInvalidToDate').html(''); $("#vehiclessearchresultsMst").hide();
            if ($('#searchVehiclesToDay').val() == "DD" || $('#searchVehiclesToMonth').val() == "MM" || $('#searchVehiclesToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').html('');
                $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
                $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();
            }
        }


        else if ($('#searchVehiclesToDay').val() == "DD" || $('#searchVehiclesToMonth').val() == "MM" || $('#searchVehiclesToYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').html('');
            $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
            $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();
            if (fromDate == false) {
                $('#ErrorMsgInvalidToDate').html('');
                $('#vehiclessearchresultsMst').hide();
                $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
                $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();
                //  $('#ErrorMsgInvalidToDate').show();s
            }
        } else if (toDate == false && fromDate == false) {
            $('#vehiclessearchresultsMst').hide(); $("#vehiclessearchresultsMst").hide();
            $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date");
            $('#ErrorMsgInvalidToDate').html('Please select a Valid To date!');
        } else if (toDate == false) {
            $('#vehiclessearchresultsMst').hide();
            $('#ErrorMsgInvalidFromDate').hide(); $("#vehiclessearchresultsMst").hide();
            $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");


        } else if (fromDate == false) {
            $('#vehiclessearchresultsMst').hide();
            $('#ErrorMsgInvalidToDate').html(''); $("#vehiclessearchresultsMst").hide();
            $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date");
        }
        else {
            //$('#ErrorMsgInvalidToDate').hide();
            //$('#ErrorMsgInvalidFromDate').hide();
            //if (fromdt > todt) {
            //    $('#vehiclessearchresultsMst').hide();
            //    $('#ProductionPlannerErrors').html('');
            //    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
            //}
            //else {
            //    $('#ErrorMsgInvalidToDate').hide();
            //    $('#ErrorMsgInvalidFromDate').hide();
            //    $('#ProductionPlannerErrors').html('');
            //    CallbleAjax();

            //}

            $('#ErrorMsgInvalidFromDate').html('');
            $('#ErrorMsgInvalidToDate').html('');//  
            var fromdt = $('#searchVehiclesFromYear').val() + "/" + $('#searchVehiclesFromMonth').val() + "/" + $('#searchVehiclesFromDay').val();
            var todt = $('#searchVehiclesToYear').val() + "/" + $('#searchVehiclesToMonth').val() + "/" + $('#searchVehiclesToDay').val();

            var dateCompareResult = CompareDate(fromdt, todt);
            if (dateCompareResult != true) {
                $('#vehiclesSearchResults').hide();
                $('#vehiclessearchresultsMst').hide(); //vehiclesSearchResults
                $("#vehiclessearchresultsChild").hide();
                $('#ProductionPlannerErrors').html('');
                $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
            }
            else {

                $('#ErrorMsgInvalidFromDate').html('');
                $('#ErrorMsgInvalidToDate').html('');
                $('#ProductionPlannerErrors').html('');
                CallbleAjax();



            }



        }
    }
    if (searchBy_String == "product-code") {
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ErrorMsgInvalidToDate').html('');
        $('#ProductionPlannerErrors').html('');
        if ($("#ProductCodeList option:selected").text() == "Select by ProductCode  ") {
            $('#vehiclesSearchResults').hide();
            $("#vehiclessearchresultsMst").hide();
            $('#ProductionPlannerErrors').html('');
            $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Select Product Code !</p>");
           
        }
        else {

            $('#ErrorMsgInvalidFromDate').html('');
            $('#ErrorMsgInvalidToDate').html('');
            $('#ProductionPlannerErrors').html('');

            if ($('#searchVehiclesFromDay').val() == "DD" && $('#searchVehiclesFromMonth').val() == "MM" && $('#searchVehiclesFromYear').val() == "YYYY"
   && $('#searchVehiclesToDay').val() == "DD" && $('#searchVehiclesToMonth').val() == "MM" && $('#searchVehiclesToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
                $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
                $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();

            } else if ($('#searchVehiclesFromDay').val() == "DD" || $('#searchVehiclesFromMonth').val() == "MM" || $('#searchVehiclesFromYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
                $("#vehiclesSearchResults").hide();
                $('#ErrorMsgInvalidToDate').html('');
                if ($('#searchVehiclesToDay').val() == "DD" || $('#searchVehiclesToMonth').val() == "MM" || $('#searchVehiclesToYear').val() == "YYYY") {
                    $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
                    $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();
                }
            }
            else if ($('#searchVehiclesToDay').val() == "DD" || $('#searchVehiclesToMonth').val() == "MM" || $('#searchVehiclesToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').html(''); $("#vehiclessearchresultsMst").hide();
                $("#vehiclesSearchResults").hide();
                $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
                if (fromDate == false) {
                    $('#vehiclessearchresultsMst').hide();
                    $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date");
                    //  $('#ErrorMsgInvalidToDate').show();s
                }
            } else if (toDate == false && fromDate == false) {
                $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
                $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");
                $("#vehiclesSearchResults").hide(); $("#vehiclessearchresultsMst").hide();

            } else if (toDate == false) {
                $('#vehiclessearchresultsMst').hide(); $("#vehiclessearchresultsMst").hide();
                $('#ErrorMsgInvalidFromDate').html('');
                $('#ErrorMsgInvalidToDate').html("<p class='error-msg'>Please select a Valid To date!</p>");

            } else if (fromDate == false) {
                $('#vehiclessearchresultsMst').hide(); $("#vehiclessearchresultsMst").hide();
                $('#ErrorMsgInvalidToDate').html('');
                $('#ErrorMsgInvalidFromDate').html("<p class='error-msg'>Please select a Valid From date!</p>");
            }
            else {
                //$('#ErrorMsgInvalidToDate').hide();
                //$('#ErrorMsgInvalidFromDate').hide();
                //if (fromdt > todt) {
                //    $('#vehiclessearchresultsMst').hide();
                //    $('#ProductionPlannerErrors').html('');
                //    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
                //}
                //else {
                //    $('#ErrorMsgInvalidToDate').hide();
                //    $('#ErrorMsgInvalidFromDate').hide();
                //    $('#ProductionPlannerErrors').html('');
                //    CallbleAjax();
                //}
                $('#ErrorMsgInvalidFromDate').html('');
                $('#ErrorMsgInvalidToDate').html('');//  
                var fromdt = $('#searchVehiclesFromYear').val() + "/" + $('#searchVehiclesFromMonth').val() + "/" + $('#searchVehiclesFromDay').val();
                var todt = $('#searchVehiclesToYear').val() + "/" + $('#searchVehiclesToMonth').val() + "/" + $('#searchVehiclesToDay').val();

                var dateCompareResult = CompareDate(fromdt, todt);
                if (dateCompareResult != true) {
                    $('#vehiclessearchresultsMst').hide();
                    $("#vehiclessearchresultsChild").hide();
                    $('#ProductionPlannerErrors').html('');
                    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
                }
                else {

                    $('#ErrorMsgInvalidFromDate').html('');
                    $('#ErrorMsgInvalidToDate').html('');
                    $('#ProductionPlannerErrors').html('');
                    CallbleAjax();

                }

            }

        }

    }

    if (searchBy_String == "vehicle-no") {
        $('#ErrorMsgInvalidFromDate').html('');
        $('#ErrorMsgInvalidToDate').html('');
        $('#ProductionPlannerErrors').html('');
        var searchBy_Vechical = $('#searchByVehicleNo .text-input').val();
        if (searchBy_Vechical == "Type Vehicle #") {
            $("#vehiclesSearchResults").hide();
            $("#vehiclessearchresultsChild").hide(); $("#vehiclessearchresultsMst").hide();
            $('#ProductionPlannerErrors').html('');
            $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Enter Vechical No !</p>");
           
        }
        else {
            $('#ErrorMsgInvalidFromDate').html('');
            $('#ErrorMsgInvalidToDate').html('');
            $('#ProductionPlannerErrors').html('');
            // $('#ProductionPlannerErrors').html('');
            CallbleAjax();

        }
    }












    //  quickViewRecentOrders
});
$('#searchVehicles').on('click', 'a[href="#closeSearchResults"]', function () {

    $('#vehiclesSearchResults').hide();

    $('html,body').animate({
        scrollTop: $('#searchVehicles').position().top
    }, 400);


});
$('#vehiclesSearchResults').on('change', 'select', function () {
    var currentVal = $(this).val();

    if (currentVal === 'show-arrived') {
        $(this).next('.vehicle_results').find('li.arrived').show();
        $(this).next('.vehicle_results').find('li.not_arrived').hide();
        $(this).next('.vehicle_results').show();
    } else if (currentVal === 'show-departed') {
        $(this).next('.vehicle_results').find('li.not_departed').hide();
        $(this).next('.vehicle_results').show();
    } else if (currentVal === 'all-vehicles' || currentVal === 'show-in-queue') {
        $(this).next('.vehicle_results').find('li').show();
        $(this).next('.vehicle_results').show();
    }
});
$('.vehicle_results').on('click', 'a[href="#seeVehicleDetails"]', function () {
    var $dispatchDetails = $('#dispatchDetails-' + $(this).attr('data-id'));

    $('#vehiclesSearchResults').hide();
    $dispatchDetails.show();

    $('html,body').animate({
        scrollTop: $dispatchDetails.position().top
    }, 400);


});
$('#searchByName input')
.on('focus', function () {
    if ($(this).val() === 'Type Name') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Name');
    }
});
$('#searchByProductCode input')
.on('focus', function () {
    if ($(this).val() === 'Type Product Code') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Product Code');
    }
});
$('#searchByVehicleNo input')
.on('focus', function () {
    if ($(this).val() === 'Type Vehicle #') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Vehicle #');
    }
});

$(function () {
    $(document).on('click', 'a', function (evt) {
        var href = $(this).attr('href');

        if (href.indexOf(document.domain) > -1 || href.indexOf(':') === -1) {
            history.pushState({}, '', '');
        }


    });
});
function CompareDate(fromdtChk, todtChk) {

    // if (Date.parse(fromdate) > Date.parse(todate)) {
    if (Date.parse(fromdtChk) > Date.parse(todtChk)) {
        return false;
    }
    else {
        return true;
    }
}

$('#searchVehicles').on('change', '#AgentList', function () {

    $("#vehiclesSearchResults").hide();
});
$('#searchVehicles').on('change', '#ProductCodeList', function () {
    $("#vehiclesSearchResults").hide();
});

$('#searchVehicles').on('change', 'select.search_by', function () {
    $("#vehiclesSearchResults").hide();
});

function clrDates()
{
    $("#searchVehiclesFromDay").val("DD").trigger('chosen:updated');
    $("#searchVehiclesFromMonth").val("MM").trigger('chosen:updated');
    $("#searchVehiclesFromYear").val("YYYY").trigger('chosen:updated');
    $("#searchVehiclesToDay").val("DD").trigger('chosen:updated');
    $("#searchVehiclesToMonth").val("MM").trigger('chosen:updated');
    $("#searchVehiclesToYear").val("YYYY").trigger('chosen:updated');

}

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
    GetDispatchDashboard();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetDispatchDashboard();
});

//$('#tempbodySection').on('click', 'a[href="#close-panel"]', function () {
//    $("#tempbodySection").find('#ChangePasswordinner').hide();
//    $("#tempbodySection").find("#changePasswordSuccess").hide();
//    GetDispatchDashboard();
//});

$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetDispatchDashboard();
});

$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
    event.stopImmediatePropagation();
    GetDispatchDashboard();
});

function GetDispatchDashboard() {
    $.ajax(
                 {
                     cache: false,
                     type: "POST",
                     url: "/Dispatch/DiapatcherBackToHome/",
                     contentType: "application/json; charset=utf-8",
                     success: function (data) {

                         $('#tempbodySection').show();
                         $('#tempbodySection').html(data);

                     },
                     error: function (xhr, ajaxOptions, thrownError) {

                     }
                 });



}