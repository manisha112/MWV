$('#tempbodySection').on('click', 'a[href="#openMessage"]', function () {
    $(this).removeClass("unread");
    $(this).closest('li').find('.message_content').show();
    var msgid = $(this).attr("data-id");
    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/UpdateToReadMsg/",
        data: { msgid: msgid },
        contentType: "application/json; charset=utf-8",
        success: function (data) {


        }
    });
    SetalertsMessages();
    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 75
    });
});

$('#tempbodySection').on('click', 'a[href="#openAlerts"]', function () {
    $(this).removeClass("unread");

    $(this).closest('li').find('.message_content').show();
    var msgid = $(this).attr("data-id");
    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/UpdateToReadAlerts/",
        data: { msgid: msgid },
        contentType: "application/json; charset=utf-8",
        success: function (data) {


        }
    });
    SetalertsMessages();
    $('html,body').animate({
        scrollTop: $(window).scrollTop() + 75
    });
});

$('#tempbodySection').on('click', 'a[href="#deleteCurrentMessage"]', function () {
    $(this).closest('li').remove();
});
$('#tempbodySection').on('click', 'a[href="#closeCurrentMessage"]', function () {
    $(this).closest('.message_content').hide();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() - 75
    });
});
$('#yourMessages').on('change', '#deleteSelectedMessages', function () {

    if ($(this).is(':checked')) {
        $('#deleteMessagesAlert').show();
    }
});
$('#tempbodySection').on('click', 'a[href="#deleteMessages"]', function () {


    //$('#deleteMessagesAlert').hide();

    //$('#deleteSelectedMessages').attr('checked', false);
});
$('#tempbodySection').on('click', 'a[href="#closeDeleteAlert"]', function () {
    $('input:checkbox').removeAttr('checked');
    $('#deleteMessagesAlert').hide();
});

$('#tempbodySection').on('click', 'a[href="#backtoDash"]', function () {





});
//For alerts
$('#welcomePanel').on('click', 'a[href="#showAlerts"]', function () {

    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/ShowAleartMsg/",
        contentType: "application/json; charset=utf-8",

        success: function (data) {

            $("#tempbodySection").html('');
            $("#tempbodySection").html(data);
        }
    });
    SetalertsMessages();
});
//For Messages
$('#welcomePanel').on('click', 'a[href="#showMessages"]', function () {

    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/ShowUsermsgs/",
        contentType: "application/json; charset=utf-8",

        success: function (data) {

            $("#tempbodySection").html('');
            $("#tempbodySection").html(data);
        }
    });
    SetalertsMessages();
});

$(document).ready(function () {
    
    SetalertsMessages();
});

function SetalertsMessages() {
    $.ajaxSetup({ cache: false });
    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/SetAlertIntervalsAlerts/",
        contentType: "application/json; charset=utf-8",

        success: function (data) {
            if (data != 0) {
                $("#dashboard").find("#imgAlert").trigger('chosen:updated');
                $("#dashboard").find("#imgAlert").attr("src", data);


            }

        }
    });
    $.ajaxSetup({ cache: false });
    $.ajax({
        cache: false,
        type: "GET",
        url: "/UserAlerts/SetAlertIntervalsMessages/",
        contentType: "application/json; charset=utf-8",

        success: function (data) {
            if (data != 0) {
                $("#dashboard").find("#imgMsg").trigger('chosen:updated');
                $("#dashboard").find("#imgMsg").attr("src", data);

            }

        }
    });

}
//For Messages Ratings
$('#yourMessages').on('click', 'a[href="#submitRating"]', function () {
    $(this).closest('li').find('.rate_experience').hide();
    $(this).closest('li').find('.rate_experience_confirm').show();
});

$('#individualMessages .rate_satisfaction').on('click', 'a[href="#rate"]', function () {
    var rating = $(this).attr('data-rating'),
        $radioOptions = $(this).closest('.color_bands').next('.radio_options');

    // clear current checked
    $radioOptions
        .find('input')
        .prop('checked', false);
    // select appropriate radio input
    $radioOptions
        .find('input[value="' + rating + '"]')
        .prop('checked', true);
});

//For Change Password All Module
$('#welcomePanel').on('click', 'a[href="#settings"]', function () {

    $.ajax({
        cache: false,
        type: "GET",
        url: "/Agent/ShowChangePassword/",
        contentType: "application/json; charset=utf-8",

        success: function (data) {

            $("#tempbodySection").html('');
            $("#tempbodySection").html(data);
        }
    });



    //return false;
});
//Change Password Conformation
$('#tempbodySection').on('click', 'a[href="#changePassword"]', function () {
    //$("#tempbodySection").find('#ChangePasswordinner').hide();
    //$("#tempbodySection").find("#changePasswordSuccess").show();
   
    var Result = {
        OldPassword: $('#OldPassword').val(),
        NewPassword: $('#NewPassword').val(),
        ConfirmPassword: $('#ConfirmPassword').val(),
    };
    var stringReqdata = JSON.stringify(Result);
    $.ajax({
        //async: true,
        type: "POST",
        url: "/Agent/ShowChangePassword/",
        data: stringReqdata,
        dataType: "html",
        context: document.body,
        contentType: 'application/json; charset=utf-8'
    })
                     .success(function (data) {
                         $("#tempbodySection").find('#ChangePasswordinner').html('');
                         $("#tempbodySection").find('#ChangePasswordinner').html(data);
                         var count = $(".validation-summary-errors ul").children().length;
                         var count1 = $("#tempbodySection").find("#invalidErrors ul").children().length;
                         if (count == 0) {
                            
                             $("#tempbodySection").find("#changePasswordSuccess").show();
                         }
                         if (count1 == 1) {
                             $("#tempbodySection").find("#changePasswordSuccess").hide();
                         }
                       
                     })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })

    
});

////Change Password Close
//$('#tempbodySection').on('click', 'a[href="#close-panel"]', function () {
//    $("#tempbodySection").find('#ChangePasswordinner').hide();
//    $("#tempbodySection").find("#changePasswordSuccess").hide();
//    window.location = "Home/Index";

//});
//$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function () {
//    $("#tempbodySection").find('#ChangePasswordinner').hide();
//    $("#tempbodySection").find("#changePasswordSuccess").hide();
//    window.location = "Home/Index";
//});

//$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function () {
//    $("#tempbodySection").find('#ChangePasswordinner').hide();
//    $("#tempbodySection").find("#changePasswordSuccess").hide();
//    window.location = "Home/Index";
//});
 