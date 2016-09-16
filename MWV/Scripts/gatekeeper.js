

$('#incomingVehicles').on('click', 'a[href="#incomingVehicles"]', function () {

    $.ajax({
        url: "/GateKeeper/TruckInward/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {

                        if ($('#incomingVehiclesPanel').is(':hidden')) {
                            $('#arrivedVehiclesPanel').hide();
                            $('#incomingVehiclesPanel').html(result);
                            $('#incomingVehiclesPanel').show();
                        } else {
                            $('#incomingVehiclesPanel').hide();
                        }

                        return false;
                    })
                        .error(function (xhr, ajaxOptions, thrownError) {
                        })


});
$('#arrivedVehicles').on('click', 'a[href="#arrivedVehicles"]', function () {
    $.ajax({
        url: "/GateKeeper/TruckOutward/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                  .success(function (result) {
                      if ($('#arrivedVehiclesPanel').is(':hidden')) {
                          $('#incomingVehiclesPanel').hide();
                          $('#arrivedVehiclesPanel').show();
                          $('#arrivedVehiclesListDiv').show();
                          $('#arrivedVehiclesListDiv').html(result);

                      } else {
                          $('#arrivedVehiclesPanel').hide();
                      }
                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })



});

$('#incomingVehicles').on('click', 'a[href="#closeIncomingVehicles"]', function () {
    $('#incomingVehiclesPanel').hide();


});
$('#incomingVehicles').on('change', 'input[type="checkbox"]', function () {
    if ($(this).prop("checked") == true) {
        $(this).closest('li').find('.vehicle_attention').show();
        $('html,body').animate({
            scrollTop: $(window).scrollTop() + 260
        });
    }
    else {
        $(this).closest('li').find('.vehicle_attention').hide();
        $('html,body').animate({
            scrollTop: $(window).scrollTop() - 260
        });
    }
    
});
$('.fix-overflow').on('click', 'a[href="#closeArrivedVehicles"]', function () {
    $('#arrivedVehiclesPanel').hide();

    return false;

});
//Commented By manisha
//$('.vehicle_attention.scheduled').on('click', 'a[href="#closeVehicleAttention"]', function () {
//    $(this).closest('.vehicle_attention').hide();
//    $(this).closest('li').find('input[type="checkbox"]').prop('checked', false);

//  
//});
//$('.vehicle_attention').on('click', 'a[href="#markVehicleArrived"]', function () {
//    $(this).closest('li').remove();

//  
//});

$('#arrivedVehiclesPanel').on('click', 'a[href="#closeArrivedVehicles"]', function () {
    $('#arrivedVehiclesPanel').hide();


});

$('.vehicle_attention.arrived').on('click', 'a[href="#closeVehicleAttention"]', function () {
    $(this).closest('.vehicle_attention').hide();
    $(this).closest('li').find('input[type="checkbox"].mark_departed').prop('checked', false);


});
$('#arrivedVehicles').on('change', 'input[type="checkbox"].mark_departed', function () {
    if ($(this).prop("checked") == true) {
        $(this).closest('li').find('.vehicle_attention').show();
        $('html,body').animate({
            scrollTop: $(window).scrollTop() + 260
        });
    }
    else {
        $(this).closest('li').find('.vehicle_attention').hide();
        $('html,body').animate({
            scrollTop: $(window).scrollTop() - 260
        });
    }
  
});
$('.vehicle_attention').on('click', 'a[href="#markVehicleDeparted"]', function () {
    $(this).closest('li').remove();


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
 
$(document).ready(function () {

    $('#welcomePanel').removeClass('six_icons').addClass('four_icons');
    $('#welcomePanel').find('.tickets').remove();
    $('#welcomePanel').find('.schedule').remove();
    $('#welcomePanel').find("a[href='#settings']").removeAttr("href");
    $('#welcomePanel').find("a[href='#showMessages']").removeAttr("href");
    $('#welcomePanel').find("a[href='#showAlerts']").removeAttr("href");
});



