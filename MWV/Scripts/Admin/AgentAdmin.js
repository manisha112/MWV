$('#MasterDiv').on('click', 'a[href="#Create"]', function () {
    $("#ErrorMsgAgent").html('');

    $.ajax({
        cache: false,
        type: "GET",
        url: "/AgentAdmin/Create/",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#MasterDiv").html('');
            $("#MasterDiv").html(data);

        }

    });
});

$('#MasterDiv').on('click', 'a[href="#SaveAgnet"]', function () {
    debugger;
    $("#ErrorMsgAgent").html('');

    var Result = {
        name: $('#name').val(),
        firstname: $('#firstname').val(),
        lastname: $('#lastname').val(),
        email: $('#email').val(),
        mobile: $('#mobile').val(),
        landline: $('#landline').val(),
        address: $('#address').val(),
        address2: $('#address2').val(),
        address3: $('#address3').val(),
        city: $('#city').val(),
        state: $('#state').val(),
        pincode: $('#pincode').val(),
        credit_days: $('#credit_days').val(),
        credit_limit: $('#credit_limit').val(),
        password: $('#txtPassword').val(),
        ConformPassowrd: $('#txtConfirmPassword').val(),

    };
    var name = $('#name').val();
    var email = $('#email').val();
    var password = $('#txtPassword').val();
    var ConformPassowrd = $('#txtConfirmPassword').val();
    var firstname = $('#firstname').val();
    var mobile = $('#mobile').val();
    var city = $('#city').val();
    var result = SetPasswordValidations(name, email, password, ConformPassowrd, firstname, city, mobile)
    if (result == true) {
        debugger;
        $("#loginErrormsg").html('');
        var stringReqdata = JSON.stringify(Result);
        //Add Login in Aspnet Table
        $.ajax({
            //async: true,
            type: "POST",
            url: "/AgentAdmin/AgentRoles/",
            data: stringReqdata,
            contentType: "application/json; charset=utf-8",
        })
          .success(function (result) {
              //Save Record To Agent Table if result true

              if (result == true) {
                  $.ajax({
                      //async: true,
                      type: "POST",
                      url: "/AgentAdmin/Create/",
                      data: stringReqdata,
                      dataType: "html",
                      context: document.body,
                      contentType: 'application/json; charset=utf-8'
                  })
                     .success(function (data) {

                         msg = 'Agent details Added successfully!';
                         window.location.href = "/AgentAdmin/Index?msg=" + msg;

                     })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })
              }
              else {

                  $("#loginErrormsg").html('');
                  $("#loginErrormsg").html(result);
              }

          })
               .error(function (xhr, ajaxOptions, thrownError) {

               })

    }
});

$('#MasterDiv').on('click', 'a[href="#EditAgnt"]', function () {
    debugger;
    $("#ErrorMsgAgent").html('');

    var Result = {
        id: $(this).attr('id'),
        agent_id: $(this).attr('id'),
        name: $('#name').val(),
        firstname: $('#firstname').val(),
        lastname: $('#lastname').val(),
        email: $('#email').val(),
        mobile: $('#mobile').val(),
        landline: $('#landline').val(),
        address: $('#address').val(),
        address2: $('#address2').val(),
        address3: $('#address3').val(),
        city: $('#city').val(),
        state: $('#state').val(),
        pincode: $('#pincode').val(),
        credit_days: $('#credit_days').val(),
        credit_limit: $('#credit_limit').val(),
        password: $('#txtPassword').val(),
        ConformPassowrd: $('#txtConfirmPassword').val(),

    };
    var name = $('#name').val();
    var email = $('#email').val();
    var firstname = $('#firstname').val();
    var city = $('#city').val();

    var result = SetEditValidation(name, email, firstname, city, mobile)
    if (result == true) {
        $("#loginErrormsg").html('');
        var stringReqdata = JSON.stringify(Result);
        $.ajax({
            //async: true,
            type: "POST",
            url: "/AgentAdmin/EditAgentLogin/",
            data: stringReqdata,
            contentType: "application/json; charset=utf-8",
        })
        .success(function (result) {
            debugger;
            if (result != true) {
                $("#loginErrormsg").html('');
                $("#loginErrormsg").html(result);
            }
            else {


                $.ajax({
                    //async: true,
                    type: "POST",
                    url: "/AgentAdmin/Edit/",
                    data: stringReqdata,
                    dataType: "html",
                    context: document.body,
                    contentType: 'application/json; charset=utf-8'
                })
                         .success(function (data) {
                             debugger;
                             if (data == "false") {
                                 var email = $('#email').val();
                                 var newmsg = email + " is already taken!"
                                 $("#loginErrormsg").html('');
                                 $("#loginErrormsg").html(newmsg);

                                 // window.location.href = "/AgentAdmin/Index?msg=" + msg;
                             }
                             else {
                                 var msg = 'Changes has been Saved!';
                                 window.location.href = "/AgentAdmin/Index?msg=" + msg;

                             }




                         })
                        .error(function (xhr, ajaxOptions, thrownError) {
                        })

            }

        })

    }
});


function SetPasswordValidations(name, email, password, ConformPassowrd, firstname, city, mobile) {
    re0 = /[0-9]/;
    re1 = /[a-z]/;
    re2 = /[A-Z]/;
    re3 = /[!"#$%&'()*+.\/:;<=>?@\[\\\]^_`{|}~-]/;
    re4 = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (name == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Name Cannot Be Empty!');


    }
    else if (firstname == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('First Name Cannot Be Empty!');

    }
    else if (city == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('City Cannot Be Empty!');

    }
    else if (email == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Email Cannot Be Empty!');

    }
    else if (mobile == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Mobile No Cannot Be Empty!');

    }
    else if (!re4.test(email)) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Invalid Email Address!');


    }

    else if ($('#txtPassword').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password Cannot Be Empty!');


    }
    else if ($('#txtConfirmPassword').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Confirm Password Cannot Be Empty!');


    }

    else if ($('#txtPassword').val() != $('#txtConfirmPassword').val()) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password and conformation Password does not match!');

        return false;
    }
    else if ($('#txtPassword').val().length < 6) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must contain at least six characters!');

        $('#txtPassword').focus();
        $('#txtConfirmPassword').focus();
        return false;
    }
    else if ($('#txtPassword').val() == name || $('#txtConfirmPassword').val() == name) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Name!');
        $('#txtPassword').focus();

        return false;
    }
    else if ($('#txtPassword').val() == email || $('#txtConfirmPassword').val() == email) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Email!');
        $('#txtPassword').focus();

        return false;
    }


    else if (!re0.test($('#txtPassword').val()) || !re0.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one number (0-9)!');

        return false;
    }

    else if (!re1.test($('#txtPassword').val()) || !re1.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one lowercase letter (a-z)!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }

    else if (!re2.test($('#txtPassword').val()) || !re2.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one uppercase letter (A-Z)!');


        return false;
    }

    else if (!re3.test($('#txtPassword').val()) || !re3.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password atleast One Special Charter!');

        return false;
    }
    else {
        return true;

    }

}


function SetEditValidation(name, email, firstname, city, mobile) {

    if (name == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Name Cannot Be Empty!');
        return false;

    }
    else if (email == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Email Cannot Be Empty!');
        return false;
    }
    else if (firstname == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Firstname Cannot Be Empty!');
        return false;
    }
    else if (mobile == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Mobile No Cannot Be Empty!');
        return false;
    }
    else if (city == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('City Cannot Be Empty!');
        return false;
    }
    else {
        return true;

    }

}

$('#MasterDiv').on('click', 'a[href="#Edit"]', function () {
    var id = $(this).attr('id');
    $.ajax({
        cache: false,
        type: "GET",
        url: "/AgentAdmin/Edit/" + id,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#MasterDiv").html('');
            $("#MasterDiv").html(data);

        }

    });
});

//Function For Create Login
$('#MasterDiv').on('click', 'a[href="#Delete"]', function () {
    var id = $(this).attr('id');
    $.ajax({
        cache: false,
        type: "GET",
        url: "/AgentAdmin/Delete/" + id,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#MasterDiv").html('');
            $("#MasterDiv").html(data);

        }

    });
});

$('#MasterDiv').on('click', 'a[href="#DeleteAgnt"]', function () {
    var id = $(this).attr('id');
    $.ajax({
        cache: false,
        type: "POST",
        url: "/AgentAdmin/DeleteConfirmed/" + id,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            window.location.href = "/AgentAdmin/Index?msg=" + data;

        }

    });
});
$('#MasterDiv').on('click', 'a[href="#Reset"]', function () {
    var id = $(this).attr('id');
    $.ajax({
        cache: false,
        type: "GET",
        url: "/AgentAdmin/resetPassword/" + id,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#MasterDiv").html('');
            $("#MasterDiv").html(data);

        }

    });
});
$('#MasterDiv').on('click', 'a[href="#ResetPassword"]', function () {
    var Result = {
        id: $("#hfid").val(),
        name: $('#name').val(),
        Email: $('#Email').val(),
        Password: $('#Password').val(),
        ConfirmPassword: $('#ConfirmPassword').val(),

    };
    var name = $('#name').val();
    var Email = $('#Email').val();
    var Password = $('#Password').val();
    var ConfirmPassword = $('#ConfirmPassword').val();

    var result = SetResetPasswordValidations(name, Email, Password, ConfirmPassword)
    if (result == true) {
        var stringReqdata = JSON.stringify(Result);
        var id = $(this).attr('id');
        $.ajax({
            cache: false,
            type: "POST",
            url: "/AgentAdmin/resetPassword/" + id,
            contentType: "application/json; charset=utf-8",
            data: stringReqdata,
            success: function (data) {

                window.location.href = "/AgentAdmin/Index?msg=" + data;
            }

        });
    }

});

function SetResetPasswordValidations(name, email, password, ConformPassowrd) {
    re0 = /[0-9]/;
    re1 = /[a-z]/;
    re2 = /[A-Z]/;
    re3 = /[!"#$%&'()*+.\/:;<=>?@\[\\\]^_`{|}~-]/;
    re4 = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (name == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Name Cannot Be Empty!');


    }

    else if (email == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Email Cannot Be Empty!');

    }
    else if (!re4.test(email)) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Invalid Email Address!');


    }

    else if ($('#Password').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password Cannot Be Empty!');


    }
    else if ($('#ConfirmPassword').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Confirm Password Cannot Be Empty!');


    }

    else if ($('#Password').val() != $('#ConfirmPassword').val()) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password and conformation Password does not match!');

        return false;
    }
    else if ($('#Password').val().length < 6) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must contain at least six characters!');

        $('#Password').focus();
        $('#ConfirmPassword').focus();
        return false;
    }
    else if ($('#Password').val() == name || $('#ConfirmPassword').val() == name) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Name!');
        $('#Password').focus();

        return false;
    }
    else if ($('#Password').val() == email || $('#ConfirmPassword').val() == email) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Email!');
        $('#Password').focus();

        return false;
    }


    else if (!re0.test($('#Password').val()) || !re0.test($('#ConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one number (0-9)!');

        return false;
    }

    else if (!re1.test($('#Password').val()) || !re1.test($('#ConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one lowercase letter (a-z)!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }

    else if (!re2.test($('#Password').val()) || !re2.test($('#ConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one uppercase letter (A-Z)!');


        return false;
    }

    else if (!re3.test($('#Password').val()) || !re3.test($('#ConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password atleast One Special Charter!');

        return false;
    }
    else {
        return true;

    }

}

