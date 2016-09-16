$('.customer_orders').on('click', 'a[href="#customerOrders"]', function (event) {

    event.stopImmediatePropagation();
    $('#EnterPO').val('Type Purchase Order #');
    if ($('#customerOrders').is(':hidden')) {

        $('#customerOrders').show();

        $('html,body').animate({
            scrollTop: $('#customerOrders').position().top
        }, 400);
    } else {
        $('#customerOrders').hide();
    }

});
$('#customerOrders').on('click', 'a[href="#searchOrders"]', function () {
    $('#errormsgForOrderSearch').hide();

    var allValsSave = [];

    allValsSave = $('#EnterPO').val();
    if (allValsSave == "Type Purchase Order #") {
        $('#errormsgForOrderSearch').show();
        $('#errormsgForOrderSearch').html('Please enter atleast one customer PO !');
    }
    else {
        $.ajax({
            url: "/Customer/OrdersSearchResultsByCustomerPO/?allVals=" + allValsSave,
            contentType: "application/html; charset=utf-8",
            type: "Post",
            dataType: 'html',
            context: document.body,
        })
                       .success(function (result) {
                           $('#ordersResultsPanel').show();
                           $('#ordersResultsPanel').html(result);
                           $('html,body').animate({
                               scrollTop: $('#customerOrders').position().top + 400
                           }, 400);
                       })
    }
});
$('#customerOrders').on('click', 'a[href="#closeCustomerOrders"]', function () {
    $('#customerOrders').hide();
    $('#ordersResultsPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);
});
$('#ordersResultsPanel').on('click', 'a[href="#closeOrdersResultsPanel"]', function () {
    $('#ordersResultsPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);
});
$('#ordersResultsPanel').on('click', 'a[href="#seeOrderDetails"]', function () {
    var id = $(this).attr('data-order-id');
    debugger;
    var id = $(this).attr('data-id');
    $.ajax({
        url: "/Customer/GetOrderDetailsMaster/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                           .success(function (result) {
                               $('.close_customer_orders').hide();
                               $('#customerOrders').hide();
                               $('#ordersResultsPanel').hide();
                               $('#orderDetails').show();
                               $('#orderDetailsMst').show();
                               $('#orderDetailsMst').html(result);

                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })
    $.ajax({
        url: "/Customer/GetOrderDetailsChild/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                      .success(function (result) {
                          $('.close_customer_orders').hide();
                          $('#customerOrders').hide();
                          $('#ordersResultsPanel').hide();
                          $('#orderDetails').show();
                          $('#orderDetailsChild').show();
                          $('#orderDetailsChild').html(result);

                      })
                     .error(function (xhr, ajaxOptions, thrownError) {
                     })


    $('html,body').animate({
        scrollTop: 180
    }, 400);


    $('html,body').animate({
        scrollTop: 180
    }, 400);
});
$('.order_details').on('click', 'a[href="#closeOrderDetails"]', function () {
    $(this).closest('.order_details').hide();
    $('.close_customer_orders').show();
    $('#customerOrders').show();
    $('#ordersResultsPanel').show();

    $('html,body').animate({
        scrollTop: $('#customerOrders').position().top + 400
    }, 400);
});

$('#customerOrders input')
.on('focus', function () {
    if ($(this).val() === 'Type Purchase Order #') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Purchase Order #');
    }
});

//Change Password Close
$('#tempbodySection').on('click', 'a[href="#close-panel"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardCustomer();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardCustomer();
});


$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardCustomer();
});
$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
   
    event.stopImmediatePropagation();
    RefreshDashboardCustomer();
});

function RefreshDashboardCustomer() {
   
    $('#tempParentDiv').show();
    $('#yourMessages').hide();
    $('#customerOrders').hide();
    $('#yourAlerts').hide();
    $('#ordersResultsPanel').hide();
    $('#usersChangePassword').hide();
}

$(document).ready(function () {

    $('#welcomePanel').removeClass('six_icons').addClass('four_icons');
    $('#welcomePanel').find('.tickets').remove();
    $('#welcomePanel').find('.schedule').remove();

});

//Download Orders
$('#ordersResultsPanel').on('click', 'a[href="#downloadOrder"]', function () {

    var orderid = $(this).attr("data-id");
    window.location = "Agent/GetPdffile?orderid=" + orderid


});