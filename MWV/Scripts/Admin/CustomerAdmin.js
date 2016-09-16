//---COMMON FUNCTION...

function CheckValidation() {
    $('#ErrorMsgs').html('');
    var agent=$('#lstAgents option:selected').val();
    if ($('name').val() == "Enter name(upto 100 char)" || $('#name').val().trim() == "" || $('#name').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter name!</p>");
        return false;
    }
    else if ($('#firstname').val() == "Enter First Name(upto 100 char)" || $('#firstname').val().trim() == "" || $('#firstname').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter First Name!</p>");
        return false;
    }
    else if (EmailValidation($('#email').val()) == false || $('#email').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Email Address!</p>");
        return false;
    }
    else if (agent == "")
    {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select Agent!</p>");
    }
   else if ($('city').val() == "Enter city(upto 50 char)" || $('#city').val().trim() == "" || $('#city').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter city!</p>");
        return false;
    }
    else {
        return true;
    }
}

function EmailValidation(email) {

    //var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    //if ($('#email').val() == "Enter email(upto 100 char)" || $('#email').val().trim() == "" || $('#email').val() == "") {
    //    $('#ErrorMsgs').html("<p class='error-msg'>Please Enter email!</p>");
    //    return false;
    //}
    //else if (filter.test(email)) {
    //    return true;
    //}
    //else {
    //    $('#ErrorMsgs').html("<p class='error-msg'>Email address is not valid!</p>");
    //    return false;
    //}

    var sEmail = $("#email").val();
    var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (filter.test(sEmail) || $("#email").val() == "" || $('#email').val() == "Enter email(upto 100 char)") {
        return true;
    }
    else {
        $('#ErrorMsgs').html("<p class='error-msg'>Email address is not valid!</p>");
        return false;
    }

}


//-------------------FOR EDIT customer----


$('#btnEdiCust').click(function () {
     
    var CustomerIDToEdit = $('#CustomerIDToEdit').val();
    var name = $('#name').val();
    var email = $('#email').val();
    var phone = $('#phone').val();
    var address1 = $('#address1').val();
    var address2 = $('#address2').val();
    var address3 = $('#address3').val();
    var city = $('#city').val();
    var pincode = $('#pincode').val();
    var state = $('#state').val();
    var country = $('#country').val();
    var fax = $('#fax').val();
    var firstname = $('#firstname').val();
    var agent_id = $('#lstAgents option:selected').val();
    var lastname = $('#lastname').val();

    // var status = $('#status').val();

    var remarks = $('#remarks').val();
    var msg = "";
    
    if (CheckValidation() == true) {
        var datatosend = {
            CustomerIDToEdit: CustomerIDToEdit,
            name: name,
            email: email,
            phone: phone,
            address1: address1,
            address2: address2,
            address3: address3,
            city: city,
            pincode: pincode,
            state: state,
            country: country,
            fax: fax,
            //remarks: remarks,
            firstname: firstname,
            agent_id: agent_id,
            lastname: lastname


        }

        $.ajax({
            url: "/CustomerAdmin/EditCustomer/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'Customer details Updated successfully.!';
                          window.location.href = "/CustomerAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.Customer details are already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete customer--------------------------


//$('#btnDeleteCust').click(function () {
//    //
//    var CustomerIDToDel = $('#CustomerIDToDel').val();
//    var msg = "";
//    var datatosend = {
//        CustomerIDToDel: CustomerIDToDel
//    }

//    $.ajax({
//        url: "/CustomerAdmin/DeleteCustomer/",
//        data: datatosend,
//        contentType: "application/html; charset=utf-8",
//        type: "GET",
//        dataType: 'html'
//    })
//              .success(function (result) {
//                  if (result == "True") {
//                      msg = 'Customer Details Deleted successfully.!';
//                      window.location.href = "/CustomerAdmin/Index?msg=" + msg;
//                  }
//                  else {
//                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not Delete.Customer details are already in use.!' + "</p>");
//                  }

//              })
//             .error(function (xhr, ajaxOptions, thrownError) {
//             })

//});

///-------------------------------------------------------

//For Approve Customer Login
$('input[type = checkbox]').change(function () {

    $("#loginErrormsg").html('');
    $("#ErrorMsgs").html('');
    if ($(this).is(":checked")) {
        var CustomerIDToEdit = $('#CustomerIDToEdit').val();
        var name = $('#name').val();
        var email = $('#email').val();
        var phone = $('#phone').val();
        var address1 = $('#address1').val();
        var city = $('#city').val();
        var pincode = $('#pincode').val();
        var state = $('#state').val();
        var country = $('#country').val();
        var remarks = $('#remarks').val();
        var firstname = $('#firstname').val();
        var agent_id = $('#lstAgents option:selected').val();
        var msg = "";
        var result = SetPasswordValidations(name, email, firstname, agent_id);
        if (CheckValidation() == true && result == true) {
            var datatosend = {
                location: CustomerIDToEdit,  //assume location as customer id
                name: name,
                Email: email,
                mobile: phone,
                address: address1,
                city: city,
                state: state,
                Password: $('#txtPassword').val(),
                ConfirmPassword: $('#txtConfirmPassword').val(),

            }
            var stringReqdata = JSON.stringify(datatosend);
            $.ajax({
                //async: true,
                type: "POST",
                url: "/CustomerAdmin/CustomerRoles/",
                data: stringReqdata,
                contentType: "application/json; charset=utf-8",
            })
            .success(function (result) {

                if (result == true) {
                    $("#loginErrormsg").html('');
                    $("#loginErrormsg").html("Customer Login Sucessfully Enabled!");
                    $('input[type = checkbox]').prop('checked', true);
                }
                else {
                    $("#loginErrormsg").html('');
                    $("#loginErrormsg").html(result);
                    $('input[type = checkbox]').prop('checked', false);
                }


            })
                 .error(function (xhr, ajaxOptions, thrownError) {

                 })
        }
    }
    else {
        var CustomerIDToEdit = $('#CustomerIDToEdit').val();
        var datatosend = {
            CustomerIDToEdit: CustomerIDToEdit
        }

        $.ajax({
            url: "/CustomerAdmin/RemoveCustomerLogin/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
            .success(function (result) {

                $("#loginErrormsg").html('');
                $("#loginErrormsg").html("Customer Login Sucessfully Disable!");
                $('input[type = checkbox]').prop('checked', false);

            })
           .error(function (xhr, ajaxOptions, thrownError) {
           })

    }

});

//Function For Password Validation
function SetPasswordValidations(name, email, firstname, agent_id) {
    re0 = /[0-9]/;
    re1 = /[a-z]/;
    re2 = /[A-Z]/;
    re3 = /[!"#$%&'()*+.\/:;<=>?@\[\\\]^_`{|}~-]/;
    re4 = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (name == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Name Cannot Be Empty!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    if (firstname == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('First Name Cannot Be Empty!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    if (agent_id == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Please Select Agent!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else if (email == "") {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Email Cannot Be Empty!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else if (!re4.test(email)) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Invalid Email Address!');


    }

    else if ($('#txtPassword').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password Cannot Be Empty!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else if ($('#txtConfirmPassword').val().length === 0) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Confirm Password Cannot Be Empty!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }

    else if ($('#txtPassword').val() != $('#txtConfirmPassword').val()) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password and conformation Password does not match!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else if ($('#txtPassword').val().length < 6) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must contain at least six characters!');
        $('input[type = checkbox]').prop('checked', false);
        $('#txtPassword').focus();
        $('#txtConfirmPassword').focus();
        return false;
    }
    else if ($('#txtPassword').val() == name || $('#txtConfirmPassword').val() == name) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Name!');
        $('#txtPassword').focus();
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else if ($('#txtPassword').val() == email || $('#txtConfirmPassword').val() == email) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password must be different from Email!');
        $('#txtPassword').focus();
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }


    else if (!re0.test($('#txtPassword').val()) || !re0.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('password must contain at least one number (0-9)!');
        $('input[type = checkbox]').prop('checked', false);
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
        $('input[type = checkbox]').prop('checked', false);

        return false;
    }

    else if (!re3.test($('#txtPassword').val()) || !re3.test($('#txtConfirmPassword').val())) {
        $("#loginErrormsg").html('');
        $("#loginErrormsg").html('Password atleast One Special Charter!');
        $('input[type = checkbox]').prop('checked', false);
        return false;
    }
    else {
        return true;

    }

}

//Create Customer
$('#createCustomerDiv').on('click', 'a[href="#Create"]', function () {
    $.ajax({
        cache: false,
        type: "GET",
        url: "/CustomerAdmin/Create/",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#createCustomerDiv").html('');
            $("#createCustomerDiv").html(data);
        }

    });
});

//SAve Customer
$('#createCustomerDiv').on('click', 'a[href="#CreateCustomer"]', function () {
    
    var name = $('#name').val();
    var email = $('#email').val();
    var firstname = $('#firstname').val();
    var agent_id = $('#lstAgents option:selected').val();
    var lastname = $('#lastname').val();
    var phone = $('#phone').val();
    var address1 = $('#address1').val();
    var address2 = $('#address2').val();
    var address3 = $('#address3').val();
    var city = $('#city').val();
    var pincode = $('#pincode').val();
    var state = $('#state').val();
    var country = $('#country').val();
    var fax = $('#fax').val();
    var result = SetPasswordValidations(name, email, firstname, agent_id);
    if (result == true) {
        var datatosend = {
            firstname: firstname,
            lastname: lastname,
            name: name,
            email: email,
            phone: phone,
            agent_id: agent_id,
            address1: address1,
            address2: address2,
            address3: address3,
            city: city,
            pincode: pincode,
            state: state,
            country: country,
            fax: fax,
        };
        var datatosendCreate = {
            name: $('#name').val(),
            Email: $('#email').val(),
            mobile: $('#phone').val(),
            address: $('#address1').val(),
            city: $('#city').val(),
            state: $('#state').val(),
            Password: $('#txtPassword').val(),
            ConfirmPassword: $('#txtConfirmPassword').val(),

        };
        var stringReqdataCreate = JSON.stringify(datatosendCreate);//Create User in Aspnettable
        var stringReqdata = JSON.stringify(datatosend);//add Entry in customer tables
        $.ajax({
            //async: true,
            type: "POST",
            url: "/CustomerAdmin/CustomerRoles/",
            data: stringReqdataCreate,
            contentType: "application/json; charset=utf-8",
        })
                  .success(function (result) {
                      if (result == true) {
                          $.ajax({
                              //async: true,
                              type: "POST",
                              url: "/CustomerAdmin/Create/",
                              data: stringReqdata,
                              contentType: "application/json; charset=utf-8",
                          })
         .success(function (result) {
             msg = 'Customer details Added successfully.!';
             window.location.href = "/CustomerAdmin/Index?msg=" + msg;


         })
        .error(function (xhr, ajaxOptions, thrownError) {
        })
                      }
                      else {
                          $("#loginErrormsg").html(result);
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});