
$('#incomingVehicles').on('click', 'a[href="#incomingVehicles"]', function () {
   // alert("Hello");
    //Addede by manisha

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
                    })
                        .error(function (xhr, ajaxOptions, thrownError) {
                        })

    //if ($('#incomingVehiclesPanel').is(':hidden')) {
    //    $('#arrivedVehiclesPanel').hide();
    //    $('#incomingVehiclesPanel').show();
    //} else {
    //    $('#incomingVehiclesPanel').hide();
    //}

    return false;
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
                        }

                        //if ($('#arrivedvehiclespanel').is(':hidden')) {
                        //    $('#incomingvehiclespanel').hide();
                          // $('#arrivedvehiclespanel').html(result);
                          //  $('#arrivedvehiclespanel').show();
                        //}

                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })


    if ($('#arrivedVehiclesPanel').is(':hidden')) {
        $('#incomingVehiclesPanel').hide();
        $('#arrivedVehiclesPanel').show();
    }

    return false;
});

$('#incomingVehicles').on('click', 'a[href="#closeIncomingVehicles"]', function () {
    $('#incomingVehiclesPanel').hide();

    return false;
});
$('#incomingVehicles').on('change', 'input[type="checkbox"]', function () {
    $(this).closest('li').find('.vehicle_attention').show();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 260
    });
});
$('.vehicle_attention.scheduled').on('click', 'a[href="#closeVehicleAttention"]', function () {
    $(this).closest('.vehicle_attention').hide();
    $(this).closest('li').find('input[type="checkbox"]').prop('checked', false);

    return false;
});
$('.vehicle_attention').on('click', 'a[href="#markVehicleArrived"]', function () {
    $(this).closest('li').remove();

    return false;
});

$('#arrivedVehicles').on('click', 'a[href="#closeArrivedVehicles"]', function () {
    $('#arrivedVehiclesPanel').hide();

    return false;
});
$('.vehicle_attention.arrived').on('click', 'a[href="#closeVehicleAttention"]', function () {
    $(this).closest('.vehicle_attention').hide();
    $(this).closest('li').find('input[type="checkbox"].mark_departed').prop('checked', false);

    return false;
});
$('#arrivedVehicles').on('change', 'input[type="checkbox"].mark_departed', function () {
    $(this).closest('li').find('.vehicle_attention').show();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 260
    });
});
$('.vehicle_attention').on('click', 'a[href="#markVehicleDeparted"]', function () {
    $(this).closest('li').remove();

    return false;
});