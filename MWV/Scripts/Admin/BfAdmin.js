//---COMMON FUNCTION...

function CheckBFValidation() {
    var BFCode = $('#BFCode').val();
    var BFDescription = $('#BFDescription').val();

    if ($('#BFCode').val() == "Enter BF Code(upto 15 char)" || $('#BFCode').val().trim() == "" || $('#BFCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter BFCode!</p>");
        return false;
    }
    else if ($('#BFCode').val().length > 15) {
        $('#ErrorMsgs').html("<p class='error-msg'>BFCode should contain 15 characters.!</p>");
        return false;
    }
    else if ($('#BFDescription').val() == "Enter BF Description(upto 100 char)" || $('#BFDescription').val().trim() == "" || $('#BFDescription').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter BF Description!</p>");
        return false;
    }
    else if ($('#BFDescription').val().length > 100) {
        $('#ErrorMsgs').html("<p class='error-msg'>BF Description should contain 100 characters.!</p>");
        return false;
    }

    else {
        return true;
    }
}


///// ------------------FOR ADD BF-----------------------



$('#btnAddBF').click(function () {
    debugger;
    var BFCode = $('#BFCode').val();
    var BFDescription = $('#BFDescription').val();
    var msg = "";

    if (CheckBFValidation() == true) {
        var datatosend = {
            BFCode: BFCode,
            BFDescription: BFDescription
        }

        $.ajax({
            url: "/BfAdmin/AddBF/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "True") {
                          msg = 'BF details added successfully.!';
                          window.location.href = "/BfAdmin/Index?msg=" + msg;
                      }
                      else {
                          msg = "BF Code already exists.!";
                          window.location.href = "/BfAdmin/Index?msg=" + msg;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});


//-------------------FOR EDIT BF----


$('#btnEditBF').click(function () {
    debugger;
    var BFCodeToEdit = $('#BFCodeToEdit').val();
    var BFCode = $('#BFCode').val();
    var BFDescription = $('#BFDescription').val();
    var msg = "";

    if (CheckBFValidation() == true) {
        var datatosend = {
            BFCodeToEdit: BFCodeToEdit,
            BFCode: BFCode,
            BFDescription: BFDescription
        }

        $.ajax({
            url: "/BfAdmin/EditBF/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'BF details Updated successfully.!';
                          window.location.href = "/BfAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.BF Code is already in use.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete BF--------------------------


$('#btnDeleteBF').click(function () {
    //

    var msg = "";
    var BFCode = $('#BFCode').val();

    var datatosend = {
        BFCode: BFCode
    }

    $.ajax({
        url: "/BfAdmin/DeleteBF/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  if (result == "True") {
                      msg = 'BF details Deleted successfully.!';
                      window.location.href = "/BfAdmin/Index?msg=" + msg;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'BF Code is already in use.!' + "</p>");
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })

});

///-------------------------------------------------------

