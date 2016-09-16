
function ValidateDate(dt) {
    try {
        var isValidDate = false;
        var arr1 = dt.split('/');
        var year = 0; var month = 0; var day = 0; var
        hour = 0;
        if (arr1.length == 3) {
            var arr2 = arr1[2].split(' ');
            if (arr2.length == 1) {
                var arr3 = arr2;
                try {
                    year = parseInt(arr1[2], 10);
                    month = parseInt(arr1[1], 10);
                    day = parseInt(arr1[0], 10);

                    var isValidTime = false;
                    if (hour >= 0 && hour <= 23)
                        isValidTime = true;
                    else if (hour == 24 && minute == 0)
                        isValidTime = true;

                    if (isValidTime) {
                        var isLeapYear = false;
                        if (year % 4 == 0)
                            isLeapYear = true;

                        if ((month == 4 || month == 6 || month == 9 || month == 11) && (day >= 0 && day <= 30))
                            isValidDate = true;
                        else if ((month != 2) && (day >= 0 && day <= 31))
                            isValidDate = true;

                        if (!isValidDate) {
                            if (isLeapYear) {
                                if (month == 2 && (day >= 0 && day <= 29))
                                    isValidDate = true;
                            }
                            else {
                                if (month == 2 && (day >= 0 && day <= 28))
                                    isValidDate = true;
                            }
                        }
                    }
                }
                catch (er) { isValidDate = false; }
            }

        }

        return isValidDate;
    }
    catch (err) {
        // alert('ValidateDate: ' + err);
    }
}



//For loading image
$.ajaxSetup({
    beforeSend: function () {
        $("#loading").show();
    },
    complete: function () {
        $("#loading").hide();
    }
});
 


