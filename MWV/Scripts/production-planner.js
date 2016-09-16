/* -----------------------------------------------------------------------
 * Panel Grid Tabs
 * ----------------------------------------------------------------------- */


$('.panel_grid').on('click', 'a[href="#newOrders"]', function () {
    triggerDropdownNewordersForIphone();
    // hide other panels
    $.ajax({
        url: "/ProductionPlanner/GetNewOrderList/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                        $('#pendingApprovalsPanel, #mismatchesPanel, #productionPlansPanel').hide();
                        //if ($('#newOrdersPanel').is(':hidden')) {
                        $('#newOrdersPanel').show();
                        $('#NewOrdeListDiv').show();
                        $('#NewOrdeListDiv').html(result);
                        $('html,body').animate({
                            scrollTop: $('#newOrdersPanel').position().top
                        }, 400);
                        //} else {
                        //    $('#newOrdersPanel').hide();
                        //}
                    })
                        .error(function (xhr, ajaxOptions, thrownError) {
                        })
    // $('#newOrdersBulkAction').find('option:first').attr('selected', 'selected')

});
$('.panel_grid').on('click', 'a[href="#pendingApprovals"]', function () {
    triggerDropdownApprovalesForIphone();
    $.ajax({
        url: "/ProductionPlanner/GetPendingApproval/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                   .success(function (result) {
                       // hide other panels
                       $('#newOrdersPanel, #mismatchesPanel, #productionPlansPanel').hide();

                       if ($('#pendingApprovalsPanel').is(':hidden')) {
                           $('#pendingApprovalsPanel').show();
                           $('#pendingApprovalsPanel').html(result);

                           $('html,body').animate({
                               scrollTop: $('#pendingApprovalsPanel').position().top
                           }, 400);
                       } else {
                           $('#pendingApprovalsPanel').hide();

                           $('html,body').animate({
                               scrollTop: 0
                           }, 400);
                       }
                   })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })

});
$('.panel_grid').on('click', 'a[href="#mismatches"]', function () {
    // hide other panels
    $.ajax({
        url: "/ProductionPlanner/Mismatch/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                  .success(function (result) {
                      // hide other panels
                      $('#newOrdersPanel, #pendingApprovalsPanel, #productionPlansPanel').hide();

                      if ($('#mismatchesPanel').is(':hidden')) {
                          $('#mismatchesPanel').show();
                          $('#mismatchesPanel').html(result);

                          $('html,body').animate({
                              scrollTop: $('#mismatchesPanel').position().top
                          }, 400);
                      } else {
                          $('#mismatchesPanel').hide();

                          $('html,body').animate({
                              scrollTop: 0
                          }, 400);
                      }
                  })

});
$('.panel_grid').on('click', 'a[href="#productionPlans"]', function () {
    triggerDropdownDateForIphone();
    $("#SearchProductionPlanner")[0].selectedIndex = 0;
    $("#searchByType")[0].selectedIndex = 0;
    $("#CustomerList")[0].selectedIndex = 0;
    $("#AgentList")[0].selectedIndex = 0;

    $("#SrnoList")[0].selectedIndex = 0;
    $("#SrnoListBymillid")[0].selectedIndex = 0;

    $('#newOrdersPanel, #pendingApprovalsPanel, #mismatchesPanel').hide();
    //  alert("Hiii");

    $("#productionPlansSearchResults").hide();
    $('#ErrorMsgInvalidToDate').hide();
    $('#ErrorMsgInvalidFromDate').hide();
    $('#ProductionPlannerErrors').html('');
    if ($('#productionPlansPanel').is(':hidden')) {
        $('#productionPlansPanel').show();

        $('html,body').animate({
            scrollTop: $('#productionPlansPanel').position().top
        }, 400);
    } else {
        $('#productionPlansPanel').hide();

        $('html,body').animate({
            scrollTop: 0
        }, 400);
    }
    $('#searchByType').val("All");
    $('#searchByType').change();
    $('#searchPlansSrNo').find('option:first').attr('selected', 'selected');
    $('#productionPlansSearchPanel').find('option:first').attr('selected', 'selected');
    //$('#searchByType').find('option:first').attr('selected', 'selected');
    $('#searchPlansByAgent').find('option:first').attr('selected', 'selected');
    $('#searchPlansCustomerName').find('option:first').attr('selected', 'selected');

    //$('#searchPlansFromDay').find('option:first').attr('selected', 'selected');
    //$('#searchPlansFromMonth').find('option:first').attr('selected', 'selected');
    //$('#searchPlansFromYear').find('option:first').attr('selected', 'selected');
    //$('#searchPlansToDay').find('option:first').attr('selected', 'selected');
    //$('#searchPlansToMonth').find('option:first').attr('selected', 'selected');
    //$('#searchPlansToYear').find('option:first').attr('selected', 'selected');

    $('#searchPlansSrNo').hide();
    $('#searchPlansByAgent').hide();
    $('#searchPlansCustomerName').hide();
    //   $('#searchPlansByAgent').find('option:first').attr('selected', 'selected')

});
$('#newOrdersList').on('click', 'a[href="#seeOrderDetails"]', function () {
    debugger;
    var ordID = $(this).attr("data-id");
    var shadecode = $("#newShades option:selected").text();
    var allval = {
        papermillname: shadecode

    }
   
    $.ajax({
        url: "/ProductionPlanner/GetNewOrdersSeeDetailsMst/" + ordID,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                  .success(function (result) {
                      $('#newOrdersList').hide();
                      $('#closeNewOrders').hide();
                      $('#newOrdersBulkAction').hide();
                      $('#newOrderDetails').show();
                      $('#tmpOrderDetailsMst').html(result);
                  })

    $.ajax({
        url: "/ProductionPlanner/GetNewOrdersSeeDetailsChild/" + ordID,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                  .success(function (result) {
                      $('#newOrdersList').hide();
                      $('#closeNewOrders').hide();
                      $('#newOrdersBulkAction').hide();
                      $('#newOrderDetails').show();
                      $('#tmpOrderDetailsChild').html(result);
                      binddataval(allval);
                  });
    $('html,body').animate({
        scrollTop: $('#newOrdersList').position().top
    }, 400);


});
function binddataval(allval)
{
   
    //$.ajax({
    //    cache: false,
    //    type: "GET",
    //    url: "/ProductionPlanner/GetPapermillNameonShade/",
    //    data: allval,
    //    contentType: "application/json; charset=utf-8",
    //    success: function (data) {
    //        $("#tmpOrderDetailsMst").find("#ddlPapermillname").html('');
    //        $("#tmpOrderDetailsMst").find("ddlPapermillname").append($('<option>Select Papermill</option>'));
    //        $.each(data, function (id, option) {

    //            $("#tmpOrderDetailsMst").find("#ddlPapermillname").append($('<option></option>').val(option.schedule_id).html(option.name));

    //        });
    //    }
    //});


}
//Calling from -NeworderSeedetailsMst
$('.new_order_details').on('change', 'select#ddlPapermillname', function () {
     

    $("#spanForSeeErrors").html('');
    var selectMachName = $("option:selected",this).text();//Select Bulk Action
    if (selectMachName == "Select Bulk Action") {
        $(this).closest('.new_order_details').find('.take_action_panel').hide();
        $(".take_action_panel").find('.details').html("Confirm assignment to this Machine?");
    }
    else {
        $(this).closest('.new_order_details').find('.take_action_panel').show();
        $(".take_action_panel").find('.details').html("Confirm assignment to this" + "  " + selectMachName + "?");
    }

    
});
$('.take_action_panel').on('click', 'a[href="#closeTakeAction"]', function () {
  
    var liid = $(this).closest("li").attr('id');
    $('#' + liid).find('.error-msg').hide();
    //$(this).closest('.take_action_panel').find('.error-msg').hide();
    $(this).closest('.take_action_panel').hide();
    $('#ddlPapermillname').find('option:first').prop('selected', 'selected')

});
$('#newOrdersPanel').on('change', 'select#ddlPapermillname', function () {
     
    var selectMachName = $("#ddlPapermillname option:selected").text();//Select Bulk Action
    if (selectMachName == "Select Bulk Action") {
        $(this).next('.take_action_panel').hide();
        $(".take_action_panel").find('.details').html("Confirm assignment to this Machine?");
    }
    else {
        $(".take_action_panel").find('.details').html("Confirm assignment to this" + "  " + selectMachName + "?");
        $(this).next('.take_action_panel').show();
    }
   

   
});
$('.new_order_details').on('click', 'a[href="#closeOrderDetailsPanel"]', function () {
    $(this).closest('.new_order_details').hide();
    $('#closeNewOrders').show();
    $('#newOrdersBulkAction').show();
    $('#newOrdersList').show();
    $('#newOrdersPanel').show();


});
$('#newOrdersPanel').on('click', 'a[href="#closeNewOrdersPanel"]', function () {
    $('#newOrdersPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('#pendingApprovalsPanel').on('click', 'a[href="#seeConflict"]', function () {
    $(this).closest('li').find('.see_conflict_details').show();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 450
    });


});
$('#pendingApprovalsPanel').on('click', 'a[href="#closeSeeConflict"]', function () {
    var liid = $(this).closest("li").attr('id');
    $('#' + liid).find('.error-msg').hide();
    $(this).closest('.see_conflict_details').hide();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() - 225
    });


});
$('#pendingApprovalsPanel').on('click', 'a[href="#closePendingApprovalsPanel"]', function () {
    $('#pendingApprovalsPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('.pending_approvals_list').on('change', 'select', function () {
    $(this).next('.take_action_panel').show();
});

$('#mismatchesPanel').on('click', 'a[href="#takeMismatchAction"]', function () {

    $(this).closest('li').find('.choose_action').show();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 200
    });


});
function RefreshRecentMismatches() {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "/ProductionPlanner/RecentMismatch/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
              .success(function (result) {
                  $('#quickView').show();
                  $('#quickViewRecentMismatches').html(result);
              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })
}
$('#mismatchesPanel').on('click', 'a[href="#sendToLowerBf"]', function () {
   

    var DeckleId = $(this).attr("data-id");
    $("#mismatchesPanel").find("#mismatchErrors-" + DeckleId).hide();
    var selectedValues =
{
    sendToLowerBf: "sendToLowerBf",
    ActionRemark: $('.remark-' + DeckleId).val()
}
    if ($('.remark-' + DeckleId).val().trim() == "") {
        $("#mismatchesPanel").find("#mismatchErrorsLowerbf-" + DeckleId).show();
        $("#mismatchesPanel").find('#mismatchErrorsLowerbf-' + DeckleId).html("<p class='error-msg'>Please Enter Remark !");

    }
    else {


        $(this).closest('.take_action_panel').hide();
        $(this).closest('li').find('.take_action_panel.to_bf').show();

        $.ajax({
            url: "/ProductionPlanner/SubmitDeckleToLowerBfORproduction/" + DeckleId,
            data: selectedValues,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                    .success(function (result) {

                    });
        refreshRecentMismatch();
    }
});
function refreshRecentMismatch() {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "/ProductionPlanner/RecentMismatch/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                        $('#quickView').show();
                        $('#quickViewRecentMismatches').html(result);
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })


}
$('#mismatchesPanel').on('click', 'a[href="#sendToProduction"]', function () {


    var DeckleId = $(this).attr("data-id");
    $("#mismatchesPanel").find("#mismatchErrorsLowerbf-" + DeckleId).hide();
    var selectedValues =
{
    sendToProduction: "sendToProduction",
    ActionRemark: $('.remark-' + DeckleId).val()
}

    if ($('.remark-' + DeckleId).val().trim() == "") {
        $("#mismatchesPanel").find("#mismatchErrors-" + DeckleId).show();
        $("#mismatchesPanel").find('#mismatchErrors-' + DeckleId).html("<p class='error-msg'>Please Enter Remark !");

    }
    else {

        $(this).closest('.take_action_panel').hide();
        $(this).closest('li').find('.take_action_panel.to_production').show();
        $.ajax({
            url: "/ProductionPlanner/SubmitDeckleToLowerBfORproduction/" + DeckleId,
            data: selectedValues,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                    .success(function (result) {

                    })();

        refreshRecentMismatch();
    }
});


$('#mismatchesPanel').on('click', 'a[href="#closeTakeAction"]', function () {

   
    $('#newOrdersBulkAction').find('option:first').attr('selected', 'selected')
    $('.textarea-input').val('');
    var liid = $(this).closest("li").attr('id');
    $('#' + liid).find('.error-msg').hide();

    $(this).closest('.take_action_panel').hide();
    $(this).closest('li').remove();
    $.ajaxSetup({ cache: false });

    RefreshRecentMismatches();

    //if ($(this).attr("href") == "#closeTakeAction/close") {
    //    $(this).closest('li').remove();
    //} else {
    //$(this).closest('li').remove();
    //   RefreshRecentMismatches();
    //}

    // $('#mismatchesPanel').hide();
    $('html,body').animate({
        scrollTop: $(window).scrollTop() - 225
    });


});

$('#mismatchesPanel').on('click', 'a[href="#closeTakeAction/close"]', function () {
    //$('#mismatchesPanel').hide();
    

    $('#newOrdersBulkAction').find('option:first').attr('selected', 'selected')
    $(this).closest('.take_action_panel').hide();
    if ($(this).attr("href") == "#closeTakeAction/close") {
        $(this).closest('li').remove();
    }
    $.ajaxSetup({ cache: false });
    RefreshRecentMismatches();
    $('html,body').animate({
        scrollTop: $(window).scrollTop() - 225
    });


});
$('#mismatchesPanel').on('click', 'a[href="#closeMismatchesPanel"]', function () {
    $('#mismatchesPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});

$('#productionPlansPanel').on('click', 'a[href="#searchProductionPlans"]', function () {

    function CallbleAjax() {
        $.ajax({
            url: "/ProductionPlanner/clearSession/",
            data: selectedOrder,
            contentType: "application/html; charset=utf-8",
            type: "POST",
            dataType: 'html',
            context: document.body,
        })
                          .success(function (result) {

                          })
                         .error(function (xhr, ajaxOptions, thrownError) {
                         })

        var allmills = $("#SearchProductionPlanner option:selected").text();
        var searchByAll = $("#searchByType option:selected").text();
        var selectedOrder =
        {
            SelectedMillId: $("#SearchProductionPlanner").val(),
            SelectedAgent: $("#AgentList").val(),
            SelectedSrno: $("#SrnoList").val(),
            SrnoBymillid: $("#SrnoListBymillid").val(),
            SelectedCustomer: $("#CustomerList").val(),
            FromDateTime: $('#searchPlansFromDay').val() + "-" + $('#searchPlansFromMonth').val() + "-" + $('#searchPlansFromYear').val(),
            ToDateTime: $('#searchPlansToDay').val() + "-" + $('#searchPlansToMonth').val() + "-" + $('#searchPlansToYear').val(),
        }
        //alert($("#SrnoListBymillid").val());
        if (allmills == "All PMs " && searchByAll == "All") {

            $.ajax({
                url: "/ProductionPlanner/GetAllProPlanFromtoDate/",
                data: selectedOrder,
                contentType: "application/html; charset=utf-8",
                type: "GET",
                dataType: 'html',
                context: document.body,
            })
                           .success(function (result) {
                               $('#productionPlansSearchPanel').show();
                               $('#productionPlansSearchResults').show();
                               $('#productionPlansSearchResults').html(result);
                               $('html,body').animate({
                                   scrollTop: $('#productionPlansPanel').position().top + 575
                               }, 400);

                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })
        } else
            if (allmills != "All PMs " && searchByAll == "All") {
                var selectedvalues =
        {
            SelectedMillId: $("#SearchProductionPlanner").val(),
            FromDateTime: $('#searchPlansFromDay').val() + "-" + $('#searchPlansFromMonth').val() + "-" + $('#searchPlansFromYear').val(),
            ToDateTime: $('#searchPlansToDay').val() + "-" + $('#searchPlansToMonth').val() + "-" + $('#searchPlansToYear').val(),
        }
                $.ajax({
                    url: "/ProductionPlanner/GetProPlanByMill/",
                    data: selectedvalues,
                    contentType: "application/html; charset=utf-8",
                    type: "GET",
                    dataType: 'html',
                    context: document.body,
                })
                               .success(function (result) {
                                   $('#productionPlansSearchPanel').show();
                                   $('#productionPlansSearchResults').show();
                                   $('#productionPlansSearchResults').html(result);
                                   $('html,body').animate({
                                       scrollTop: $('#productionPlansPanel').position().top + 575
                                   }, 400);

                               })
                              .error(function (xhr, ajaxOptions, thrownError) {
                              })
            }
            else {
                $.ajax({
                    url: "/ProductionPlanner/GetSearchProductionPlan/",
                    data: selectedOrder,
                    contentType: "application/html; charset=utf-8",
                    type: "GET",
                    dataType: 'html',
                    context: document.body,
                })
                                .success(function (result) {
                                    $('#productionPlansSearchPanel').show();
                                    $('#productionPlansSearchResults').show();
                                    $('#productionPlansSearchResults').html(result);
                                })
                               .error(function (xhr, ajaxOptions, thrownError) {
                               })
            }

    }

    var fromdt = $('#searchPlansFromDay').val() + "/" + $('#searchPlansFromMonth').val() + "/" + $('#searchPlansFromYear').val();
    var todt = $('#searchPlansToDay').val() + "/" + $('#searchPlansToMonth').val() + "/" + $('#searchPlansToYear').val();
    //if ($('#searchPlansFromDay').val() == "DD" || $('#searchPlansFromMonth').val() == "MM" || $('#searchPlansFromYear').val() == "YYYY") {
    var fromdateTime = ValidateDate(fromdt);
    var todateTime = ValidateDate(todt);
    $('#ProductionPlannerErrors').html('');
    $('#ErrorMsgInvalidToDate').hide();
    $('#ErrorMsgInvalidFromDate').hide();

    //var searchtxt = $("#searchByType option:selected").text();
    //alert(searchtxt);
    if ($("#searchByType option:selected").text() == "All") {
        $('#ProductionPlannerErrors').html('');
        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        if ($('#searchPlansFromDay').val() == "DD" && $('#searchPlansFromMonth').val() == "MM" && $('#searchPlansFromYear').val() == "YYYY"
    && $('#searchPlansToDay').val() == "DD" && $('#searchPlansToMonth').val() == "MM" && $('#searchPlansToYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').show();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').show();
        } else if ($('#searchPlansFromDay').val() == "DD" || $('#searchPlansFromMonth').val() == "MM" || $('#searchPlansFromYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').show();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').hide();
            if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidToDate').show();

                $("#productionPlansSearchResults").hide();
            }
        }
        else if ($('#searchPlansFromDay').val() == null || $('#searchPlansFromMonth').val() == null || $('#searchPlansFromYear').val() == null) {
            $('#ErrorMsgInvalidFromDate').show();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').hide();

        }
        else if ($('#searchPlansToDay').val() == null || $('#searchPlansToMonth').val() == null || $('#searchPlansToYear').val() == null) {
            $('#ErrorMsgInvalidFromDate').hide();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').show();
        }

        else if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').hide();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').show();
            if (fromdateTime == false) {
                $('#ErrorMsgInvalidFromDate').show();
                $("#productionPlansSearchResults").hide();
                //  $('#ErrorMsgInvalidToDate').show();s
            }
        } else if (todateTime == false && fromdateTime == false) {
            $('#ErrorMsgInvalidToDate').show();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidFromDate').show();
        } else if (todateTime == false) {
            $('#ErrorMsgInvalidFromDate').hide();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidToDate').show();

        } else if (fromdateTime == false) {
            $('#ErrorMsgInvalidToDate').hide();
            $("#productionPlansSearchResults").hide();
            $('#ErrorMsgInvalidFromDate').show();
        }
        else {
            //$('#ErrorMsgInvalidToDate').hide();
            //$('#ErrorMsgInvalidFromDate').hide();
            //if (fromdt > todt) {
            //    $('#productionPlansSearchResults').hide();
            //    $('#ProductionPlannerErrors').html('');
            //    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
            //}
            //else {
            //    $('#ErrorMsgInvalidToDate').hide();
            //    $('#ErrorMsgInvalidFromDate').hide();
            //    $('#ProductionPlannerErrors').html('');
            //    CallbleAjax();
            //}
            $('#ProductionPlannerErrors').html('');
            $('#ErrorMsgInvalidToDate').hide();//$
            $('#ErrorMsgInvalidFromDate').hide();//$('#orderToDateDay').val() 
            var fromdtChk = $('#searchPlansFromYear').val() + "/" + $('#searchPlansFromMonth').val() + "/" + $('#searchPlansFromDay').val();
            var todtChk = $('#searchPlansToYear').val() + "/" + $('#searchPlansToMonth').val() + "/" + $('#searchPlansToDay').val();

            var dateCompareResult = CompareDate(fromdtChk, todtChk);
            if (dateCompareResult != true) {
                $("#productionPlansSearchResults").hide();
                $('#ProductionPlannerErrors').html('');
                $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
            }
            else {

                $('#ErrorMsgInvalidToDate').hide();
                $('#ErrorMsgInvalidFromDate').hide();
                $('#ProductionPlannerErrors').html('');
                CallbleAjax();

            }

        }

    }


    else if ($("#searchByType option:selected").text() == "By Sr. No." && $("#SrnoListBymillid option:selected").text() == "Select Sr No" && $("#SrnoList option:selected").text() == "Select Sr No. ") {
        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        $('#ProductionPlannerErrors').html('');
        $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Select Sr No. !</p>");
        $("#productionPlansSearchResults").hide();
    }

    else if ($("#searchByType option:selected").text() == "By Sr. No." && $("#SrnoListBymillid option:selected").text() != "Select Sr No") {

        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        $('#ProductionPlannerErrors').html('');
        CallbleAjax();

    }
    else if ($("#searchByType option:selected").text() == "By Sr. No." && $("#SrnoList option:selected").text() != "Select Sr No. ") {
        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        $('#ProductionPlannerErrors').html('');
        CallbleAjax();

    }

    if ($("#searchByType option:selected").text() == "By Agent") {


        if ($("#AgentList option:selected").text() == "Select Agent Name ") {
            $('#ErrorMsgInvalidToDate').hide();
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ProductionPlannerErrors').html('');
            $("#productionPlansSearchResults").hide();
            $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Select Agent Name!</p>");
        }
        else {
            $('#ErrorMsgInvalidToDate').hide();
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ProductionPlannerErrors').html('');
            if ($('#searchPlansFromDay').val() == "DD" && $('#searchPlansFromMonth').val() == "MM" && $('#searchPlansFromYear').val() == "YYYY"
    && $('#searchPlansToDay').val() == "DD" && $('#searchPlansToMonth').val() == "MM" && $('#searchPlansToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
            } else if ($('#searchPlansFromDay').val() == "DD" || $('#searchPlansFromMonth').val() == "MM" || $('#searchPlansFromYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').hide();
                if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
                    $('#ErrorMsgInvalidToDate').show();
                }
            }
            else if ($('#searchPlansFromDay').val() == null || $('#searchPlansFromMonth').val() == null || $('#searchPlansFromYear').val() == null) {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').hide();

            }
            else if ($('#searchPlansToDay').val() == null || $('#searchPlansToMonth').val() == null || $('#searchPlansToYear').val() == null) {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
            }
            else if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
                if (fromdateTime == false) {
                    $('#ErrorMsgInvalidFromDate').show();
                    //  $('#ErrorMsgInvalidToDate').show();s
                }
            } else if (todateTime == false && fromdateTime == false) {
                $('#ErrorMsgInvalidToDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidFromDate').show();
            } else if (todateTime == false) {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();

            } else if (fromdateTime == false) {
                $('#ErrorMsgInvalidToDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidFromDate').show();
            }
            else {
                $('#ErrorMsgInvalidToDate').hide();//
                $('#ErrorMsgInvalidFromDate').hide();//$('#orderToDateDay').val() 
                var fromdtChk = $('#searchPlansFromYear').val() + "/" + $('#searchPlansFromMonth').val() + "/" + $('#searchPlansFromDay').val();
                var todtChk = $('#searchPlansToYear').val() + "/" + $('#searchPlansToMonth').val() + "/" + $('#searchPlansToDay').val();

                var dateCompareResult = CompareDate(fromdtChk, todtChk);
                if (dateCompareResult != true) {
                    $('#productionPlansSearchResults').hide();
                    $('#ProductionPlannerErrors').html('');
                    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
                }
                else {

                    $('#ErrorMsgInvalidToDate').hide();
                    $('#ErrorMsgInvalidFromDate').hide();
                    $('#ProductionPlannerErrors').html('');
                    CallbleAjax();

                }

            }

        }

    }

    if ($("#searchByType option:selected").text() == "By Customer Name") {
        $('#ErrorMsgInvalidToDate').hide(); $("#productionPlansSearchResults").hide();
        $('#ErrorMsgInvalidFromDate').hide();
        if ($("#CustomerList option:selected").text() == "Select Customer Name ") {
            $('#ErrorMsgInvalidToDate').hide();
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ProductionPlannerErrors').html('');
            $('#ProductionPlannerErrors').html("<p class='error-msg'>Please Select Customer Name !</p>");
        }
        else {
            $('#ProductionPlannerErrors').html('');
            if ($('#searchPlansFromDay').val() == "DD" && $('#searchPlansFromMonth').val() == "MM" && $('#searchPlansFromYear').val() == "YYYY"
   && $('#searchPlansToDay').val() == "DD" && $('#searchPlansToMonth').val() == "MM" && $('#searchPlansToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
            } else if ($('#searchPlansFromDay').val() == "DD" || $('#searchPlansFromMonth').val() == "MM" || $('#searchPlansFromYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').hide();
                if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
                    $('#ErrorMsgInvalidToDate').show(); $("#productionPlansSearchResults").hide();
                }
            }
            else if ($('#searchPlansFromDay').val() == null || $('#searchPlansFromMonth').val() == null || $('#searchPlansFromYear').val() == null) {
                $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').hide();

            }
            else if ($('#searchPlansToDay').val() == null || $('#searchPlansToMonth').val() == null || $('#searchPlansToYear').val() == null) {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
            }
            else if ($('#searchPlansToDay').val() == "DD" || $('#searchPlansToMonth').val() == "MM" || $('#searchPlansToYear').val() == "YYYY") {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();
                if (fromdateTime == false) {
                    $('#ErrorMsgInvalidFromDate').show(); $("#productionPlansSearchResults").hide();
                    //  $('#ErrorMsgInvalidToDate').show();s
                }
            } else if (todateTime == false && fromdateTime == false) {
                $('#ErrorMsgInvalidToDate').show(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidFromDate').show();
            } else if (todateTime == false) {
                $('#ErrorMsgInvalidFromDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidToDate').show();

            } else if (fromdateTime == false) {
                $('#ErrorMsgInvalidToDate').hide(); $("#productionPlansSearchResults").hide();
                $('#ErrorMsgInvalidFromDate').show();
            }
            else {
                $('#ErrorMsgInvalidToDate').hide();//
                $('#ErrorMsgInvalidFromDate').hide();//$('#orderToDateDay').val() 
                var fromdtChk = $('#searchPlansFromYear').val() + "/" + $('#searchPlansFromMonth').val() + "/" + $('#searchPlansFromDay').val();
                var todtChk = $('#searchPlansToYear').val() + "/" + $('#searchPlansToMonth').val() + "/" + $('#searchPlansToDay').val();

                var dateCompareResult = CompareDate(fromdtChk, todtChk);
                if (dateCompareResult != true) {
                    $('#productionPlansSearchResults').hide();
                    $('#ProductionPlannerErrors').html('');
                    $('#ProductionPlannerErrors').html("<p class='error-msg'>To Date should be greater than equal to From Date !</p>");
                }
                else {

                    $('#ErrorMsgInvalidToDate').hide();
                    $('#ErrorMsgInvalidFromDate').hide();
                    $('#ProductionPlannerErrors').html('');
                    CallbleAjax();

                }

            }



        }
    }


});



function refreshDateTime() {
    $('#searchPlansFromDay').find('option:first').attr('selected', 'selected')
    $('#searchPlansFromMonth').find('option:first').attr('selected', 'selected')
    $('#searchPlansFromYear').find('option:first').attr('selected', 'selected')
    $('#searchPlansToDay').find('option:first').attr('selected', 'selected')
    $('#searchPlansToMonth').find('option:first').attr('selected', 'selected')
    $('#searchPlansToYear').find('option:first').attr('selected', 'selected')
}
function ShowdatetimeDDL() {
    $('.form-label').show();
    $('#searchPlansFromDay').show();
    $('#searchPlansFromMonth').show();
    $('#searchPlansFromYear').show();
    $('#searchPlansToDay').show();
    $('#searchPlansToMonth').show();
    $('#searchPlansToYear').show();
}
$('#productionPlansSearchPanel').on('change', '#searchByType', function () {

    $("#productionPlansSearchResults").hide();
    if ($(this).val() === 'by-agent') {
        // refreshDateTime();
        $('#AgentList').val('').trigger('chosen:updated');
        $("#SrnoList option[value='']").attr('selected', true)
        $("#CustomerList option[value='']").attr('selected', true)
        $('#searchPlansSrNo').hide();
        $('#searchPlansCustomerName').hide();
        $('#SrnoListBymillid').hide();
        $('#searchPlansByAgent').show();
        ShowdatetimeDDL();

    } else if ($(this).val() === 'by-customer-name') {
        //  refreshDateTime();
        $('#CustomerList').val('').trigger('chosen:updated');
        $("#SrnoList option[value='']").attr('selected', true)
        $("#AgentList option[value='']").attr('selected', true)
        $('#searchPlansSrNo').hide();
        $('#searchPlansByAgent').hide();
        $('#SrnoListBymillid').hide();

        $('#searchPlansCustomerName').show();
        ShowdatetimeDDL();

    }
    else if ($(this).val() === 'by-sr-no') {
        $('#SrnoListBymillid').val('').trigger('chosen:updated');
        $('#SrnoListBymillid').find('option:first').prop('selected', 'selected');
        $('#SrnoList').val('').trigger('chosen:updated');
        $('#SrnoList').find('option:first').prop('selected', 'selected');

        $('.form-label').hide();
        $('#searchPlansFromDay').hide();
        $('#searchPlansFromMonth').hide();
        $('#searchPlansFromYear').hide();
        $('#searchPlansToDay').hide();
        $('#searchPlansToMonth').hide();
        $('#searchPlansToYear').hide();
        var selectedOrder =
             {
                 searchBy: $("#SearchProductionPlanner option:selected").val(),
             }
        var selectedItem = $("#SearchProductionPlanner option:selected").val()
        var SrnoBymillid = $("#SrnoListBymillid");
        if (selectedItem != "") {

            $.ajax(
                       {
                           cache: false,
                           type: "GET",
                           data: selectedOrder,
                           url: "/ProductionPlanner/GetSrNoByMillId/",
                           contentType: "application/json; charset=utf-8",
                           success: function (data) {
                               SrnoBymillid.html('');
                               SrnoBymillid.append($('<option>Select Sr No</option>'));
                               $.each(data, function (id, option) {

                                   SrnoBymillid.append($('<option></option>').val(option.srno).html(option.srno));
                               });
                           },
                           error: function (xhr, ajaxOptions, thrownError) {
                               //alert("ajaxOptions " + ajaxOptions);
                               // alert('Failed to retrieve states.');
                           }
                       });
            $("#CustomerList option[value='']").attr('selected', true)
            $("#AgentList option[value='']").attr('selected', true)
            $('#searchPlansByAgent').hide();
            $('#searchPlansCustomerName').hide();
            $('#searchPlansSrNo').hide();
            $('#SrnoListBymillid').show();
            $('#srnolist').show();
        }
        else {
            $("#CustomerList option[value='']").attr('selected', true)
            $("#AgentList option[value='']").attr('selected', true)
            $('#searchPlansByAgent').hide();
            $('#searchPlansCustomerName').hide();
            $('#SrnoListBymillid').hide();
            $('#searchPlansSrNo').show();
            //ShowdatetimeDDL();
        }

    }
    else if ($(this).val() === 'All') {
        // refreshDateTime();
        $('#searchPlansSrNo').hide();
        $('#searchPlansByAgent').hide();
        $('#searchPlansCustomerName').hide();
        $('#SrnoListBymillid').hide();
        $('#srnolist').hide();
        ShowdatetimeDDL();
    }
    //else {
    //    $('#searchPlansCustomerName').hide();
    //    $('#searchPlansByAgent').hide();
    //    $('#searchPlansSrNo').hide();
    //}
});
$('#productionPlansSearchPanel').on('change', '#SearchProductionPlanner', function () {


    $("#productionPlansSearchResults").hide();
    $('#AgentList').find('option:first').attr('selected', 'selected')
    $('#SrnoList').find('option:first').attr('selected', 'selected')
    $('#SrnoListBymillid').find('option:first').attr('selected', 'selected')

    $('#CustomerList').find('option:first').attr('selected', 'selected')
    //  $('#searchByType').find('option:first').attr('selected', 'selected').change();
    $('#searchByType').change();
});
$('#productionPlansSearchPanel').on('change', '#searchPlansSrNo', function () {
    // $('#SrnoList').find('option:first').attr('selected', 'selected')
    //  refreshDateTime();
});
$('#productionPlansSearchPanel').on('change', '#searchPlansByAgent', function () {
    //   refreshDateTime();
});
$('#productionPlansSearchPanel').on('change', '#searchPlansCustomerName', function () {
    //refreshDateTime();
});

$('#productionPlansSearchResults').on('click', 'a[href="#closeSearchResults"]', function () {
    $('#productionPlansSearchResults').hide();

    $('html,body').animate({
        scrollTop: $('#productionPlansPanel').position().top
    }, 400);


});
$('#productionPlansSearchResults').on('click', 'a[href="#seeDetails"]', function () {
    var id = $(this).attr('data-id');
    $.ajax({
        url: "/ProductionPlanner/ProductionPlanHeaderDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#productionPlansSearchResults').hide();
                           $('#plans-details-june182015-pm02').show();
                           $('#plans-details-june182015-pm02').html(result);
                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

    $('#productionPlansSearchResults').hide();
    $('#plans-details-' + $(this).attr('data-id')).show();


});
$('#productionPlansPanel').on('click', 'a[href="#closePlanDetailsPanel"]', function () {
    $(this).closest('.production_plans_details').hide();
    $('#productionPlansSearchResults').show();


});
$('#productionPlansPanel').on('click', 'a[href="#closeProductionPlansPanel"]', function () {
    $('#productionPlansPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});

function seeMismatches() {

    $.ajax({
        url: "/ProductionPlanner/Mismatch/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                 .success(function (result) {
                     // hide other panels
                     $('#newOrdersPanel').hide();
                     $('#pendingApprovalsPanel').hide();
                     $('#productionPlansPanel').hide();
                     $('#mismatchesPanel').show();
                     $('#mismatchesPanel').html(result);
                 })



    $('html,body').animate({
        scrollTop: $('#mismatchesPanel').position().top
    }, 400);
}

$('#quickView').on('click', 'a[href="#seeAllRecentMismatches"]', function () {
    seeMismatches();


});
$('#quickView').on('click', 'a[href="#seeMismatchDetails"]', function () {
    //seeMismatches();
    var id = $(this).attr('data-id');
    $.ajax({
        url: "/ProductionPlanner/SeeMismatchDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
               .success(function (result) {

                   $('#newOrdersPanel').hide();
                   $('#pendingApprovalsPanel').hide();
                   $('#productionPlansPanel').hide();
                   $('#mismatchesPanel').show();
                   $('#mismatchesPanel').html(result);
               })
    $('html,body').animate({
        scrollTop: $('#mismatchesPanel').position().top
    }, 400);

});
function seeApprovals() {
    $.ajax({
        url: "/ProductionPlanner/GetPendingApproval/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#newOrdersPanel').hide();
                           $('#mismatchesPanel').hide();
                           $('#productionPlansPanel').hide();
                           $('#pendingApprovalsPanel').show();
                           $('#pendingApprovalsPanel').html(result);

                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

    $('html,body').animate({
        scrollTop: $('#pendingApprovalsPanel').position().top
    }, 400);
}
$('#quickView').on('click', 'a[href="#seeAllRecentApprovals"]', function () {
    seeApprovals();


});
$('#quickView').on('click', 'a[href="#seeApprovalsDetails"]', function () {
    //seeApprovals();
    var id = $(this).attr('data-id');

    $.ajax({
        url: "/ProductionPlanner/SeeApprovalsDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#newOrdersPanel').hide();
                           $('#mismatchesPanel').hide();
                           $('#productionPlansPanel').hide();
                           $('#pendingApprovalsPanel').show();
                           $('#pendingApprovalsPanel').html(result);
                           $('html,body').animate({
                               scrollTop: $('#pendingApprovalsPanel').position().top
                           }, 400);

                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })



});
$('#quickView').on('click', 'a[href="#seeAllRecentProductionPlans"]', function () {

    $.ajax({
        url: "/ProductionPlanner/clearSession/",
        //  data: selectedOrder,
        contentType: "application/html; charset=utf-8",
        type: "POST",
        dataType: 'html',
        context: document.body,
    })
                     .success(function (result) {

                     })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })

    $.ajax({
        url: "/ProductionPlanner/SeeAllRecentProductionPlans/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#newOrdersPanel').hide();
                           $('#mismatchesPanel').hide();
                           $('#pendingApprovalsPanel').hide();
                           $('#productionPlansPanel').show();

                           $('#productionPlansSearchResults').show();
                           $('#productionPlansSearchResults').html(result);

                           $('html,body').animate({
                               scrollTop: $('#productionPlansPanel').position().top + 575
                           }, 400);
                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

});
$('#quickView').on('click', 'a[href="#seePlansDetails"]', function () {

    var id = $(this).attr('data-id');
    $.ajax({
        url: "/ProductionPlanner/ProductionPlanHeaderDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#newOrdersPanel').hide();
                           $('#mismatchesPanel').hide();
                           $('#pendingApprovalsPanel').hide();
                           $('#productionPlansSearchResults').hide();
                           $('#plans-details-june182015-pm02').show();
                           $('#plans-details-june182015-pm02').html(result);
                           $('#productionPlansPanel').show();


                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

    $('html,body').animate({
        scrollTop: $('#productionPlansPanel').position().top + 575
    }, 400);


});

$('.production_planner .remark')
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
$('#searchPlansSrNo input')
.on('focus', function () {
    if ($(this).val() === 'Enter Sr. No.') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Sr. No.');
    }
});
$('#searchPlansByAgent input')
.on('focus', function () {
    if ($(this).val() === 'Enter Agent Name') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Agent Name');
    }
});
$('#searchPlansCustomerName input')
.on('focus', function () {
    if ($(this).val() === 'Enter Customer Name') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Customer Name');
    }
});
$('#searchPlansFromDay')
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
$('#searchPlansFromMonth')
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
$('#searchPlansFromYear')
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
$('#searchPlansToDay')
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
$('#searchPlansToMonth')
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
$('#searchPlansToYear')
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


$(function () {
    $(document).on('click', 'a', function (evt) {
        var href = $(this).attr('href');

        if (href.indexOf(document.domain) > -1 || href.indexOf(':') === -1) {
            history.pushState({}, '', '');
        }
        return false;
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

$('#productionPlansSearchPanel').on('change', '#AgentList', function () {
    $("#productionPlansSearchResults").hide();
});

$('#productionPlansSearchPanel').on('change', '#CustomerList', function () {
    $("#productionPlansSearchResults").hide();
});

//By sagar For Trigger Dropdowns
function triggerDropdownDateForIphone() {

    $('#searchPlansFromDay').val('DD').trigger('chosen:updated');
    $('#searchPlansFromMonth').val('MM').trigger('chosen:updated');
    $('#searchPlansFromYear').val('YYYY').trigger('chosen:updated');
    $('#searchPlansToDay').val('DD').trigger('chosen:updated');
    $('#searchPlansToMonth').val('MM').trigger('chosen:updated');
    $('#searchPlansToYear').val('YYYY').trigger('chosen:updated');

}

function triggerDropdownNewordersForIphone() {

    $('#newOrdersBulkAction').val('').trigger('chosen:updated');
    $('#newOrdersBulkAction').find('option:first').prop('selected', 'selected');
    //$("#NewOrdeListDiv").find('#newOrdersBulkAction').val('').trigger('chosen:updated');
    //$("#NewOrdeListDiv").find('#newOrdersBulkAction').find('option:first').prop('selected', 'selected');

}
function triggerDropdownApprovalesForIphone() {

    $('#createOrderCurrentProducts').find('.select-input').val('Approve').trigger('chosen:updated');
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
    GetPPDashboard();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetPPDashboard();
});

 
$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    GetPPDashboard();
});

$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
    event.stopImmediatePropagation();
    GetPPDashboard();
});

function GetPPDashboard() {

    $('#tempParentDiv').show();
    $('#yourMessages').hide();
    $('#NewOrdeListDiv').hide();
    $('#pendingApprovalsPanel').hide();
    $('#yourAlerts').hide();
    $('#mismatchesPanel').hide();
    $("#usersChangePassword").hide();
    
}
//Download PP Plana
 
$('.production_plans_details').on('click', 'a[href="#downloadDetails"]', function () {

    var orderid = $(this).attr("data-id");
    window.location = "ProductionPlanner/GetPdffileForPP?orderid=" + orderid


});