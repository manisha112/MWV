$('.stakeholder_tab').on('click', 'a[href="#stakeholderReports"]', function (event) {
    event.stopImmediatePropagation();
    $('#reportsddl').find('option:first').prop('selected', 'selected');
    $('#search_by_bd').find('option:first').prop('selected', 'selected');
    $('#agentDropdown').find('option:first').prop('selected', 'selected');
    $('#search_by_orderbook').find('option:first').prop('selected', 'selected');
    $('#agentDropdownbdReport').find('option:first').prop('selected', 'selected');
    $('#papermillList').find('option:first').prop('selected', 'selected');

    $('#fromDateDay').find('option:first').prop('selected', 'selected');
    $('#fromDateMonth').find('option:first').prop('selected', 'selected');
    $('#fromDateYear').find('option:first').prop('selected', 'selected');
    $('#toDateDay').find('option:first').prop('selected', 'selected');
    $('#toDateMonth').find('option:first').prop('selected', 'selected');
    $('#toDateYear').find('option:first').prop('selected', 'selected');

    if ($('#stakeholderReports').is(':hidden')) {
        $('#stakeholderReports').show();
        $('#orderBook').hide();
        $('#bdReport').hide();
        $('#fgReport').hide();
        $('#customersAgentsOptions').hide();
        $('#reportByDateRange').hide();
        $('#searchAgents').hide();
        $('.search_reports_button').hide();

    } else {
        $('#stakeholderReports').hide();
    }
});

$('#stakeholderReports').on('click', 'a[href="#closeStakeholderReports"]', function () {
    $('#stakeholderReports').hide();
});

$('#stakeholderReports').on('change', 'select.choose_report', function () {
    $('#stackHolderErrorFromdate').hide();
    $('#stackHolderErrorTodate').hide();

    $('.option-pod').hide();
    debugger;
    if ($(this).val() !== '') {
        $('.report_by_date_range').show();
        $('.search_reports_button').show();
    }

    switch ($(this).val()) {
        case 'customer':
            $('#searchCustomers').show();
            $('#customersAgentsOptions').show();
            break;
        case 'agent':
            $('#searchAgents').show();
            $('#customersAgentsOptions').show();
            break;
        case 'order-book':
            $('#orderBook').show();
            break;
        case 'bd':
            $('#bdReport').show();
            break;
        case 'fg':
            $('#fgReport').show();
            break;
    }
});

$('#searchAgents').on('click', 'input.all_agents_checkbox', function () {
    if ($(this).is(':checked')) {
        $('#agentDropdown').addClass('disabled').prop('disabled', true);
    } else {
        $('#agentDropdown').removeClass('disabled').prop('disabled', false);
    }
});
$('#orderBook').on('change', 'select.search_by', function () {
    debugger;
    if ($(this).val() === 'by-agent') {
        $('#orderBook .auto_search_input_field').hide();
        $('#orderBook .select_agent').show();
        $('#agentDropdownOrderBook').show();

    } else {

        $('#agentDropdownOrderBook').hide();
        $('#orderBook .select_agent').hide();
        $('#orderBook .auto_search_input_field').show();
    }
});


$('#bdReport').on('change', 'select.search_by', function () {
    debugger;
    if ($(this).val() === 'by-agent') {
        $('#bdReport .auto_search_input_field').hide();
        $('#bdReport .select_agent').show();
    } else {
        $('#bdReport .select_agent').hide();
        $('#bdReport .auto_search_input_field').show();
    }
});
//$('#stakeholderReports').on('click', 'a[href="#searchReports"]', function () {
//    $('#reportResultsPanel').show();

//    $('html,body').animate({
//        scrollTop: $('#reportResultsPanel').position().top
//    }, 400);
//});


$('#stakeholderReports').on('click', 'a[href="#searchReports"]', function () {
    debugger;
    var fromdt = $('#fromDateDay').val() + "/" + $('#fromDateMonth').val() + "/" + $('#fromDateYear').val();
    var todt = $('#toDateDay').val() + "/" + $('#toDateMonth').val() + "/" + $('#toDateYear').val();
    var fromdateTime = ValidateDate(fromdt);
    var todateTime = ValidateDate(todt);
    var fromdt = $('#fromDateYear').val() + "/" + $('#fromDateMonth').val() + "/" + $('#toDateDay').val();
    var todt = $('#fromDateYear').val() + "/" + $('#toDateMonth').val() + "/" + $('#toDateDay').val();
    var dateCompare = CompareDate(fromdt, todt); //stackHolderErrorFromdate stackHolderErrorTodate
    if (fromdateTime == false && todateTime == false) {
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorTodate').show();
        $('#stackHolderErrorFromdate').html("<p class='error-msg'>Please select a Valid From date !");
        $('#stackHolderErrorTodate').html("Please select a Valid To date !");
    }

    else if ($('#fromDateDay').val() == "DD" && $('#fromDateMonth').val() == "MM" && $('#fromDateYear').val() == "YYYY"
&& $('#toDateDay').val() == "DD" && $('#toDateMonth').val() == "MM" && $('#toDateYear').val() == "YYYY") {
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
    } else if ($('#fromDateDay').val() == "DD" || $('#fromDateMonth').val() == "MM" || $('#fromDateYear').val() == "YYYY") {
        $('#stackHolderErrorTodate').hide();
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
        if ($('#fromDateDay').val() == "DD" || $('#fromDateMonth').val() == "MM" || $('#fromDateYear').val() == "YYYY") {
            $('#stackHolderErrorTodate').hide();
            $('#stackHolderErrorFromdate').show();
            $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
        }

        else if ($('#toDateDay').val() == "DD" || $('#toDateMonth').val() == "MM" || $('#toDateYear').val() == "YYYY") {
            $('#stackHolderErrorTodate').show();
            $('#stackHolderErrorFromdate').hide();
            $('#stackHolderErrorTodate').html("Please select a Valid To date !");
        }
    }
    else if ($('#fromDateDay').val() == "DD" || $('#fromDateMonth').val() == "MM" || $('#fromDateYear').val() == "YYYY") {
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
        if ($('#toDateDay').val() == "DD" || $('#toDateMonth').val() == "MM" || $('#toDateYear').val() == "YYYY") {
            $('#stackHolderErrorFromdate').hide();
            $('#stackHolderErrorTodate').show();
            $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
            $('#stackHolderErrorTodate').html("Please select a Valid To date !");
        }
    } else if ($('#toDateDay').val() == "DD" || $('#toDateMonth').val() == "MM" || $('#toDateYear').val() == "YYYY") {
        $('#stackHolderErrorFromdate').hide();
        $('#stackHolderErrorTodate').show();
        $('#stackHolderErrorTodate').html("Please select a Valid To date !");
    }
    else if ($('#toDateDay').val() == "DD" && $('#toDateMonth').val() == "MM" && $('#toDateYear').val() == "YYYY") {
        $('#stackHolderErrorFromdate').hide();
        $('#stackHolderErrorTodate').show();
        $('#stackHolderErrorTodate').html("Please select a Valid To date !");
    }

    else if (fromdateTime == false) {
        $('#stackHolderErrorTodate').hide();
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorFromdate').html("Please select a Valid From date !");
    }
    else if (todateTime == false) {
        $('#stackHolderErrorFromdate').hide();
        $('#stackHolderErrorTodate').show();
        $('#stackHolderErrorTodate').html("Please select a Valid To date !");
    }
    else if (dateCompare == false) {
        $('#stackHolderErrorFromdate').show();
        $('#stackHolderErrorFromdate').html("To Date should be greater than equal to From Date !");
    }
    else {
        $('#stackHolderErrorFromdate').hide();
        $('#stackHolderErrorTodate').hide();
        var allVals = [];
        $('#customersAgentsOptions :checked').each(function () {
            allVals.push($(this).val());
        });
        var type = $('#reportsddl').val();
        var agentId = $('#agentDropdown').val();
        var fromdt = $('#fromDateDay').val() + "/" + $('#fromDateMonth').val() + "/" + $('#fromDateYear').val();
        var todt = $('#toDateDay').val() + "/" + $('#toDateMonth').val() + "/" + $('#toDateYear').val();
        if (type == "agent") {

            //var allagent = $('#allagent').val();
            // var className = $('#allagent:checked').val();;
            //if (className == "all_agents_checkbox")
            //{
            //}
            var allvalues = {
                agentId: $('#agentDropdown').val(),
                status: allVals,
                fromdt: $('#fromDateDay').val() + "/" + $('#fromDateMonth').val() + "/" + $('#fromDateYear').val(),
                todt: $('#toDateDay').val() + "/" + $('#toDateMonth').val() + "/" + $('#toDateYear').val(),
            }
            $.ajax(
        {
            cache: false,
            type: "get",
            url: "/home/GetAgentReport/?allVals=" + allVals,
            data: allvalues,
            processData: true,
            // contentType: "application/text; charset=utf-8",
            success: function (data) {
                $('#downloadAgentReport').show();
                $('#downloadAgentReport').html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
        }
        else if (type == "order-book") {
            var customerOrOrderType = $('#search_by_orderbook').val();
            var allvalues = "";
            if (customerOrOrderType == "by-customer") {
                allvalues = {
                    customerId: $('#hfForOrderBook').val(),
                    customerOrOrderType: customerOrOrderType,
                    fromdt: fromdt,
                    todt: todt,
                }
            } else if (customerOrOrderType == "by-agent") {
                allvalues = {
                    agentId: $('#agentDropdownOrderBook').val(),
                    customerOrOrderType: customerOrOrderType,
                    fromdt: fromdt,
                    todt: todt,
                }
            }

            $.ajax(
        {
            cache: false,
            type: "get",
            url: "/home/OrderBookReport/",
            data: allvalues,
            contentType: "application/text; charset=utf-8",
            success: function (data) {
                $('#downloadAgentReport').show();
                $('#downloadAgentReport').html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
        }
        else if (type == "fg") {
            var allvalues = {
                agentId: agentId,
                status: allVals,
                fromdt: fromdt,
                todt: todt,
            }
            $.ajax(
        {
            cache: false,
            type: "get",
            url: "/home/FGReport/",
            data: allvalues,
            contentType: "application/text; charset=utf-8",
            success: function (data) {
                $('#downloadAgentReport').show();
                $('#downloadAgentReport').html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
        }
        else if (type == "bd") {

            var customerOrOrderType = $('#search_by_bd').val();
            var allvalues = "";
            if (customerOrOrderType == "by-customer") {
                allvalues = {
                    customerId: $('#hfForBdreport').val(),
                    customerOrOrderType: customerOrOrderType,
                    fromdt: fromdt,
                    todt: todt,
                }
            } else if (customerOrOrderType == "by-agent") {
                allvalues = {
                    agentId: $('#agentDropdownbdReport').val(),
                    customerOrOrderType: customerOrOrderType,
                    fromdt: fromdt,
                    todt: todt,
                }
            }
            //var allvalues = {
            //    agentId: agentId,
            //    customerOrOrderType: customerOrOrderType,
            //    fromdt: fromdt,
            //    todt: todt,
            //}
            $.ajax(
        {
            cache: false,
            type: "get",
            url: "/home/BDReport/",
            data: allvalues,
            contentType: "application/text; charset=utf-8",
            success: function (data) {
                $('#downloadAgentReport').show();
                $('#downloadAgentReport').html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        });
        }

        else if (type == "sourcing") {

            var allvaluesSourcing = {

                fromdt: $('#fromDateDay').val() + "/" + $('#fromDateMonth').val() + "/" + $('#fromDateYear').val(),
                todt: $('#toDateDay').val() + "/" + $('#toDateMonth').val() + "/" + $('#toDateYear').val(),
            }
            $.ajax({
                cache: false,
                type: "GET",
                url: "/home/SourcingReportGetData/",
                data: allvaluesSourcing,
                contentType: "application/text; charset=utf-8",
                success: function (data) {
                    $('#downloadAgentReport').show();
                    $('#downloadAgentReport').html(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }

        $('html,body').animate({
            scrollTop: $('#reportResultsPanel').position().top
        }, 400);
    }
});

$('#reportResultsPanel').on('click', 'a[href="#closeReportResultsPanel"]', function () {
    $('#reportResultsPanel').hide();
});


/* -----------------------------------------------------------------------------------------------------------
 * CUSTOMERS AUTO-SEARCH
 * -------------------------------------------------------------------------------------------------------- */

var readyToClose = false,
    customerList = [
        { label: "Alesha Appelbaum", value: "alesha-appelbaum", address: "4321 Somewhere Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-9999", fax: "800-888-7777" },
        { label: "Customer A", value: "Customer A", address: "523 Anywhere Ave.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-777-5555", fax: "800-777-3333" },
        { label: "Customer B", value: "Customer B", address: "1234 Nowhere Ln.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-888-5555", fax: "800-888-3333" },
        { label: "Customer C", value: "Customer C", address: "16B Forest Rd.", city: "Funtown", state: "Motania", pin: "PIN", country: "Sri Lanka", phone: "800-542-5555", fax: "800-542-3333" }
    ];

function buildOrderBookAutoComplete() {

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
    $('#autoSearchCustomersOrderBook').autocomplete({

        // the box where the suggestion list will be displayed
        appendTo: '#autoSearchCustomerReportCustomersResults',
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
            // close the list of items once a customer is selected
            $(this).data('ui-autocomplete').close = function () {
                $('#orderBook .auto_search_results').hide();
                this._close();
            };
        },
        select: function (evt, ui) {
            var name = ui.item.label;
            var value = ui.item.value;

            BindData(name, value);
            ("#autoSearchCustomersOrderBook").append();
            $('#orderBook .auto_search_results').hide();
        },
        open: function (evt, ui) {
            $('#orderBook .auto_search_results').show();
            $('#autoSearchCustomersOrderBook').addClass('search-input-clear');
            $('#orderBook .auto_search_input_field').find('a').show();

        }
    });
} // buildOrderBookAutoComplete()

function BindData(name, value) {
    $("#autoSearchCustomerReportCustomersResults").find("#autoSearchCustomersOrderBook").val('');
    $("#autoSearchCustomersOrderBook").val(name);
    $("#hfForOrderBook").val(value);
    $('#autoSearchCustomersOrderBook').addClass('search-input-clear');
    $('#orderBook .auto_search_results').hide();
    $('#autoSearchCustomerReportCustomersResults').find('li').hide();
}

function buildBdReportAutoComplete() {
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
    $('#autoSearchCustomersBdReport').autocomplete({
        // the box where the suggestion list will be displayed
        appendTo: '#autoSearchCustomerReportBdResults',
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
                   .attr('data-value', item.value) //Binded Customer Id here
                   .append('<p class="customer_name">' + item.label + '</p>')
                   .appendTo(ul);
            };
            // close the list of items once a customer is selected
            $(this).data('ui-autocomplete').close = function () {
                $('#bdReport .auto_search_results').hide();
                this._close();
            };
        },
        select: function (evt, ui) {
            var name = ui.item.label;
            var id = ui.item.value;
            BindDataBD(name, id);
            ("#autoSearchCustomersBdReport").append();
            $('#bdReport .auto_search_results').hide();
        },
        open: function (evt, ui) {
            $('#bdReport .auto_search_results').show();
            $('#bdReportCustomersSearch').addClass('search-input-clear');
            $('#bdReport .auto_search_input_field').find('a').show();
        }
    });
} // buildBdReportAutoComplete()
function BindDataBD(name, id) {
    $("#autoSearchCustomerReportBdResults").find("#autoSearchCustomersBdReport").val('');
    $("#autoSearchCustomersBdReport").val(name);
    $("#hfForBdreport").val(id);
    $('#stakeholderReports .auto_search_results').hide();
    $('#autoSearchCustomersOrderBook').val('Search Customer').blur();
    $('#autoSearchCustomersOrderBook').removeClass('search-input-clear');
    $('#orderBook .auto_search_input_field').find('a').hide();
}

$('#orderBookCustomersSearch').on('click', 'a[href="#clearSearch"]', function () {
    readyToClose = true;

    $('#autoSearchCustomersOrderBook').autocomplete('close');
    $('#autoSearchCustomersOrderBook').autocomplete('destroy');
    $('#stakeholderReports .auto_search_results').hide();
    $('#autoSearchCustomersOrderBook').val('Search Customer').blur();
    $('#autoSearchCustomersOrderBook').removeClass('search-input-clear');
    $('#orderBook .auto_search_input_field').find('a').hide();

    buildOrderBookAutoComplete();
});
$('#bdReportCustomersSearch').on('click', 'a[href="#clearSearch"]', function () {
    readyToClose = true;

    $('#autoSearchCustomerReportBdResults').autocomplete('close');
    $('#autoSearchCustomerReportBdResults').autocomplete('destroy');
    $('#bdReport .auto_search_results').hide();
    $('#bdReportCustomersSearch').val('Search Customer').blur();
    $('#bdReportCustomersSearch').removeClass('search-input-clear');
    $('#bdReport .auto_search_input_field').find('a').hide();

    buildBdReportAutoComplete();
});

$('#autoSearchCustomersOrderBook')
.on('focus', function () {
    if ($(this).val() === 'Search Customer') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Search Customer');
    }
});
$('#autoSearchCustomersBdReport')
.on('focus', function () {
    if ($(this).val() === 'Search Customer') {
        $(this).val('');
    }
})
.on('blur', function () {
    if ($(this).val().length === 0) {
        $(this).val('Search Customer');
    }
});

$(function () {
    buildOrderBookAutoComplete();
    buildBdReportAutoComplete();
});

$('#quickView').on('click', 'a[href="#closeReportResultsPanel"]', function () {
    $("#downloadAgentReport").html('');
});



function CompareDate(fromdtChk, todtChk) {

    // if (Date.parse(fromdate) > Date.parse(todate)) {
    if (Date.parse(fromdtChk) > Date.parse(todtChk)) {
        return false;
    }
    else {
        return true;
    }
}
$(document).ready(function () {

    $('#welcomePanel').removeClass('six_icons').addClass('four_icons');
    $('#welcomePanel').find('.tickets').remove();
    $('#welcomePanel').find('.schedule').remove();
});
//Change Password Close
$('#tempbodySection').on('click', 'a[href="#close-panel"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardStackholder();
});
$('#tempbodySection').on('click', 'a[href="#PasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardStackholder();
});

$('#tempbodySection').on('click', 'a[href="#ClosePasswordSucess"]', function (event) {
    event.stopImmediatePropagation();
    $("#tempbodySection").find('#ChangePasswordinner').hide();
    $("#tempbodySection").find("#changePasswordSuccess").hide();
    RefreshDashboardStackholder();
});
$('#welcomePanel').on('click', 'a[href="#showHome"]', function (event) {
    event.stopImmediatePropagation();
    RefreshDashboardStackholder();
});

function RefreshDashboardStackholder() {
    $.ajax(
                 {
                     cache: false,
                     type: "POST",
                     url: "/Stakeholder/StackholderBacktoHome/",
                     contentType: "application/json; charset=utf-8",
                     success: function (data) {

                         $('#tempbodySection').show();
                         $('#tempbodySection').html(data);

                     },
                     error: function (xhr, ajaxOptions, thrownError) {

                     }
                 });
}