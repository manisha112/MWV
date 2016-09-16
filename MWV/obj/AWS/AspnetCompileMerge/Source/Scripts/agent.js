/* -----------------------------------------------------------------------------------------------------------
 * RAJNI /
 * -------------------------------------------------------------------------------------------------------- */
// on change of bf - gsms are listed in selectGsm
$('#selectBf').on('change', function () {


    //        ////------------code to get the Gsms------------//
    var selectedItem = $(this).val();
    var ddlGsms = $("#selectGsm");
    //alert("Selected bf is " + selectedItem);
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
                       //alert("ajaxOptions " + ajaxOptions);
                       // alert('Failed to retrieve states.');
                   }
               });
    //        ////------------code to get the Gsms------------//



});
// on change of gsm - based on the bf and gsm combination the product code is shown on the screen
$('#selectGsm').on('change', function () {

    var selectedBf = $("#selectBf").val();
    var selectedGsm = $("#selectGsm").val();
   // var hidVal = $("#hCustomer_id").val();
     //alert("Selected bf and gsm is " + selectedBf + selectedGsm);
    var hidVal = $("#selectedCustomerId").val();
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "Order_product/GetProductCode/" + selectedBf + "/" + selectedGsm,
                   //contentType: "application/json; charset=utf-8",
                   success: function (data) {
                        //alert("success of product code" + data);
                       $("#productCode").text(data);
                   },
                  
               });
});

$('#selectShade').on('change', function () {
   // var CustId = $("#hCustomer_id").val();
    //genericFunc(this.value, null, null, CustId);
    //        ////------------code to get the Gsms------------//
    var shade_code = $(this).val();
    //var ddlGsms = $("#selectGsm");
    //alert("Selected bf is " + selectedItem);
    var CustId = $("#selectedCustomerId").val();///////////////////////
    var prodcode = $("#productCode").text();
    //alert("codes before calling the GetPrice " + CustId + "/" + prodcode + "/" + shade_code);
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Order_product/GetPrice/" + CustId + "/" + prodcode + "/" + shade_code,
                   contentType: "application/text; charset=utf-8",
                   success: function (data) {
                       //alert("Price is " + data);
                       if (data == 0)
                       {
                           $("#productPrice").text("No active price available for the selected product code and shade");
                       }
                       else
                       {
                            $("#productPrice").text(data);
                       }
                   },
                   error: function (xhr, ajaxOptions, thrownError) {
                       // alert("ajaxOptions " + ajaxOptions);
                       // alert('Failed to retrieve states.');
                   }
               });
    ///////////////////////



});
// on change of LOCATION of papermill -orders are listed in selectOrder
$('#newTransportationLocation').on('change', function () {


    //        ////------------code to get the Orders------------//
    var selectedItem = $(this).val();
    var ddlOrders = $("#Orderddl");
   // alert("Selected location is " + selectedItem);
    $.ajax(
               {
                   cache: false,
                   type: "GET",
                   url: "/Home/GetAllOrdersbyAgentandLocation/" + selectedItem,
                   contentType: "application/json; charset=utf-8",
                   success: function (data) {
                       ddlOrders.html('');
                       ddlOrders.append($('<option>Select Purchase Order</option>'));
                       $.each(data, function (id, option) {

                           ddlOrders.append($('<option></option>').val(option.id).html(option.name));
                           
                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {
                       //alert("ajaxOptions " + ajaxOptions);
                       // alert('Failed to retrieve states.');
                   }
               });
    
    //        ////------------code to get the Orders------------//
});
// on change of order, order products are loaded
$('#Orderddl').on('change', function () {


    //        ////------------code to get the Products------------//
    var selectedItem = $(this).val();
    var ddlProducts = $("#Productsddl");
    //alert("Selected Order is " + selectedItem);
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

                           ddlProducts.append($('<option></option>').val(option.id).html(option.name));

                       });
                   },
                   error: function (xhr, ajaxOptions, thrownError) {
                       //alert("ajaxOptions " + ajaxOptions);
                       // alert('Failed to retrieve states.');
                   }
               });
    
    //        ////------------code to get the Products------------//
});

/* -----------------------------------------------------------------------------------------------------------
 * DASHBOARD / GENERAL
 * -------------------------------------------------------------------------------------------------------- */

$('#dashboard').on('click', 'a[href="#showCreateOrderPanel"]', function () {
    $('#customersPanel').hide();
    $('#transportationPanel').hide();
    $('#ordersPanel').hide();
    $('#createOrderPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top - 50
    }, 400);

    return false;
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

});
$('#createOrder').on('click', 'a[href="#closeAddProductPanel"]', function () {
    $('#addProduct').hide();

    return false;
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

    // $('#txtName').value = "";
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

    return false;
});
$('#createCustomerPanel').on('click', 'a[href="#createNewCustomer"]', function () {
    ReviewCustomerDetails.innerHTML = "<ul>" +
   "<li>" + txtName.value + "</li>" +
   "<li>" + txtAddress1.value + "</li>" +
   "<li>" + txtAddress2.value + "</li>" +
   "<li>" + txtAddress3.value + "</li>" +
  " <li>" + txtCity.value + "</li>" +
  " <li>" + txtPincode.value + " </li>" +
   "<li>" + txtState.value + "</li>" +
  " <li>" + txtCountry.value + "</li>" +
  " <li>" + txtPhone.value + "</li>" +
   "<li>" + txtFax.value + "</li>" +
   "<li>" + txtEmail.value + "</li>" +

     "</ul>";
    $('#createCustomerPanel').hide();
    $('#reviewCustomerPanel').show();
    // $('#ReviewCustomerDetails').hide();
    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#reviewCustomerPanel').on('click', 'a[href="#cancelCreateCustomer"]', function () {
    $('a.create_order').show();
    $('#createCustomerPanel').hide();
    $('#reviewCustomerPanel').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#reviewCustomerPanel').on('click', 'a[href="#editCreateCustomer"]', function () {
    $('#reviewCustomerPanel').hide();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#reviewCustomerPanel').on('click', 'a[href="#createCustomerConfirmation"]', function () {

    //Added by Mnanisha
    //For create new customer

    // $('#reviewCustomerPanel').slideUp();
    //alert("reviewCustomerPanel");
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
                        //alert("Successfully Created");
                        $('#reviewCustomerPanel').hide();
                        $('#confirmCustomerPanel').slideDown();
                        $('#confirmCustomerPanel').html(data);
                        //  $('#reviewCustomerPanel').hide();
                        //$('#confirmCustomerPanel').show();
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })

    return false;

    //$('#reviewCustomerPanel').hide();
    //$('#confirmCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#confirmCustomerPanel').on('click', 'a[href="#closeCreateCustomerConfirmation"]', function () {
    $('a.create_order').show();
    $('#createCustomerPanel').hide();
    $('#confirmCustomerPanel').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#confirmCustomerPanel').on('click', 'a[href="#createAnotherCustomer"]', function () {
    $('#confirmCustomerPanel').hide();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
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
    // build an autocomplete widget for customer selection from a pre-defined list
    $('#ordersAutoSearchCustomers').autocomplete({
        // the box where the suggestion list will be displayed
        appendTo: '#ordersAutoSearchCustomersResults',
        // data source; currently local, can/should be AJAX
        source: ordersCustomerList,
        // when the widget is created, there are some custom actions needed to
        // force necessary behavior, such as data attributes and leaving the list open
        create: function () {
            ordersReadyToClose = false;

            // override default autocomplete single item rendering functionality
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                // append a data-value attribute to the created <li> so it can be selected
                // later for appending customer data and allowing selection after the fact.
                return $('<li>')
                   .attr('data-value', item.value)
                   .append('<p class="customer_name">' + item.label + '</p>')
                   .appendTo(ul);
            };
            // do not allow the list of items to close until the ready flag has been set
            $(this).data('ui-autocomplete').close = function () {
                if (!ordersReadyToClose) {
                    this.cancelSearch = false;
                    return false;
                }

                this._close();
            };
        },
        // when a list item is selected from the autocomplete suggestion list
        select: function (evt, ui) {
            // append full customer data to the selected list item and allow further
            // deep selection of individual customer
            $(evt.currentTarget)
                .find('li[data-value="' + ui.item.value + '"]')
                .append('<div class="list-item-wrapper"><div class="customer-list-item"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="images/close-panel.png"></a> <p class="title"><strong>' + ui.item.label + '</strong></p><p class="details">' + ui.item.address + ' / ' + ui.item.city + ' / ' + ui.item.state + ' / ' + ui.item.pin + '</p><p class="details">' + ui.item.country + ' / ' + ui.item.phone + ' / ' + ui.item.fax + '</p> <a href="#selectCustomerFromList" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Select</a></div></div>');

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
} // buildAutoComplete()

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
// close an individually selected customer so autocomplete can re-select
$('#ordersAutoSearchCustomersResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    ordersReadyToClose = false;

    $('#ordersAutoSearchCustomersResults .list-item-wrapper').remove();
    $('#ordersAutoSearchCustomers').autocomplete('enable');
    // fake click because of some abstract bug not allowing to click on result
    $('#createOrderPanel').click();

    return false;
});
$('#createOrderSelectCustomer').on('click', 'a[href="#clearSearch"]', function () {
    ordersReadyToClose = true;

    $('#ordersAutoSearchCustomers').autocomplete('close');
    $('#ordersAutoSearchCustomers').autocomplete('destroy');
    $('#createOrderSelectCustomer .auto_search_results').hide();
    $('#ordersAutoSearchCustomers').val('Search').blur();
    $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();

    buildAutoComplete();

    return false;
});
function selectCustomer() {
    ordersReadyToClose = true;
   // alert("inside selectCustomer()");
    // close the autocomplete results by triggering its close() event
    $('#ordersAutoSearchCustomers').autocomplete('close');
    $('#ordersAutoSearchCustomers').autocomplete('destroy');
    $('#createOrderSelectCustomer .auto_search_results').hide();
    $('#ordersAutoSearchCustomers').val('Search').blur();
    $('#ordersAutoSearchCustomers').removeClass('search-input-clear');
    $('#createOrderSelectCustomer .auto_search_input_field').find('a').hide();
    $('#createOrderSelectedCustomerResults').hide();
    //added by Rajni
    $('#selectedCustomerId').val($('#selectCustomerDropdown').val());
    //added by Rajni
    buildAutoComplete();

    // close the select customer panel
    $('#createOrderSelectCustomer').hide();
    // show the add products panel

    $('#createOrderAddProducts').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

   // alert("outside selectCustomer()");
}
$('#ordersAutoSearchCustomersResults').on('click', 'a[href="#selectCustomerFromList"]', function () {
    
    selectCustomer();

    return false;
});
$('#selectCustomerDropdownPanel').on('change', 'select#selectCustomerDropdown', function () {
    ordersReadyToClose = true;

    if ($(this).val() !== '') {
        // call ajax to get the list of customers
        $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Customer/GetCustomerbyAgentId/",
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

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#createOrderSelectedCustomerResults').on('click', 'a[href="#selectCustomerFromList"]', function () {
    //alert("customer is selected");
    selectCustomer();

    return false;
});
$('#createOrderSelectedCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    $('#createOrderSelectedCustomerResults').hide();

    return false;
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
    if ($(this).val() === 'Enter Quantity (MT)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Quantity (MT)');
    }
});
$('#orderPurchaseOrderNumber')
.on('focus', function () {
    if ($(this).val() === 'Type Purchase Order #') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Type Purchase Order #');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: ADD PRODUCTS
 * -------------------------------------------------------------------------------------------------------- */

$('#createOrderAddProducts').on('click', 'a[href="#back"]', function () {
    $('#createOrderAddProducts').hide();
    $('#createOrderPanel .orders_auto_search_results').hide();
    $('#createOrderSelectCustomer').show();

    return false;
});
$('#createOrderAddProducts').on('click', 'a[href="#addProduct"]', function () {
    // added by Rajni
    
    ////////////////
   // alert("Add Product has been clicked, add order in session");
    var Result = {
        customer_id: $('#selectedCustomerId').val(),
        requested_delivery_date: ($('#requestedDeliveryDateDay').val()) + "/" + ($('#requestedDeliveryDateMonth').val()) + "/" + ($('#requestedDeliveryDateYear').val()),
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
                        // alert("success");
                         //$('#addProductPanel').slideDown();
                         //$('#createOrderCurrentProducts').html(data);
                      })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })

    // added by Rajni
if ($('#addProductPanel').is(':hidden')) {
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').hide();
    $('#addProductPanel').show();
    $('html,body').animate({
        scrollTop: $('#addProductPanel').position().top + $('#createOrderPanel').position().top
    }, 400);
}
    
    return false;
});
$('#addProductPanel').on('click', 'a[href="#closeAddProductPanel"]', function () {
    $('#addProductPanel').hide();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#addProductPanel').on('click', 'a[href="#addProductToOrder"]', function () {
    //alert("add product has been clicked");
    // added by Rajni
    // to create a temp order and order product until client finally clicks to save the order
    //alert("add product is clicked");

    /* stop form from submitting normally */
    //event.preventDefault();
    //alert("submit button is pressed");
    //$('form#FormOrderProdDetails').submit();

    var Result = {
        hCustomer_id: $('#selectedCustomerId').val(),
        selectBf: $('#selectBf').val(),
        selectGsm: $('#selectGsm').val(),
        selectShade: $('#selectShade').val(),
        selectWidth: $('#selectWidth').val(),
        selectCore: $('#selectCore').val(),
        inputDiaCm: $('#inputDiaCm').val(),
        inputQuantityMt: $('#inputQuantityMt').val(),
    };
    //alert("selectedCustomerId : " + $('#selectedCustomerId').val());
    $.ajax({
        //async: true,
        type: "POST",
        url: "/Order/addProductToOrderinSession/",
        data: Result,
        dataType: "html",
        context: document.body,
        //contentType: 'application/html; charset=utf-8'
    })
                    .success(function (data) {
                        //alert("Successfully Created temp product session");
                        //alert("data is" + data);
                        //$('#createOrderCurrentProducts').html(data); //otherwise i loose the button of 'cancel' and 'place order'
                        $('#lstProds').html(data);
                        
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })
    // added by Rajni
    $('#addProductPanel').hide();
    $('#createOrderDeliveryDate .add_product').show();
    $('#createOrderCurrentProducts').find('.product_totals, .order_buttons').show();
    $('#createOrderCurrentProducts').show();

    $('html,body').animate({
        scrollTop: $('#createOrderCurrentProducts').position().top + $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#createOrderCurrentProducts').on('click', 'a[href="#cancelPlaceOrder"]', function () {
    $('#createOrderAddProducts').hide();
    $('#createOrderCurrentProducts').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#createOrderCurrentProducts').on('click', 'a[href="#placeOrder"]', function () {
    // ajax call to load the review order partial view
    //alert("order is placed");
        // call ajax to get the list of customers
        $.ajax(
              {
                  cache: false,
                  type: "GET",
                  url: "/Order/OrderReview/",
                  contentType: "application/json; charset=utf-8",
                  success: function (data) {
                      
                     // $('#createOrderSelectedCustomerResults').show();
                      $('#ShowOrderReview').html(data);
                    }
              });
       
    $('#createOrderAddProducts').hide();
    $('#createOrderCurrentProducts').hide();
    $('#reviewOrder').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#inputDiaCm')
.on('focus', function () {
    if ($(this).val() === 'Enter DIA (cm)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter DIA (cm)');
    }
});
$('#inputQuantityMt')
.on('focus', function () {
    if ($(this).val() === 'Enter Quantity (MT)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Quantity (MT)');
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

    return false;
});
$('#reviewOrder').on('click', 'a[href="#cancelOrder"]', function () {
    $('#createOrderAddProducts').hide();
    $('#reviewOrder').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#reviewOrder').on('click', 'a[href="#submitOrder"]', function () {

    //alert('order has been submitted');

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

    return false;
});

/* -----------------------------------------------------------------------------------------------------------
 * CREATE ORDER: ORDER CONFIRMATION
 * -------------------------------------------------------------------------------------------------------- */

$('#orderConfirmation').on('click', 'a[href="#addAnotherOrder"]', function () {
    $('#orderConfirmation').hide();
    $('#createOrderCurrentProducts').hide();
    $('#createOrderAddProducts').hide();
    $('#createOrderSelectCustomer').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});
$('#orderConfirmation').on('click', 'a[href="#closeOrder"]', function () {
    $('#orderConfirmation').hide();
    $('#createOrderSelectCustomer').show();
    $('#createOrderPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);

    return false;
});

/* -----------------------------------------------------------------------------------------------------------
 * PANEL GRID
 * -------------------------------------------------------------------------------------------------------- */

$('.panel_grid').on('click', 'a[href="#customers"]', function () {
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

        $('html,body').animate({
            scrollTop: 0
        }, 400);
    }

    return false;
});
$('.panel_grid').on('click', 'a[href="#orders"]', function () {
    //alert("orders");
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

    return false;
});
$('.panel_grid').on('click', 'a[href="#transportation"]', function () {
    $('.order_details').hide();
    $('#customersPanel').hide();
    $('#ordersPanel').hide();
    $('#createOrderPanel').hide();

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

    return false;
});
$('.panel_grid').on('click', 'a[href="#reports"]', function () {
    return false;
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

    return false;
});
$('#customersPanel').on('click', 'a[href="#createCustomer"]', function () {
    $('a.create_order').show();
    $('#customersPanel').hide();
    $('#createOrderSelectCustomer').hide();
    $('#createOrderPanel').show();
    $('#createCustomerPanel').show();

    $('html,body').animate({
        scrollTop: $('#createOrderPanel').position().top
    }, 400);

    return false;
});

/* -----------------------------------------------------------------------------------------------------------
 * CUSTOMERS: AUTO-SEARCH
 * -------------------------------------------------------------------------------------------------------- */

var readyToClose = false,
    customerList = [
        { label: "Customer A", value: "customer-a", address: "523 Anywhere Ave.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-777-5555", fax: "800-777-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }] },
        { label: "Customer B", value: "customer-b", address: "1234 Nowhere Ln.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-5555", fax: "800-888-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }] },
        { label: "Customer C", value: "customer-c", address: "16B Forest Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-542-5555", fax: "800-542-3333", orders: [{ po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }, { po: 'MWV-1234-123456-1234', placedOnDay: '12', placedOnMonth: '10', placedOnYear: '2015' }] }
    ];

function buildCustomersAutoComplete() {
    // build an autocomplete widget for customer selection from a pre-defined list
    $('#autoSearchCustomers').autocomplete({
        // the box where the suggestion list will be displayed
        appendTo: '#autoSearchCustomersResults',
        // data source; currently local, can/should be AJAX
        source: customerList,
        // when the widget is created, there are some custom actions needed to
        // force necessary behavior, such as data attributes and leaving the list open
        create: function () {
            readyToClose = false;

            // override default autocomplete single item rendering functionality
            $(this).data('ui-autocomplete')._renderItem = function (ul, item) {
                // append a data-value attribute to the created <li> so it can be selected
                // later for appending customer data and allowing selection after the fact.
                return $('<li>')
                   .attr('data-value', item.value)
                   .append('<p class="customer_name">' + item.label + '</p>')
                   .appendTo(ul);
            };
            // do not allow the list of items to close until the ready flag has been set
            $(this).data('ui-autocomplete').close = function () {
                if (!readyToClose) {
                    this.cancelSearch = false;
                    return false;
                }

                this._close();
            };
        },
        // when a list item is selected from the autocomplete suggestion list
        select: function (evt, ui) {
            var customerDetails = '<p>' + ui.item.address + ' / ' + ui.item.city + ' / ' + ui.item.state + ' / ' + ui.item.pin + '</p> <p>' + ui.item.country + ' / ' + ui.item.phone + ' / ' + ui.item.fax + '</p> <a href="#createOrderFromCustomer" data-value="' + ui.item.value + '" data-name="' + ui.item.label + '" class="btn">Create Order</a>',
                orderHistory = '<p>View Order History</p> <select name="show_orders_type" class="select-input"><option>Show All Orders</option></select> <div class="from_to"><p class="form-label">From</p> <input type="text" value="DD" class="date-input"> <input type="text" value="MM" class="date-input"> <input type="text" value="YYYY" class="date-input-year"> <p class="form-label">To</p> <input type="text" value="DD" class="date-input"> <input type="text" value="MM" class="date-input"> <input type="text" value="YYYY" class="date-input-year"></div> <a href="#viewOrderHistory" class="btn">View</a>',
                customerOrders = '<div class="customer_orders hidden"><div class="fix-overflow"><a href="#closeCustomerOrdersPanel" class="close-panel"><img src="images/close-panel.png"></a></div> <div class="pagination"><a href="#" class="next"><img src="images/pagination-next.png"></a> <a href="#" class="prev"><img src="images/pagination-prev.png"></a> <a href="#" class="page">1</a> <a href="#" class="page">2</a> <a href="#" class="page current">3</a> <a href="#" class="page">4</a></div> <ul>';

            $.each(ui.item.orders, function (index, value) {
                customerOrders += '<li><div class="fix-overflow"><p class="title"><strong>PO: ' + value.po + '</strong></p> <a href="#downloadOrder" class="download_order"><img src="images/download.png"></a></div> <p class="details">Placed On: ' + value.placedOnDay + '-' + value.placedOnMonth + '-' + value.placedOnYear + '</p> <p><a href="#seeOrderDetails" class="see-details" data-id="' + value.po.toLowerCase() + '">See Details</a></p></li>';
            });

            customerOrders += '</ul></div>';

            // append full customer data to the selected list item and allow further
            // deep selection of individual customer
            $(evt.currentTarget)
                .find('li[data-value="' + ui.item.value + '"]')
                .append('<div class="list-item-wrapper"><div class="customer-list-item"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="images/close-panel.png"></a> <p class="title"><strong>' + ui.item.label + '</strong></p></div> ' + customerDetails + orderHistory + customerOrders + '</div></div>');

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
} // buildCustomersAutoComplete()

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
// close an individually selected customer so autocomplete can re-select
$('#autoSearchCustomersResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $('#autoSearchCustomersResults .list-item-wrapper').remove();
    $('#autoSearchCustomers').autocomplete('enable');
    // fake click because of some abstract bug not allowing to click on result
    $('#customersPanel').click();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);

    return false;
});
$('#selectCustomer').on('click', 'a[href="#clearSearch"]', function () {
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();

    buildCustomersAutoComplete();

    return false;
});
function createOrderFromCustomer() {
    readyToClose = true;

    // close the autocomplete results by triggering its close() event
    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();

    buildCustomersAutoComplete();

    // close the select customer panel
    $('#selectCustomer').hide();
    $('#customersPanel').hide();
    // show the add products panel
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
    createOrderFromCustomer();

    return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#createOrderFromCustomer"]', function () {
    createOrderFromCustomer();

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#createOrderFromCustomer"]', function () {
    createOrderFromCustomer();

    return false;
});
$('#customersSelectCustomerDropdownPanel').on('change', 'select', function () {
    readyToClose = true;

    // close the autocomplete results by triggering its close() event
    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    $('#alphabetCustomerResults').hide();

    buildCustomersAutoComplete();

    if ($(this).val() !== '') {
        $('#selectedCustomerResults').show();

        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top + 350
        }, 400);
    } else {
        $('#selectedCustomerResults').hide();
    }
});
$('#autoSearchCustomersResults').on('click', 'a[href="#viewOrderHistory"]', function () {
    $(this).next('.customer_orders').show();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + $(this).closest('.customer-list-item').height() - 340
    }, 400);

    return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#viewOrderHistory"]', function () {
    $(this).next('.customer_orders').show();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + $(this).closest('.customer-list-item').height() - 400
    }, 400);

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#viewOrderHistory"]', function () {
    $(this).next('.customer_orders').show();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + $(this).closest('.customer-list-item').height()
    }, 400);

    return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $(this).closest('.customer-list-item').find('.customer_orders').hide();
    $('#selectedCustomerResults').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCustomerResults"]', function () {
    readyToClose = false;

    $('#alphabetCustomerResults').find('.customer-list-item .customer_orders').hide();
    $('#alphabetCustomerResults .customer-list-item').hide();
    $('#alphabetCustomerResults').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;

    $(this).closest('.customer-list-item').find('.customer_orders').hide();
    $(this).closest('.customer-list-item').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 75
    }, 400);

    return false;
});
function customersSeeOrderDetails(id) {
    $('#customersPanel').hide();
    $('#orderDetails-' + id).show();
    $('#selectOrder').hide();
    $('#ordersPanel .close_orders_panel').hide();
    $('#ordersPanel .order_details').find('.arrange_new_transportation .cargo_list').hide();
    $('#ordersPanel').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top
    }, 400);
}
$('#autoSearchCustomersResults').on('click', 'a[href="#seeOrderDetails"]', function () {
    customersSeeOrderDetails($(this).attr('data-id'));

    return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#seeOrderDetails"]', function () {
    customersSeeOrderDetails($(this).attr('data-id'));

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#seeOrderDetails"]', function () {
    customersSeeOrderDetails($(this).attr('data-id'));

    return false;
});
$('#autoSearchCustomersResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 200
    }, 400);

    return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 225
    }, 400);

    return false;
});
$('#alphabetCustomerResults').on('click', 'a[href="#closeCustomerOrdersPanel"]', function () {
    $(this).closest('.customer_orders').hide();

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 250
    }, 400);

    return false;
});
$('#browseCustomerAlphabetically').on('change', 'select', function () {
    readyToClose = true;

    // close the autocomplete results by triggering its close() event
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

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 425
    }, 400);

    return false;
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

    $('html,body').animate({
        scrollTop: 0
    }, 400);

    return false;
});
$('#selectOrderPanel').on('change', '#selectOrderTypeDropdown', function () {
    if ($(this).val() === 'purchase-order-number') {
        $('#orderByDateRange').hide();
        $('#orderByPurchaseOrderNumber').show();
    } else {
        $('#orderByPurchaseOrderNumber').hide();
        $('#orderByDateRange').show();
    }
});
$('#selectOrderPanel').on('click', 'a[href="#searchOrders"]', function () {

    //Added by Manisha
    //for searcha order according status
    //alert("searchOrders");
    var selectedOrder =
        {
            SelectedOrderType: $('#selectOrderTypeDropdown').val(),
            FromDateTime: $('#orderFromDateDay').val() + "-" + $('#orderFromDateMonth').val() + "-" + $('#orderFromDateYear').val(),
            ToDateTime: $('#orderToDateDay').val() + "-" + $('#orderToDateMonth').val() + "-" + $('#orderToDateYear').val(),
        }
    //if (selectedOrder !== 0) {

    //}
    //else { alert("order"); }
    // selectOrderStatus = JSON.stringify(selectedOrder);
    //  alert(selectOrderStatus);
    $.ajax({
        url: "/Order/OrdersSearchResults/",
        data: selectedOrder,
        //contentType: "application/html; charset=utf-8",
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

    // $('#ordersResultsPanel').show();

    $('html,body').animate({
        scrollTop: $('#ordersResultsPanel').position().top + $('#selectOrderPanel').height() + 40
    }, 400);

    return false;
});
$('#ordersResultsPanel').on('click', 'a[href="#closeOrdersResultsPanel"]', function () {
    $('#ordersResultsPanel').hide();

    return false;
});
$('#ordersResultsPanel').on('click', 'a[href="#seeOrderDetails"]', function () {
    var id = $(this).attr('data-order-id');

   // alert("id");
    //Addede by manisha
    //for show order details

    var selectedOrder = {
        OrderId: $('#OrderId').val()
    }
    // selectOrderStatus = JSON.stringify(selectedOrder)
    $.ajax({
        url: "/Order/OrdersDetails/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                    .success(function (result) {
                        $('.close_orders_panel').hide();
                        $('#selectOrder').hide();
                        $('#ordersResultsPanel').hide();
                        $('#orderDetails-' + id).show();
                        //$('#ordersResultsPanel').slideDown();
                        //$('#OrderSearchDetails').html(result);
                    })
                   .error(function (xhr, ajaxOptions, thrownError) {
                   })
    //$('.close_orders_panel').hide();
    //$('#selectOrder').hide();
    //$('#ordersResultsPanel').hide();
    //$('#orderDetails-' + id).show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top
    }, 400);

    return false;
});
$('#ordersPanel .order_details').on('click', 'a[href="#arrangeTransportation"]', function () {

    if ($(this).next('.arrange_new_transportation').is(':hidden')) {
        $(this).next('.arrange_new_transportation').show();

        $('html,body').animate({
            scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 70
        }, 400);
    }

    return false;
});
$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#closeArrangeTransportationPanel"]', function () {
    $(this).closest('.arrange_new_transportation').hide();

    return false;
});
$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#addCargoToTransportation"]', function () {
   
    var $detailsPanel = $(this).closest('.order_details'),
        $transportationPanel = $(this).closest('.arrange_new_transportation'),
        detailsFull = $detailsPanel.find('.order_details_full').height(),
        locationDetails = $detailsPanel.find('.location_details').height(),
        timeFrames = $detailsPanel.find('.start_arrival_times').height(),
        cargoList = 0;

    $transportationPanel.find('.submit_transportation_button').hide();
    $transportationPanel.find('.add_cargo_panel').show();

    if ($detailsPanel.find('.cargo_list').is(':visible')) {
        cargoList = $detailsPanel.find('.cargo_list').height();
    }

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + detailsFull + 300 + locationDetails + timeFrames + cargoList
    }, 400);

    return false;
});
$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#addNewCargoItem"]', function () {
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

    return false;
});
$('#ordersPanel .arrange_new_transportation').on('click', 'a[href="#submitTransportation"]', function () {
    $(this).closest('.arrange_new_transportation').hide();
    $(this).closest('.order_details').find('.transportation_details').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);

    return false;
});
$('#ordersPanel .transportation_details').on('click', 'a[href="#closeReviewTransportationDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.arrange_new_transportation').show();

    return false;
});
$('#ordersPanel .transportation_details').on('click', 'a[href="#cancelReviewDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('arrange_new_transportation').hide();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);

    return false;
});
$('#ordersPanel .transportation_details').on('click', 'a[href="#editReviewDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.arrange_new_transportation').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);

    return false;
});
$('#ordersPanel .transportation_details').on('click', 'a[href="#submitReviewDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $(this).closest('.order_details').find('.review_transportation_confirmation').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + $(this).closest('.order_details').find('.order_details_full').height() + 80
    }, 400);

    return false;
});
$('#ordersPanel .review_transportation_confirmation').on('click', 'a[href="#closeTransportationConfirmation"]', function () {
    $(this).closest('.review_transportation_confirmation').hide();

    return false;
});
$('.order_details').on('click', 'a[href="#closeOrderDetails"]', function () {
    $(this).closest('.order_details').hide();
    $('.close_orders_panel').show();
    $('.select_order').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top
    }, 400);

    return false;
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
//added by Rajni
$("#SelectOrder").change(function () {
    var selectedItem = $(this).val();
    var ddlStates = $("#SelectOrderProds");
    //var statesProgress = $("#states-loading-progress");
    //statesProgress.show();
    //alert(selectedItem);
    $.ajax(
        {
            cache: false,
            type: "GET",
            url: "/Home/GetProductsByOrderId/" + selectedItem,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                ddlStates.html('');
                $.each(data, function (id, option) {
                    ddlStates.append($('<option></option>').val(option.id).html(option.name));
                });
                //statesProgress.hide();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                //alert("ajaxOptions " + ajaxOptions);
               // alert('Failed to retrieve states.');
               // statesProgress.hide();
            }
        });
});
// added by Rajni
$('#transportationPanel').on('click', 'a[href="#closeTransportationPanel"]', function () {
    $('a.create_order').show();
    $('#transportationPanel').hide();

    $('html,body').animate({
        scrollTop: 0
    }, 400);

    return false;
});
$('#selectTransportationPanel').on('click', 'a[href="#searchTransportation"]', function () {

    var Result = {
        selectVehicleTypeddl: $('#selectVehicleTypeDropdown').val(),
        FromDateTime: $('#transportationFromDateDay').val() + "-" + $('#transportationFromDateMonth').val() + "-" + $('#transportationFromDateYear').val(),
        ToDateTime: $('#transportationToDateDay').val() + "-" + $('#transportationToDateMonth').val() + "-" + $('#transportationToDateYear').val()
    };

    //var stringReqdata = JSON.stringify(Result);
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


    //$('#transportationResultsPanel').show();
    //$('#selectTransportationPanel .or_wording').hide();
    //$('#arrangeNewTransportation').hide();
    //$('#selectTransportationPanel .transportation_button').css('marginTop', '+=40');

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 380
    }, 400);

    return false;
});
$('#selectTransportationPanel').on('click', 'a[href="#closeTransportationResultsPanel"]', function () {
    $('#transportationResultsPanel').hide();
    $('#selectTransportationPanel .or_wording').show();
    $('#selectTransportationPanel .transportation_button').css('marginTop', '-=40');

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

    return false;
});
$('#selectTransportationPanel').on('change', 'select#selectVehicleTypeDropdown', function () {
    if ($(this).val() === 'vehicle-number') {
        $('#transportationFromTo').hide();
        $('#vehicleNumberInput').show();
    } else {
        $('#vehicleNumberInput').hide();
        $('#transportationFromTo').show();
    }
});
$('#selectTransportation').on('click', 'a[href="#arrangeTransportation"]', function () {
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

    return false;
});
$('#arrangeNewTransportation').on('click', 'a[href="#closeArrangeTransportationPanel"]', function () {
    $('#arrangeNewTransportation').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 200
    }, 400);

    return false;
});
$('.transportation_details').on('click', 'a[href="#addCargoToCargoList"]', function () {
    $(this).closest('.cargo_list').find('.add_cargo_panel').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + $(this).closest('.transportation_details').find('.transportation_details_full').height() + $(this).closest('.cargo_list').height() + 275
    }, 400);

    return false;
});
$('.transportation_details').on('click', 'a[href="#addNewCargoItem"]', function () {
    var $cargoItems = $(this).closest('.cargo_list').find('ul');

    $cargoItems.find('li:last-child').clone().appendTo($cargoItems);
    $(this).closest('.add_cargo_panel').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + $(this).closest('.transportation_details').find('.transportation_details_full').height() + 375
    });

    return false;
});
$('#transportationResultsPanel').on('click', 'a[href="# "]', function () {
    var panelID = $(this).attr('data-id');

    $('#transportationResultsPanel').hide();
    $('#transportationDetails-' + panelID).show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 400
    }, 400);

    return false;
});
$('#transportationPanel .transportation_details').on('click', 'a[href="#closeTransportationDetails"]', function () {
    $(this).closest('.transportation_details').hide();
    $('#selectTransportationPanel .or_wording').show();
    $('#selectTransportationPanel .transportation_button').css('marginTop', '-=40');

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

    return false;
});
$('#arrangeNewTransportation').on('click', 'a[href="#addCargoToTransportation"]', function () {
   // alert("add cargo is clicked");
    // added by Rajni
    // save the temp truck dispatch record in session

    var Result = {
        loc_id: $('#newTransportationLocation').val(),
        vehicle_num: $('#newTransportationVehicleNumber').val(),
        vehicle_capacity: $('#newTransportationVehicleCapacity').val(),

        agent_disp_on: $('#newTransportationStartDay').val() + "/" + $('#newTransportationStartMonth').val() + "/" + $('#newTransportationStartYear').val(),
        agent_disp_on_time: $('#newTransportationStartTime').val(),
        agent_disp_on_time_ampm: $('#newTransportationStatTimeAmPm').val(),

        estimated_arr_date: $('#newTransportationArrivalDay').val() + "/" + $('#newTransportationArrivalMonth').val() + "/" + $('#newTransportationArrivalYear').val(),
        estimated_arr_date_time: $('#newTransportationArrivalTime').val(),
        estimated_arr_date_time_ampm: $('#newTransportationArrivalTimeAmPm').val(),
    };
    //alert("location id: " + $('#newTransportationLocation').val());
    $.ajax({
        type: "POST",
        url: "/Truck_dispatch/addTempDispatchinSession/",
        context: document.body,
        data: Result,
        dataType: "html",
        context: document.body,
    })
                     .success(function (data) {
                         //alert("Successfully Created in session, data is  " + data);
                     })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })

   // alert("out of ajax");
    // added by Rajni
        $(this).closest('.arrange_new_transportation').find('.add_cargo_panel').show();
        $('html,body').animate({
            scrollTop: $('#transportationPanel').position().top + $('#arrangeNewTransportation').height() + 50
        }, 400);
    

    return false;
});
$('#arrangeNewTransportation').on('click', 'a[href="#addNewCargoItem"]', function () {
    // added by Rajni
    //alert("save the cargo in session");
    // save the temp cargo record in session

    var Result = {
        order_id: $('#Orderddl').val(),
        prod_id: $('#Productsddl').val(),
        qty: $('#addCargoPanelEnterQuantity').val()
    };
    
    $.ajax({
        type: "POST",
        url: "/Truck_dispatch/addTempCargoinSession/",
        context: document.body,
        data: Result,
        dataType: "html",
        context: document.body,
    })
                     .success(function (data) {
                         //alert("Successfully Created cargo in session, data is  " + data);
                            $('#arrangeNewTransportation .cargo_list').show();
                            $('#showCargoList').html(data);
                     })
                    .error(function (xhr, ajaxOptions, thrownError) {
                    })

    // added by Rajni

    
    $('#arrangeNewTransportation .submit_transportation_button').show();
    $(this).closest('.add_cargo_panel').hide();

    return false;
});
$('.add_cargo_panel').on('click', 'a[href="#closeAddCargoPanel"]', function () {
    $(this).closest('.add_cargo_panel').hide();

    return false;
});
$('#arrangeNewTransportation').on('click', 'a[href="#submitTransportation"]', function () {
    // call review page
   // alert("call review method now");
    $.ajax(
        {
                      cache: false,
                      type: "GET",
                      url: "/Truck_dispatch/GetReviewTransportation/",
                      contentType: "application/json; charset=utf-8",
                      success: function (data) {
                         // alert("on the success of call review method");
                          $('#reviewTransport').html(data);

                      },
                      error: function (xhr, ajaxOptions, thrownError) {
                          //alert("ajaxOptions " + ajaxOptions);
                          // alert('Failed to retrieve states.');
                      }
                  });
    $('#arrangeNewTransportation').hide();
    $('#reviewTransportationDetails').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 500
    }, 400);


    return false;
});
$('#reviewTransportationDetails').on('click', 'a[href="#closeReviewTransportationDetails"]', function () {
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

    return false;
});
$('#reviewTransportationDetails').on('click', 'a[href="#cancelReviewDetails"]', function () {
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').hide();
    $('#selectTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

    return false;
});
$('#reviewTransportationDetails').on('click', 'a[href="#editReviewDetails"]', function () {
    $('#reviewTransportationDetails').hide();
    $('#arrangeNewTransportation').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 500
    }, 400);

    return false;
});
$('#reviewTransportationDetails').on('click', 'a[href="#submitReviewDetails"]', function () {
    //alert("Here, save the temp transportation created in session to the database");
    $.ajax(
        {
        url: "/Truck_dispatch/confirmTransportation/",
        contentType: "application/html; charset=utf-8",
        type: "POST",
        dataType: 'html'
    })
                  .success(function (result) {
                   //alert("saved in db");
                  })
                 .error(function (xhr, ajaxOptions, thrownError) {
                 })

    $('#reviewTransportationDetails').hide();
    $('#reviewTransportationConfirmation').show();
    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 500
    }, 400);

    return false;
});
$('#reviewTransportationConfirmation').on('click', 'a[href="#closeConfirmation"]', function () {
    $('#reviewTransportationConfirmation').hide();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top
    }, 400);

    return false;
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
    if ($(this).val() === 'Enter Quantity (MT)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Quantity (MT)');
    }
});
$('#transportationDetailsAddCargoPanelEnterQuantity')
.on('focus', function () {
    if ($(this).val() === 'Enter Quantity (MT)') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Enter Quantity (MT)');
    }
});

/* -----------------------------------------------------------------------------------------------------------
 * QUICK VIEW
 * -------------------------------------------------------------------------------------------------------- */

$('#quickView').on('click', 'a[href="#seeAllRecentOrders"]', function () {
    //  alert("QuickView");
    $.ajax({
        url: "/QuickView/seeAllRecentOrders/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                //$('#quickViewRecentOrders').html(result);
                                $('#customersPanel').hide();
                                $('#transportationPanel').hide();
                                $('.order_details').hide();
                                $('#ordersResultsPanel').show();
                                $('#ordersResultsPanel').html(result);
                                $('#ordersPanel').show();
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })
    //});
    //$('#customersPanel').hide();
    //$('#transportationPanel').hide();
    //$('.order_details').hide();
    ////$('#ordersResultsPanel').show();
    //$('#ordersPanel').show();

    $('html,body').animate({
        scrollTop: $('#ordersPanel').position().top + 260
    }, 400);

    return false;
});

//$('#quickView').on('click', 'a[href="#seeOrderDetails"]', function () {
//    var id = $(this).attr('data-order-id');


//    $('#customersPanel').hide();
//    $('#transportationPanel').hide();
//    $('.close_orders_panel').hide();
//    $('#selectOrder').hide();
//    $('#ordersResultsPanel').hide();
//    $('#orderDetails-' + id).show();
//    $('#ordersPanel').show();

//    $('html,body').animate({
//        scrollTop: $('#ordersPanel').position().top
//    }, 400);

//    return false;
//});

$('#quickView').on('click', 'a[href="#seeAllRecentTransportation"]', function () {
    //alert("seeAllRecentTransportation");
    $.ajax({
        url: "/QuickView/seeAllRecentTransportation/",
        contentType: "application/html; charset=utf-8",
        type: "GET",
        dataType: 'html',
        context: document.body,
    })
                            .success(function (result) {
                                // $('#quickViewRecentOrders').html(result);
                                $('#customersPanel').hide();
                                $('#transportationPanel').hide();
                                $('.order_details').hide();
                                $('#ordersResultsPanel').show();
                                $('#ordersResultsPanel').html(result);
                                $('#ordersPanel').show();
                            })
                           .error(function (xhr, ajaxOptions, thrownError) {
                           })

    //$('#customersPanel').hide();
    //$('#ordersPanel').hide();
    //$('.order_details').hide();
    //$('#transportationResultsPanel').show();
    //$('#transportationPanel').show();

    $('html,body').animate({
        scrollTop: $('#transportationPanel').position().top + 400
    }, 400);

    return false;
});
//$('#seeTransportationDetails').on('click', 'a[href="#seeTransportationDetails"]', function () {
//    // var id = $(this).attr('Vehicleno');
//    var selectedOrder = {
//        Vehicleno: $('#Vehicleno').val()
//    }
//    $.ajax({
//        url: this.href,
//        contentType: "application/html; charset=utf-8",
//        type: "GET",
//        dataType: 'html',
//        context: document.body,
//    })
//                           .success(function (result) {
//                               // $('#quickViewRecentOrders').html(result);
//                               //$('#customersPanel').hide();
//                               //$('#transportationPanel').hide();
//                               //$('.order_details').hide();
//                               //$('#ordersResultsPanel').show();
//                               //$('#ordersResultsPanel').html(result);
//                               //$('#ordersPanel').show();
//                           })
//                          .error(function (xhr, ajaxOptions, thrownError) {
//                          })

//    $('#customersPanel').hide();
//    $('#ordersPanel').hide();
//    $('#transportationResultsPanel').hide();
//    $('#transportationDetails-' + id).show();
//    $('#transportationPanel').show();

//    $('html,body').animate({
//        scrollTop: $('#transportationPanel').position().top + 400
//    }, 400);

//    return false;
//});

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