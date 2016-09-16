//---COMMON FUNCTION...

function CheckCoreValidation() {
    var CoreCode = $('#CoreCode').val();
    var CoreDescription = $('#CoreDescription').val();

    if ($('#CoreCode').val() == "Enter Core Code(upto 15 char)" || $('#CoreCode').val().trim() == "" || $('#CoreCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Core Code!</p>");
        return false;
    }
    else if ($('#CoreCode').val().length > 15) {
        $('#ErrorMsgs').html("<p class='error-msg'>Core Code should contain 15 characters.!</p>");
        return false;
    }
    else if ($('#CoreDescription').val() == "Enter Core Description(upto 100 char)" || $('#CoreDescription').val().trim() == "" || $('#CoreDescription').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Core Description!</p>");
        return false;
    }
    else if ($('#CoreDescription').val().length > 100) {
        $('#ErrorMsgs').html("<p class='error-msg'>Core Description should contain 100 characters.!</p>");
        return false;
    }

    else {
        return true;
    }
}

//function ClearBFs(BFActionType) {
//    if (BFActionType == "AddBF") {
//        $('#BFCode').val('');
//    }
//    $('#BFDescription').val('');
//    $('#ErrorMsgs').html('');
//}


///// ------------------FOR ADD core-----------------------

//$('#btnCancel').click(function () {
//    // ClearBFs("AddBF");
//});

$('#btnAddCore').click(function () {
    // 
    var CoreCode = $('#CoreCode').val();
    var CoreDescription = $('#CoreDescription').val();
    var msg = "";
   // var ActionType;

    if (CheckCoreValidation() == true) {
        var datatosend = {
            CoreCode: CoreCode,
            CoreDescription: CoreDescription
        }

        $.ajax({
            url: "/CoreAdmin/AddCore/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "True") {
                          msg = 'Core details added successfully.!';
                          window.location.href = "/CoreAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Core Code already exists.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }


});



//-------------------FOR EDIT core---------------------


$('#btnEditCore').click(function () {
    // 
    var CoreCodeToEdit = $('#CoreCodeToEdit').val();
    var CoreCode = $('#CoreCode').val();
    var CoreDescription = $('#CoreDescription').val();
    var msg = "";

    if (CheckCoreValidation() == true) {
        var datatosend = {
            CoreCodeToEdit: CoreCodeToEdit,
            CoreCode: CoreCode,
            CoreDescription: CoreDescription
        }

        $.ajax({
            url: "/CoreAdmin/EditCore/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'Core details Updated successfully.!';
                          window.location.href = "/CoreAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.Core Code is already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete core--------------------------


$('#btnDeleteCore').click(function () {
    //

    var CoreCode = $('#CoreCode').val();
    var msg = "";

    var datatosend = {
        CoreCode: CoreCode
    }

    $.ajax({
        url: "/CoreAdmin/DeleteCore/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  if (result == "True") {
                      msg = 'Core Deleted successfully.!';
                      window.location.href = "/CoreAdmin/Index?msg=" + msg;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not delete.Core Code is already in use.!' + "</p>");
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })

});

///-------------------------------------------------------

