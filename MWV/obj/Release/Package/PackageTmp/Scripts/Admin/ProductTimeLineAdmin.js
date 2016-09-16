//---COMMON FUNCTION...


function CheckProductTimeLineValidation() {
  //  
    if ($('#PaperMill').val() == "Select PaperMill" || $('#PaperMill').val().trim() == "" || $('#PaperMill').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select PaperMill!</p>");
        return false;
    }
    else if ($('#BFCode').val() == "Select BF" || $('#BFCode').val().trim() == "" || $('#BFCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select BF Code!</p>");
        return false;
    }
    else if ($('#GSMCode').val() == "Select GSM" || $('#GSMCode').val().trim() == "" || $('#GSMCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select GSM Code!</p>");
        return false;
    }
    else if ($('#ShadeCode').val() == "Select Shade" || $('#ShadeCode').val().trim() == "" || $('#ShadeCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select Shade!</p>");
        return false;
    }
    else if (Check_Speed($('#speed').val()) == false) {
        return false;
    }
    else if (CheckTon_per_hour($('#TonPrHr').val()) == false) {
        return false;
    }
    else if (CheckTime_per_Ton($('#TimePrTon').val()) == false) {
        return false;
    }
    else {
        return true;
    }
}



function Check_Speed(speed) {
    // var len = TonPrHr.length;
    if (speed.charAt(0) == '.') {
        $('#ErrorMsgs').html("<p class='error-msg'>speed should be numeric.!</p>");
        return false;
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(speed)) {
        $('#ErrorMsgs').html("<p class='error-msg'>speed should be numeric.!</p>");
        return false;
    }
    return true;
}


function CheckTon_per_hour(TonPrHr) {
    var len = TonPrHr.length;

    if (TonPrHr == "Enter Ton Per Hr" || TonPrHr == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter Ton Per Hr!</p>");
        return false;
    }
    else if (TonPrHr.charAt(0) == '.') {
        $('#ErrorMsgs').html("<p class='error-msg'>Ton Per Hr should be numeric.!</p>");
        return false;
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(TonPrHr)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Ton Per Hr should be numeric.!</p>");
        return false;
    }
        // else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(UnitPrice)) {
    else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(TonPrHr)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Only four decimal places are allowed in Ton Per Hr!</p>");
        return false;
    }

    return true;
}

function CheckTime_per_Ton(TimePrTon) {

    if (TimePrTon == "Enter Time Per Ton in Minutes" || TimePrTon == "" || TimePrTon.trim() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Enter Time Per Ton in Minutes!</p>");
        return false;
    }
    else if (TimePrTon.charAt(0) == '.') {
        $('#ErrorMsgs').html("<p class='error-msg'>Time Per Ton should be numeric.!</p>");
        return false;
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(TimePrTon)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Time Per Ton should be numeric.!</p>");
        return false;
    }
        // else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(UnitPrice)) {
    else if (!/^(\d+)?(?:\.\d{1,2})?$/.test(TimePrTon)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Only two decimal places are allowed in Time Per Ton!</p>");
        return false;
    }

    return true;
}


///// ------------------FOR AddProductTL-----------------------

$('#btnAddProductTimeLine').click(function () {
    //
    var PaperMillID = $('#PaperMill').val();
    var BFCode = $('#BFCode').val();
    var GSMCode = $('#GSMCode').val();
    var ShadeCode = $('#ShadeCode').val();
    var speed;
    if ($('#speed').val() == "Enter speed" || $('#speed').val().trim() == "" || $('#speed').val() == "")
        speed = 0;
    else
        var speed = $('#speed').val();

    var TonPrHr = $('#TonPrHr').val();
    var TimePrTon = $('#TimePrTon').val();
    var msg = "";

    if (CheckProductTimeLineValidation() == true) {
        //
        var datatosend = {
            PaperMillID: PaperMillID,
            BFCode: BFCode,
            GSMCode: GSMCode,
            ShadeCode: ShadeCode,
            speed: speed,
            TonPrHr: TonPrHr,
            TimePrTon: TimePrTon
        }
        $.ajax({
            url: "/ProductTimeLineAdmin/AddProductTimeLine/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      //alert(result);
                      if (result == "2") {
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'This ProductTimeLine already added for this combination.!' + "</p>");
                          return false;
                      }
                      else if (result == "3") {
                          msg = 'ProductTimeLine details added successfully.!';
                          window.location.href = "/ProductTimeLineAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while adding ProductTimeLine.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
//-------------------FOR EDIT AddProductTL--------------------------


$('#btnEditProductTL').click(function () {
    // 
    var ProductTLIDToEdit = $('#hdnProductTLIDToEdit').val();
    var speed;

    if ($('#speed').val() == "Enter speed" || $('#speed').val().trim() == "" || $('#speed').val() == "")
        speed = 0;
    else
        var speed = $('#speed').val();

    var TonPrHr = $('#TonPrHr').val();
    var TimePrTon = $('#TimePrTon').val();
    var msg = "";

    if (CheckProductTimeLineValidation() == true) {
        var datatosend = {
            ProductTLIDToEdit: ProductTLIDToEdit,
            speed: speed,
            TonPrHr: TonPrHr,
            TimePrTon: TimePrTon
        }

        $.ajax({
            url: "/ProductTimeLineAdmin/EditProductTimeLine/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "True") {
                          msg = 'ProductTimeLine details edited successfully.!';
                          window.location.href = "/ProductTimeLineAdmin/Index?msg=" + msg;
                         // return true;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while editing ProductTimeLine.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});

///-------------------------------------for delete ProductTL------------------

$('#btnDeleteProductTL').click(function () {
   // 
    var ProductTLIDToDel = $('#hdnProductTLIDToDel').val();
    var PaperMillID = $('#hdnPaperMillID').val();
    var BFCode = $('#BFCode').val();
    var GSMCode = $('#GSMCode').val();
    var ShadeCode = $('#ShadeCode').val();
    var msg = "";

    var datatosend = {
        ProductTLIDToDel: ProductTLIDToDel,
        PaperMillID: PaperMillID,
        BFCode: BFCode,
        GSMCode: GSMCode,
        ShadeCode: ShadeCode,
    }

    $.ajax({
        url: "/ProductTimeLineAdmin/DeleteProductTimeLine/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  // alert(result);
                  if (result == "True") {
                      msg = 'ProductTimeLine deleted successfully.!';
                      window.location.href = "/ProductTimeLineAdmin/Index?msg=" + msg;
                    //  return true;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not delete.This ProductTimeLine is already in use.!' + "</p>");
                      return false;
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })


});

//-------------------------------------------