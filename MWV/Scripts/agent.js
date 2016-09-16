


function RefreshSearchTransporationPanel() {
    $('#selectVehicleTypeDropdown').val('All-Vehicles');
    $('#transportationFromDateDay').val('DD');
    $('#transportationFromDateMonth').val('MM');
    $('#transportationFromDateYear').val('YYYY');
    $('#transportationToDateDay').val('DD');
    $('#transportationToDateMonth').val('MM');
    $('#transportationToDateYear').val('YYYY');
    $('#transportationVehicleNumber').val('Type Vehicle Number');
}
function RefreshArrangeTransporationPanel() {
    $('#newTransportationLocation').val('Choose Papermill Location');
    $('#newTransportationVehicleNumber').val('');
    $('#newTransportationVehicleCapacity').val('');
    $('#newTransportationStartDay').val('DD');
    $('#newTransportationStartMonth').val('MM');
    $('#newTransportationStartYear').val('YYYY');
    $('#newTransportationStartTime').val('1:00');
    $('#newTransportationStatTimeAmPm').val('AM');
    $('#newTransportationArrivalDay').val('DD');
    $('#newTransportationArrivalMonth').val('MM');
    $('#newTransportationArrivalYear').val('YYYY');
    $('#newTransportationArrivalTime').val('1:00');
    $('#newTransportationArrivalTimeAmPm').val('AM');
}
function clearOrderSearchArrangeTransportation() {
    $('#OrderSearchTransportationVehicleNumber').val('');
    $('#OrderSearchTransportationVehicleCapacity').val('');
    $('#ordersNewTransportationStartDay').val('');
    $('#orderNewTransportationStartMonth').val('');
    $('#ordersNewTransportationStartYear').val('');
    $('#ordersNewTransportationStartTime').val('');
    $('#ordersNewTransportationStatTimeAmPm').val('');

    $('#ordersNewTransportationArrivalDay').val('');
    $('#ordersNewTransportationArrivalMonth').val('');
    $('#ordersNewTransportationArrivalYear').val('');

    $('#ordersNewTransportationArrivalTime').val('');
    $('#ordersNewTransportationArrivalTimeAmPm').val('');
    $('#tmpOrderSearchshowCargoList').html('');
}
function clearCreateCustomerForm() {
    $('#txtName').val('');
    $('#txtAddress1').val('');
    $('#txtAddress2').val('');
    $('#txtAddress3').val('');
    $('#txtCity').val('');
    $('#txtPincode').val('');
    $('#txtState').val('');
    $('#txtCountry').val('');
    $('#txtPhone').val('');
    $('#txtFax').val('');
    $('#txtEmail').val('');
    $('#createOrderSelectCustomer').hide();
    $('#createCustomerPanel').show();
}
function clearAllCustomerddls() {
    //
    var a = $("#autoSearchCustomers").val();
    $("#autoSearchCustomers").val('Search');
    //$("#customersSelectCustomerDropdown").find('option:first').attr('selected', 'selected');
    //$("#selectCustomerAlphabetically").find('option:first').attr('selected', 'selected');

    //$('#autoSearchCustomers').autocomplete('close'); //autoSearchCustomers
    // $('#autoSearchCustomers').autocomplete('destroy');
    // $('#autoSearchCustomers').val('Search').blur();
    //$('#autoSearchCustomers').val('');
    $("#customersSelectCustomerDropdown")[0].selectedIndex = 0;
    $("#selectCustomerAlphabetically")[0].selectedIndex = 0;
    //Added By Sagar
    $('#customersSelectCustomerDropdown').val('').trigger('chosen:updated');
    $('#customersSelectCustomerDropdown').find('option:first').prop('selected', 'selected');
    $('#selectCustomerAlphabetically').val('').trigger('chosen:updated');
    $('#selectCustomerAlphabetically').find('option:first').prop('selected', 'selected');



}
function clearCustomerddl() {
    $("#selectCustomerDropdown").find('option:first').attr('selected', 'selected'); //val('Select Customer');
}
function clearReqDelDateddl() {
    $('#requestedDeliveryDateDay').val('DD');
    $('#requestedDeliveryDateMonth').val('MM');
    $('#requestedDeliveryDateYear').val('YYYY');
}
function RemoveOrderAndProductsddlItems() {

    $('#Orderddl')
    .find('option')
    .remove()
    .end()
    .append('<option>Select PO #</option>')
    // .val('')
    ;
    $('#Productsddl')
    .find('option')
    .remove()
    .end()
    .append('<option>Select Product</option>')
    // .val('')
    ;
}


$('#selectBf').on('change', function () {
    var selectedItem = $(this).val();
    var ddlGsms = $("#selectGsm");

    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order_product/GetGsms/" + selectedItem,
                   contentType: "application/json; charset=utf-8",
                   success: function (data) {
                       ddlGsms.html('');
                       ddlGsms.append($('<option>Select Gsm</option>'));
                       $.each(data, function (id, option) {

                           ddlGsms.append($('<option></option>').val(option.id).html(option.name));
                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });
    //var selectedBf = selectedItem;
    //var selectedGsm = "";
    //var selectedGsm = $("#selectGsm").val();
    //var selectShade = $("#selectShade").val(); //$(this).val();
    //var CustId = $("#selectedCustomerId").val();
    //var prodcode = $("#productCode").text();
    //ProductCombination(selectedBf, selectedGsm, selectShade, prodcode, CustId);
    //        ////------------code to get the Gsms------------//
});
$('#selectGsm').on('change', function () {

    var selectedBf = $("#selectBf").val();
    var selectedGsm = $(this).val(); //$("#selectGsm").val();
    var hidVal = $("#selectedCustomerId").val();
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "Order_product/GetProductCode/" + selectedBf + "/" + selectedGsm,
                   //contentType: "application/json; charset=utf-8",
                   success: function (data) {
                       $("#productCode").text(data);
                   },

               });
    //var selectShade = $("#selectShade").val(); //$(this).val();
    //var CustId = $("#selectedCustomerId").val();
    //var prodcode = $("#productCode").text();
    // ProductCombination(selectedBf, selectedGsm, selectShade, prodcode, CustId);

});
$('#selectShade').on('change', function () {

    //var selectedBf = $("#selectBf").val();
    //var selectedGsm = $("#selectGsm").val();
    //var selectShade = $(this).val();
    //var CustId = $("#selectedCustomerId").val();
    //var prodcode = $("#productCode").text();
    //ProductCombination(selectedBf, selectedGsm, selectShade, prodcode, CustId);
});


function ProductCombination(selectedBf, selectedGsm, selectShade, prodcode, CustId) {

    var CustName = $("#selectedCustomerName").html();
    var NoPricetext = "No Active Price available for " + CustName + "," + " " + selectedBf + " BF" + ", " + selectedGsm + " GSM" + ", " + "Shade" + " " + selectShade + "";
    var newUrl = "Order_product/GetProductCodebyCustId/";
    var allvalues = {
        CustId: CustId,
        selectedBf: selectedBf,
        selectedGsm: selectedGsm,
        selectShade: selectShade,
    }
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: newUrl,
                   data: allvalues,
                   contentType: "application/text; charset=utf-8",
                   success: function (data) {
                       if (data == 0) {
                           $("#productPrice").show();
                           $("#productPrice").text("" + NoPricetext);
                           $('#editproductPrice').text("" + NoPricetext);
                       }
                       else {
                           $("#productPrice").text("INR " + data);
                           $("#editproductPrice").text("INR " + data);
                           $('#AddOrdersErrorMsg').html('');
                       }
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });

}
$('#arrangeNewTransportation').find('#Orderddl').on('change', function () {

    var selectedItem = $(this).val();
    var ddlProducts = $('#arrangeNewTransportation').find("#Productsddl");
    $("#tmpAvailableQty").text("");
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Home/GetProductsByOrderId/" + selectedItem,
                   contentType: "application/json; charset=utf-8",
                   success: function (data) {

                       ddlProducts.html('');
                       ddlProducts.append($('<option>Select Product</option>'));
                       $.each(data, function (id, option) {
                           //alert(option.width.toFixed(2val(option.id).html((option.id) + ", " + parseFloat(option.width).toFixed(2) + " cm x " + option.name));
                           ddlProducts.append($('<option></option>').val(option.id).html(parseFloat(option.width).toFixed(2) + " cm x " + option.name));

                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });
});
$('#arrangeNewTransportation').find('#Productsddl').on('change', function () {

    var selectedItem = $(this).val();
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order/GetProductsAvailableQty/" + selectedItem,
                   contentType: "application/html; charset=utf-8",
                   success: function (data) {
                       $('#qtyErrorMsg').hide();
                       $('#tmpAvailableQty').html("Available Quantity: " + data);
                       $('#AvailableQty').html(data);
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });


});
$('#tmpOrderDetails').on('click', 'a[href="#duplicateOrder"]', function () {

    var orderid = $(this).attr("data-id");
    var custname = $(this).attr("data-value");

    $('#inputCustomerpo').val('');
    $('#reqCustPO').html('');
    //$.ajax(
    //          {
    //              cache: false,
    //              type: "GET",
    //              url: "/Order/GenerateDuplicateOrder/" + orderid,
    //              contentType: "application/text; charset=utf-8",
    //              success: function (data) {
    //                  $('#lstProds').html(data);
    //                  $("#selectedCustomerName").html(custname);

    //              },
    //              error: function (xhr, ajaxOptions, thrownError) {

    //              }
    //          });


    $.ajax(
            {
                cache: false,
                type: "GET",
                url: "/Order/GenerateDuplicateOrder/" + orderid,
                contentType: "application/text; charset=utf-8",
                success: function (data) {

                    $('#lstProds').html(data);
                    $("#selectedCustomerName").html(custname);
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });

    $('#ordersPanel').hide();
    $('#createOrderPanel').show();
    $('#createOrderSelectCustomer').hide();
    $('#createOrderAddProducts').show();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();
    $('#createOrderCurrentProducts').show();
    $('#orderConfirmation').hide();

    $('html,body').animate({
        scrollTop: $('#createOrderCurrentProducts').position().top + $('#createOrderPanel').position().top
    }, 400);

});
$('#orderConfirmation').on('click', 'a[href="#duplicateOrder"]', function () {

    $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Order/GenerateDuplicateOrder/",
                  contentType: "application/text; charset=utf-8",
                  success: function (data) {

                      $('#lstProds').html(data);

                  },
                  error: function (xhr, ajaxOptions, thrownError) {

                  }
              });
    $('#inputCustomerpo').val('');
    $('#createOrderAddProducts').show();
    $('#orderConfirmation').hide();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();
    $('#createOrderCurrentProducts').show();

    $('html,body').animate({
        scrollTop: $('#createOrderCurrentProducts').position().top + $('#createOrderPanel').position().top
    }, 400);


});
function destroyOrderSession() {

    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order/DestroyOrderSession/",
                   contentType: "application/text; charset=utf-8",
                   success: function () {
                       $('#lstProds').html('');
                       $('#currentProductsTotal').html('');
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });
}
/* -----------------------------------------------------------------------------------------------------------
 * DASHBOARD / GENERAL
 * -------------------------------------------------------------------------------------------------------- */

$('#dashboard').on('click', 'a[href="#showCreateOrderPanel"]', function (event) {
    $('#confirmCustomerPanel').hide();
    $('#reviewOrder').hide();
    event.stopImmediatePropagation();
    $('#emailErrmsg').hide();
    $('#createOrderSelectedCustomerResults').hide();
    // clear all the dopdowns on clcik of 'showCreateOrderPanel' button
    $("#reqCustPO").html('');
    clearCustomerddl()
    destroyOrderSession();
    clearReqDelDateddl();
    $('#selectCustomerDropdown').val('').trigger('chosen:updated');
    $("#selectCustomerDropdown").find('option:first').prop('selected', 'selected');
    $('#inputCustomerpo').val('');
    $('#tmpReqDelDateReqd').html("");
    $('#customersPanel').hide();
    $('#transportationPanel').hide();
    $('#ordersPanel').hide();
    if ($('#createOrderPanel').is(':hidden')) {
        $('#createOrderPanel').show();
        $('#createOrderSelectCustomer').show();
    }
    else {
        $('#createOrderPanel').hide();
    }
    $('#createOrderCurrentProducts').hide();
    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top - 50
    }, 400);


    //clear auto search
    ClearAutoSearchForCreateNewOrderBtn();
    //--reset drp down
    $('#selectCustomerDropdown')[0].selectedIndex = 0;
    $('#createCustomerPanel').hide();
    clearAddProductForm();
    CloseAddProductPanel();
    $("#orderConfirmation").hide();
});

/* -----------------------------------------------------------------------------------------------------------
 * VIEW SCHEDULE
 * -------------------------------------------------------------------------------------------------------- */

$('#welcomePanel').on('click', 'a[href="#showSchedule"]', function (event) {
    debugger;
    event.stopImmediatePropagation();
    if ($(".change_password").is(":visible")) {

        $(".change_password").hide();
        ShowagntDashboard();


    }
        //else if ($(".your_messages").is(":visible")) {

        //  //  ShowagntDashboard();
        //}


    else {
        ShowagntDashboard();
        $("#viewSchedulePaperMills").find('option:first').prop('selected', 'selected');
        $('#schedulePaperMill').hide();
        $('#welcomePanel').css('height', '58px');
        $('#viewSchedulePanel').show();

        $('html,body').animate({
            scrollTop: 0
        }, 400);

    }


});



$('#viewSchedulePanel').on('click', 'a[href="#closeViewSchedulePanel"]', function () {
    $('#welcomePanel').css('height', '44px');
    $('#viewSchedulePanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);

    //return false;
});
$('#viewSchedulePanel').on('change', 'select#viewSchedulePaperMills', function () {

    //if ($(this).val() !== '') {
    //    $('.schedule_paper_mill').hide();
    //    $('#' + $(this).val()).show();
    //}
    var selectedItem = $(this).val();
    if ($(this).val() !== '') {
        $.ajax(
                   {
                       cache: false,
                       type: "GET",
                       url: "/Agent/ScheduleDetails/" + selectedItem,
                       contentType: "application/json; charset=utf-8",
                       success: function (data) {
                           $('.schedule_paper_mill').hide();
                           $('#schedulePaperMill').show();
                           $('#schedulePaperMill').html(data);
                       },
                       error: function (xhr, ajaxOptions, thrownError) {

                       }
                   });
    }
    else {
        $('#schedulePaperMill').hide();
    }
});



/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER
 * -------------------------------------------------------------------------------------------------------- */

$('#createOrderPanel').on('click', 'a[href="#closeCreateOrderPanel"]', function () {
    $('#createOrderPanel').hide();

    $('#createOrderSelectCustomer').show();
    $('a[href="#showCreateOrderPanel"]').show();
    $('#createOrderAddProducts').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);
    ClearAutoSearchForCreateNewOrderBtn();
    $('#selectCustomerDropdown')[0].selectedIndex = 0;
    clearAddProductForm();
    CloseAddProductPanel();

});
$('#createOrder').on('click', 'a[href="#closeAddProductPanel"]', function () {

    clearAddProductForm();
    $('.delivery_date_conflict').hide();
});
$('#requestedDeliveryDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#requestedDeliveryDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#requestedDeliveryDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: CREATE CUSTOMER
 * -------------------------------------------------------------------------------------------------------- */

$('#createOrderPanel').on('click', 'a[href="#createCustomer"]', function () {

    $('#createNewCustomerErrmsg').hide();
    clearCreateCustomerForm();

});
$('#createCustomerPanel').on('click', 'a[href="#createNewCustomer"]', function () {

    $('#createNewCustomerErrmsg').hide();
    if (FormcustomerDetails.txtName.value == "") {
        $('#createNewCustomerErrmsg').show();
        $('#createNewCustomerErrmsg').html("<p class='error-msg'>Name cannot be empty !");
    }
    else if (FormcustomerDetails.txtName.value == "" || FormcustomerDetails.txtName.value.trim() == "") {
        $('#createNewCustomerErrmsg').show();
        $('#createNewCustomerErrmsg').html("<p class='error-msg'>Name cannot be empty !");
    }
        //else if (FormcustomerDetails.txtCity.value == "" || FormcustomerDetails.txtCity.value.trim() == "") {
        //    $('#createNewCustomerErrmsg').show();
        //    $('#createNewCustomerErrmsg').html("<p class='error-msg'>City cannot be empty !");
        //}
    else {
        function emailValidate() {
            var sEmail = $("#txtEmail").val();
            var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
            if (filter.test(sEmail)) {
                return true;
            }

            else {
                return false;
            }
        }
        var emailValidate = emailValidate();
        if ($('#txtEmail').val() == "" || emailValidate == true) {
            ReviewCustomerDetails.innerHTML = "<ul>" +
           "<li>" + FormcustomerDetails.txtName.value + "</li>" +
           "<li>" + FormcustomerDetails.txtAddress1.value + "</li>" +
           "<li>" + FormcustomerDetails.txtAddress2.value + "</li>" +
           "<li>" + FormcustomerDetails.txtAddress3.value + "</li>" +
          " <li>" + FormcustomerDetails.txtCity.value + "</li>" +
          " <li>" + FormcustomerDetails.txtPincode.value + " </li>" +
           "<li>" + FormcustomerDetails.txtState.value + "</li>" +
          " <li>" + FormcustomerDetails.txtCountry.value + "</li>" +
          " <li>" + FormcustomerDetails.txtPhone.value + "</li>" +
           "<li>" + FormcustomerDetails.txtFax.value + "</li>" +
           "<li>" + FormcustomerDetails.txtEmail.value + "</li>" +
             "</ul>";
            $('#createCustomerPanel').hide();
            $('#reviewCustomerPanel').show();
            $('html,body').animate({
                scrollTop: $('#createOrderPanel').position().top
            }, 400);
        }
    }
});

$("#txtEmail").focusout(function () {
    $('#emailErrmsg').hide();

    var sEmail = $("#txtEmail").val();
    var filter = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,4}$/;
    if (filter.test(sEmail)) {
        return true;
    }
    else {
        $('#emailErrmsg').show();
        $('#emailErrmsg').text("Email address is not valid !");
        return false;
    }
});


$('#reviewCustomerPanel').on('click', 'a[href="#cancelCreateCustomer"]', function () {
    $('a.create_order').show();
    $('#createCustomerPanel').hide();
    $('#reviewCustomerPanel').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#reviewCustomerPanel').on('click', 'a[href="#editCreateCustomer"]', function () {
    $('#emailErrmsg').hide();
    $('#reviewCustomerPanel').hide();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#reviewCustomerPanel').on('click', 'a[href="#createCustomerConfirmation"]', function () {

    var Result = {
        Name: $('#txtName').val(),
        Address1: $('#txtAddress1').val(),
        Address2: $('#txtAddress2').val(),
        Address3: $('#txtAddress3').val(),
        City: $('#txtCity').val(),
        Pincode: $('#txtPincode').val(),
        State: $('#txtState').val(),
        Country: $('#txtCountry').val(),
        Phone: $('#txtPhone').val(),
        Fax: $('#txtFax').val(),
        Email: $('#txtEmail').val(),

    };
    var stringReqdata = JSON.stringify(Result);
    $.ajax({
        //async: true,
        type: "POST",
        url: "/Customer/CreateCustomer/",
        data: stringReqdata,
        dataType: "html",
        context: document.body,
        contentType: 'application/json; charset=utf-8'
    })
                    .success(function (data) {
                        $('#reviewCustomerPanel').hide();
                        $('#confirmCustomerPanel').slideDown();
                        $('#confirmCustomerPanel').html(data);
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#confirmCustomerPanel').on('click', 'a[href="#closeCreateCustomerConfirmation"]', function () {
    $('a.create_order').show();
    $('#createCustomerPanel').hide();
    $('#confirmCustomerPanel').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#confirmCustomerPanel').on('click', 'a[href="#createAnotherCustomer"]', function () {

    $('#emailErrmsg').hide();
    $('#txtName').val('');
    $('#txtAddress1').val('');
    $('#txtAddress2').val('');
    $('#txtAddress3').val('');
    $('#txtCity').val('');
    $('#txtPincode').val('');
    $('#txtState').val('');
    $('#txtCountry').val('');
    $('#txtPhone').val('');
    $('#txtFax').val('');
    $('#txtEmail').val('');
    $('#createOrderSelectCustomer').hide();
    $('#createCustomerPanel').show();
    $('#confirmCustomerPanel').hide();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

});

/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: AUTO-SEARCH
 * -------------------------------------------------------------------------------------------------------- */

var ordersReadyToClose = false,
   ordersCustomerList = [
       { label: "Alesha Appelbaum", value: "alesha-appelbaum", address: "4321 Somewhere Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-9999", fax: "800-888-7777" },
       { label: "Customer A", value: "customer-a", address: "523 Anywhere Ave.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-777-5555", fax: "800-777-3333" },
       { label: "Customer B", value: "customer-b", address: "1234 Nowhere Ln.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-5555", fax: "800-888-3333" },
       { label: "Customer C", value: "customer-c", address: "16B Forest Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-542-5555", fax: "800-542-3333" }
   ];

function buildAutoComplete() {

    var getData = function (request, response) {
        $.ajax({
            url: '/Agent/GetSearchResult', type: "GET", dataType: "json",
            data: { query: request.term },
            success: function (data) {
                response(data);
            }
        })
    };

    $('#ordersAutoSearchCustomers').autocomplete({
        appendTo: '#ordersAutoSearchCustomersResults',
        source: getData,
        create: function () {
            ordersReadyToClose = false;
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li>')
                   .attr('data-value', item.value)
                   .append('<p class="customer_name">' + item.label + '</p>')
                   .appendTo(ul);
            };
            $(this).data('ui-autocomplete').close = function () {
                if (!ordersReadyToClose) {
                    this.cancelSearch = false;
                    return false;
                }

                this._close();
            };
        },

        select: function (evt, ui) {


            var address1 = ui.item.address1;
            var address2 = ui.item.address2;
            var address3 = ui.item.address3;
            var city = ui.item.city;
            var state = ui.item.state;
            var country = ui.item.country;
            var pin = ui.item.pincode;
            if (address1 == null) { address1 = "" }
            if (address2 == null) { address2 = "" }
            if (address3 == null) { address3 = "" }
            if (city == null) { city = "" }
            if (state == null) { state = ""; }
            if (country == null) { country = ""; }
            if (pin == null) { pin = "" }

            var finaladdress = address1 + address2 + address3;
            var Getaddress = city + " / " + state + " / " + country + " / " + pin;
            var customerDtls = "";
            var customerDtls = "";
            if (Getaddress.indexOf(" /  /  / ") > -1) {
                Getaddress = Getaddress.replace(' /  / ', '');
            }
            if (Getaddress.indexOf(" /  / ") > -1) {
                Getaddress = Getaddress.replace('  / ', '');
            }
            if (Getaddress.substr(Getaddress.length - 1) == "/") {
                Getaddress = (Getaddress.substring(0, Getaddress.length - 3));
            }
            if (Getaddress.substr(Getaddress.length - 3) == " / ") {
                Getaddress = (Getaddress.substring(0, Getaddress.length - 3));
            }

            if (address1 == "")
                customerDtls = '<p>' + address2 + '</p> <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else if (address2 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else if (address3 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + address2 + '</p> <p>' + Getaddress + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else if (address2 == "" && address3 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else if (address1 == "" && address2 == "")
                customerDtls = '<p>' + address3 + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else if (address1 == "" && address3 == "")
                customerDtls = '<p>' + address2 + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';
            else
                customerDtls = '<p>' + address1 + '</p> <p>' + address2 + '</p>  <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a>';


            var customerDetails = customerDtls,
            orderHistory = '',
            customerOrders = '';

            $(evt.currentTarget)
                .find('li[data-value="' + ui.item.value + '"]')
                .append('<div class="list-item-wrapper"><div class="customer-list-item"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="/images/close-panel.png"></a> <p class="title"><strong>' + ui.item.label + '</strong></p></div> ' + customerDetails + orderHistory + customerOrders + '</div></div>');

            $('#ordersAutoSearchCustomers').autocomplete('disable');

            return false;
        },
        open: function (evt, ui) {
            $('#createOrderSelectedCustomerResults').hide();
            $('#createOrderSelectCustomer .auto_search_results').show();
            $('#ordersAutoSearchCustomers').addClass('search-input-clear');
            $('#createOrderSelectCustomer .auto_search_input_field').find('a').show();
        }
    });
}
$('#createOrderSelectCustomer').on('focus', '#ordersAutoSearchCustomers', function () {

    if ($('#createOrderSelectCustomer .auto_search_results').is(':visible')) {
        ordersReadyToClose = true;

        $('#ordersAutoSearchCustomers').autocomplete('close');
        $('#ordersAutoSearchCustomers').autocomplete('destroy');
        $('#createOrderSelectCustomer .auto_search_results').hide();
        $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
        $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();

        buildAutoComplete();
        $('#ordersAutoSearchCustomers').val('').focus();
    }
});

$('#ordersAutoSearchCustomersResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    ordersReadyToClose = false;

    $('#ordersAutoSearchCustomersResults .list-item-wrapper').remove();
    $('#ordersAutoSearchCustomers').autocomplete('enable');
    $('#createOrderPanel').click();


});
$('#createOrderSelectCustomer').on('click', 'a[href="#clearSearch"]', function () {
    //
    //ordersReadyToClose = true;

    //$('#ordersAutoSearchCustomers').autocomplete('close');
    //$('#ordersAutoSearchCustomers').autocomplete('destroy');
    //$('#createOrderSelectCustomer .auto_search_results').hide();
    //$('#ordersAutoSearchCustomers').val('Search').blur();
    //$('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    //$('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();

    //buildAutoComplete();
    ClearAutoSearchForCreateNewOrderBtn();//clears auto search for create new order btn

});

function ClearAutoSearchForCreateNewOrderBtn() {
    ordersReadyToClose = true;

    $('#ordersAutoSearchCustomers').autocomplete('close');
    $('#ordersAutoSearchCustomers').autocomplete('destroy');
    $('#createOrderSelectCustomer .auto_search_results').hide();
    $('#ordersAutoSearchCustomers').val('Search').blur();
    $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();

    buildAutoComplete();
}

function selectCustomer() {
    ordersReadyToClose = true;

    $('#ordersAutoSearchCustomers').autocomplete('close');
    $('#ordersAutoSearchCustomers').autocomplete('destroy');
    $('#createOrderSelectCustomer .auto_search_results').hide();
    $('#ordersAutoSearchCustomers').val('Search').blur();
    $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();
    $('#createOrderSelectedCustomerResults').hide();
    $('#selectedCustomerId').val($('#selectCustomerDropdown').val());

    buildAutoComplete();

    $('#createOrderSelectCustomer').hide();
    $('#createOrderAddProducts').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

}
$('#ordersAutoSearchCustomersResults').on('click', 'a[href="#selectCustomerFromList"]', function () {

    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    destroyOrderSession();
    $('#reviewOrder').hide();
    $('#selectedCustomerId').val($(this).attr('data-value'));
    $('#selectedCustomerName').html($(this).attr('data-name'));
    createOrderFromCustomer();
    clearAddProductForm();
    $('#addProduct').hide();

});
$('#selectCustomerDropdownPanel').on('change', 'select#selectCustomerDropdown', function () {
    ordersReadyToClose = true;
    var selectedCustomerid = $('select#selectCustomerDropdown').val();
    var selCustName = $('select#selectCustomerDropdown option:selected').text();

    if ($(this).val() !== '') {
        $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Customer/GetCustomerbyAgentId/" + selectedCustomerid,
                  contentType: "application/json; charset=utf-8",
                  success: function (data) {
                      $('#createOrderSelectedCustomerResults').show();
                      $('#createOrderSelectedCustomerResults').html(data);
                  }
              });

        $('html,body').animate({
            scrollTop: $('#createOrderPanel').position().top + 150
        }, 400);
    } else {
        $('#selectedCustomerResults').hide();
    }

    $('#ordersAutoSearchCustomers').autocomplete('close');
    $('#ordersAutoSearchCustomers').autocomplete('destroy');
    $('#createOrderSelectCustomer .auto_search_results').hide();
    $('#ordersAutoSearchCustomers').val('Search').blur();
    $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();
    buildAutoComplete();

});
$('#createOrderSelectedCustomerResults').on('click', 'a[href="#selectCustomerFromList"]', function () {

    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    selectCustomer();
    var selectedCustid = $('#selectedCustomerId').val();

    $.ajax(
                   {
                       cache: false,
                       type: "GET",
                       url: "/Customer/GetCustomerNameByCustomerId/" + selectedCustid,
                       //contentType: "application/json; charset=utf-8",
                       success: function (data) {
                           $("#selectedCustomerName").html(data);
                           $("#tmpCustNameReview").html(data);
                           $("#tmpCustNameOrderConfirm").html(data);
                       }
                   });

});
$('#createOrderSelectedCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    $('#createOrderSelectedCustomerResults').hide();


});
$('#ordersAutoSearchCustomers')
.on('focus', function () {
    if ($(this).val() === 'Search') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Search');
    }
});
$('#ordersAddCargoEnterQuantity')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});
$('#orderPurchaseOrderNumber')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: ADD PRODUCTS
 * -------------------------------------------------------------------------------------------------------- */

$('#createOrderAddProducts').on('click', 'a[href="#back"]', function () {
    clearCustomerddl();
    // destroy the order and temporder session on pressing back button
    // and empty the html content of divs 'lstProds' and 'currentProductsTotal'
    $("#reqCustPO").html('');
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order/DestroyOrderSession/",
                   contentType: "application/text; charset=utf-8",
                   success: function () {
                       $('#lstProds').html('');
                       $('#currentProductsTotal').html('');
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });
    $('#createOrderAddProducts').hide();
    $('#createOrderPanel .orders_auto_search_results').hide();
    $('#createOrderSelectCustomer').show();
    $('#createOrderCurrentProducts').hide();
    $('#AddOrdersErrorMsg').html('');
});
function clearAddProductForm() {

    $("#TestAddProduct").find('#selectBf').find('option:first').prop('selected', 'selected');
    $("#TestAddProduct").find('#selectGsm').find('option:first').prop('selected', 'selected');
    $("#TestAddProduct").find('#selectShade').find('option:first').prop('selected', 'selected');
    $("#TestAddProduct").find('#selectCore').val('');
    $("#TestAddProduct").find('#selectWidth').val('');
    $("#TestAddProduct").find('#inputDiaCm').val('');
    $("#TestAddProduct").find('#inputQuantityMt').val('');
    $('#AddOrdersErrorMsg').html('');
    $("#inputprice").val('');
}

$('#createOrderAddProducts').on('click', 'a[href="#addProduct"]', function () {

    $('#viewSchedule').hide();
    clearAddProductForm();
    DropdownTriggerAddOrder();
    $("#TestAddProduct").find('#requestedDeliveryDateDay').find('option:first').prop('selected', 'selected');
    $("#TestAddProduct").find('#requestedDeliveryDateMonth').find('option:first').prop('selected', 'selected');
    $("#TestAddProduct").find('#requestedDeliveryDateYear').find('option:first').prop('selected', 'selected');

    $('#requestedDeliveryDateDay').find('option:first').prop('selected', 'selected');
    $('#requestedDeliveryDateMonth').find('option:first').prop('selected', 'selected');
    $('#requestedDeliveryDateYear').find('option:first').prop('selected', 'selected');

    //$("#TestAddProduct")[0].reset();
    // clear the values of product code and product price also
    $('#productCode').html('');
    $('#productPrice').html('');
    var a = $('#inputCustomerpo').val();
    if ($('#inputCustomerpo').val() == "Enter Customer PO" || $('#inputCustomerpo').val().trim() == "" || $('#inputCustomerpo').val() == "") {
        $('#reqCustPO').html("<p class='error-msg'>Please specify the customer PO !</p>");
        return false;
    }
    else {
        $('#reqCustPO').html('');
        var Result = {
            customer_id: $('#selectedCustomerId').val(),
            //requested_delivery_date: ($('#requestedDeliveryDateDay').val()) + "/" + ($('#requestedDeliveryDateMonth').val()) + "/" + ($('#requestedDeliveryDateYear').val()),
            customerpo: $('#inputCustomerpo').val(),
        };
        $.ajax({
            type: "POST",
            url: "/Order/addOrderinSession/",
            context: document.body,
            data: Result,
            dataType: "html",
            context: document.body,
        })
                             .success(function (data) {

                             })
                            .error(function (xhr, ajaxOptions, thrownError) {
                            });
    }

    if ($('#addProductPanel').is(':hidden')) {
        $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').hide();
        $('#addProductPanel').show();
        $('html,body').animate({
            scrollTop: $('#addProductPanel').position().top + $('#createOrderPanel').position().top
        }, 400);
    }
    //reset drp down here
    $('#selectBf')[0].selectedIndex = 0;
    $('#selectGsm')[0].selectedIndex = 0;
    $('#selectShade')[0].selectedIndex = 0;
    $('#selectCore')[0].selectedIndex = 0;

});


$("#inputCustomerpo").blur(function () {
    if ($('#inputCustomerpo').val() != "Enter Customer PO" || $('#inputCustomerpo').val().trim() != "" || $('#inputCustomerpo').val() != "") {
        $('#reqCustPO').html('');
    }
});

$('#createOrderCurrentProducts').on('click', 'a[href="#editCurrentProduct"]', function () {
    //
    if ($(this).closest('.current_product').find('.edit_product_panel').is(':hidden')) {
        $(this).closest('.current_product').find('.edit_product_panel').show();

        //---show prev values
        var editFormID = $(this).attr('value');
        $('#frmEditProd_' + editFormID).find('#reqCustUpdatePO').html('');

        var gsm = $('#hdnGsmCode_' + editFormID).attr('value');
        $('#frmEditProd_' + editFormID).find('#editselectGsm option[value=' + gsm + ']').prop('selected', true);
        var shade_code = $('#hdnShade_' + editFormID).attr('value');
        // $('#frmEditProd_'+editFormID).find('#editselectShade option')[shade_code].selected = true;
        $('#frmEditProd_' + editFormID).find('#editselectShade option[value=' + shade_code + ']').prop('selected', true);
        var core = $('#hdnCore_' + editFormID).attr('value');
        $('#frmEditProd_' + editFormID).find('#editselectCore option[value=' + core + ']').prop('selected', true);
        var price = $('#hdnPrice_' + editFormID).attr('value');
        $('#editproductPrice_' + editFormID).text(price);

        var day = $('#hdnDt_' + editFormID).attr('value');
        $('#frmEditProd_' + editFormID).find('#requestedDeliveryDateDay option')[day].selected = true;
        var month = $('#hdnMon_' + editFormID).attr('value');
        $('#frmEditProd_' + editFormID).find('#requestedDeliveryDateMonth option')[month].selected = true;
        var year = $('#hdnYr_' + editFormID).attr('value');
        $('#frmEditProd_' + editFormID).find('#requestedDeliveryDateYear option[value=' + year + ']').prop('selected', true);
        ////---

        $('html,body').animate({
            scrollTop: $(window).scrollTop() + 220
        });
    }

});
$('#createOrderCurrentProducts').on('click', 'a[href="#confirmEditProduct"]', function () {
    $(this).closest('.edit_product_panel').hide();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top + 200
    }, 400);

});
$('#createOrderCurrentProducts').on('click', 'a[href="#cancelEditProduct"]', function () {
    $(this).closest('.edit_product_panel').hide();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top + 200
    }, 400);

});
$('.edit_product_panel').on('click', 'a[href="#closeEditProductPanel"]', function () {
    $(this).closest('.edit_product_panel').hide();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top + 200
    }, 400);

});
$('#createOrderCurrentProducts').on('click', 'a[href="#removeCurrentProduct"]', function () {
    if ($(this).closest('.current_product').find('.product_attention').is(':hidden')) {
        $(this).closest('.current_product').find('.product_attention').show();

        $('html,body').animate({
            scrollTop: $(window).scrollTop() + 120
        });
    }

});
$('.product_attention').on('click', 'a[href="#closeProductAttention"]', function () {
    $(this).closest('.product_attention').hide();

    $('html,body').animate({
        scrollTop: $(window).scrollTop() - 120
    });

});
$('.product_attention').on('click', 'a[href="#confirmRemoveProduct"]', function () {
    $(this).closest('.current_product').remove();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top + 140
    }, 400);

});

$('#addProductPanel').on('click', 'a[href="#closeAddProductPanel"]', function () {

    $('#addProductPanel').hide();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);
    // CloseAddProductPanel();

});

$('#addProductPanel').on('click', 'a[href="#closeAddProductPanel1"]', function () {

    $('.delivery_date_conflict ').hide();
    //$('#createOrderDeliveryDate .add_product').show();
    //$('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();

    $('html,body').animate({
        scrollTop: $('#addProductPanel').position().top
    }, 400);
    // CloseAddProductPanel();

});

function CloseAddProductPanel() {
    $('#addProductPanel').hide();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();

    ///the tabs from where CloseAddProductPanel() function called , already contain animate function, so comment it here, only close the Div's.
    //$('html,body').animate({
    //    scrollTop: $('#createOrderPanel').position().top
    //}, 400);

}



$('#addProductPanel').on('click', 'a[href="#addProductToOrder"]', function () {



    var editselectBf = $("#selectBf option:selected").text();
    var selectGsm = $("#selectGsm option:selected").text();
    var selectShade = $("#selectShade option:selected").text();
    var selectWidth = $("#selectWidth").val();
    var selectCore = $("#selectCore option:selected").text();
    var inputDiaCm = $("#inputDiaCm").val();
    var inputQuantityMt = $("#inputQuantityMt").val();
    var fromdt = $("#addProductPanel").find('#requestedDeliveryDateDay').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateMonth').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateYear').val();
    var fromdtChkDate = $("#addProductPanel").find('#requestedDeliveryDateYear').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateMonth').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateDay').val();


    //Calling Function
    // var ChkGreaterDate = CheckRequestDeliveryDate(fromdtChkDate);
    var req_del_date = TestAddProduct.requestedDeliveryDateDay.value + "/" + TestAddProduct.requestedDeliveryDateMonth.value + "/" + TestAddProduct.requestedDeliveryDateYear.value;

    var dateresult = CheckDate(fromdt);
    var widthchk = CheckWidth(selectWidth);
    var DiamwterChk = CheckDiamter(inputDiaCm);
    var qtyChk = CheckQty(inputQuantityMt);

    var CustName = $("#selectedCustomerName").html();
    var priceText = $('#productPrice').html();
    var checkPrice = CheckPrice($('#inputprice').val());
    //var checkPrice = CheckPrice(priceText, CustName);
    var DropdownStatus = ChkDropdown(editselectBf, selectGsm, selectShade, selectCore);
    if ($('#inputCustomerpo').val() == "" || $('#inputCustomerpo').val().trim() == "") {

        $('#reqCustPO').html('');
        $('#reqCustPO').html("<p class='error-msg'>Please specify the customer PO !</p>");
        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top
        }, 400);
        return false;
    }

    //var Scheduleflag = validateShadeSchedule(selectShade, req_del_date);

    if (dateresult == false) {
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>Please select a Valid date !</p>");
    }
        //else if (ChkGreaterDate != true) {
        //    $('#viewSchedule').show();
        //}
    else if (DropdownStatus != true) {
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + DropdownStatus + "</p>");
    }

    else if (widthchk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#selectWidth").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + widthchk + "</p>");
    }

    else if (DiamwterChk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#inputDiaCm").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + DiamwterChk + "</p>");
    }

    else if (qtyChk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#inputQuantityMt").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + qtyChk + "</p>");
    }
    else if (checkPrice != true) {
        id = "stakeholderReports"
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>Please enter price ! </p>");
    }
    else {

        $('#AddOrdersErrorMsg').html("");

        var req_del_date = TestAddProduct.requestedDeliveryDateDay.value + "/" + TestAddProduct.requestedDeliveryDateMonth.value + "/" + TestAddProduct.requestedDeliveryDateYear.value;
        //($('#requestedDeliveryDateDay').val()) + "/" + ($('#requestedDeliveryDateMonth').val()) + "/" + ($('#requestedDeliveryDateYear').val())

        var schduleCheck = validateSchedule(selectShade, req_del_date);
        //$.ajaxSetup({ cache: false });
        if (schduleCheck != "True") {
            $('#viewSchedule').show();
            $('html,body').animate({
                scrollTop: $(window).scrollTop() + 320
            });
        }
        else {

            var Result = {
                hCustomer_id: $('#selectedCustomerId').val(),
                selectBf: $('#selectBf').val(),
                selectGsm: $('#selectGsm').val(),
                selectShade: $('#selectShade').val(),
                selectWidth: $('#selectWidth').val(),
                selectCore: $('#selectCore').val(),
                inputDiaCm: $('#inputDiaCm').val(),
                inputQuantityMt: $('#inputQuantityMt').val(),
                requested_delivery_date: req_del_date,
                unit_price: $('#inputprice').val(),
                inputwidthInInch: $('#inputwidthInInch').val(),
            };
            var ProdSeqNoToCopy = 0;
            $.ajax({
                //async: true,
                type: "POST",
                url: "/Order/addProductToOrderinSession/" + ProdSeqNoToCopy,
                data: Result,
                dataType: "html",
                context: document.body,
                //contentType: 'application/html; charset=utf-8'
            })
                            .success(function (data) {
                                $('#lstProds').html(data);
                                $('#createOrderCurrentProducts .fix-overflow').show();
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })
            $('#addProductPanel').hide();
            $('#createOrderDeliveryDate .add_product').show();
            $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();
            $('#createOrderCurrentProducts').show();

            $('html,body').animate({
                scrollTop: $('#createOrderCurrentProducts').position().top + $('#createOrderPanel').position().top
            }, 400);
        }
    }
});
$('#createOrderCurrentProducts').on('click', 'a[href="#cancelPlaceOrder"]', function () {
    clearCustomerddl();
    destroyOrderSession();
    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    $('#createOrderAddProducts').hide();
    $('#createOrderCurrentProducts').hide();
    $('#createOrderSelectCustomer').show();
    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);
});
$('#createOrderCurrentProducts').on('click', 'a[href="#placeOrder"]', function () {

    if ($('#inputCustomerpo').val() == "Enter Customer PO" || $('#inputCustomerpo').val().trim() == "") {
        $('#reqCustPO').html("<p class='error-msg'>Please specify the customer PO !</p>");
        return false;
    }
    else if ($("#lstProds").html() == '') {
        $('#reqCustPO').html("<p class='error-msg'>There is no product to place order.</p>");
        return false;
    }
    else {
        var datatosend = {
            customer_id: $('#selectedCustomerId').val(),
            // requested_delivery_date: ($('#requestedDeliveryDateDay').val()) + "/" + ($('#requestedDeliveryDateMonth').val()) + "/" + ($('#requestedDeliveryDateYear').val()),
            customerpo: $('#inputCustomerpo').val()
        };

        $.ajax(
              {
                  cache: false,
                  type: "POST",
                  url: "/Order/OrderReview/",
                  context: document.body,
                  data: datatosend,
                  dataType: "html",
                  success: function (data) {
                      if (data == "0") {
                          $('#ShowOrderReview').html("<p class='error-msg'>There is no product to place order.</p>");
                          $('#btnSubmitOrder').hide();
                      }
                      else {
                          $('#ShowOrderReview').html('');
                          $('#ShowOrderReview').html(data);
                          $('#btnSubmitOrder').show();
                      }
                  }
              });

        $('#createOrderAddProducts').hide();
        $('#createOrderCurrentProducts').hide();
        $('#reviewOrder').show();

        $('html,body').animate({
            scrollTop: $('#createOrderPanel').position().top
        }, 400);
    }

});

$('#inputWidthCm')
.on('focus', function () {
    if ($(this).val() === 'Enter Width (cm)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Width (cm)');
    }
});


//Testing with clearing value Parag
//////////////////////////////////////
$('#selectWidth')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});

////End Test

$('#inputDiaCm')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});
$('#inputQuantityMt')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});
$('#inputCustomerpo')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});
/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: REVIEW ORDER
 * -------------------------------------------------------------------------------------------------------- */

$('#reviewOrder').on('click', 'a[href="#editOrder"]', function () {

    $('#reviewOrder').hide();
    $('#createOrderAddProducts').show();
    $('#createOrderCurrentProducts').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#reviewOrder').on('click', 'a[href="#cancelOrder"]', function () {
    destroyOrderSession();
    $('#createOrderAddProducts').hide();
    $('#reviewOrder').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});

$('#reviewOrder').on('click', 'a[href="#submitOrder"]', function () {

    // alert('order has been submitted');
    $.ajax({
        url: "/Order/OrderConfirmation/",
        contentType: "application/html; charset=utf-8",
        type: "POST",
        dataType: 'html'
    })
                   .success(function (result) {

                       $('#reviewOrder').slideUp();
                       $('#orderConfirmation').slideDown();
                       $('#OrderConfirmed').html(result);
                   })
                  .error(function (xhr, ajaxOptions, thrownError) {
                  })

    $('#reviewOrder').hide();
    $('#orderConfirmation').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);
    setTimeout(function () {
        //$.ajaxSetup({ cache: false });
        $.ajax({
            url: "/QuickView/QuickViewDetails/",
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                           .success(function (result) {
                               $('#quickViewRecentOrders').html(result);

                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })
    }, 500);
});


/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: ORDER CONFIRMATION
 * -------------------------------------------------------------------------------------------------------- */

$('#orderConfirmation').on('click', 'a[href="#addAnotherOrder"]', function () {
    $("#selectCustomerDropdown")[0].selectedIndex = 0;
    $('#orderConfirmation').hide();
    $('#createOrderCurrentProducts').hide();
    $('#createOrderAddProducts').hide();
    $('#createOrderSelectCustomer').show();
    clearCustomerddl();
    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});
$('#orderConfirmation').on('click', 'a[href="#closeOrder"]', function () {

    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    $('#orderConfirmation').hide();
    $('#createOrderSelectCustomer').show();
    $('#createOrderPanel').hide();

    // $.ajaxSetup({ cache: false });

    $.ajax({
        url: "/QuickView/QuickViewDetails/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#quickViewRecentOrders').html(result);
                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

    $('html,body').animate({
        scrollTop: 0
    }, 400);
});

/* -----------------------------------------------------------------------------------------------------------
 * PANEL GRID
 * -------------------------------------------------------------------------------------------------------- */

$('.panel_grid').on('click', 'a[href="#customers"]', function () {

    $('#emailErrmsg').hide();
    $('#createNewCustomerErrmsg').hide();
    clearAllCustomerddls();



    $("#reqCustPO").html('');
    $('#selectedCustomerResults').hide();
    $('#alphabetCustomerResults').hide();
    $('.order_details').hide();
    $('#ordersPanel').hide();
    $('#transportationPanel').hide();
    $('#createOrderPanel').hide();

    if ($('#customersPanel').is(':hidden')) {
        $('#customersPanel').show();
        $('#selectCustomer').show();

        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top
        }, 400);
    } else {
        $('#customersPanel').hide();
        selectOrderTypeDropdown
        $('html,body').animate({
            scrollTop: 0
        }, 400);

        //reset drp down
        // $('#customersSelectCustomerDropdown')[0].selectedIndex = 0;
        //  $('#selectCustomerAlphabetically')[0].selectedIndex = 0;
        // clearAllCustomerddls();
        //clearsearch
        ClearAutoCompleteSearchForCustomerTab();
    }
    //---hide previously opned div's
    $("#orderConfirmation").hide();
    clearAddProductForm();
    CloseAddProductPanel();


});
function clearOrderPanel() {
    //changed By Sagar 20-8-2015
    $('#selectOrderTypeDropdown').val('').trigger('chosen:updated');
    $('#selectOrderTypeDropdown').find('option:first').prop('selected', 'selected').trigger('change');
    $("#selectOrderTypeDropdown").click();


    //$('#orderFromDateDay').val('DD');
    //$('#orderFromDateMonth').val('MM');
    //$('#orderFromDateYear').val('YYYY');
    //$('#orderToDateDay').val('DD');
    //$('#orderToDateMonth').val('MM');
    //$('#orderToDateYear').val('YYYY');
    $('#orderFromDateDay').val('DD').trigger('chosen:updated');
    $('#orderFromDateMonth').val('MM').trigger('chosen:updated');
    $('#orderFromDateYear').val('YYYY').trigger('chosen:updated');
    $('#orderToDateDay').val('DD').trigger('chosen:updated');
    $('#orderToDateMonth').val('MM').trigger('chosen:updated');
    $('#orderToDateYear').val('YYYY').trigger('chosen:updated');





    $('#orderPurchaseOrderNumber').val("");
}
$('.panel_grid').on('click', 'a[href="#orders"]', function () {
    $("#ordersResultsPanel").hide();
    clearOrderPanel();
    $("#DateGreaterError").hide();
    $("#ErrorMsgInvalidFromDate").hide();
    $("#ErrorMsgInvalidToDate").hide();
    $("#PONoErrors").hide();
    $('.order_details').hide();
    $('#customersPanel').hide();
    $('#transportationPanel').hide();
    $('#createOrderPanel').hide();

    if ($('#ordersPanel').is(':hidden')) {
        $('#ordersPanel').show();
        $('#selectOrder').show();

        $('html,body').animate({
            scrollTop: $('#ordersPanel').position().top
        }, 400);
    } else {
        $('#ordersPanel').hide();

        $('html,body').animate({
            scrollTop: 0
        }, 400);
    }


    //reset drp down order status and show date div when order tab is clicked.
    $("#selectOrderTypeDropdown")[0].selectedIndex = 0;
    $("#orderByPurchaseOrderNumber").hide();
    $("#orderByDateRange").show();
    //-----
});

$('.panel_grid').on('click', 'a[href="#transportation"]', function () {
    $('#searchTransportationErrmsg').hide();
    $('#searchTransportationErrmsg1').hide();
    $('#arrangeNewTransportation').hide();//hide this div..it will only opened when user clicks on "arrange transportation" btn.
    $('.order_details').hide();
    $('#customersPanel').hide();
    $('#ordersPanel').hide();
    $('#createOrderPanel').hide();
    RefreshSearchTransporationPanel();
    RefreshArrangeTransporationPanel();
    $('#transportationFromTo').show();
    $('#transportationVehicleNumber').hide();
    $('#Productsddl').val('Select Product');
    $('#addCargoPanelEnterQuantity').val('');
    $('#transportationResultsPanel').hide();
    $('#transportationDetails').hide();
    DropdownTriggerOfTransport();

    if ($('#transportationPanel').is(':hidden')) {
        $('#transportationPanel').show();
        $('#selectTransportation').show();

        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top
        }, 400);
    } else {
        $('#transportationPanel').hide();

        $('html,body').animate({
            scrollTop: 0
        }, 400);
    }
    //--hide vehicleNumberInput and show transportationFromTo div ...when user clicks on transportation tab.
    $('#vehicleNumberInput').hide();
    $('#transportationFromTo').show();


});
$('.panel_grid').on('click', 'a[href="#reports"]', function () {

});

/* -----------------------------------------------------------------------------------------------------------
 * CUSTOMERS
 * -------------------------------------------------------------------------------------------------------- */

$('#customersPanel').on('click', 'a[href="#closeCustomersPanel"]', function () {
    $('a.create_order').show();
    $('#customersPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);
    ClearAutoCompleteSearchForCustomerTab();
    clearAllCustomerddls();
});
$('#customersPanel').on('click', 'a[href="#createCustomer"]', function () {
    clearCreateCustomerForm();
    $('a.create_order').show();
    $('#customersPanel').hide();
    $('#createOrderSelectCustomer').hide();
    $('#createOrderPanel').show();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);


});

/* -----------------------------------------------------------------------------------------------------------
 * CUSTOMERS: AUTO-SEARCH
 * -------------------------------------------------------------------------------------------------------- */

var readyToClose = false,
    customerList = [
    {
        label: "Customer A", value: "customer-a", address: "523 Anywhere Ave.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-777-5555", fax: "800-777-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, {
            po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015'
        }]
    },
{
    label: "Customer B", value: "customer-b", address: "1234 Nowhere Ln.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-5555", fax: "800-888-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, {
        po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015'
    }]
},
{
    label: "Customer C", value: "customer-c", address: "16B Forest Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-542-5555", fax: "800-542-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, {
        po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015'
    }]
}
    ];

function buildCustomersAutoComplete() {
    var getData = function (request, response) {
        $.ajax({
            url: '/Agent/GetSearchResult', type: "GET", dataType: "json",

            // query will be the param used by your action method
            data: {
                query: request.term
            },
            success: function (data) {
                response(data);
            }
        })
    };

    $('#autoSearchCustomers').autocomplete({
        appendTo: '#autoSearchCustomersResults',
        source: getData,
        create: function () {
            readyToClose = false;
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                return $('<li>')
                   .attr('data-value', item.value)
                   .append('<p class="customer_name">' + item.label + '</p>')
                   .appendTo(ul);
            };
            $(this).data('ui-autocomplete').close = function () {
                if (!readyToClose) {
                    this.cancelSearch = false;
                    return false;
                }

                this._close();
            };
        },
        select: function (evt, ui) {


            var address1 = ui.item.address1;
            var address2 = ui.item.address2;
            var address3 = ui.item.address3;
            var city = ui.item.city;
            var state = ui.item.state;
            var country = ui.item.country;
            var pin = ui.item.pincode;
            if (address1 == null) { address1 = "" }
            if (address2 == null) { address2 = "" }
            if (address3 == null) { address3 = "" }
            if (city == null) { city = "" }
            if (state == null) { state = ""; }
            if (country == null) { country = ""; }
            if (pin == null) { pin = "" }

            var finaladdress = address1 + address2 + address3;
            var Getaddress = city + " / " + state + " / " + country + " / " + pin;
            var customerDtls = "";
            if (Getaddress.indexOf(" /  /  / ") > -1) {
                Getaddress = Getaddress.replace(' /  / ', '');
            }
            if (Getaddress.indexOf(" /  / ") > -1) {
                Getaddress = Getaddress.replace('  / ', '');
            }
            if (Getaddress.substr(Getaddress.length - 1) == "/") {
                Getaddress = (Getaddress.substring(0, Getaddress.length - 3));
            }
            if (Getaddress.substr(Getaddress.length - 3) == " / ") {
                Getaddress = (Getaddress.substring(0, Getaddress.length - 3));
            }
            if (address1 == "")
                customerDtls = '<p>' + address2 + '</p> <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else if (address2 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else if (address3 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + address2 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else if (address2 == "" && address3 == "")
                customerDtls = '<p>' + address1 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else if (address1 == "" && address2 == "")
                customerDtls = '<p>' + address3 + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else if (address1 == "" && address3 == "")
                customerDtls = '<p>' + address2 + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';
            else
                customerDtls = '<p>' + address1 + '</p> <p>' + address2 + '</p>  <p>' + address3 + '</p> <p>' + Getaddress + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>';


            var customerDetails = customerDtls,
            orderHistory = '',
            customerOrders = '';

            $(evt.currentTarget)
                .find('li[data-value="' + ui.item.value + '"]')
                .append('<div class="list-item-wrapper"><div class="customer-list-item"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="/images/close-panel.png"></a> <p class="title"><strong>' + ui.item.label + '</strong></p></div> ' + customerDetails + orderHistory + customerOrders + '</div></div>');

            // stop the event here or the list will autoclose on select
            $('#autoSearchCustomers').autocomplete('disable');

            return false;
        },
        open: function (evt, ui) {
            $('#selectCustomer .auto_search_results').show();
            $('#autoSearchCustomers').addClass('search-input-clear');
            $('#selectCustomer .auto_search_input_field').find('a').show();
        }
    });
}

$('#selectCustomer').on('focus', '#autoSearchCustomers', function () {
    if ($('#selectCustomer .auto_search_results').is(':visible')) {
        readyToClose = true;

        $('#autoSearchCustomers').autocomplete('close');
        $('#autoSearchCustomers').autocomplete('destroy');
        $('#selectCustomer .auto_search_results').hide();
        $('#autoSearchCustomers').removeClass('search-input-clear');
        $('#selectCustomer .auto_search_input_field').find('a').hide();

        buildCustomersAutoComplete();
        $('#autoSearchCustomers').val('').focus();
    }
});
$('#autoSearchCustomersResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $('#autoSearchCustomersResults .list-item-wrapper').remove();
    $('#autoSearchCustomers').autocomplete('enable');
    $('#customersPanel').click();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);
});
$('#selectCustomer').on('click', 'a[href="#clearSearch"]', function () {
    ClearAutoCompleteSearchForCustomerTab(); //call same fun when user clicks on customer tab
});

function ClearAutoCompleteSearchForCustomerTab() {
    //  
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();

    buildCustomersAutoComplete();
}


function createOrderFromCustomer() {
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();

    buildCustomersAutoComplete();

    $('#selectCustomer').hide();
    $('#customersPanel').hide();
    $('#createOrderSelectCustomer').hide();
    $('#createCustomerPanel').hide();
    $('#selectedCustomerResults').hide();
    $('#alphabetCustomerResults').hide();
    $('#createOrderAddProducts').show();
    $('#createOrderPanel').show();

    $('html,body').animate({
        scrollTop: 0
    }, 400);
}
$('#autoSearchCustomersResults').on('click', 'a[href="#createOrderFromCustomer"]', function () {

    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    destroyOrderSession();
    $('#reviewOrder').hide();
    $('#selectedCustomerId').val($(this).attr('data-value'));
    $('#selectedCustomerName').html($(this).attr('data-name'));
    createOrderFromCustomer();
    clearAddProductForm();
    CloseAddProductPanel();
    $('#createOrderCurrentProducts').hide();

});
$('#selectedCustomerResults').on('click', 'a[href="#createOrderFromCustomer"]', function () {

    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    destroyOrderSession();
    $('#reviewOrder').hide();
    $('#selectedCustomerId').val($(this).attr('data-value'));

    $('#selectedCustomerName').html($(this).attr('data-name'));
    createOrderFromCustomer();
    clearAddProductForm();
    CloseAddProductPanel();
    $('#createOrderCurrentProducts').hide();

});
$('#alphabetCustomerResults').on('click', 'a[href="#createOrderFromCustomer"]', function () {

    destroyOrderSession();
    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    $('#reviewOrder').hide();
    $('#selectedCustomerName').html($(this).attr('data-name'));
    $('#selectedCustomerId').val($(this).attr('data-id'));
    createOrderFromCustomer();
    clearAddProductForm();
    CloseAddProductPanel();
    $('#createOrderCurrentProducts').hide();

});
$('#customersSelectCustomerDropdownPanel').on('change', 'select', function () {
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    $('#alphabetCustomerResults').hide();

    buildCustomersAutoComplete();
    var selectedCustomerid = $('select#customersSelectCustomerDropdown').val();
    var selCustName = $('select#customersSelectCustomerDropdown option:selected').text();

    if ($(this).val() !== '') {
        $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Customer/GetCustomer/" + selectedCustomerid,
                  contentType: "application/html; charset=utf-8",
                  success: function (data) {
                      $('#selectedCustomerResults').show();
                      $('#selectedCustomerResults').html(data);

                  }
              });
        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top + 350
        }, 400);
    } else {
        $('#selectedCustomerResults').hide();
    }

    //Added BY Sagar
    $("#selectedCustomerResults").find('#customerOrderHistoryFromDateDay').val("DD").trigger('chosen:updated');
    $("#selectedCustomerResults").find('#customerOrderHistoryFromDateMonth').val("MM").trigger('chosen:updated');
    $("#selectedCustomerResults").find('#customerOrderHistoryFromDateYear').val("YYYY").trigger('chosen:updated');
    $("#selectedCustomerResults").find('#customerOrderHistoryToDateDay').val("DD").trigger('chosen:updated');
    $("#selectedCustomerResults").find('#customerOrderHistoryToDateMonth').val("MM").trigger('chosen:updated');
    $("#selectedCustomerResults").find('#customerOrderHistoryToDateYear').val("YYYY").trigger('chosen:updated');






});
$('#autoSearchCustomersResults').on('click', 'a[href="#viewOrderHistory"]', function () {
    $(this).next('.customer_orders').show();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + $(this).closest('.customer-list-item').height() - 340
    }, 400);


});

$('#selectedCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $(this).closest('.customer-list-item').find('.customer_orders').hide();
    $('#selectedCustomerResults').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);


});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCustomerResults"]', function () {
    readyToClose = false;


    $('#alphabetCustomerResults').find('.customer-list-item .customer_orders').hide();
    $('#alphabetCustomerResults .customer-list-item').hide();
    $('#alphabetCustomerResults').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);


});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $(this).closest('.customer-list-item').find('.customer_orders').hide();
    $(this).closest('.customer-list-item').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);


});


$('#autoSearchCustomersResults').on('click', 'a[href="#seeOrderDetails"]', function () {
    customersSeeOrderDetails($(this).attr('data-id'));


});

$('#alphabetCustomerResults').on('click', 'a[href="#seeOrderDetails"]', function () {
    customersSeeOrderDetails($(this).attr('data-id'));


});
$('#autoSearchCustomersResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 200
    }, 400);


});

$('#selectedCustomerResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 225
    }, 400);


});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 250
    }, 400);


});
$('#browseCustomerAlphabetically').on('change', 'select', function () {
    $.ajax(
            {
                cache: false,
                type: "POST",
                url: "/Customer/clearAlphabetcustomerSession/",
                data: datatosend,
                dataType: "html",
                success: function (data) {

                }
            });
    var selectedCustomerAlphabet = $('select#selectCustomerAlphabetically').val();
    if (selectedCustomerAlphabet != "") {
        var datatosend = {
            selectedAlphabet: selectedCustomerAlphabet
        }
        $.ajax(
                 {
                     cache: false,
                     type: "POST",
                     url: "/Customer/GetCustomersbyAlphabet/",
                     data: datatosend,
                     dataType: "html",
                     success: function (data) {
                         $('#alphabetCustomerResults').show();
                         $('#tmpSearchResCustsAlphabetically').html(data);

                     }
                 });
    }
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    $('#selectedCustomerResults').hide();

    buildCustomersAutoComplete();

    if ($(this).val() !== '') {
        $('#alphabetCustomerResults').show();

        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top + 400
        }, 400);
    } else {
        $('#alphabetCustomerResults').hide();
    }
});
$('#alphabetCustomerResults').on('click', 'a[href="#selectCustomer"]', function () {
    $('#alphabetCustomerResults .customer-list-item').hide();
    $(this).closest('li').find('.customer-list-item').show();

    var liid = $(this).closest("li").attr('id');//alphabetOrderHistoryFromDateDay
    $("#" + liid).find('#ErrorMsgInvalidFromDate').hide();
    $("#" + liid).find('#ErrorMsgInvalidToDate').hide();
    $("#" + liid).find('#reqCustViewalphaErrors').hide();
    $("#" + liid).find('#PONoErrors').hide();
    $("#" + liid).find('#alphabetOrderHistoryFromDateDay').val("DD").trigger('chosen:updated');
    $("#" + liid).find('#alphabetOrderHistoryFromDateMonth').val("MM").trigger('chosen:updated');
    $("#" + liid).find('#alphabetOrderHistoryFromDateYear').val("YYYY").trigger('chosen:updated');
    $("#" + liid).find('#alphabetOrderHistoryToDateDay').val("DD").trigger('chosen:updated');
    $("#" + liid).find('#alphabetOrderHistoryToDateMonth').val("MM").trigger('chosen:updated');
    $("#" + liid).find('#alphabetOrderHistoryToDateYear').val("YYYY").trigger('chosen:updated');

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 425
    }, 400);

});
$('#autoSearchCustomers')
.on('focus', function () {
    if ($(this).val() === 'Search') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Search');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * ORDERS
 * -------------------------------------------------------------------------------------------------------- */

$('#ordersPanel').on('click', 'a[href="#closeOrdersPanel"]', function () {

    $('a.create_order').show();
    $('#ordersPanel').hide();
    $("#ErrorMsgInvalidFromDate").hide();
    $("#ErrorMsgInvalidToDate").hide();
    $("#DateGreaterError").hide();
    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('#selectOrderPanel').on('change', '#selectOrderTypeDropdown', function () {
    $('#PONoErrors').hide();
    $('#ErrorMsgInvalidToDate').hide();
    $('#ErrorMsgInvalidFromDate').hide();
    $('#DateGreaterError').hide();
    $("#ordersResultsPanel").hide();

    if ($(this).val() === 'Purchase-Order-Number') {
        $('#orderByDateRange').hide();
        $('#orderByPurchaseOrderNumber').show();
    } else {
        $('#orderByPurchaseOrderNumber').hide();
        $('#orderByDateRange').show();
    }
});
$('#selectOrderPanel').on('click', 'a[href="#searchOrders"]', function () {

    OnOrderSearchClearSeesion();

    var fromdt = $('#orderFromDateDay').val() + "/" + $('#orderFromDateMonth option:selected').text() + "/" + $('#orderFromDateYear').val();
    var todt = $('#orderToDateDay').val() + "/" + $('#orderToDateMonth option:selected').text() + "/" + $('#orderToDateYear').val();
    var FromDateTime = ValidateDate(fromdt);
    var ToDateTime = ValidateDate(todt);

    function SearchOrderAjax() {
        if ($('#selectOrderPanel').find('#selectOrderTypeDropdown').val() == 'Purchase-Order-Number') {
            var datatosend =
           {
               SelectedOrderType: $('#selectOrderTypeDropdown').val(),
               poNumber: $('#orderPurchaseOrderNumber').val()
           }
        }
        else {
            var datatosend =
           {
               SelectedOrderType: $('#selectOrderTypeDropdown').val(),
               FromDateTime: $('#orderFromDateDay').val() + "-" + $('#orderFromDateMonth').val() + "-" + $('#orderFromDateYear').val(),
               ToDateTime: $('#orderToDateDay').val() + "-" + $('#orderToDateMonth').val() + "-" + $('#orderToDateYear').val(),
           }
        }


        $.ajax({
            url: "/Order/OrdersSearchResults/",
            data: datatosend,
            type: "POST",
            dataType: 'html',
            context: document.body,
        })
                        .success(function (result) {
                            $('#ordersResultsPanel').slideDown();
                            $('#ordersResultsPanel').html(result);
                        })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })

        $('html,body').animate({
            scrollTop: $('#ordersResultsPanel').position().top + $('#selectOrderPanel').height() + 40
        }, 400);

    }


    function CheckDateValidation() {
        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        $('#DateGreaterError').hide();
        $('#PONoErrors').hide();
        if ($('#orderFromDateDay').val() == "DD" && $('#orderFromDateMonth').val() == "MM" && $('#orderFromDateYear').val() == "YYYY"
    && $('#orderToDateDay').val() == "DD" && $('#orderToDateMonth').val() == "MM" && $('#orderToDateYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').show();
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').show();
        } else if ($('#orderFromDateDay').val() == "DD" || $('#orderFromDateMonth').val() == "MM" || $('#orderFromDateYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').show();
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').hide();
            if ($('#orderToDateDay').val() == "DD" || $('#orderToDateMonth').val() == "MM" || $('#orderToDateYear').val() == "YYYY") {
                $('#ErrorMsgInvalidToDate').show();
                $('#ordersResultsPanel').hide();
            }
        }
        else if ($('#orderFromDateDay').val() == null && $('#orderFromDateMonth').val() == null && $('#orderFromDateYear').val() == null && $('#orderToDateDay').val() == null && $('#orderToDateMonth').val() == null && $('#orderToDateYear').val() == null) {
            $('#ErrorMsgInvalidFromDate').show();
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').show();

        }
        else if ($('#orderFromDateDay').val() == null || $('#orderFromDateMonth').val() == null || $('#orderFromDateYear').val() == null) {
            $('#ErrorMsgInvalidFromDate').show();
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').hide();

        }
        else if ($('#orderToDateDay').val() == null || $('#orderToDateMonth').val() == null || $('#orderToDateYear').val() == null) {
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ErrorMsgInvalidToDate').show();
            $('#ordersResultsPanel').hide();

        }

        else if ($('#orderToDateDay').val() == "DD" || $('#orderToDateMonth').val() == "MM" || $('#orderToDateYear').val() == "YYYY") {
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').show();
            if (FromDateTime == false) {
                $('#ordersResultsPanel').hide();
                $('#ErrorMsgInvalidFromDate').show();
            }
        } else if (ToDateTime == false && FromDateTime == false) {
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').show();
            $('#ErrorMsgInvalidFromDate').show();
        } else if (ToDateTime == false) {
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidFromDate').hide();
            $('#ErrorMsgInvalidToDate').show();

        } else if (FromDateTime == false) {
            $('#ordersResultsPanel').hide();
            $('#ErrorMsgInvalidToDate').hide();
            $('#ErrorMsgInvalidFromDate').show();
        }
        else {
            $('#ErrorMsgInvalidToDate').hide();//
            $('#ErrorMsgInvalidFromDate').hide();//$('#orderToDateDay').val() 
            var fromdtChk = $('#orderFromDateYear').val() + "/" + $('#orderFromDateMonth option:selected').text() + "/" + $('#orderFromDateDay').val();
            var todtChk = $('#orderToDateYear').val() + "/" + $('#orderToDateMonth option:selected').text() + "/" + $('#orderToDateDay').val();

            var dateCompareResult = CompareDate(fromdtChk, todtChk);
            if (dateCompareResult != true) {
                $('#ordersResultsPanel').hide();
                $('#DateGreaterError').show();
            }
            else {
                $('#PONoErrors').hide();
                $('#ErrorMsgInvalidToDate').hide();
                $('#ErrorMsgInvalidFromDate').hide();
                $('#DateGreaterError').hide();
                SearchOrderAjax();

            }

        }


    }

    if ($('#selectOrderPanel').find('#selectOrderTypeDropdown').val() == 'Purchase-Order-Number') {
        $('#DateGreaterError').hide();
        $('#ErrorMsgInvalidToDate').hide();
        $('#ErrorMsgInvalidFromDate').hide();
        if ($("#orderPurchaseOrderNumber").val() == "Type Purchase Order #" || $("#orderPurchaseOrderNumber").val() == "") {
            $('#PONoErrors').show();
        }
        else {
            $('#PONoErrors').hide();
            $('#ErrorMsgInvalidToDate').hide();
            $('#ErrorMsgInvalidFromDate').hide();
            $('#DateGreaterError').hide();
            SearchOrderAjax();
        }
    }
    else {

        CheckDateValidation();

    }

});


function OnOrderSearchClearSeesion() {

    $.ajax({
        url: "/Order/ClearSessionOnOrdersSearch/",
        //data: datatosend,
        type: "POST",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })
}
function OnVehiclesSearchClearSession() {

    $.ajax({
        url: "/Truck_dispatch/ClearSessionOnVehiclesSearch/",
        //data: datatosend,
        type: "POST",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })
}
$('#ordersResultsPanel').on('click', 'a[href="#closeOrdersResultsPanel"]', function () {
    $('#ordersResultsPanel').hide();

});

$('#ordersResultsPanel').on('click', 'a[href="#seeOrderDetails"]', function () {
    var id = $(this).attr('data-order-id');
    $('#tmporderid').val(id);
    $('.arrange_new_transportation ').hide();
    $.ajax({
        url: "/quickView/GetOrderDetailsMaster/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                           .success(function (result) {
                               $('#customersPanel').hide();
                               $('#transportationPanel').hide();
                               $('.close_orders_panel').hide();
                               $('#selectOrder').hide();
                               $('#ordersResultsPanel').hide();
                               $('#orderDetails').show();
                               $('#tmpOrderDetails').html(result);
                               $('#ordersPanel').show();
                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })

    $.ajax({
        url: "/quickView/GetOrderDetailsChild/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                           .success(function (result) {
                               $('#customersPanel').hide();
                               $('#transportationPanel').hide();
                               $('.close_orders_panel').hide();
                               $('#selectOrder').hide();
                               $('#ordersResultsPanel').hide();
                               //$('#orderDetails-' + id).show();
                               $('#orderDetails').show();
                               $('#tmpOrderProdsList').html(result);
                               $('#ordersPanel').show();
                               // $('#ordersPanel').html(result);
                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top
    }, 400);


});

$('#ordersPanel .order_details').on('click', 'a[href="#arrangeTransportation"]', function () {

    DropdownTriggerForArrangeTransport();
    $('.submit_transportation_button').hide();
    $('#EditCargoToerrorMsg').hide();
    $('#addCargoErrMsg').hide();
    $('#addCargoErrMsg1').hide();
    $('.add_cargo_panel').hide();
    $('#tmpOrderSearchshowCargoList').html('');
    var selectedorderid = $('#tmporderid').val();
    var selectedPapermillid = $('#tmplocationid').val();
    if ($('#tmpOrderDetails .order_details_full span').attr('data-status') == "Created") {

    }
    else {
        //if ($(this).next('.arrange_new_transportation').is(':hidden')) {
        $.ajax(
                   {
                       //  cache: false,
                       type: "GET",
                       url: "/Truck_dispatch/DestroyTransportationSession/",
                       contentType: "application/text; charset=utf-8",
                       success: function () {
                       },
                       error: function (xhr, ajaxOptions, thrownError) {
                       }
                   });
        $.ajax({
            cache: false,
            url: "/Order/GetOrderDetails/" + selectedorderid,
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body
        })
                                .success(function (result) {
                                    //
                                    $('#tmpOrderDetailsArrNewTransportation').html(result);
                                })
                               .error(function (xhr, ajaxOptions, thrownError) {
                               })

        //  var tmpddlOrders = $('#orderDetails').find("#tmpOrderddl");
        $.ajax(
               {

                   type: "GET",
                   url: "/Home/GetAllOrdersbyAgentandLocation/" + selectedPapermillid,
                   contentType: "application/json; charset=utf-8",
                   success: function (data) {

                       $('#tmpOrderddl').html('');
                       $('#tmpOrderddl').append($('<option>Select Purchase Order</option>'));
                       $.each(data, function (id, option) {

                           $('#tmpOrderddl').append($('<option>Select Purchase Order</option>').val(option.id).html((option.id) + ", " + option.name));

                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {
                   }
               });

        $(this).next('.arrange_new_transportation').show();//line 688 on dashboard 

        $('html,body').animate({
            scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 70
        }, 400);
        //}
    }

});

$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#closeArrangeTransportationPanel"]', function () {
    $(this).closest('.arrange_new_transportation').hide();


});


$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#addCargoToTransportation"]', function () {

    dropdownTriggerForAddCargo();
    $('#qtyErrorMsg').hide();
    $('#tmpOrderddl').val('Select Purchase Order');
    $('#tmpProductsddl').val('Select Product');
    $('#ordersAddCargoEnterQuantity').val('');
    $('#tmpProdAvailableQty').html('');
    var vechVal = $('#OrderSearchTransportationVehicleCapacity').val();
    var selectedPapermillid = $('#tmplocationid').val();
    var datatosend = {
        loc_id: selectedPapermillid, // i need to supply
        vehicle_num: $('#OrderSearchTransportationVehicleNumber').val(),
        vehicle_capacity: $('#OrderSearchTransportationVehicleCapacity').val(),

        agent_disp_on: $('#ordersNewTransportationStartDay').val() + "/" + $('#orderNewTransportationStartMonth').val() + "/" + $('#ordersNewTransportationStartYear').val(),
        agent_disp_on_time: $('#ordersNewTransportationStartTime').val(),
        agent_disp_on_time_ampm: $('#ordersNewTransportationStatTimeAmPm').val(),

        estimated_arr_date: $('#ordersNewTransportationArrivalDay').val() + "/" + $('#ordersNewTransportationArrivalMonth').val() + "/" + $('#ordersNewTransportationArrivalYear').val(),
        estimated_arr_date_time: $('#ordersNewTransportationArrivalTime').val(),
        estimated_arr_date_time_ampm: $('#ordersNewTransportationArrivalTimeAmPm').val(),
    };
    var selectedTodate = $('#ordersNewTransportationStartDay').val() + "/" + $('#orderNewTransportationStartMonth').val() + "/" + $('#ordersNewTransportationStartYear').val();
    var selectedFromdate = $('#ordersNewTransportationArrivalDay').val() + "/" + $('#ordersNewTransportationArrivalMonth').val() + "/" + $('#ordersNewTransportationArrivalYear').val();

    var fromdateTime = ValidateDate(selectedFromdate);
    var todateTime = ValidateDate(selectedTodate);

    var startdt = $('#orderNewTransportationStartMonth').val() + "/" + $('#orderNewTransportationStartMonth').val() + "/" + $('#ordersNewTransportationStartYear').val();
    var arrivaldt = $('#ordersNewTransportationArrivalMonth').val() + "/" + $('#ordersNewTransportationArrivalDay').val() + "/" + $('#ordersNewTransportationArrivalYear').val();

    var dateCompare = CompareDate(startdt, arrivaldt);

    $('#EditCargoToerrorMsg').hide();
    $('#EditCargoToerrorMsg1').hide();

    if ($('#OrderSearchTransportationVehicleNumber').val() == "") {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select Papermill and enter vehicle Number !");
        if ($('#newTransportationVehicleCapacity').val() == "") {
            $('#EditCargoToerrorMsg').show();
            $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please enter vehicle Number and Vehicle Capacity in MT !");
        }
    }



    else if ($('#OrderSearchTransportationVehicleCapacity').val() == "") {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please enter Vehicle Capacity in MT !");
    }


    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(vechVal)) {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Only numbers are allowd in Vechicle Capacity !");
    }


    else if (fromdateTime == false && todateTime == false) {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg1').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid start date !");
        $('#EditCargoToerrorMsg1').html('Please select a Valid arrival date !');
    }
    else if ($('#orderNewTransportationStartMonth').val() == "DD" || $('#orderNewTransportationStartMonth').val() == "MM" || $('#ordersNewTransportationStartYear').val() == "YYYY") {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid Start date.");
        if ($('#ordersNewTransportationArrivalMonth').val() == "DD" || $('#ordersNewTransportationArrivalDay').val() == "MM" || $('#ordersNewTransportationArrivalYear').val() == "YYYY") {
            $('#EditCargoToerrorMsg1').show();
            $('#EditCargoToerrorMsg').show();
            $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid start date !");
            $('#EditCargoToerrorMsg1').html('Please select a Valid arrival date !');

        }
    } else if ($('#ordersNewTransportationArrivalMonth').val() == "DD" || $('#ordersNewTransportationArrivalDay').val() == "MM" || $('#ordersNewTransportationArrivalYear').val() == "YYYY") {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid expected Arrival date !");
    }
    else if (fromdateTime == false) {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid expected arrival date !");
    }
    else if (todateTime == false) {
        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Please select a Valid start date !");
    } else if (dateCompare == false) {

        $('#EditCargoToerrorMsg').show();
        $('#EditCargoToerrorMsg').html("<p class='error-msg'>Expected Arrival date should be greater than or equal to Start date !");
    }
    else {
        //$.ajax({
        //    cache: false,
        //    type: "POST",
        //    url: "/Truck_dispatch/addTempDispatchinSession/",
        //    context: document.body,
        //    data: datatosend,
        //    dataType: "html",
        //    context: document.body,
        //})
        //                 .success(function (result) {

        //                 })
        //                .error(function (xhr, ajaxOptions, thrownError) {
        //                })

        //$.ajax(
        //      {
        //          // cache: false,
        //          type: "GET",
        //          url: "/Home/GetAllOrdersbyAgentandLocation/" + selectedPapermillid,
        //          contentType: "application/json; charset=utf-8",
        //          success: function (data) {

        //              //tmpddlOrders.html('');
        //              $('#tmpOrderddl').html('');
        //              $('#tmpOrderddl').append($('<option>Select Purchase Order</option>'));
        //              $.each(data, function (id, option) {

        //                  $('#tmpOrderddl').append($('<option>Select Purchase Order</option>').val(option.id).html((option.id) + ", " + option.name));

        //              });
        //          },
        //          error: function (xhr, ajaxOptions, thrownError) {
        //          }
        //      });

        var $detailsPanel = $(this).closest('.order_details'),
            $transportationPanel = $(this).closest('.arrange_new_transportation'),
            detailsFull = $detailsPanel.find('.order_details_full').height(),
            locationDetails = $detailsPanel.find('.location_details').height(),
            timeFrames = $detailsPanel.find('.start_arrival_times').height(),
            cargoList = 0;

        //$transportationPanel.find('.submit_transportation_button').hide();
        $transportationPanel.find('.add_cargo_panel').show();

        if ($detailsPanel.find('.cargo_list').is(':visible')) {
            cargoList = $detailsPanel.find('.cargo_list').height();
        }

        $('html,body').animate({
            scrollTop: $('#ordersPanel').position().top + detailsFull + 300 + locationDetails + timeFrames + cargoList
        }, 400);
    }
});

$('#orderDetails').find("#tmpOrderddl").on('change', function () {
    var selectedItem = $(this).val();
    var tmpddlProducts = $('#orderDetails').find("#tmpProductsddl");

    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Home/GetProductsByOrderId/" + selectedItem,
                   contentType: "application/json; charset=utf-8",
                   success: function (data) {

                       tmpddlProducts.html('');
                       tmpddlProducts.append($('<option>Select Product</option>'));
                       $.each(data, function (id, option) {

                           tmpddlProducts.append($('<option></option>').val(option.id).html(parseFloat(option.width).toFixed(2) + " cm x " + option.name));

                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });

});

$('#orderDetails').find('#tmpProductsddl').on('change', function () {

    var selectedItem = $(this).val();
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order/GetProductsAvailableQty/" + selectedItem,
                   contentType: "application/html; charset=utf-8",
                   success: function (data) {
                       $('#tmpProdAvailableQty').html("Available Quantity: " + data);
                       $('#OnAddCargoAvailableQty').html(data);
                   },
                   error: function (xhr, ajaxOptions, thrownError) {

                   }
               });
});

$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#addNewCargoItem"]', function () {
    // make the ajax call to add the temp dispatch in session starts

    var prod_code = $('#orderDetails').find("#tmpProductsddl option:selected").text();

    var order_id = $('#orderDetails').find('#tmpOrderddl').val();
    var prod_id = $('#orderDetails').find('#tmpProductsddl').val();
    var enterdQty = $('#ordersAddCargoEnterQuantity').val();
    var availableQty = $('#OnAddCargoAvailableQty').text();

    if ((order_id == "Select Purchase Order" && prod_id == "Select Product") || (order_id == "" && prod_id == "")) {
        $('#addCargoMsg').show();
        $('#addCargoMsg').text("Order and Product code needs to be selected !");
    }
    else
        if ((order_id == "Select Purchase Order" || order_id == "")) {
            $('#addCargoMsg').show();
            $('#addCargoMsg').text("Order needs to be selected !");
        }
        else
            if ((prod_id == "Select Product" || prod_id == "")) {
                $('#addCargoMsg').show();
                $('#addCargoMsg').text("Product code needs to be selected !");
            }
            else {
                if (enterdQty == "") {
                    $('#addCargoMsg').show();
                    $('#addCargoMsg').text("Enter Quantity (MT) greater than zero !");
                } else
                    if (enterdQty == "Enter Quantity (MT)") {
                        $('#addCargoMsg').show();
                        $('#addCargoMsg').text("Enter Quantity (MT) greater than zero !");
                    } else
                        if (enterdQty == "0") {
                            $('#addCargoMsg').show();
                            $('#addCargoMsg').text("Enter Quantity (MT) greater than zero !");
                        }
                        else
                            if (Math.round(enterdQty * 100) == Math.round(0 * 100)) {
                                $('#qtyErrorMsg').show();
                                $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                            }
                            else if (Math.round(enterdQty * 100) <= Math.round(availableQty * 100)) { //if (enterdQty < availableQty) {
                                var $detailsPanel = $(this).closest('.order_details'),
                                               $transportationPanel = $(this).closest('.arrange_new_transportation'),
                                               detailsFull = $detailsPanel.find('.order_details_full').height(),
                                               locationDetails = $detailsPanel.find('.location_details').height(),
                                               timeFrames = $detailsPanel.find('.start_arrival_times').height();

                                $transportationPanel.find('.cargo_list').show();
                                $transportationPanel.find('.add_cargo_panel').hide();
                                $transportationPanel.find('.submit_transportation_button').show();

                                $('html,body').animate({
                                    scrollTop: $('#ordersPanel').position().top + detailsFull + 300 + locationDetails + timeFrames
                                }, 400);

                                var allValues = {
                                    order_id: $('#orderDetails').find('#tmpOrderddl').val(),
                                    prod_id: $('#orderDetails').find('#tmpProductsddl').val(),
                                    prod_code: $('#orderDetails').find("#tmpProductsddl option:selected").text(),
                                    qty: $('#orderDetails').find('#ordersAddCargoEnterQuantity').val()
                                };

                                $.ajax({
                                    type: "Get",
                                    url: "/Truck_dispatch/OrderSearchaddTempCargoinSession/",
                                    context: document.body,
                                    data: allValues,
                                    dataType: "html",
                                    context: document.body,
                                })
                                                 .success(function (data) {
                                                     $('#orderDetails .cargo_list').show();
                                                     $('#orderDetails').find('#tmpOrderSearchshowCargoList').html(data);
                                                 })
                                                .error(function (xhr, ajaxOptions, thrownError) {
                                                })
                                $('#arrangeNewTransportation .submit_transportation_button').show();
                                $(this).closest('.add_cargo_panel').hide();

                                $('#addCargoMsg').hide();
                            }
                            else {
                                $('#addCargoMsg').show();
                                $('#addCargoMsg').text("Quantity for Loading cannot be more than Available in-Stock.");
                            }
            }



});
$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#submitTransportation"]', function () {

    var selectedPapermillid = $('#tmplocationid').val();
    var datatosend = {
        loc_id: selectedPapermillid,
        vehicle_num: $('#OrderSearchTransportationVehicleNumber').val(),
        vehicle_capacity: $('#OrderSearchTransportationVehicleCapacity').val(),

        agent_disp_on: $('#ordersNewTransportationStartDay').val() + "/" + $('#orderNewTransportationStartMonth').val() + "/" + $('#ordersNewTransportationStartYear').val(),
        agent_disp_on_time: $('#ordersNewTransportationStartTime').val(),
        agent_disp_on_time_ampm: $('#ordersNewTransportationStatTimeAmPm').val(),

        estimated_arr_date: $('#ordersNewTransportationArrivalDay').val() + "/" + $('#ordersNewTransportationArrivalMonth').val() + "/" + $('#ordersNewTransportationArrivalYear').val(),
        estimated_arr_date_time: $('#ordersNewTransportationArrivalTime').val(),
        estimated_arr_date_time_ampm: $('#ordersNewTransportationArrivalTimeAmPm').val(),
    }
    $.ajax(
    {
        cache: false,
        type: "POST",
        url: "/Truck_dispatch/ReviewEditTransportation/",
        //url: "/Truck_dispatch/OrderSearchReviewEditTransportation/",
        data: datatosend,
        dataType: "html",
        success: function (data) {
            $('#tmpOrderSearchReviewTransportation').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
    $(this).closest('.arrange_new_transportation').hide();
    $(this).closest('.order_details').find('.transportation_details').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);


});
$('#ordersPanel .transportation_details').on('click', 'a[href="#closeReviewTransportationDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.arrange_new_transportation').show();


});
$('#ordersPanel .transportation_details').on('click', 'a[href="#cancelReviewDetails"]', function () {

    DestroyTransportationSession();
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('arrange_new_transportation').hide();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);


});
$('#ordersPanel .transportation_details').on('click', 'a[href="#editReviewDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.arrange_new_transportation').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);


});
$('#ordersPanel .transportation_details').on('click', 'a[href="#submitReviewDetails"]', function () {

    $.ajax(
        {
            url: "/Truck_dispatch/confirmTransportation/",
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                  .success(function (result) {
                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.review_transportation_confirmation').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);

    ;
});
$('#ordersPanel .review_transportation_confirmation').on('click', 'a[href="#closeTransportationConfirmation"]', function () {
    $(this).closest('.review_transportation_confirmation').hide();


});
$('.order_details').on('click', 'a[href="#closeOrderDetails"]', function () {
    $(this).closest('.order_details').hide();
    $('.close_orders_panel').show();
    $('.select_order').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top
    }, 400);


});
$('#orderFromDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#orderFromDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#orderFromDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#orderToDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#orderToDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#orderToDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * TRANSPORTATION
 * -------------------------------------------------------------------------------------------------------- */
$("#SelectOrder").change(function () {
    var selectedItem = $(this).val();
    var ddlStates = $("#SelectOrderProds");
    $.ajax(
        {
            cache: false,
            type: "GET",
            url: "/Home/GetProductsByOrderId/" + selectedItem,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                ddlStates.html('');
                $.each(data, function (id, option) {
                    ddlStates.append($('<option></option>').val(option.id).html(parseFloat(option.width).toFixed(2) + " cm x " + option.name));
                });
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
});
$('#transportationPanel').on('click', 'a[href="#closeTransportationPanel"]', function () {
    $('a.create_order').show();
    $('#transportationPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);


});
$('#selectTransportationPanel').on('click', 'a[href="#searchTransportation"]', function () {
    OnVehiclesSearchClearSession();
    $('#searchTransportationErrmsg').hide();
    $('#searchTransportationErrmsg1').hide();
    $('#transportationResultsPanel').hide();




    //alert($('#selectVehicleTypeDropdown').val());
    if ($('#selectVehicleTypeDropdown').val() == 'Vehicle-Number') {
        var Result = {
            selectVehicleTypeddl: $('#selectVehicleTypeDropdown').val(),
            vehicleNumber: $('#transportationVehicleNumber').val()
        };
        if ($('#transportationVehicleNumber').val() == "Type Vehicle Number") {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please enter vehicle Number !");
        }
        else {
            transportationSearch(Result);
            $('html,body').animate({
                scrollTop: $('#transportationPanel').position().top + 380
            }, 400);
        }
    }
    else {
        var Result = {
            selectVehicleTypeddl: $('#selectVehicleTypeDropdown').val(),
            FromDateTime: $('#transportationFromDateDay').val() + "-" + $('#transportationFromDateMonth').val() + "-" + $('#transportationFromDateYear').val(),
            ToDateTime: $('#transportationToDateDay').val() + "-" + $('#transportationToDateMonth').val() + "-" + $('#transportationToDateYear').val()
        };
        var fromdt = $('#transportationFromDateDay').val() + "/" + $('#transportationFromDateMonth').val() + "/" + $('#transportationFromDateYear').val();
        var todt = $('#transportationToDateDay').val() + "/" + $('#transportationToDateMonth').val() + "/" + $('#transportationToDateYear').val();
        var fromdateTime = ValidateDate(fromdt);
        var todateTime = ValidateDate(todt);
        var fromdt1 = $('#transportationFromDateMonth').val() + "/" + $('#transportationFromDateDay').val() + "/" + $('#transportationFromDateYear').val();
        var todt1 = $('#transportationToDateMonth').val() + "/" + $('#transportationToDateDay').val() + "/" + $('#transportationToDateYear').val();
        var dateCompare = CompareDate(fromdt1, todt1);
        if (fromdateTime == false && todateTime == false) {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg1').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
            $('#searchTransportationErrmsg1').html("Please select a Valid To date !");
        }
        else if ($('#transportationFromDateDay').val() == "DD" || $('#transportationFromDateMonth').val() == "MM" || $('#transportationFromDateYear').val() == "YYYY") {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
            if ($('#transportationToDateDay').val() == "DD" || $('#transportationToDateMonth').val() == "MM" || $('#transportationToDateYear').val() == "YYYY") {
                $('#searchTransportationErrmsg').show();
                $('#searchTransportationErrmsg1').show();
                $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
                $('#searchTransportationErrmsg1').html("Please select a Valid To date !");
            }
        }
        else if ($('#transportationFromDateDay').val() == "DD" || $('#transportationFromDateMonth').val() == "MM" || $('#transportationFromDateYear').val() == "YYYY") {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
            if ($('#transportationToDateDay').val() == "DD" || $('#transportationToDateMonth').val() == "MM" || $('#transportationToDateYear').val() == "YYYY") {
                $('#searchTransportationErrmsg').show();
                $('#searchTransportationErrmsg1').show();
                $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
                $('#searchTransportationErrmsg1').html("Please select a Valid To date !");
            }
        } else if ($('#transportationToDateDay').val() == "DD" || $('#transportationToDateMonth').val() == "MM" || $('#transportationToDateYear').val() == "YYYY") {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid To date !");
        }
        else if ($('#transportationFromDateDay').val() == null || $('#transportationFromDateMonth').val() == null || $('#transportationFromDateYear').val() == null) {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
            if ($('#transportationToDateDay').val() == null || $('#transportationToDateMonth').val() == null || $('#transportationToDateYear').val() == null) {
                $('#searchTransportationErrmsg').show();
                $('#searchTransportationErrmsg1').show();
                $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
                $('#searchTransportationErrmsg1').html("Please select a Valid To date !");
            }
        }
        else if (fromdateTime == false) {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid From date !");
        }
        else if (todateTime == false) {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>Please select a Valid To date !");
        }
        else if (dateCompare == false) {
            $('#searchTransportationErrmsg').show();
            $('#searchTransportationErrmsg').html("<p class='error-msg'>To Date should be greater than equal to From Date !");
        }
        else {
            transportationSearch(Result);
            $('html,body').animate({
                scrollTop: $('#transportationPanel').position().top + 380
            }, 400);
        }
    }

});

function transportationSearch(Result) {
    $.ajax({
        //async: true,
        type: "POST",
        url: "/Truck_dispatch/searchResultTransportation/",
        data: Result,
        dataType: "html",
        context: document.body,
        //contentType: 'application/json; charset=utf-8'
    })

                   .success(function (result) {
                       $('#transportationResultsPanel').show();
                       $('#transportationResultsPanel').html(result);
                   })
                  .error(function (xhr, ajaxOptions, thrownError) {
                  })
    $('#transportationDetails').hide();
}

$('#selectTransportationPanel').on('click', 'a[href="#closeTransportationResultsPanel"]', function () {
    $('#transportationResultsPanel').hide();
    $('#selectTransportationPanel .or_wording').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

});
$('#selectTransportationPanel').on('change', 'select#selectVehicleTypeDropdown', function () {

    $('#searchTransportationErrmsg').hide();
    $('#searchTransportationErrmsg1').hide();
    if ($(this).val() === 'Vehicle-Number') {
        $('#transportationFromTo').hide();
        $('#transportationVehicleNumber').show();
        $('#vehicleNumberInput').show();
    } else {
        $('#vehicleNumberInput').hide();
        $('#transportationVehicleNumber').hide();
        $('#transportationFromTo').show();
    }
});
$('#selectTransportation').on('click', 'a[href="#arrangeTransportation"]', function () {

    $('.add_cargo_panel').hide();
    $('#addCargoErrMsg').hide();
    $('#addCargoErrMsg1').hide();
    RemoveOrderAndProductsddlItems();
    $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Truck_dispatch/DestroyTransportationSession/",
                  contentType: "application/text; charset=utf-8",
                  success: function () {
                  },
                  error: function (xhr, ajaxOptions, thrownError) {
                  }
              });
    // 
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Truck_dispatch/GetArrangeNewForm/",
                   contentType: "application/text; charset=utf-8",
                   success: function (result) {
                       $('#arrangeNewTransportation').find('#tmpArrangeNewTransportation').show();
                       $('#arrangeNewTransportation').find('#tmpArrangeNewTransportation').html(result);

                   },
                   error: function (xhr, ajaxOptions, thrownError) {
                   }
               });

    if ($('#arrangeNewTransportation').is(':hidden')) {
        $('#arrangeNewTransportation').show();
        $('#arrangeNewTransportation .cargo_list').hide();
        $('#transportationResultsPanel').hide();
        $('.transportation_details').hide();
        $('#arrangeNewTransportation .submit_transportation_button').hide();

        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top + 450
        }, 400);
    }

});

//changes by designer
$('#arrangeNewTransportation').on('change', '#newTransportationLocation', function () {

    $(this).next('.vehicles_for_location_panel').show();
});
$('#arrangeNewTransportation').on('click', 'a[href="#seeVehiclesForLocation"]', function () {
    $(this).next('.vehicles_for_location').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 600
    }, 400);

});
$('#arrangeNewTransportation').on('click', 'a[href="#closeVehiclesForLocation"]', function () {
    $(this).closest('.vehicles_for_location').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 600
    }, 400);

});



$('#arrangeNewTransportation').on('click', 'a[href="#closeArrangeTransportationPanel"]', function () {
    $('#arrangeNewTransportation').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 200
    }, 400);


});
// this is on _TransportationDetails.cshtml page
//$('.transportation_details').on('click', 'a[href="#addCargoToCargoList"]', function () {
//    alert("here populate the Purchase orders");

//    $(this).closest('.cargo_list').find('.add_cargo_panel').show();


//   
//});
$('.transportation_details').on('click', 'a[href="#addNewCargoItem"]', function () {

    //alert("addNewCargoItem");
    // added by Rajni
    // save the temp cargo record in session
    // 
    var prod_code = $('.transportation_details').find("#tdProductddl option:selected").text();
    var order_id = $('.transportation_details').find('#tdOrderddl').val();
    var prod_id = $('.transportation_details').find('#tdProductddl').val();
    var enterdQty = $('#addCargoPanelEnterQuantity').val();
    var availableQty = $('#AvailableQty').text();

    if (order_id == "Select Purchase Order" && prod_id == "Select Product") {
        $('#qtyErrorMsg').show();
        $('#qtyErrorMsg').text("Order and Product code needs to be selected !");
    }
    else
        if (order_id == "Select Purchase Order") {
            $('#qtyErrorMsg').show();
            $('#qtyErrorMsg').text("Order needs to be selected !");
        }
        else
            if (prod_id == "Select Product") {
                $('#qtyErrorMsg').show();
                $('#qtyErrorMsg').text("Product code needs to be selected !");
            }
            else {
                if (enterdQty == "Enter Quantity (MT)") {
                    $('#qtyErrorMsg').show();
                    $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                } else
                    if (enterdQty == "0") {
                        $('#qtyErrorMsg').show();
                        $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                    }

                    else if (Math.round(enterdQty * 100) <= Math.round(availableQty * 100)) {
                        var datatosend = {
                            order_id: $('.transportation_details').find('#tdOrderddl').val(),
                            prod_id: $('.transportation_details').find('#tdProductddl').val(),
                            prod_code: $('.transportation_details').find("#tdProductddl option:selected").text(),
                            qty: $('.transportation_details').find('#transportationDetailsAddCargoPanelEnterQuantity').val()
                        };

                        $.ajax({
                            type: "POST",//url: "/Truck_dispatch/addTempCargoinSession/",
                            url: "/Truck_dispatch/addTempCargoinExistingDispatchAndCargoinSession/",
                            context: document.body,
                            data: datatosend,
                            dataType: "html",
                            context: document.body,
                        })
                                         .success(function (data) {
                                             $('.transportation_details .cargo_list').show();
                                             $('.transportation_details').find('#showCargoList').html(data);
                                         })
                                        .error(function (xhr, ajaxOptions, thrownError) {
                                        })
                        $('.transportation_details .submit_transportation_button').show();
                        $(this).closest('.add_cargo_panel').hide();
                        $('#qtyErrorMsg').hide();
                    }
                    else {
                        $('#qtyErrorMsg').show();
                        $('#qtyErrorMsg').text("Quantity for Loading cannot be more than Available in-Stock !");
                    }
            }
    var $cargoItems = $(this).closest('.cargo_list').find('ul');
    $cargoItems.find('li:last-child').clone().appendTo($cargoItems);
    $(this).closest('.add_cargo_panel').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + $(this).closest('.transportation_details').find('.transportation_details_full').height() + 375
    });


});
$('#transportationResultsPanel').on('click', 'a[href="# "]', function () {
    var panelID = $(this).attr('data-id');
    $('#transportationResultsPanel').hide();
    $('#transportationDetails-' + panelID).show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 400
    }, 400);

});
$('#transportationResultsPanel').on('click', 'a[href="#seeDetails"]', function () {
    var id = $(this).attr('data-id');
    $.ajax({
        url: "/quickView/seeTransportationDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                $('#customersPanel').hide();
                                $('#ordersPanel').hide();
                                $('#transportationResultsPanel').hide();
                                $('#transportationDetails').show();
                                $('#transportationDetails').html(result);
                                $('#transportationPanel').show();
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })

    $('#transportationDetails').show();
    $('#transportationResultsPanel').hide();
    $('.product_attention').hide();
    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 400
    }, 400);


});
$('#transportationPanel .transportation_details').on('click', 'a[href="#closeTransportationDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $('#selectTransportationPanel .or_wording').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);


});
$('#arrangeNewTransportation').on('click', 'a[href="#addCargoToTransportation"]', function () {

    $('#qtyErrorMsg').hide();
    $('#Orderddl').val('Select Purchase Order');
    $('#Productsddl').val('Select Product');
    $('#addCargoPanelEnterQuantity').val('');
    $('#tmpAvailableQty').html('');

    var agent_disp_on = $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartYear').val();
    var estimated_arr_date = $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalYear').val();
    var fromdateTime = ValidateDate(agent_disp_on);
    var todateTime = ValidateDate(estimated_arr_date);
    var fromdt = $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartYear').val();
    var todate = $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalYear').val();
    var dateCompare = CompareDate(fromdt, todate);
    var vechVal = $('#newTransportationVehicleCapacity').val();
    $('#addCargoErrMsg').hide();
    $('#addCargoErrMsg1').hide();
    if ($('#editTransportationLocation').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill !");
        if ($('#newTransportationVehicleNumber').val() == "") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill and enter vehicle Number !");
            if ($('#newTransportationVehicleCapacity').val() == "") {
                $('#addCargoErrMsg').show();
                $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill, enter vehicle Number and Vehicle Capacity in MT !");
            }
        }
    }
    else if ($('#newTransportationVehicleNumber').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill and enter vehicle Number !");
        if ($('#newTransportationVehicleCapacity').val() == "") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please enter vehicle Number and Vehicle Capacity in MT !");
        }
    }

    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(vechVal)) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Only numbers are allowd in Vechicle Capacity !");
    }
    else if ($('#newTransportationVehicleCapacity').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please enter Vehicle Capacity in MT !");
    }
    else if (fromdateTime == false && todateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg1').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
        $('#addCargoErrMsg1').html("Please select a Valid Expected Arrival date !");
    }
    else if ($('#newTransportationStartDay').val() == "DD" || $('#newTransportationStartMonth').val() == "MM" || $('#newTransportationStartYear').val() == "YYYY") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
        if ($('#newTransportationArrivalDay').val() == "DD" || $('#newTransportationArrivalMonth').val() == "MM" || $('#newTransportationArrivalYear').val() == "YYYY") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg1').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
            $('#addCargoErrMsg1').html("Please select a Valid Expected Arrival date !");
        }
    } else if ($('#newTransportationArrivalDay').val() == "DD" || $('#newTransportationArrivalMonth').val() == "MM" || $('#newTransportationArrivalYear').val() == "YYYY") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Expected Arrival date !");
    }
    else if (fromdateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
    }
    else if (todateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Expected Arrival date !");
    } else if (dateCompare == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Expected Arrival date should be greater than or equal to Start date !");
    }
    else {
        $('#Productsddl').val('Select Product');
        $('#addCargoPanelEnterQuantity').val('');
        $('#tmpAvailableQty').html('');
        $(this).closest('.arrange_new_transportation').find('.add_cargo_panel').show();
        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top + $('#arrangeNewTransportation').height() + 50
        }, 400);
    }
});
$('#arrangeNewTransportation').on('click', 'a[href="#addNewCargoItem"]', function () {
    // modified by manisha
    // save the temp cargo record in session

    var prod_code = $('#arrangeNewTransportation').find("#Productsddl option:selected").text();
    var order_id = $('#arrangeNewTransportation').find('#Orderddl').val();
    var prod_id = $('#arrangeNewTransportation').find('#Productsddl').val();
    var enterdQty = $('#addCargoPanelEnterQuantity').val();
    var availableQty = $('#AvailableQty').text();

    if (order_id == "Select Purchase Order" && prod_id == "Select Product") {
        $('#qtyErrorMsg').show();
        $('#qtyErrorMsg').text("Order and Product code needs to be selected !");
    }
    else
        if (order_id == "Select Purchase Order") {
            $('#qtyErrorMsg').show();
            $('#qtyErrorMsg').text("Order needs to be selected !");
        }
        else
            if (prod_id == "Select Product") {
                $('#qtyErrorMsg').show();
                $('#qtyErrorMsg').text("Product code needs to be selected !");
            }
            else {
                if (enterdQty == "Enter Quantity (MT)") {
                    $('#qtyErrorMsg').show();
                    $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                } else
                    if (enterdQty == "0") {
                        $('#qtyErrorMsg').show();
                        $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                    }
                    else
                        if (Math.round(enterdQty * 100) == Math.round(0 * 100)) {
                            $('#qtyErrorMsg').show();
                            $('#qtyErrorMsg').text("Enter Quantity (MT) greater than zero !");
                        }
                        else if (Math.round(enterdQty * 100) <= Math.round(availableQty * 100)) { //if (enterdQty < availableQty) {
                            var datatosend = {
                                order_id: $('#arrangeNewTransportation').find('#Orderddl').val(),
                                prod_id: $('#arrangeNewTransportation').find('#Productsddl').val(),
                                prod_code: $('#arrangeNewTransportation').find("#Productsddl option:selected").text(),
                                qty: $('#arrangeNewTransportation').find('#addCargoPanelEnterQuantity').val()
                            };

                            $.ajax({
                                type: "POST",
                                url: "/Truck_dispatch/addTempCargoinSession/",
                                context: document.body,
                                data: datatosend,
                                dataType: "html",
                                context: document.body,
                            })
                                             .success(function (data) {
                                                 $('#arrangeNewTransportation .cargo_list').show();
                                                 $('#arrangeNewTransportation').find('#showCargoList').html(data);
                                             })
                                            .error(function (xhr, ajaxOptions, thrownError) {
                                            })
                            $('#arrangeNewTransportation .submit_transportation_button').show();
                            $(this).closest('.add_cargo_panel').hide();
                            $('#qtyErrorMsg').hide();
                        } else {
                            $('#qtyErrorMsg').show();
                            $('#qtyErrorMsg').text("Quantity for Loading cannot be more than Available in-Stock !");
                        }
            }

});

$('.add_cargo_panel').on('click', 'a[href="#closeAddCargoPanel"]', function () {

    $(this).closest('.add_cargo_panel').hide();
});
$('#arrangeNewTransportation').on('click', 'a[href="#submitTransportation"]', function () {
    $('#tmpSubmitTransportation').show();

    var agent_disp_on = $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartYear').val();
    var estimated_arr_date = $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalYear').val();
    var fromdateTime = ValidateDate(agent_disp_on);
    var todateTime = ValidateDate(estimated_arr_date);
    var fromdt = $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartYear').val();
    var todate = $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalYear').val();
    var dateCompare = CompareDate(fromdt, todate);
    var vechVal = $('#newTransportationVehicleCapacity').val();
    $('#addCargoErrMsg').hide();
    $('#addCargoErrMsg1').hide();
    if ($('#editTransportationLocation').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill !");
        if ($('#newTransportationVehicleNumber').val() == "") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill and enter vehicle Number !");
            if ($('#newTransportationVehicleCapacity').val() == "") {
                $('#addCargoErrMsg').show();
                $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill, enter vehicle Number and Vehicle Capacity in MT !");
            }
        }
    }
    else if ($('#newTransportationVehicleNumber').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select Papermill and enter vehicle Number !");
        if ($('#newTransportationVehicleCapacity').val() == "") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please enter vehicle Number and Vehicle Capacity in MT !");
        }
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(vechVal)) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Only numbers are allowd in Vechicle Capacity !");
    }
    else if ($('#newTransportationVehicleCapacity').val() == "") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please enter Vehicle Capacity in MT !");
    }
    else if (fromdateTime == false && todateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg1').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
        $('#addCargoErrMsg1').html("Please select a Valid Expected Arrival date !");
    }
    else if ($('#newTransportationStartDay').val() == "DD" || $('#newTransportationStartMonth').val() == "MM" || $('#newTransportationStartYear').val() == "YYYY") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
        if ($('#newTransportationArrivalDay').val() == "DD" || $('#newTransportationArrivalMonth').val() == "MM" || $('#newTransportationArrivalYear').val() == "YYYY") {
            $('#addCargoErrMsg').show();
            $('#addCargoErrMsg1').show();
            $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
            $('#addCargoErrMsg1').html("Please select a Valid Expected Arrival date !");
        }
    } else if ($('#newTransportationArrivalDay').val() == "DD" || $('#newTransportationArrivalMonth').val() == "MM" || $('#newTransportationArrivalYear').val() == "YYYY") {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Expected Arrival date !");
    }
    else if (fromdateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Start date !");
    }
    else if (todateTime == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Please select a Valid Expected Arrival date !");
    } else if (dateCompare == false) {
        $('#addCargoErrMsg').show();
        $('#addCargoErrMsg').html("<p class='error-msg'>Expected Arrival date should be greater than or equal to Start date !");
    }
    else {
        var datatosend = {
            // loc_id: $('#frmNewTransportation').find('#newTransportationLocation').val(),
            loc_id: $('#editTransportationLocation').val(),
            vehicle_num: $('#newTransportationVehicleNumber').val(),
            vehicle_capacity: $('#newTransportationVehicleCapacity').val(),

            agent_disp_on: $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartYear').val(),
            agent_disp_on_time: $('#newTransportationStartTime').val(),
            agent_disp_on_time_ampm: $('#newTransportationStatTimeAmPm').val(),

            estimated_arr_date: $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalYear').val(),
            estimated_arr_date_time: $('#newTransportationArrivalTime').val(),
            estimated_arr_date_time_ampm: $('#newTransportationArrivalTimeAmPm').val(),
        }
        $.ajax(
           {
               cache: false,
               type: "POST",
               url: "/Truck_dispatch/ReviewEditTransportation/",
               data: datatosend,
               dataType: "html",
               success: function (data) {
                   $('#reviewTransport').html(data);

               },
               error: function (xhr, ajaxOptions, thrownError) {

               }
           });


        $('#arrangeNewTransportation').hide();
        $('#reviewTransportationDetails').show();

        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top + 500
        }, 400);
    }
});
$('#reviewTransportationDetails').on('click', 'a[href="#closeReviewTransportationDetails"]', function () {
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);


});


$("#tmpSubmitTransportation").click(function () {

    $.ajaxSetup({ cache: false });
    $.ajax(
        {
            url: "/Truck_dispatch/confirmTransportation/",
            contentType: "application/html; charset=utf-8",
            type: "GET",
            dataType: 'html',
            context: document.body,
        })
                  .success(function (result) {
                      $('#tmpSubmitTransportation').hide();
                      //alert("saved in db");
                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })


    ///load the recent view again to see the recent truck that has been created

    $('#reviewTransportationDetails').hide();
    $('#reviewTransportationConfirmation').show();
    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 500
    }, 400);
    // return true;

    ///load the recent view again to see the recent truck that has been created
    $.ajax({
        url: "/QuickView/QuickViewDetails/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                       .success(function (result) {
                           $('#quickViewRecentOrders').html(result);
                       })
                      .error(function (xhr, ajaxOptions, thrownError) {
                      })

});


function DestroyTransportationSession() {

    $.ajax(
             {
                 cache: false,
                 type: "GET",
                 url: "/Truck_dispatch/DestroyTransportationSession/",
                 contentType: "application/text; charset=utf-8",
                 success: function () {

                 },
                 error: function (xhr, ajaxOptions, thrownError) {

                 }
             });
}

$('#reviewTransportationDetails').on('click', 'a[href="#cancelReviewDetails"]', function () {
    DestroyTransportationSession();
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').hide();
    $('#selectTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);


});
$('#reviewTransportationDetails').on('click', 'a[href="#editReviewDetails"]', function () {
    //alert("edit transportation is clicked");
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 500
    }, 400);


});
//$('#reviewTransportationDetails').on('click', 'a[href="#submitReviewDetails"]', function () {


//    //    $('#reviewTransportationDetails').hide();
//    //    $('#reviewTransportationConfirmation').show();
//    //    $('html,body').animate({
//    //        scrollTop: $('#transportationPanel').position().top + 500
//    //    }, 400);


//});
$('#reviewTransportationConfirmation').on('click', 'a[href="#closeConfirmation"]', function () {
    $('#reviewTransportationConfirmation').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);


});
$('#transportationFromDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#customerOrderHistoryFromDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#alphabetOrderHistoryFromDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#newTransportationStartDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#ordersNewTransportationStartDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#newTransportationArrivalDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#ordersNewTransportationArrivalDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#transportationFromDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#customerOrderHistoryFromDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#alphabetOrderHistoryFromDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#newTransportationStartMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#ordersNewTransportationStartMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#newTransportationArrivalMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#ordersNewTransportationArrivalMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#transportationFromDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#customerOrderHistoryFromDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#alphabetOrderHistoryFromDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#newTransportationStartYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#ordersNewTransportationStartYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#newTransportationArrivalYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#transportationToDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#customerOrderHistoryToDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#alphabetOrderHistoryToDateDay')
.on('focus', function () {
    if ($(this).val() === 'DD') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('DD');
    }
});
$('#transportationToDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#customerOrderHistoryToDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#alphabetOrderHistoryToDateMonth')
.on('focus', function () {
    if ($(this).val() === 'MM') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('MM');
    }
});
$('#transportationToDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#customerOrderHistoryToDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#alphabetOrderHistoryToDateYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#ordersNewTransportationArrivalYear')
.on('focus', function () {
    if ($(this).val() === 'YYYY') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('YYYY');
    }
});
$('#transportationVehicleNumber')
.on('focus', function () {
    if ($(this).val() === 'Type Vehicle Number') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Vehicle Number');
    }
});
$('#newTransportationVehicleNumber')
.on('focus', function () {
    if ($(this).val() === 'Type Vehicle Number') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Vehicle Number');
    }
});
$('#newTransportationVehicleCapacity')
.on('focus', function () {
    if ($(this).val() === 'Type Vehicle Capacity (MT)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Vehicle Capacity (MT)');
    }
});
$('#addCargoPanelEnterQuantity')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});
$('#transportationDetailsAddCargoPanelEnterQuantity')
.on('focus', function () {
    if ($(this).val() === '') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * QUICK VIEW
 * -------------------------------------------------------------------------------------------------------- */

$('#quickView').on('click', 'a[href="#seeAllRecentOrders"]', function () {
    //$('a.create_order').show();
    $('#ordersPanel').hide();
    $('.close_orders_panel').show();
    $('.select_order').show();
    $('#createOrderPanel').hide();


    clearOrderPanel();
    $.ajax({
        url: "/QuickView/seeAllRecentOrders/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                $('#customersPanel').hide();
                                $('#transportationPanel').hide();
                                $('.order_details').hide();
                                $('#ordersPanel').show();
                                $('#ordersResultsPanel').show();
                                $('#ordersResultsPanel').html(result);
                                //$('#ordersPanel').show();
                                $('html,body').animate({
                                    scrollTop: $('#ordersPanel').position().top + 260
                                }, 400);
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })



});

$('#quickView').on('click', 'a[href="#seeTransportationDetails"]', function () {

    var id = $(this).attr('data-id');
    $.ajax({
        url: "/quickView/seeTransportationDetails/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                $('#customersPanel').hide();
                                $('#ordersPanel').hide();
                                $('#transportationResultsPanel').hide();
                                $('#transportationDetails').show();
                                $('#transportationDetails').html(result);
                                $('#transportationPanel').show();
                                $('html,body').animate({
                                    scrollTop: $('#transportationPanel').position().top + 400
                                }, 400);

                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })

});
$('#quickView').on('click', 'a[href="#seeOrderDetails"]', function () {
    $('.arrange_new_transportation ').hide();
    $('#orderDetails').show();

    var id = $(this).attr('data-order-id');
    $.ajax({
        url: "/quickView/GetOrderDetailsMaster/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                $('#customersPanel').hide();
                                $('#transportationPanel').hide();
                                $('.close_orders_panel').hide();
                                $('#selectOrder').hide();
                                $('#ordersResultsPanel').hide();
                                $('#orderDetails').show();
                                $('#tmpOrderDetails').show();
                                $('#tmpOrderDetails').html(result);
                                $('#ordersPanel').show();
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })

    $.ajax({
        url: "/quickView/GetOrderDetailsChild/" + id,
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                           .success(function (result) {
                               $('#customersPanel').hide();
                               $('#transportationPanel').hide();
                               $('.close_orders_panel').hide();
                               $('#selectOrder').hide();
                               $('#ordersResultsPanel').hide();
                               $('#orderDetails').show();
                               $('#tmpOrderProdsList').html(result);
                               $('#ordersPanel').show();
                               $('html,body').animate({
                                   scrollTop: $('#ordersPanel').position().top
                               }, 400);
                           })
                          .error(function (xhr, ajaxOptions, thrownError) {
                          })

});


$('#quickView').on('click', 'a[href="#seeAllRecentTransportation"]', function () {

    $.ajax({
        url: "/QuickView/seeAllRecentTransportation/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                $('#customersPanel').hide();
                                $('#ordersPanel').hide();
                                $('.order_details').hide();
                                $('#transportationResultsPanel').show();
                                $('#transportationResultsPanel').html(result);
                                $('#transportationPanel').show();
                                $('#transportationDetails').hide();
                                $('html,body').animate({
                                    scrollTop: $('#ordersPanel').position().top
                                }, 400);
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })
});

/* -----------------------------------------------------------------------------------------------------------
 * LOGIN
 * -------------------------------------------------------------------------------------------------------- */

$('#login input.username')
.on('focus', function () {
    if ($(this).val() === 'Username') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Username');
    }
});
$('#login input.pass')
.on('focus', function () {
    if ($(this).val() === 'Password') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Password');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * DOM ONLOAD
 * -------------------------------------------------------------------------------------------------------- */

$(function () {
    buildAutoComplete();
    buildCustomersAutoComplete();
});


$(function () {
    $(document).on('click', 'a', function (evt) {
        var href = $(this).attr('href');

        if (href.indexOf(document.domain) > -1 || href.indexOf(':') === -1) {
            history.pushState({
            }, '', '');
        }

        return false;
    });
});

//Edit Order Form Validations
var k;
function ValidateEditOrder(k, seqNum) {

    var editselectBf = k.editselectBf.value;
    var selectGsm = k.editselectGsm.value;
    var selectShade = k.editselectShade.value;
    var selectWidth = k.editinputWidthCm.value;
    var selectCore = k.editselectCore.value;
    var inputDiaCm = k.editinputDiaCm.value;
    var inputQuantityMt = k.editinputQuantityMt.value;
    var fromdt = $(k).find('#requestedDeliveryDateDay').val() + "/" + $(k).find('#requestedDeliveryDateMonth').val() + "/" + $(k).find('#requestedDeliveryDateYear').val();
    var chkdateGreater = $(k).find('#requestedDeliveryDateYear').val() + "/" + $(k).find('#requestedDeliveryDateMonth').val() + "/" + $(k).find('#requestedDeliveryDateDay').val();
    var dtGreaterresult = CheckRequestDeliveryDate(chkdateGreater);
    var dateresult = CheckDate(fromdt);
    var widthchk = CheckWidth(selectWidth);
    var DiamwterChk = CheckDiamter(inputDiaCm);
    var qtyChk = CheckQty(inputQuantityMt);
    var CustName = $("#selectedCustomerName").html();
    //var priceText = $('#editPrice_' + seqNum).html();
    var priceText = k.editinputPrice.value;
    var checkPrice = CheckPrice(priceText);
    var DropdownStatus = ChkDropdown(editselectBf, selectGsm, selectShade, selectCore);

    if (DropdownStatus != true) {
        return DropdownStatus;
    }
    else if (dateresult == false) {
        return "Please select a Valid date !"
    }
    else if (dtGreaterresult == false) {
        return "Request Delivery Date should not less than today's date !"

    }
    else if (widthchk != true) {
        return widthchk;
    }

    else if (DiamwterChk != true) {
        return DiamwterChk;
    }

    else if (qtyChk != true) {
        return qtyChk;
    }
    else if (checkPrice != true) {
        return checkPrice;
    }
    else {
        return true;

    }
}

//function For Dropdown
var editselectBf; var selectGsm; var selectShade; var selectCore; var dt; var width; var diameter; var qtyFunc;
function ChkDropdown(editselectBf, selectGsm, selectShade, selectCore) {


    if (editselectBf == "Select Bf" || editselectBf == "") {
        return "Please Select Bf !"
    }
    else if (selectGsm == "Select Gsm" || selectGsm == "") {
        return "Please Select Gsm !"
    }
    else if (selectShade == "Select Shade" || selectShade == "") {
        return "Please Select Shade !"
    }
    else if (selectCore == "Select Core" || selectCore == "") {
        return "Please Select Core !"
    }
    else {

        return true;

    }

}
//function for date
function CheckDate(dt) {
    var chk = ValidateDate(dt);
    if (dt.indexOf("DD") != -1 || dt.indexOf("null") != -1) {
        return false;
    }
    else if (dt.indexOf("MM") != -1 || dt.indexOf("null") != -1) {
        return false;
    }
    else if (dt.indexOf("YYYY") != -1 || dt.indexOf("null") != -1) {
        return false;
    }
    else if (chk == true) {
        return true;
    }
    return false;
}
//Function for width
function CheckWidth(width) {
    var len = width.length;
    if (width == "Enter Width (40-250 CM)" || width == "") {
        return "Width cannot be empty !";

    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(width)) {

        return "Width should be numeric, in Cms!";
    }
    else if (!/^(\d+)?(?:\.\d{1,2})?$/.test(width)) {
        return "Only Two decimal places are allow in Width!";

    }

    else if (width > 250 || width < 40) {
        return "Width should be between 40 and 250 CM!";
    }
    return true;
}

//Function For Dimeter
function CheckDiamter(diameter) {

    var len = diameter.length;
    if (diameter == "Enter DIA (5-120 CM)" || diameter == "") {
        return "Diameter cannot be empty !";
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(diameter)) {
        return "Diameter should be numeric, in Cms!";
    }
    else if (!/^(\d+)?(?:\.\d{1,2})?$/.test(diameter)) {
        return "Only Two decimal places are allow in Diameter!";
        ///^\s*\d*\.?\d{>2}\s*$/

    }
    else if (diameter > 120 || diameter < 5) {
        return "Diameter should be between 5 and 120 CM!";
    }
    return true;
}
//Function For Qty
function CheckQty(qtyFunc) {
    // 
    var len = qtyFunc.length;
    var getWid = parseFloat(qtyFunc);
    if (qtyFunc == "Enter Quantity (0.500-100 MT)" || qtyFunc == "") {
        return "Qty cannot be empty!";
    }
    else if (qtyFunc.charAt(0) == '.') {
        return "Qty should be numeric, in MT!";
    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(qtyFunc)) {
        return "Qty should be numeric, in MT!";
    }
    else if (!/^(\d+)?(?:\.\d{1,4})?$/.test(qtyFunc)) {
        return "Only four decimal places are allow in Qty!";
    }
        //else if (qtyFunc.charAt(0) == '0' && qtyFunc.length == 4 || qtyFunc.charAt(0) == '0' && qtyFunc.length == 3) {
        //    return "Qty should be between 0.500 and 100 MT!";
        //}
    else if (getWid < 0.500 || getWid > 100) {
        return "Qty should be between 0.500 and 100 MT!";
    }

    return true;

}
//Function For check
function CheckPrice(Price) {

    var len = Price.length;
    if (Price == "Enter Price" || Price == "") {
        return "Price cannot be empty !";

    }
    else if (!/^(?=.)(?:\d+,)*\d*(?:\.\d+)?$/.test(Price)) {

        return "Price should be numeric !";
    }
    else if (!/^(\d+)?(?:\.\d{1,2})?$/.test(Price)) {
        return "Only Two decimal places are allow in Price!";

    }
    else if ((Price == 0) && (Price == parseInt(Price, 10))) {
        // return "Price cannot be empty !";
        return "Enter valid price !";
        ///^\s*\d*\.?\d{>2}\s*$/
    }
    else if (Price < 1) {
        return "Enter valid price !";
    }
    return true;
}



//function CheckPrice(priceText, CustName) {
//    if (priceText.indexOf(CustName) > 0) {
//        return "This order has products with zero unit price, so the order can not be added to the db!";
//    }
//    return true;
//}
//Function Compare Date
function CompareDate(fromdtChk, todtChk) {

    // if (Date.parse(fromdate) > Date.parse(todate)) {
    if (Date.parse(fromdtChk) > Date.parse(todtChk)) {
        return false;
    }
    else {
        return true;
    }
}

function DropdownTriggerAddOrder() {
    $('#requestedDeliveryDateDay').val('DD').trigger('chosen:updated');
    $('#requestedDeliveryDateMonth').val('MM').trigger('chosen:updated');
    $('#requestedDeliveryDateYear').val('YYYY').trigger('chosen:updated');
    //For Bf
    $('#selectBf').val('').trigger('chosen:updated');
    $('#selectBf').find('option:first').prop('selected', 'selected');
    //for Gsm
    $('#selectGsm').val('').trigger('chosen:updated');
    $('#selectGsm').find('option:first').prop('selected', 'selected');
    //for shade
    $('#selectShade').val('').trigger('chosen:updated');
    $('#selectShade').find('option:first').prop('selected', 'selected');
}

function DropdownTriggerOfTransport() {
    //for Transport Search
    $('#selectVehicleTypeDropdown').val('').trigger('chosen:updated');
    $('#selectVehicleTypeDropdown').find('option:first').prop('selected', 'selected');
    $('#transportationFromDateDay').val('DD').trigger('chosen:updated');
    $('#transportationFromDateMonth').val('MM').trigger('chosen:updated');
    $('#transportationFromDateYear').val('YYYY').trigger('chosen:updated');
    $('#transportationToDateDay').val('DD').trigger('chosen:updated');
    $('#transportationToDateMonth').val('MM').trigger('chosen:updated');
    $('#transportationToDateYear').val('YYYY').trigger('chosen:updated');
}

function DropdownTriggerForArrangeTransport() {
    //For Arrange Transport editTransportationLocation
    $('#editTransportationLocation').val('').trigger('chosen:updated');
    $('#editTransportationLocation').find('option:first').prop('selected', 'selected');

    $('#newTransportationStartDay').val('DD').trigger('chosen:updated');
    $('#newTransportationStartMonth').val('MM').trigger('chosen:updated');
    $('#newTransportationStartYear').val('YYYY').trigger('chosen:updated');

    $('#newTransportationStartTime').val('').trigger('chosen:updated');
    $('#newTransportationStartTime').find('option:first').prop('selected', 'selected');
    $('#newTransportationStatTimeAmPm').val('').trigger('chosen:updated');
    $('#newTransportationStatTimeAmPm').find('option:first').prop('selected', 'selected');

    $('#newTransportationArrivalDay').val('DD').trigger('chosen:updated');
    $('#newTransportationArrivalMonth').val('MM').trigger('chosen:updated');
    $('#newTransportationArrivalYear').val('YYYY').trigger('chosen:updated');

    $('#newTransportationArrivalTime').val('').trigger('chosen:updated');
    $('#newTransportationArrivalTime').find('option:first').prop('selected', 'selected');

    $('#newTransportationArrivalTimeAmPm').val('').trigger('chosen:updated');
    $('#newTransportationArrivalTimeAmPm').find('option:first').prop('selected', 'selected');


}

function dropdownTriggerForAddCargo() {
    //For Add Cargo items    
    $('#Orderddl').val('').trigger('chosen:updated');
    $('#Orderddl').find('option:first').prop('selected', 'selected');//Productsddl

    $('#Productsddl').val('').trigger('chosen:updated');
    $('#Productsddl').find('option:first').prop('selected', 'selected');

}


function CheckRequestDeliveryDate(fromdtChk) {
    return true;
}


function validateSchedule(shadeCode, req_del_date) {
    var flag = "";
    var data = {
        shadeCode: shadeCode,
        requested_delivery_date: req_del_date
    };
    $.ajax({

        //cache: false,
        type: "GET",
        data: data,
        url: "/Order/CheckShadeScheduleTime/",
        contentType: "application/html; charset=utf-8",
        //})
        success: function (result) {
            flag = result;

        },
        async: false
    });

    return flag;
    //$.ajaxSetup({ cache: false });
}

//Added By sagar download order details in pdf for single order click
$('#tmpOrderDetails').on('click', 'a[href="#downloadOrder"]', function () {
    var orderid = $('#tmpOrderDetails').find('a[href="#duplicateOrder"]').attr("data-id");
    window.location = "Agent/GetPdffile?orderid=" + orderid //?orderid=" + orderid


});
$('#addProductPanel').on('click', 'a[href="#closeAddProductPanelErr"]', function () {

    $('.delivery_date_conflict ').hide();
    $('html,body').animate({
        scrollTop: $('#addProductPanel').position().top
    }, 400);
});
$('#addProductPanel').on('click', 'a[href="#addProductToOrderErr"]', function () {
    $('#addProductPanel').hide();

    var editselectBf = $("#selectBf option:selected").text();
    var selectGsm = $("#selectGsm option:selected").text();
    var selectShade = $("#selectShade option:selected").text();
    var selectWidth = $("#selectWidth").val();
    var selectCore = $("#selectCore option:selected").text();
    var inputDiaCm = $("#inputDiaCm").val();
    var inputQuantityMt = $("#inputQuantityMt").val();
    var fromdt = $("#addProductPanel").find('#requestedDeliveryDateDay').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateMonth').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateYear').val();
    var fromdtChkDate = $("#addProductPanel").find('#requestedDeliveryDateYear').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateMonth').val() + "/" + $("#addProductPanel").find('#requestedDeliveryDateDay').val();


    //Calling Function
    // var ChkGreaterDate = CheckRequestDeliveryDate(fromdtChkDate);
    var req_del_date = TestAddProduct.requestedDeliveryDateDay.value + "/" + TestAddProduct.requestedDeliveryDateMonth.value + "/" + TestAddProduct.requestedDeliveryDateYear.value;



    var dateresult = CheckDate(fromdt);
    var widthchk = CheckWidth(selectWidth);
    var DiamwterChk = CheckDiamter(inputDiaCm);
    var qtyChk = CheckQty(inputQuantityMt);

    var CustName = $("#selectedCustomerName").html();
    var priceText = $('#productPrice').html();

    var checkPrice = CheckPrice($('#inputprice').val());

    var DropdownStatus = ChkDropdown(editselectBf, selectGsm, selectShade, selectCore);
    if ($('#inputCustomerpo').val() == "" || $('#inputCustomerpo').val().trim() == "") {

        $('#reqCustPO').html('');
        $('#reqCustPO').html("<p class='error-msg'>Please specify the customer PO !</p>");
        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top
        }, 400);
        return false;
    }

    //var Scheduleflag = validateShadeSchedule(selectShade, req_del_date);

    if (dateresult == false) {
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>Please select a Valid date !</p>");
    }
        //else if (ChkGreaterDate != true) {
        //    $('#viewSchedule').show();
        //}
    else if (DropdownStatus != true) {
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + DropdownStatus + "</p>");
    }

    else if (widthchk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#selectWidth").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + widthchk + "</p>");
    }

    else if (DiamwterChk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#inputDiaCm").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + DiamwterChk + "</p>");
    }

    else if (qtyChk != true) {
        $('#AddOrdersErrorMsg').html('');
        $("#inputQuantityMt").focus();
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + qtyChk + "</p>");
    }
    else if (checkPrice != true) {
        $('#AddOrdersErrorMsg').html('');
        $('#AddOrdersErrorMsg').html("<p class='error-msg'>" + checkPrices + "</p>");
    }
    else {

        $('#AddOrdersErrorMsg').html("");

        var req_del_date = TestAddProduct.requestedDeliveryDateDay.value + "/" + TestAddProduct.requestedDeliveryDateMonth.value + "/" + TestAddProduct.requestedDeliveryDateYear.value;
        //($('#requestedDeliveryDateDay').val()) + "/" + ($('#requestedDeliveryDateMonth').val()) + "/" + ($('#requestedDeliveryDateYear').val())


        var Result = {
            hCustomer_id: $('#selectedCustomerId').val(),
            selectBf: $('#selectBf').val(),
            selectGsm: $('#selectGsm').val(),
            selectShade: $('#selectShade').val(),
            selectWidth: $('#selectWidth').val(),
            selectCore: $('#selectCore').val(),
            inputDiaCm: $('#inputDiaCm').val(),
            inputQuantityMt: $('#inputQuantityMt').val(),
            requested_delivery_date: req_del_date,
            unit_price: $('#inputprice').val(),
        };
        var ProdSeqNoToCopy = 0;
        $.ajax({
            //async: true,
            type: "POST",
            url: "/Order/addProductToOrderinSession/" + ProdSeqNoToCopy,
            data: Result,
            dataType: "html",
            context: document.body,
            //contentType: 'application/html; charset=utf-8'
        })
                        .success(function (data) {
                            $('#lstProds').html(data);
                            $('#createOrderCurrentProducts .fix-overflow').show();
                        })
                       .error(function (xhr, ajaxOptions, thrownError) {
                       })
        $('#addProductPanel').hide();
        $('#createOrderDeliveryDate .add_product').show();
        $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();
        $('#createOrderCurrentProducts').show();

        $('html,body').animate({
            scrollTop: $('#createOrderCurrentProducts').position().top + $('#createOrderPanel').position().top
        }, 400);
    }
});
$('#createOrderCurrentProducts').on('click', 'a[href="#cancelPlaceOrder"]', function () {
    clearCustomerddl();
    destroyOrderSession();
    clearReqDelDateddl();
    $('#inputCustomerpo').val('');
    $('#createOrderAddProducts').hide();
    $('#createOrderCurrentProducts').hide();
    $('#createOrderSelectCustomer').show();
    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);
});
$('#quickViewRecentOrders').on('click', 'a[href="#downloadOrder"]', function () {
    var orderid = $(this).attr("data-id");
    window.location = "Agent/GetPdffile?orderid=" + orderid


});
$('#ordersResultsPanel').on('click', 'a[href="#downloadOrder"]', function () {
    var orderid = $(this).attr("data-id");
    window.location = "Agent/GetPdffile?orderid=" + orderid


});

//Bind Current And Next Year In Year Dropdown
$(document).ready(function () {

    $.ajax({
        cache: false,
        type: "POST",
        url: "/Agent/GetCurrentyear/",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var dafulttext = "YYYY";

            //add new order Date
            $("#requestedDeliveryDateYear").empty();
            $("#requestedDeliveryDateYear").append($('<option>' + dafulttext + '</option>'));
            $("#requestedDeliveryDateYear").append($('<option value=' + data[0] + '>' + data[0] + '</option>'));
            $("#requestedDeliveryDateYear").append($('<option value=' + data[1] + '>' + data[1] + '</option>'));

        }

    });

});

//Change Password Close
$('#tempbodySection').on('click', 'a[href="#close-panel"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboard();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboard();
});

 
$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    //$("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboard();
});
$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
    event.stopImmediatePropagation();
    RefreshDashboard();
});

function RefreshDashboard() {
    debugger;
    $('#tempParentDiv').show();
    $('#yourMessages').hide();
    $('#ordersPanel').hide();
    $('#customersPanel').hide();
    $('#yourAlerts').hide();
    $('#createOrderPanel').hide();
    $('#transportationPanel').hide();
    $('#viewSchedulePanel').hide();
    $('#usersChangePassword').hide();
   
}

function ShowagntDashboard() {
    debugger;
    $("#viewSchedulePaperMills").find('option:first').prop('selected', 'selected');
    $('#schedulePaperMill').hide();
    $('#welcomePanel').css('height', '58px');
    $('#viewSchedulePanel').show();
    $('#tempParentDiv').show();
    $('#yourMessages').hide();
    $('#yourAlerts').hide();
    $("#usersChangePassword").hide();

}


$("#selectWidth").keyup(function () {


    var textvalue = $('#selectWidth').val();
    var widthInCm = (2.54) * textvalue;
    if (widthInCm != 0) {
        $('#inputwidthInInch').val(widthInCm);
    } else {
        $('#inputwidthInInch').val('');
    }
});
$("#inputwidthInInch").keyup(function () {

    var textvalue = $('#inputwidthInInch').val();
    var widthInCm = textvalue / (2.54);
    if (widthInCm != 0) {
        $('#selectWidth').val(widthInCm.toFixed(4));
    } else {
        $('#selectWidth').val('');
    }

});



