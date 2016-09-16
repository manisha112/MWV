//---COMMON FUNCTION...

function CheckShadeValidation() {
    var ShadeCode = $('#ShadeCode').val();
    var ShadeDescription = $('#ShadeDescription').val();

    if ($('#ShadeCode').val() == "Enter Shade Code(upto 15 char)" || $('#ShadeCode').val().trim() == "" || $('#ShadeCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter ShadeCode!</p>");
        return false;
    }
    else if ($('#ShadeCode').val().length > 15) {
        $('#ErrorMsgs').html("<p class='error-msg'>ShadeCode should contain 15 characters.!</p>");
        return false;
    }
    else if ($('#ShadeDescription').val() == "Enter Shade Description(upto 100 char)" || $('#ShadeDescription').val().trim() == "" || $('#ShadeDescription').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Shade Description!</p>");
        return false;
    }
    else if ($('#ShadeDescription').val().length > 100) {
        $('#ErrorMsgs').html("<p class='error-msg'>Shade Description should contain 100 characters.!</p>");
        return false;
    }

    else {
        return true;
    }
}



///// ------------------FOR ADD Shade-----------------------

$('#btnAddShade').click(function () {
    // 
    var ShadeCode = $('#ShadeCode').val();
    var ShadeDescription = $('#ShadeDescription').val();
    var msg = "";

    if (CheckShadeValidation() == true) {
        var datatosend = {
            ShadeCode: ShadeCode,
            ShadeDescription: ShadeDescription
        }

        $.ajax({
            url: "/ShadeAdmin/AddShade/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "True") {
                          msg = 'Shade details added successfully.!';
                          window.location.href = "/ShadeAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Shade Code already exists.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});


//-------------------FOR EDIT shade----

$('#btnEditShade').click(function () {
    // 
    var ShadeCodeToEdit = $('#hdnShadeCodeToEdit').val();
    var ShadeCode = $('#ShadeCode').val();
    var ShadeDescription = $('#ShadeDescription').val();
    var msg = "";

    if (CheckShadeValidation() == true) {
        var datatosend = {
            ShadeCodeToEdit: ShadeCodeToEdit,
            ShadeCode: ShadeCode,
            ShadeDescription: ShadeDescription
        }

        $.ajax({
            url: "/ShadeAdmin/EditShade/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'Shade details updated successfully.!';
                          window.location.href = "/ShadeAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.Shade Code is already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete Shade--------------------------


$('#btnDeleteShade').click(function () {
    //
    var msg = "";
    var ShadeCode = $('#ShadeCode').val();

    var datatosend = {
        ShadeCode: ShadeCode
    }

    $.ajax({
        url: "/ShadeAdmin/DeleteShade/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  if (result == "True") {
                      msg = 'Shade details deleted successfully.!';
                      window.location.href = "/ShadeAdmin/Index?msg=" + msg;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not delete.Shade Code is already in use.!' + "</p>");
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })

});

///-------------------------------------------------------

