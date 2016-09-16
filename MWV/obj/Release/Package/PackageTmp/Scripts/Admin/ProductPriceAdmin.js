//---COMMON FUNCTION...


function CheckProductPriceValidation() {

    // var UnitPriceChk = CheckUnitPrice($('#UnitPrice').val());
    var fromdt1 = $('#StartMonth').val() + "/" + $('#StartDate').val() + "/" + $('#StartYear').val();
    var todt1 = $('#EndMonth').val() + "/" + $('#EndDate').val() + "/" + $('#EndYear').val();

    // var dateCompare = CompareDate(fromdt1, todt1);

    if ($('#Customer').val() == "Select Customer" || $('#Customer').val().trim() == "" || $('#Customer').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select Customer!</p>");
        return false;
    }
    if ($('#ProductCode').val() == "Select Product" || $('#ProductCode').val().trim() == "" || $('#ProductCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please select product code!</p>");
        return false;
    }
    else if ($('#ShadeCode').val() == "Select Shade" || $('#ShadeCode').val().trim() == "" || $('#ShadeCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select Shade!</p>");
        return false;
    }
    else if (CheckUnitPrice($('#UnitPrice').val()) == false) {
        return false;
    }
    else if ($('#StartDate').val() == "DD" || $('#StartMonth').val() == "MM" || $('#StartYear').val() == "YYYY") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please select valid start Date!</p>");
        return false;
    }
    else if ($('#EndDate').val() == "DD" || $('#EndMonth').val() == "MM" || $('#EndYear').val() == "YYYY") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please select valid End Date!!</p>");
        return false;
    }
    else if (CompareDate(fromdt1, todt1) == false) {
        $('#ErrorMsgs').html("<p class='error-msg'>End Date should be greater than Start Date !");
        return false;
    }

    else {
        return true;
    }
}

function CheckUnitPrice(UnitPrice) {
    // 
    var len = UnitPrice.length;
    // var getWid = parseFloat(UnitPrice);
    if (UnitPrice == "Enter UnitPrice(upto 4 decimal digit)" || UnitPrice == "" || UnitPrice.trim() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter UnitPrice!</p>");
        return false;
    }
    else if (UnitPrice.charAt(0) == '.') {
        $('#ErrorMsgs').html("<p class='error-msg'>UnitPrice should be numeric.!</p>");
        return false;
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(UnitPrice)) {
        $('#ErrorMsgs').html("<p class='error-msg'>UnitPrice should be numeric.!</p>");
        return false;
    }
        // else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(UnitPrice)) {
    else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(UnitPrice)) {
        $('#ErrorMsgs').html("<p class='error-msg'>Only 4 decimal places are allowed in UnitPrice!</p>");
        return false;
    }

    return true;

}

function CompareDate(fromdtChk, todtChk) {
    if (Date.parse(fromdtChk) > Date.parse(todtChk)) {
        return false;
    }
    else {
        return true;
    }
}


///// ------------------FOR AddProductPrice-----------------------


$('#btnAddProductPrice').click(function () {
    //

    var CustomerID = $('#Customer').val();
    var ProductCode = $('#ProductCode').val();
    var ShadeCode = $('#ShadeCode').val();
    var UnitPrice = $('#UnitPrice').val();
    var StartDate = $('#StartDate').val() + "/" + $('#StartMonth').val() + "/" + $('#StartYear').val();
    var EndDate = $('#EndDate').val() + "/" + $('#EndMonth').val() + "/" + $('#EndYear').val();
    var msg = "";

    if (CheckProductPriceValidation() == true) {
        // 
        var datatosend = {
            CustomerID: CustomerID,
            ProductCode: ProductCode,
            ShadeCode: ShadeCode,
            UnitPrice: UnitPrice,
            StartDate: StartDate,
            EndDate: EndDate
        }
        $.ajax({
            url: "/ProductPriceAdmin/AddProductPrice/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                     // 
                      var arrMsgCode = result.split('-');
                      var msgNo, OverlappingStartDt, OverlappingEndDt;
                      msgNo = arrMsgCode[0];//get code here

                      if (msgNo == "1") {
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Price already defined for these dates !' + "</p>");
                          return false;
                      }
                      else if (msgNo == "2") {
                          $("#ErrorMsgs").html('');
                          OverlappingStartDt = arrMsgCode[1];
                          OverlappingEndDt = arrMsgCode[2];
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Dates are overlapping!.Start Date is falling between previously defined duration ' + OverlappingStartDt + ' and ' + OverlappingEndDt + ' ' + "</p>");
                          return false;
                      }
                      else if (msgNo == "3") {
                          msg = 'ProductPrice added successfully!';
                          window.location.href = "/ProductPriceAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while adding ProductPrice.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
   }

});
//-------------------FOR EDIT ProductPrice--------------------------



$('#btnEditProductPrice').click(function () {
    //
    var ProductPriceID = $('#hdnProductPriceIDToEdit').val();
    var CustomerID = $('#hdnCustomerID').val();
    var ProductCode = $('#hdnProductCode').val();
    var ShadeCode = $('#hdnShadeCode').val();
    var UnitPrice = $('#UnitPrice').val();
    var StartDate = $('#StartDate').val() + "/" + $('#StartMonth').val() + "/" + $('#StartYear').val();
    var EndDate = $('#EndDate').val() + "/" + $('#EndMonth').val() + "/" + $('#EndYear').val();
    var msg = "";

    if (CheckProductPriceValidation() == true) {
        var datatosend = {
            ProductPriceID: ProductPriceID,
            CustomerID: CustomerID,
            ProductCode: ProductCode,
            ShadeCode: ShadeCode,
            UnitPrice: UnitPrice,
            StartDate: StartDate,
            EndDate: EndDate
        }

        $.ajax({
            url: "/ProductPriceAdmin/EditProductPrice/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      var arrMsgCode = result.split('-');
                      var msgNo, OverlappingStartDt, OverlappingEndDt;
                      msgNo = arrMsgCode[0];//get code here

                      if (msgNo == "2") {
                          $("#ErrorMsgs").html('');
                          OverlappingStartDt = arrMsgCode[1];
                          OverlappingEndDt = arrMsgCode[2];
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Dates are overlapping!.Start Date is falling between previously defined duration ' + OverlappingStartDt + ' and ' + OverlappingEndDt + ' ' + "</p>");
                          return false;
                      }
                      else if (msgNo == "3") {
                          msg = 'ProductPrice edited successfully!';
                          window.location.href = "/ProductPriceAdmin/Index?msg=" + msg;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while editing ProductPrice.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});

///-------------------------------------------------------

