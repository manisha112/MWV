


//Refreshing All Div 
function RefreshDiv() {
    $('#selectCustomerDropdown').find('option:first').prop('selected', 'selected');
    //$("#selectCustomerDropdown > option:first").attr("selected", "selected");
    $('#selectCustomerAlphabetically').find('option:first').prop('selected', 'selected');
}


$('#financeCustomers').on('click', 'a[href="#customersPanel"]', function () {
    $('#alphabetCustomerResults').hide();
    if ($('#customersPanel').is(':hidden')) {

        RefreshDiv();
        $('#customersPanel').show();
    } else {
        RefreshDiv();
        $('#customersPanel').hide();
    }

    return false;
});

$('#customersPanel').on('click', 'a[href="#closeCustomersPanel"]', function () {
    $('#customersPanel').hide();

    return false;
});

/* -----------------------------------------------------------------------------------------------------------
 * FINANCE: CUSTOMER AUTO-SEARCH
 * -------------------------------------------------------------------------------------------------------- */
//For Searching Customers
var readyToClose = false,
    alreadyFocused = false,
    customerList = [
        { label: "Alesha Appelbaum", value: "alesha-appelbaum", address: "4321 Somewhere Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-9999", fax: "800-888-7777" },
        { label: "Customer A", value: "customer-a", address: "523 Anywhere Ave.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-777-5555", fax: "800-777-3333" },
        { label: "Customer B", value: "customer-b", address: "1234 Nowhere Ln.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-5555", fax: "800-888-3333" },
        { label: "Customer C", value: "customer-c", address: "16B Forest Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-542-5555", fax: "800-542-3333" }
    ];

function buildAutoComplete() {
    var getData = function (request, response) {

        $.ajax({
            url: '/FinanceHead/GetSearchResult', type: "GET", dataType: "json",

            // query will be the param used by your action method
            data: { query: request.term },
            success: function (data) {
                response(data);
            }
        })
    };
    // build an autocomplete widget for customer selection from a pre-defined list
    $('#autoSearchCustomers').autocomplete({
        // the box where the suggestion list will be displayed
        appendTo: '#autoSearchCustomersResults',
        // data source; currently local, can/should be AJAX
        source: getData,
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
        focus: function (evt, ui) {
            //evt.preventDefault();
            return false;
        },
        // when a list item is selected from the autocomplete suggestion list
        select: function (evt, ui) {
            debugger;
            var creditamt = parseFloat(ui.item.CreditLimit);
            var city = ui.item.city;
            var state = ui.item.state;
            var pin = ui.item.pin; var country = ui.item.country; var phone = ui.item.phone; var fax = ui.item.fax; var agentName = ui.item.agentname;
            var address1 = ui.item.address1; var address2 = ui.item.address2; var address3 = ui.item.address3;

            if (address1 == null) { address1 = "" } else { address1 = address1 + "," }
            if (address2 == null) { address2 = "" } else { address2 = address2 + "," }
            if (address3 == null) { address3 = "" }
            var finaladdress = address1 + address2 + address3;
            var pin = ui.item.pin; var country = ui.item.country; var phone = ui.item.phone; var fax = ui.item.fax;
            if (city == null) { city = "" } else { city = city + "," }
            if (state == null) { state = ""; city = city.substring(0, city.length - 1); }
            var Getaddress = city + state;
            if (pin == null) { pin = "" } else { pin = pin + "," }
            if (country == null) { country = ""; pin = pin.substring(0, pin.length - 1); }
            var PinCoun = pin + " " + country;
            if (PinCoun == null) { PinCoun = "" }
            if (phone == null) { phone = "" } else { phone = phone + "," }
            if (fax == null) { fax = ""; phone = phone.substring(0, phone.length - 1); }
            var phoFax = phone + fax;
            if (agentName == null) { agentName = "" }
            if (isNaN(creditamt))
            { creditamt = 0; }
            var creditDays = ui.item.CreditDays;
            //For Testing Only
            $(evt.currentTarget)
               .find('li[data-value="' + ui.item.value + '"]')
               .append('<div class="list-item-wrapper"><div class="customer-list-item"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="/images/close-panel.png"></a> <p class="title"><strong>' + ui.item.label + '</strong></p></div><p>' + finaladdress + ' </p><p>' + Getaddress + ' </p><p> ' + PinCoun + '</p><p>' + phoFax + '</p> <p class="title"><strong>' + agentName + '</strong></p> <p class="form-label credit_period">Credit Period</p> <div class="credit_period_options"><label class="toggle-btn toggle-corner-left toggle-state-active"><input type="radio" name="credit_period" class="radio-hidden" value="30">30 Days</label> <label class="toggle-btn"><input type="radio" name="credit_period" class="radio-hidden" value="60">60 Days</label> <label class="toggle-btn toggle-corner-right"><input type="radio" name="credit_period" class="radio-hidden" value="90">90 Days</label></div> <p class="form-label">Credit Amount</p> <input type="text" id="txtCreditAmt" class="text-input credit_amount" value=' + creditamt + ' placeholder="Enter Credit Amount"><span id="autosearchErrors" class="error-msg"></span> <a href="#closeCurrentCustomer" class="btn cancel_btn">Cancel</a> <a href="#saveCustomerCredit" class="btn">Save</a> <div class="save_confirmation hidden"><div class="fix-overflow"><a href="#closeCurrentCustomer" class="close-panel"><img src="/images/close-panel.png"></a> <p class="title"><strong>Your values have been saved.</strong></p></div></div></div></div>');
            $('#autoSearchCustomers').autocomplete('disable');
            SearchautocompletebindRadio(creditDays);
            return false;
        },
        open: function (evt, ui) {
            $('#selectedCustomerResults').hide();
            $('#selectCustomer .auto_search_results').show();
            // $('#autoSearchCustomers').addClass('search-input-clear');
            $('#selectCustomer .auto_search_input_field').find('a').show();
        }
    });
}; // buildAutoComplete()

var CrdDays;
//binding radio according to values in search customers
function SearchautocompletebindRadio(CrdDays) {

    //  var getVals = $('#autoSearchCustomersResults').find('input[type="radio"]'.val());
    if (CrdDays == "30") {
        $("input[name=credit_period][value='60']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value='90']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value=" + CrdDays + "]").attr('checked', 'checked');
        $("input[name=credit_period][value=" + CrdDays + "]").addClass('toggle-state-active');
    }
    else if (CrdDays == "60") {
        $("input[name=credit_period][value='30']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value='90']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value=" + CrdDays + "]").attr('checked', 'checked');
        $("input[name=credit_period][value=" + CrdDays + "]").parent().addClass('toggle-state-active');
    }
    else if (CrdDays == "90") {
        $("input[name=credit_period][value='30']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value='60']").parent().removeClass('toggle-state-active');
        $("input[name=credit_period][value=" + CrdDays + "]").attr('checked', 'checked');
        $("input[name=credit_period][value=" + CrdDays + "]").parent().addClass('toggle-state-active');

    }
    else {

        $("input[name=credit_period][value='30']").parent().addClass('toggle-state-active');
        $("input[name=credit_period][value='30']").attr('checked', 'checked');
    }
}

$('#autoSearchCustomersResults').on('click', 'input[type="text"]', function (evt) {
    var $input = $(evt.currentTarget);

    if ($input.val() != 'Enter Credit Amount') {
        var getval = $input.val();
        $input.val('');
        $input.val(getval);
    }


    $input.focus();
    alreadyFocused = true;

    // return false;
});
$('#autoSearchCustomersResults').on('click', 'input[type="radio"]', function (evt) {
    $(evt.currentTarget).closest('.credit_period_options').find('label.toggle-btn').removeClass('toggle-state-active');
    $(evt.currentTarget).closest('label.toggle-btn').addClass('toggle-state-active');
});

$('#selectCustomer').on('focus', '#autoSearchCustomers', function (evt) {
    if (!alreadyFocused) {
        if ($('#selectCustomer .auto_search_results').is(':visible')) {
            readyToClose = true;

            $('#autoSearchCustomers').autocomplete('close');
            $('#autoSearchCustomers').autocomplete('destroy');
            $('#selectCustomer .auto_search_results').hide();
            $('#autoSearchCustomers').removeClass('search-input-clear');
            $('#selectCustomer .auto_search_input_field').find('a').hide();

            buildAutoComplete();

            $('#autoSearchCustomers').val('').focus();
        }
    }
});
// close an individually selected customer so autocomplete can re-select
$('#autoSearchCustomersResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    readyToClose = false;
    var liid = $(this).closest("li").attr('id');

    $('#autoSearchCustomersResults .list-item-wrapper').remove();
    $('#autoSearchCustomers').autocomplete('enable');
    // fake click because of some abstract bug not allowing to click on result
    $('#financeCustomers').click();

    //return false;
});
//Save Customer From Search Customer
$('#autoSearchCustomersResults').on('click', 'a[href="#saveCustomerCredit"]', function () {
    readyToClose = false;
    // $(this).next('.save_confirmation').show();
    var liid = $(this).closest("li").attr('id');
    $('#' + liid).find('#autosearchErrors').hide();
    var allValues = {
        selectedCustomerid: $('#' + liid).attr("data-value"),
        CreditDays: $('input[name=credit_period]:checked').val(),
        CreditAmt: $('#' + liid).find('#txtCreditAmt').val(),
    }
    var crdamt = $('#' + liid).find('#txtCreditAmt').val();
    if (crdamt == "") {
        $('#' + liid).find('#autosearchErrors').show();
        $('#' + liid).find('#autosearchErrors').html("Please Select Credit Amount !");
    }

    else {
        $('#' + liid).find('#autosearchErrors').hide();
        $(this).next('.save_confirmation').show();
        $.ajax(
    {
        cache: false,
        type: "GET",
        url: "/FinanceHead/UpdateCustomerDetails/",
        data: allValues,
        contentType: "application/json; charset=utf-8",
        success: function (data) {

            $('#autoSearchCustomersResults').find('.save_confirmation').show();


        }

    });


    }


    //return false;
});
$('#selectCustomer').on('click', 'a[href="#clearSearch"]', function () {
    readyToClose = true;

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();

    buildAutoComplete();

    //return false;
});
function selectCustomer() {
    readyToClose = true;

    // close the autocomplete results by triggering its close() event
    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    $('#selectedCustomerResults').hide();

    buildAutoComplete();

    // close the select customer panel
    $('#selectCustomer').hide();

    $('html,body').animate({
        scrollTop: $('#financeCustomers').position().top
    }, 400);
}
$('#autoSearchCustomersResults').on('click', 'a[href="#selectCustomerFromList"]', function () {
    selectCustomer();

    //return false;
});
$('#selectCustomerDropdownPanel').on('change', 'select#selectCustomerDropdown', function () {
    readyToClose = true;

    if ($(this).val() !== '') {
        $('#selectedCustomerResults').show();

        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top + 150
        }, 400);
    } else {
        $('#selectedCustomerResults').hide();
    }

    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    buildAutoComplete();

    //return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#selectCustomerFromList"]', function () {
    selectCustomer();

    //return false;
});
$('#selectedCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {
    $(this).find(".error-msg").hide();
    $('#selectedCustomerResults').hide();

    //return false;
});
//on Customer Dropdown On Changed
$('#browseCustomerAlphabetically').on('change', 'select', function () {
    readyToClose = true;
    var selectedCustomerAlphabet = $('select#selectCustomerAlphabetically').val();
    var datatosend = {
        selectedAlphabet: selectedCustomerAlphabet
    }
    $.ajax(
             {
                 cache: false,
                 type: "POST",
                 url: "/FinanceHead/GetCustomersbyAlphabet/",
                 data: datatosend,
                 dataType: "html",
                 success: function (data) {
                     // alert(data);
                     $('#alphabetCustomerResults').show();
                     $('#alphabetCustomerResults').html(data);
                 }
             });

    // close the autocomplete results by triggering its close() event
    $('#autoSearchCustomers').autocomplete('close');
    $('#autoSearchCustomers').autocomplete('destroy');
    $('#selectCustomer .auto_search_results').hide();
    $('#autoSearchCustomers').val('Search').blur();
    $('#autoSearchCustomers').removeClass('search-input-clear');
    $('#selectCustomer .auto_search_input_field').find('a').hide();
    $('#selectedCustomerResults').hide();

    buildAutoComplete();

    if ($(this).val() !== '') {
        $('#alphabetCustomerResults').show();

        $('html,body').animate({
            scrollTop: $('#customersPanel').position().top + 400
        }, 400);
    } else {
        $('#alphabetCustomerResults').hide();
    }
});

///Select Customer from Alpha result
$('#alphabetCustomerResults').on('click', 'a[href="#selectCustomer"]', function () {
    $('#alphabetCustomerResults .customer-list-item').hide();
    $(this).closest('li').find('.customer-list-item').show();

    var liid = $(this).closest("li").attr('id');
    var allValues = {
        selectedCustomerid: $(this).attr("data-id"),
    }
    $.ajax(
         {
             cache: false,
             type: "POST",
             url: "/FinanceHead/GetCustomersbyTab/",
             data: allValues,
             dataType: "html",
             success: function (data) {
                 // alert(data);
                 $('#' + liid).find('#divCustList').html(data);
                 //$('#divCustList').show();
                 // $('#divCustList').html(data);
             }
         });

    $('html,body').animate({
        scrollTop: $('#customersPanel').position().top + 425
    }, 400);

    //return false;
});

$(function () {
    buildAutoComplete();

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
    $('.credit_amount')
    .on('blur', function () {
        if ($(this).val().length === 0) {
            $(this).val('Enter Credit Amount');
        }
    });
});


//Update Customer Details
$('#selectedCustomerResults').on('click', 'a[href="#saveCustomerCredit"]', function () {

    //alert('order has been submitted');
    var allValues = {
        selectedCustomerid: $('#selectCustomerDropdown').val(),
        CreditDays: $("#CreditDays").children(":selected").prop("id"),
        CreditAmt: $("#txtCreditAmt").val(),
    }
    var crdamt = $("#txtCreditAmt").val();
    var selectDays = $("#CreditDays").children(":selected").text();
    if (selectDays == "Select") {
        $("#financeErrors").show();
        $("#financeErrors").html("Please Select Credit Period !");
    }

    else if (crdamt == "") {
        $("#financeErrors").show();
        $("#financeErrors").html("Please Enter Credit Amount !");

    }

    else {

        $.ajax(
                 {
                     cache: false,
                     type: "GET",
                     url: "/FinanceHead/UpdateCustomerDetails/",
                     data: allValues,
                     contentType: "application/json; charset=utf-8",

                     success: function (data) {
                         $('#selectedCustomerResults').hide();

                     }
                 });
        $('#selectedCustomerResults').hide();
        $(this).closest('li').remove();
        $('#reviewOrder').hide();
        $('#orderConfirmation').show();

        $('html,body').animate({
            scrollTop: $('#createOrderPanel').position().top
        }, 400);
        $('html,body').animate({
            scrollTop: $('#createOrderPanel').position().top
        }, 400);
        // 

    }
});
//Update Customer by alpha Update Panel
$('#alphabetCustomerResults').on('click', 'a[href="#saveCustomerCredit"]', function () {

    var liid = $(this).closest("li").attr('id');
    var allValues = {
        selectedCustomerid: $(this).attr("data-id"),
        CreditDays: $('#' + liid).find('#CreditDays').children(":selected").attr("id"),
        CreditAmt: $('#' + liid).find('#txtCreditAmt').val(),
    }
    var crdamt = $('#' + liid).find('#txtCreditAmt').val();
    var selectDays = $('#' + liid).find('#CreditDays').children(":selected").text();
    if (selectDays == "Select") {
        $('#' + liid).find('#FinanceListCustErrors').show();
        $('#' + liid).find('#FinanceListCustErrors').html("Please Select Credit Period !");

    }

    else if (crdamt == "") {
        $('#' + liid).find('#FinanceListCustErrors').show();
        $('#' + liid).find('#FinanceListCustErrors').html("Please Select Credit Amount !");


    }

    else {


        $.ajax({
            cache: false,
            type: "GET",
            url: "/FinanceHead/UpdateCustomerDetails/",
            data: allValues,
            contentType: "application/json; charset=utf-8",

            success: function (data) {
                $(this).closest('li').remove();
                $('#reviewOrder').hide();
                $('#orderConfirmation').show();
            }
        });
        $('#alphabetCustomerResults').hide();
        $(this).closest('li').remove();
        $('#reviewOrder').hide();
        $('#orderConfirmation').show();

        $('html,body').animate({
            scrollTop: $('#createOrderPanel').position().top
        }, 400);
        //

    }
});

//$(function () {
//    $(document).on('click', 'a', function (evt) {
//        var href = $(this).attr('href');

//        if (href.indexOf(document.domain) > -1 || href.indexOf(':') === -1) {
//            history.pushState({}, '', '');
//        }
//        return false;
//    });
//});
//Close Alpha Panel
$('#alphabetCustomerResults').on('click', 'a[href="#closeCurrentCustomer"]', function () {

    //$('#divCustList').hide();
    var liid = $(this).closest("li").attr('id');
    $('#' + liid).find('#divCustList').hide();
    $('#' + liid).find('#FinanceListCustErrors').hide();

});

$('#alphabetCustomerResults').on('click', 'a[href="#closeCustomerResults"]', function () {

    $("#alphabetCustomerResults").hide();

});

 


