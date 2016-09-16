//---COMMON FUNCTION...

//function CheckDuplicateProductCodeAndGsmBF(ProductCode, GSMCode, BFCode) {
//    
//    var datatosend = {
//        ProductCode: ProductCode,
//        GSMCode: GSMCode,
//        BFCode: BFCode
//    }

//    $.ajax({
//        url: "/ProductAdmin/CheckDuplicateProductCodeAndGsmBF/",
//        data: datatosend,
//        contentType: "application/html; charset=utf-8",
//        type: "GET",
//        dataType: 'html'
//    })
//                  .success(function (result) {
//                      // alert(result);
//                      if (result == "1") {  //product code is duplicate.
//                          $("#ErrorMsgs").html('');
//                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Product code is already exists.!' + "</p>");
//                          return false;
//                      }
//                      else if (result == "2") {  //bf gsm pair is duplicate.
//                          $("#ErrorMsgs").html('');
//                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Bf and Gsm pair is already exists!' + "</p>");
//                          return false;
//                      }
//                      else if (result == "3") {
//                          return true;
//                      }

//                  })
//                 .error(function (xhr, ajaxOptions, thrownError) {
//                 })

//}

function CheckProductValidation() {
    if ($('#product_code').val() == "Enter Product Code(upto 15 char)" || $('#product_code').val().trim() == "" || $('#product_code').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter product code!</p>");
        return false;
    }
    else if ($('#product_code').val().length > 15) {
        $('#ErrorMsgs').html("<p class='error-msg'>Product code should contain 15 characters.!</p>");
        return false;
    }
    else if ($('#GSMCode').val() == "Select Gsm" || $('#GSMCode').val().trim() == "" || $('#GSMCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select GSM Code!</p>");
        return false;
    }
    else if ($('#BFCode').val() == "Select Bf" || $('#BFCode').val().trim() == "" || $('#BFCode').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please Select BF Code!</p>");
        return false;
    }
    if ($('#ProductDescription').val() == "Enter Product Description(upto 100 char)" || $('#ProductDescription').val().trim() == "" || $('#ProductDescription').val() == "") {
        $('#ErrorMsgs').html("<p class='error-msg'>Please enter product description!</p>");
        return false;
    }
    else if ($('#ProductDescription').val().length > 100) {
        $('#ErrorMsgs').html("<p class='error-msg'>Product Description should contain 100 characters.!</p>");
        return false;
    }
    else {
        return true;
    }
}


///// ------------------FOR AddProduct-----------------------


$('#btnAddProduct').click(function () {
    // 
    var ProductCode = $('#product_code').val();
    var GSMCode = $('#GSMCode').val();
    var BFCode = $('#BFCode').val();
    var ProductDescription = $('#ProductDescription').val();
    var msg = "";

    if (CheckProductValidation() == true) {
        
        var datatosend = {
            ProductCode: ProductCode,
            GSMCode: GSMCode,
            BFCode: BFCode,
            ProductDescription: ProductDescription
        }
        $.ajax({
            url: "/ProductAdmin/AddProduct/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                      if (result == "1") { //product code is duplicate.
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Product code is already exists.!' + "</p>");
                          return false;
                      }
                      else if (result == "2") {  //bf gsm pair is duplicate.
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Product code with BF and GSM already exists!' + "</p>");
                          return false;
                      }
                      else if (result == "3") {
                          msg = 'Product details added successfully!';
                          window.location.href = "/ProductAdmin/Index?msg=" + msg;
                         // return true;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while adding Products.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
//-------------------FOR EDIT Products----



$('#btnEditProduct').click(function () {
    // 
    var ProductCode = $('#product_code').val();
    var GSMCode = $('#GSMCode').val();
    var BFCode = $('#BFCode').val();
    var ProductDescription = $('#ProductDescription').val();
    var msg = "";

    if (CheckProductValidation() == true) {
        var datatosend = {
            ProductCode: ProductCode,
            GSMCode : GSMCode ,
            BFCode: BFCode,
            ProductDescription: ProductDescription
        }

        $.ajax({
            url: "/ProductAdmin/EditProduct/",
            data: datatosend,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html'
        })
                  .success(function (result) {
                      // alert(result);
                       if (result == "2") {  //bf gsm pair is duplicate.
                          $("#ErrorMsgs").html('');
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Bf and Gsm pair is already exists!' + "</p>");
                          return false;
                      }
                      else if (result == "3") {
                          msg = 'Product details edited successfully!';
                          window.location.href = "/ProductAdmin/Index?msg=" + msg;
                        //  return true;
                      }
                      else {
                          $("#ErrorMsgs").html("<p class='error-msg'>" + 'Error while editing Products.!' + "</p>");
                          return false;
                      }

                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    }

});
///------------------------------------FOR DELEET BF


$('#btnDeleteProduct').click(function () {
   // 
    var msg = "";
    var ProductCode = $('#product_code').val();

    var datatosend = {
        ProductCode: ProductCode
    }

    $.ajax({
        url: "/ProductAdmin/DeleteProduct/",
        data: datatosend,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html'
    })
              .success(function (result) {
                  if (result == "True") {
                      msg = 'Product details deleted successfully!';
                      window.location.href = "/ProductAdmin/Index?msg=" + msg;
                  }
                  else {
                      $("#ErrorMsgs").html("<p class='error-msg'>" + 'Can not delete.Product code is already in use!' + "</p>");
                  }

              })
             .error(function (xhr, ajaxOptions, thrownError) {
             })

});

///-------------------------------------------------------

