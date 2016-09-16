//---COMMON FUNCTION...

function CheckValidation() {

    if (CheckDecimalValidation($('#capacity').val(), $('#capacity').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#min_width').val(), $('#min_width').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#max_width').val(), $('#max_width').attr('id')) == false) {
        return false;
    }
    if ($('#location').val() == "select location" || $('#location').val().trim() == "" || $('#location').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select location!</p>");
        return false;
    }
    else if (CheckDecimalValidation($('#deckle_min').val(), $('#deckle_min').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#deckle_max').val(), $('#deckle_max').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#max_cuts').val(), $('#max_cuts').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#min_diameter').val(), $('#min_diameter').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#max_diameter').val(), $('#max_diameter').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#max_weight_child').val(), $('#max_weight_child').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#min_weight_jumbo').val(), $('#min_weight_jumbo').attr('id')) == false) {
        return false;
    }
    else if (CheckDecimalValidation($('#max_weight_jumbo').val(), $('#max_weight_jumbo').attr('id')) == false) {
        return false;
    }
    else if ($('#name').val() == "Enter name" || $('#name').val().trim() == "" || $('#name').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter name!</p>");
        return false;
    }
    else if ($('#address').val() == "Enter address" || $('#address').val().trim() == "" || $('#address').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter address!</p>");
        return false;
    }
    else {
        return true;
    }
}

function CheckDecimalValidation(val, id) {
    
    var len = val.length;

    if (val == "Enter " + id + "(upto 4 decimal digit)" || val == "" || val.trim() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter " + id + "!</p>");
        return false;
    }
    else if (val.charAt(0) == '.') {
        $('#ErrorMsgs').html("<p class='error-msg'>" + id + " should be numeric.!</p>");
        return false;
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(val)) {
        $('#ErrorMsgs').html("<p class='error-msg'>" + id + " should be numeric.!</p>");
        return false;
    }
    else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(val)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Only 4 decimal places are allowed in " + id + "!</p>");
        return false;
    }

    return true;

}



///// ------------------FOR ADD papermill-----------------------


$('#btnAddPapermill').click(function () {
   // 

    var capacity = $('#capacity').val();
    var min_width = $('#min_width').val();
    var max_width = $('#max_width').val();

    var location = $('#location').val();

    var deckle_min = $('#deckle_min').val();
    var deckle_max = $('#deckle_max').val();

    var max_cuts = $('#max_cuts').val();
    var min_diameter = $('#min_diameter').val();
    var max_diameter = $('#max_diameter').val();

    var max_weight_child = $('#max_weight_child').val();
    var min_weight_jumbo = $('#min_weight_jumbo').val();
    var max_weight_jumbo = $('#max_weight_jumbo').val();

    var name = $('#name').val();
    var address = $('#address').val();

    var msg = "";

    if (CheckValidation() == true) {
        var datatosend = {
            capacity: capacity,
            min_width: min_width,
            max_width: max_width,

            location: location,

            deckle_min: deckle_min,
            deckle_max: deckle_max,

            max_cuts: max_cuts,
            min_diameter: min_diameter,
            max_diameter: max_diameter,

            max_weight_child: max_weight_child,
            min_weight_jumbo: min_weight_jumbo,
            max_weight_jumbo: max_weight_jumbo,

            name: name,
            address: address
        }

        $.ajax({
            url: "/PaperMillAdmin/AddPaperMill/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "True") {
                          msg = 'PaperMill details added successfully.!';
                          window.location.href = "/PaperMillAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'PaperMill name already exists for this location.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }


});




//-------------------FOR EDIT papermill----


$('#btnEditPapermill').click(function () {
    // 

    var PaperMillIDToEdit = $('#PaperMillIDToEdit').val();

    var capacity = $('#capacity').val();
    var min_width = $('#min_width').val();
    var max_width = $('#max_width').val();

    var location = $('#location').val();

    var deckle_min = $('#deckle_min').val();
    var deckle_max = $('#deckle_max').val();

    var max_cuts = $('#max_cuts').val();
    var min_diameter = $('#min_diameter').val();
    var max_diameter = $('#max_diameter').val();

    var max_weight_child = $('#max_weight_child').val();
    var min_weight_jumbo = $('#min_weight_jumbo').val();
    var max_weight_jumbo = $('#max_weight_jumbo').val();

    var name = $('#name').val();
    var address = $('#address').val();

    var msg = "";

    if (CheckValidation() == true) {
        var datatosend = {
            PaperMillIDToEdit: PaperMillIDToEdit,

            capacity: capacity,
            min_width: min_width,
            max_width: max_width,

            location: location,

            deckle_min: deckle_min,
            deckle_max: deckle_max,

            max_cuts: max_cuts,
            min_diameter: min_diameter,
            max_diameter: max_diameter,

            max_weight_child: max_weight_child,
            min_weight_jumbo: min_weight_jumbo,
            max_weight_jumbo: max_weight_jumbo,

            name: name,
            address: address
        }

        $.ajax({
            url: "/PaperMillAdmin/EditPaperMill/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      if (result == "True") {
                          msg = 'PaperMill details Updated successfully.!';
                          window.location.href = "/PaperMillAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not edit.PaperMill location and name already exists.!' + "</p>");
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR delete papermill--------------------------


$('#btnDeletePaperMill').click(function () {
    //

    var msg = "";
    var PaperMillIDToDel = $('#PaperMillIDToDel').val();

    var datatosend = {
        PaperMillIDToDel: PaperMillIDToDel
    }

    $.ajax({
        url: "/PaperMillAdmin/DeletePaperMill/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  if (result == "True") {
                      msg = 'PaperMill details deleted successfully.!';
                      window.location.href = "/PaperMillAdmin/Index?msg=" + msg;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'PaperMill already in use.!' + "</p>");
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })

});

///-------------------------------------------------------

