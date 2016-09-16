//---COMMON FUNCTION...

function CheckGsmValidation() {
    var Gsm = $('#GsmCode').val();
    var GsmDescription = $('#GsmDescription').val();

    if ($('#GsmCode').val() == "Enter Gsm Code(upto 15 char)" || $('#GsmCode').val().trim() == "" || $('#GsmCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter GsmCode!</p>");
        return false;
    }
    else if ($('#GsmCode').val().length > 15) {
        $('#ErrorMsgs').html("<p class='error-msg'>GsmCode should contain 15 characters.!</p>");
        return false;
    }
    else if ($('#GsmDescription').val() == "Enter Gsm Description(upto 100 char)" || $('#GsmDescription').val().trim() == "" || $('#GsmDescription').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Gsm Description!</p>");
        return false;
    }
    else if ($('#GsmDescription').val().length > 100) {
        $('#ErrorMsgs').html("<p class='error-msg'>Gsm Description should contain 100 characters.!</p>");
        return false;
    }

    else {
        return true;
    }
}



///// ------------------FOR ADD Gsm-----------------------


$('#btnAddGsm').click(function () {
    // 
    var GsmCode = $('#GsmCode').val();
    var GsmDescription = $('#GsmDescription').val();
    var msg = "";

    if (CheckGsmValidation() == true) {
        var datatosend = {
            GsmCode: GsmCode,
            GsmDescription: GsmDescription
        }

        $.ajax({
            url: "/GsmAdmin/AddGsm/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                     // alert(result);
                      if (result == "True") {
                          msg = 'Gsm details added successfully.!';
                          window.location.href = "/GsmAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Gsm Code already exists.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});

//-------------------FOR EDIT Gsm----


$('#btnEditGsm').click(function () {
   // 
    var GsmCodeToEdit = $('#GsmCodeToEdit').val();
    var GsmCode = $('#GsmCode').val();
    var GsmDescription = $('#GsmDescription').val();
    var msg = "";

    if (CheckGsmValidation() == true) {
        var datatosend = {
            GsmCodeToEdit: GsmCodeToEdit,
            GsmCode: GsmCode,
            GsmDescription: GsmDescription
        }

        $.ajax({
            url: "/GsmAdmin/EditGsm/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'Gsm details updated successfully.!';
                          window.location.href = "/GsmAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.Gsm Code is already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete Gsm--------------------------


$('#btnDeleteGsm').click(function () {
     //
    
     var GsmCode = $('#GsmCode').val();
     var msg = "";

     var datatosend = {
         GsmCode: GsmCode
     }
   
        $.ajax({
            url: "/GsmAdmin/DeleteGsm/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'Gsm Deleted successfully.!';
                          window.location.href = "/GsmAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not delete.Gsm Code is already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })

});

///-------------------------------------------------------

