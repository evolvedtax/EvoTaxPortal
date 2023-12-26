"use strict";
//const { param } = require("jquery");
/*const { param } = require("jquery");*/
var COMMON = (function () {
    function COMMON() {
    }
    COMMON.printLabel = function (name) {
        console.log(name);
    };
    COMMON.getTimezone = function () {
        var TimeZoneOffset, daylight_time_offset, dst, hemisphere, jan1, jan2, june1, june2, rightNow, std_time_offset, temp;
        rightNow = new Date();
        jan1 = new Date(rightNow.getFullYear(), 0, 1, 0, 0, 0, 0);
        june1 = new Date(rightNow.getFullYear(), 6, 1, 0, 0, 0, 0);
        temp = jan1.toGMTString();
        jan2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
        temp = june1.toGMTString();
        june2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
        std_time_offset = (jan1 - jan2) / (1000 * 60 * 60);
        daylight_time_offset = (june1 - june2) / (1000 * 60 * 60);
        dst = void 0;
        if (std_time_offset === daylight_time_offset) {
            dst = "0";
        }
        else {
            hemisphere = std_time_offset - daylight_time_offset;
            if (hemisphere >= 0) {
                std_time_offset = daylight_time_offset;
            }
            dst = "1";
        }
        TimeZoneOffset = COMMON.convertToStandardFormat(std_time_offset);
        return TimeZoneOffset;
    };
    COMMON.convertToStandardFormat = function (value) {
        var display_hours, hours, mins, secs;
        hours = parseInt(value);
        value -= parseInt(value);
        value *= 60;
        mins = parseInt(value);
        value -= parseInt(value);
        value *= 60;
        secs = parseInt(value);
        display_hours = hours;
        if (hours === 0) {
            display_hours = "00";
        }
        else if (hours > 0) {
            display_hours = (hours < 10 ? "+0" + hours : "+" + hours);
        }
        else {
            display_hours = (hours > -10 ? "-0" + Math.abs(hours) : hours);
        }
        mins = (mins < 10 ? "0" + mins : mins);
        return display_hours + ":" + mins;
    };
    COMMON.doHighlight = function (bodyText, searchTerm, highlightStartTag, highlightEndTag) {
        var i, lcBodyText, lcSearchTerm, newText;
        if ((!highlightStartTag) || (!highlightEndTag)) {
            highlightStartTag = "<font style='color:blue; background-color:yellow;'>";
            highlightEndTag = "</font>";
        }
        newText = "";
        i = -1;
        lcSearchTerm = searchTerm.toLowerCase();
        lcBodyText = bodyText.toLowerCase();
        while (bodyText.length > 0) {
            i = lcBodyText.indexOf(lcSearchTerm, i + 1);
            if (i < 0) {
                newText += bodyText;
                bodyText = "";
            }
            else {
                if (bodyText.lastIndexOf(">", i) >= bodyText.lastIndexOf("<", i)) {
                    if (lcBodyText.lastIndexOf("/script>", i) >= lcBodyText.lastIndexOf("<script", i)) {
                        newText += bodyText.substring(0, i) + highlightStartTag + bodyText.substr(i, searchTerm.length) + highlightEndTag;
                        bodyText = bodyText.substr(i + searchTerm.length);
                        lcBodyText = bodyText.toLowerCase();
                        i = -1;
                    }
                }
            }
        }
        return newText;
    };
    COMMON.split = function (val) {
        return val.split(/,\s*/);
    };
    COMMON.extractLast = function (term) {
        return COMMON.split(term).pop();
    };
    COMMON.doAjaxPOSTRedirect = function (urlAction) {
        $.ajax({
            type: "POST",
            url: urlAction,
            data: {},
            datatype: "JSON",
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (response) {
                if (response.ok) {
                    window.location = response.url;
                }
                else {
                    window.alert(response.message);
                }
            }
        });
    };
    COMMON.doAjaxPostJSON = function (actionUrl, Params, target) {
        if (target.indexOf('#') < 0) {
            target = $('#' + target);
        }
        $.ajax({
            type: "POST",
            url: actionUrl,
            data: Params,
            async: false,
            success: function (response) {
                console.log(response);
                if ((target != null) && target !== "") {
                    $(target).html(response);
                }
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
            }
        });
    };
    COMMON.displayError = function (message, status) {
        console.log("An Error Occured:" + message);
        return false;
    };
    COMMON.notification = function (type, message) {
        if (type == 1) {
            iziToast.success({
                title: 'Success',
                position: 'topRight',
                message: message,
            });
        } else if (type == 2) {
            iziToast.error({
                title: 'Error',
                position: 'topRight',
                message: message,
            });
        } else if (type == 3) {
            iziToast.warning({
                title: 'Warning',
                position: 'topRight',
                message: message,
            });
        }
    }
    COMMON.querystring = function (key) {
        var m, r, re;
        re = new RegExp("(?:\\?|&)" + key + "=(.*?)(?=&|$)", "gi");
        r = [];
        m = void 0;
        while ((m = re.exec(document.location.search)) != null) {
            r.push(m[1]);
        }
        return r;
    };
    COMMON.doAjaxGet = function (Title, RequestedURL, Params, Target) {
        location.href = RequestedURL;
        $.ajax({
            type: "GET",
            url: RequestedURL,
            data: Params,
            success: function (response) {
                if (Target != null) {
                    $(Target).html(response);
                }
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
            }
        });
        return false;
    };
    COMMON.doAjaxPostWithJSONResponse = function (url, params) {
        var data = params;
        var data = {
            entityId: params.entityId,
            isLocked: params.isLocked
        };
        var jsonResponse = null;
        $.ajax({
            type: "POST",
            url: url,
            data: params,
            async: false,
            success: function (response) {
                $('.loading').hide();
                COMMON.notification(response.type, response.message);
                $('#changeEntity').change();
                jsonResponse = response;
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                $('.loading').hide();

                COMMON.notification(2, "There's some technical error.");
                jsonResponse = null;
            }
        });
        return jsonResponse;
    };
    COMMON.doSendEmailAjaxPostWithJSONResponse = function (url, params) {
        $('.loading').show();
        var jsonResponse = null;
        $.ajax({
            type: "POST",
            url: url,
            data: params,
            async: true,
            success: function (response) {
                $('.loading').hide();
                COMMON.notification(response.type, response.message);
                jsonResponse = response;
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                $('.loading').hide();

                COMMON.notification(2, "There's some technical error.");
                jsonResponse = null;
            }
        });
        return jsonResponse;
    };
    COMMON.doAjaxGetWithJSONResponse = function (url, params) {
        var response = null;
        $.ajax({
            type: "GET",
            url: url,
            data: params,
            async: false,
            success: function (data) {
                response = data;
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                response = null;
            }
        });
        return response;
    };
    COMMON.ajaxfailure = function (response) {
        debugger
        COMMON.notification(2, 'Unable to process your request.');
    };

    COMMON.uploadFile = function (event, url, obj) {
        var file = event.target.files[0]; // Get the selected file
        if (file == undefined) {
            return;
        }
        // Create a new XMLHttpRequest object
        var xhr = new XMLHttpRequest();

        // Set up the AJAX request
        xhr.open('POST', url, true);

        // Create a FormData object and append the file to it
        var formData = new FormData();
        formData.append('file', file);
        formData.append('EntityId', obj.entityId); // Here obj is Entity Id
        formData.append('entityName', obj.entityName); // Here obj is Entity Id
        formData.append('InstituteId', obj); // Here obj is Institute Id
        // Configure the XMLHttpRequest object
        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable) {
                var percentComplete = (e.loaded / e.total) * 100;
                var progressBar = document.getElementById('progress-bar');
                progressBar.style.width = percentComplete + '%';
                progressBar.innerHTML = Math.round(percentComplete) + '%';
            }
        };

        xhr.onload = function () {
            if (xhr.status === 200) {
                // File upload successful
                console.log('File uploaded successfully');
                // Check the response from the server
                var response = JSON.parse(xhr.responseText);
                if (response.ErrorStatus == true) {
                    COMMON.notification(2, 'There is some issue in excel file. Kindly recheck data and upload it again.');
                    console.log(response);
                    return;
                }
                if (response.Status) {
                    // Show the progress bar
                    var progressDiv = document.getElementById('progress');
                    progressDiv.style.display = 'block';
                    if (response.Param == "Client" && response.Message.length > 0) {
                        $('#dupplication-ibox').prop('hidden', false);
                        //COMMON.AlertSuccessMessage('Duplication of Data', 'The record in row (' + response.Message + ') already exist', 'info')
                        // Clear the existing table rows
                        let dataTable = $('#entity-table-dupplication').DataTable();
                        dataTable.clear().draw();

                        // Iterate over the response data and populate the table
                        $.each(response.Message, function (index, item) {
                            let ClientStatus = '';
                            if (item.ClientStatus == '@AppConstants.ClientStatusFormSubmitted') {
                                ClientStatus = 'disabled';
                            }
                            let status = '';
                            if (item.ClientStatus == '@AppConstants.ClientStatusActive') {
                                status = '<span class="fa fa-unlock"></span>';
                            } else {
                                status = '<span class="fa fa-lock"></span>';
                            }
                            let row = [
                                '<input type="checkbox" class="rowCheckbox" value="' + item.ClientId + '"' + ClientStatus + '>',
                                item.PartnerName1 + ' ' + item.PartnerName2,
                                item.Address1 + ' ' + item.Address2,
                                item.City,
                                item.State,
                                item.Province,
                                item.Zip,
                                item.Country,
                                item.PhoneNumber,
                                item.ClientEmailId,
                                item.StatusName,
                                COMMON.GetDateMMddyyyFormat(item.ClientStatusDate),
                                'Form' + ' ' + item.FormName,
                                '<a href="~/' + item.FileName + '">' + item.FileName + '</a>',
                                status
                            ];
                            dataTable.rows.add([row]);
                        });

                        // Draw the updated DataTable
                        dataTable.draw();
                        //alert('here');
                    }
                   if (response.Param == "Entity" && response.Message.length > 0) {
            
                        $('#dupplication-ibox').prop('hidden', false);
                        //COMMON.AlertSuccessMessage('Duplication of Data', 'The record in row (' + response.Message + ') already exist', 'info')
                        // Clear the existing table rows
                        let dataTable = $('#entity-table-dupplication').DataTable();
                        dataTable.clear().draw();

                        // Iterate over the response data and populate the table
                        $.each(response.Message, function (index, item) {
                            debugger
                            let row = [
                                item.EntityName,
                                item.Ein,
                                COMMON.GetDateMMddyyyFormat(item.EntityRegistrationDate),
                                item.Address1 + ' ' + item.Address2,
                                item.City,
                                item.State,
                                item.Province,
                                item.Zip,
                                item.Country,
                            ];
                            dataTable.rows.add([row]);
                        });

                        // Draw the updated DataTable
                        dataTable.draw();
                    }
                    console.log('Upload completed successfully');
                
                }
                else {
                 
                 // COMMON.notification(2, 'There is some issue in excel file. Kindly recheck data and upload it again.');
 
                    console.log(response);
                    // Continue showing the progress bar until completion record is received
                    location.reload();
                }
            } else {
                // File upload failed
                console.log('File upload failed');
             
            }
        };

        // Send the AJAX request
        xhr.send(formData);


    }
    COMMON.doAjaxToDownloadFile = function (url, params, fileName) {
        $.ajax({
            url: url,
            data: params,
            type: 'GET',
            xhrFields: {
                responseType: 'arraybuffer'
            },
            success: function (data) {
                var a = document.createElement('a');
                var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var url = window.URL.createObjectURL(blob);
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                COMMON.notification(2, "There's some error while downloading the file. Please try again.");
            }
        });
    };
    COMMON.doAjaxFormPostWithJSONResponse = function (formid, Target) {
        var $form = $(formid);
        var response = null;
        $.ajax({
            type: "POST",
            url: $form.attr("action"),
            data: $form.serialize(),
            async: false,
            success: function (data) {
                response = data;
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                response = null;
            }
        });
        return response;
    };
    COMMON.doAjaxFormPostHTMLResponse = function (formId, Target) {
        var $form = $(formId);
        var htmlResponse = null;
        if (Target !== undefined && Target !== null && Target.indexOf('#') < 0) {
            Target = $('#' + Target);
        }
        $.ajax({
            type: "POST",
            url: $form.attr("action"),
            data: $form.serialize(),
            async: Target !== undefined && Target !== null ? true : false,
            success: function (response) {
                if (Target !== null && Target !== undefined) {
                    $(Target).html(response);
                } else {
                    htmlResponse = response;
                }
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                htmlResponse = null;
            }
        });
        return htmlResponse;
    };
    COMMON.doAjaxPostHTMLResponse = function (url, params) {
        var htmlResponse = null;
        $.ajax({
            type: "POST",
            url: url,
            data: params,
            //async: target !== undefined && target !== null ? true : false,
            async: false,
            success: function (response) {
                htmlResponse = response;
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
                htmlResponse = null;
            }
        });
        return htmlResponse;
    };
    COMMON.doFormPost = function (formId) {
        var $form;
        $form = $(formId);
        $.ajax({
            type: "POST",
            url: $form.attr("action"),
            data: $form.serialize(),
            success: function (response) {
                $form.window.close();
            },
            error: function (xhr, status, error) {
                COMMON.displayError(xhr.responseText, xhr.status);
            }
        });
        return false;
    };
    COMMON.getCookieByName = function (name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
    };
    COMMON.compareValues = function (value1, value2) {
        return value1 === value2;
    };
    COMMON.dataTableInitialized = function () {
        $('.dataTables-example').DataTable({
            pageLength: 25,
            dom: '<"html5buttons"B>frtip',
            buttons: [
                { extend: 'excel', title: 'ExampleFile', text: 'Export to Excel', className: 'btn-excel' },
                { extend: 'pdf', title: 'ExampleFile', text: 'Export to PDF', className: 'btn-pdf' },

            ],
            initComplete: function () {
                $('.dataTables_length').hide(); // Hide the original page length dropdown
            }
        });
    };
    COMMON.dataTableInitializedDashboard = function () {
        $('.dataTables-dashboard').DataTable({
            pageLength: 10,
            responsive: true,
        });
    };
    COMMON.formatDateToUKFormat = function (date) {
        var now = new Date();
        var day = ("0" + now.getDate()).slice(-2);
        var month = ("0" + (now.getMonth() + 1)).slice(-2);
        var today = now.getFullYear() + "-" + (month) + "-" + (day);
        $('#datePicker').val(today);
    };
    COMMON.confirmAlertWitMessages = function (confirmTitle, confirmMessage, confirmType, confirmBtnText, canceBtnTxt, url, params, gridId) {
        var confirmed = false;

        swal({
            title: '',//confirmTitle, //"Are you sure to delete this record?",
            text: confirmTitle, // "Please check before deleting the record!",
            type: confirmType, //"warning",
            showCancelButton: true,
            confirmButtonColor: "#1ab394",
            confirmButtonText: confirmBtnText,
            cancelButtonColor: "red",
            cancelButtonText: canceBtnTxt
        },
            function (isConfirm) {
                if (isConfirm) {
                    var jsonResponse = COMMON.doAjaxPostWithJSONResponse(url, params);
                    if (jsonResponse.Deleted === true) {
                        $.notify('Deleted successful', { globalPosition: 'top center', className: 'success' });
                    }
                    confirmed = true;
                } else {
                    swal("Cancelled", "You have cancelled delete operation!", "error");
                    return false;
                }
            });
        return confirmed;
    };

    COMMON.confirmAlert = function (message, url, params, gridId) {
        var confirmed = false;

        swal({
            title: "",
            text: "Are you sure you want to delete this record?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#1ab394",
            confirmButtonText: "Delete",
            cancelButtonText: "Cancel"
        },
            function (isConfirm) {
                if (isConfirm) {
                    debugger
                    $('.loading').show();
                    var jsonResponse = COMMON.doAjaxPostWithJSONResponse(url, params);
                    if (jsonResponse.Status === true) {
                        COMMON.notification(1, "Record deleted")
                        //COMMON.dataTableInitialized();
                        setTimeout(function () {
                            window.location.reload();
                        }, 5000);
                    } else if (jsonResponse.Status === false) {
                        //COMMON.notification(2, "Something went wrong")
                        //COMMON.AlertSuccessMessage(jsonResponse.Message,'Warning','warning');
                        COMMON.notification(3, jsonResponse.Message);
                    }
                    confirmed = true;
                } else {
                    swal("Cancelled", "You have cancelled delete operation!", "error");
                    return false;
                }
            });
        return confirmed;
    };
    COMMON.confirmAlertActive = function (message, url, params, gridId) {
        var confirmed = false;

        swal({
            title: "",
            text: "Are you sure you want to " + message + " this record?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#1ab394",
            confirmButtonText: message,
            cancelButtonText: "Cancel"
        },
            function (isConfirm) {
                if (isConfirm) {
                    debugger
                    var jsonResponse = COMMON.doAjaxPostWithJSONResponse(url, params);
                    if (jsonResponse.Status === true) {
                        COMMON.notification(1, jsonResponse.Message)
                        //COMMON.dataTableInitialized();
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);
                    } else if (jsonResponse.Status !== null) {
                        COMMON.notification(jsonResponse.deleteResponse.NotifyType, jsonResponse.deleteResponse.Response)
                    }
                    confirmed = true;
                } else {
                    swal("Cancelled", "You have cancelled update operation!", "error");
                    return false;
                }
            });
        return confirmed;
    };
    COMMON.confirmAlertActiveAll = function (message, url, params, gridId) {
        var confirmed = false;

        swal({
            title: "",
            text: "Are you sure you want to " + message + " these records?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#1ab394",
            confirmButtonText: message,
            cancelButtonText: "Cancel"
        },
            function (isConfirm) {
                if (isConfirm) {
                    debugger
                    var jsonResponse = COMMON.doAjaxPostWithJSONResponse(url, params);
                    if (jsonResponse.Status === true) {
                        COMMON.notification(1, jsonResponse.Message)
                        //COMMON.dataTableInitialized();
                        setTimeout(function () {
                            window.location.reload();
                        }, 3000);
                    } else if (jsonResponse.Status !== null) {
                        COMMON.notification(jsonResponse.deleteResponse.NotifyType, jsonResponse.deleteResponse.Response)
                    }
                    confirmed = true;
                } else {
                    swal("Cancelled", "You have cancelled update operation!", "error");
                    return false;
                }
            });
        return confirmed;
    };
    COMMON.AlertSuccessMessage = function (message, tagLine, status) {
        swal(
            message,
            tagLine,
            status
        )
    };
    COMMON.alert = function (message) {
        window.alert(message);
    };
    COMMON.is = {
        array: function (a) { return Array.isArray(a); },
        object: function (a) { return stringContains(Object.prototype.toString.call(a), 'Object'); },
        pth: function (a) { return is.obj(a) && a.hasOwnProperty('totalLength'); },
        svg: function (a) { return a instanceof SVGElement; },
        input: function (a) { return a instanceof HTMLInputElement; },
        dom: function (a) { return a.nodeType || is.svg(a); },
        string: function (a) { return typeof a === 'string'; },
        function: function (a) { return typeof a === 'function'; },
        undefined: function (a) { return typeof a === 'undefined'; },
        null: function (a) { return is.und(a) || a === null; },
        hex: function (a) { return /(^#[0-9A-F]{6}$)|(^#[0-9A-F]{3}$)/i.test(a); },
        rgb: function (a) { return /^rgb/.test(a); },
        hsl: function (a) { return /^hsl/.test(a); },
        col: function (a) { return (is.hex(a) || is.rgb(a) || is.hsl(a)); },
        key: function (a) { return !defaultInstanceSettings.hasOwnProperty(a) && !defaultTweenSettings.hasOwnProperty(a) && a !== 'targets' && a !== 'keyframes'; },
        email: function (a) { return /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/.test(a); },
        html: function (a) {
            var h = document.createElement('div');
            h.innerHTML = a;
            for (var c = h.childNodes, i = c.length; i--;) {
                if (c[i].nodeType == 1) return true;
            }
            return false;
        },
        json: function (a) {
            try { return JSON.parse(a) !== this.undefined || JSON.parse(a) !== this.null } catch { return false }
        },
        specialcharacter: function (a) { return !(/^[A-Za-z0-9 ]+$/.test(a)); }
    };
    return COMMON;
}()); /*CLASS COMMON End*/
(function () {
    Array.prototype.moveUp = function (value, by_) {
        var index, newPos;
        index = this.indexOf(value);
        newPos = index - (by_ || 1);
        if (index === -1) {
            throw new Error("Element not found in array");
        }
        if (newPos < 0) {
            newPos = 0;
        }
        this.splice(index, 1);
        this.splice(newPos, 0, value);
    };
    Array.prototype.moveDown = function (value, by_) {
        var index, newPos;
        index = this.indexOf(value);
        newPos = index + (by_ || 1);
        if (index === -1) {
            throw new Error("Element not found in array");
        }
        if (newPos >= this.length) {
            newPos = this.length;
        }
        this.splice(index, 1);
        this.splice(newPos, 0, value);
    };
    Date.prototype.format = function (format) {
        var hours, k, month, monthName, o, ttime;
        hours = this.getHours();
        ttime = "AM";
        if (hours === 12) {
            ttime = "PM";
        }
        else if (hours > 12) {
            hours = hours - 12;
            ttime = "PM";
        }
        month = this.getMonth() + 1;
        monthName = "";
        switch (month) {
            case 1:
                monthName = "Jan";
                break;
            case 2:
                monthName = "Feb";
                break;
            case 3:
                monthName = "Mar";
                break;
            case 4:
                monthName = "Apr";
                break;
            case 5:
                monthName = "May";
                break;
            case 6:
                monthName = "Jun";
                break;
            case 7:
                monthName = "Jul";
                break;
            case 8:
                monthName = "Aug";
                break;
            case 9:
                monthName = "Sep";
                break;
            case 10:
                monthName = "Oct";
                break;
            case 11:
                monthName = "Nov";
                break;
            case 12:
                monthName = "Dec";
        }
        o = {
            "d+": this.getDate(),
            "h+": hours,
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            S: this.getMilliseconds()
        };
        if (RegExp("(t+)").test(format)) {
            format = format.replace(RegExp.$1, ttime);
        }
        if (RegExp("(y+)").test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        if (RegExp("(M+)").test(format)) {
            if (RegExp.$1.length === 3) {
                format = format.replace(RegExp.$1, monthName);
            }
            else {
                format = format.replace(RegExp.$1, month);
            }
        }
        for (k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, (RegExp.$1.length === 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length)));
            }
        }
        return format;
    };
}());
(function ($) {
    $.fn.serializeObject = function () {
        var a, o;
        o = {};
        a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || "");
            }
            else {
                o[this.name] = this.value || "";
            }
        });
        return o;
    };
    $.fn.localTimeFromUTC = function (format) {
        return this.each(function () {
            var currentDate, givenDate, hours, localDateString, min, offset, tagText, ttime;
            tagText = $(this).html();
            if (tagText === "") {
                return;
            }
            givenDate = new Date(tagText.split("+", 1)[0]);
            if (givenDate === "NaN") {
                return;
            }
            offset = -(givenDate.getTimezoneOffset() / 60.0);
            hours = givenDate.getHours();
            hours += offset;
            givenDate.setHours(hours);
            currentDate = new Date();
            localDateString = "";
            if (currentDate.getFullYear() === givenDate.getFullYear() && currentDate.getMonth() === givenDate.getMonth() && currentDate.getDay() === givenDate.getDay()) {
                ttime = "AM";
                if (hours === 12) {
                    ttime = "PM";
                }
                else if (hours > 12) {
                    hours = hours - 12;
                    ttime = "PM";
                }
                min = givenDate.getMinutes();
                localDateString = (hours < 10 ? "0" + hours : hours) + ":" + (min < 10 ? "0" + min : min) + " " + ttime;
            }
            else if (currentDate.getFullYear() === givenDate.getFullYear() && $(this).hasClass("ConvertUtcToLocal")) {
                localDateString = givenDate.format("MMM dd");
            }
            else {
                localDateString = givenDate.format(format);
            }
            $(this).html(localDateString);
            $(this).removeClass("ConvertUtcToLocal");
            $(this).removeClass("ConvertUtcToLocalDateTime");
        });
    };
    $.fn.wrapInTag = function (opts) {
        var o;
        o = $.extend({
            words: [],
            tag: "<strong>"
        }, opts);
        return this.each(function () {
            var html, i, len, re;
            html = $(this).html();
            i = 0;
            len = o.words.length;
            while (i < len) {
                re = new RegExp(o.words[i], "gi");
                html = html.replace(re, o.tag + "$&" + o.tag.replace("<", "</"));
                i++;
            }
            $(this).html(html);
        });
    };
})(jQuery);
$(function () {
    $("a.buttonDelete").on("click", function (event) {
        var $url, Message, currentTr;
        event.preventDefault();
        $url = $(this).attr("href");
        $title = $(this).attr("title");
        currentTr = $(this).closest("tr");
        Message = "Do you want to delete?";
        if (confirm(Message)) {
            $.ajax({
                type: "POST",
                url: $url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 30000,
                success: function (response) {
                    if (currentTr != null) {
                        currentTr.remove();
                    }
                },
                beforeSend: function () {
                },
                complete: function () { },
                error: function (xhr, status, error) {
                    COMMON.displayError(xhr.responseText, xhr.status);
                }
            });
        }
        return false;
    });
    $("a.doAjaxGet").on({
        click: function (event) {
            var $title, $url;
            event.preventDefault();
            $(this).attr("target", "_self");
            $url = $(this).attr("href");
            $title = $(this).attr("title");
            COMMON.doAjaxGet($title, $url, "");
            return false;
        }
    });

});
COMMON.GenerateOtherTextbox = function (obj, param, counter) {
    let element =
        "<div class='col-sm-3 form-group other'> <lable class='col-form-label'>Other</label> <input class='form-control' name='ViolenceTypesCategories[" + counter + "]." + param.replace(/ /g, '') + "Other'/> </div>";
    if (obj.checked == true) {
        $(obj).parent().after(element);
    } else {
        $(obj).parent().parent().find('.other').remove();
    }
};
COMMON.GenerateOtherTextboxForDropdown = function (obj, param, colClass) {
    let element =
        "<div class='" + colClass + " form-group other " + param + "Other'> <label class='col-form-label'>" + $(obj).parent().find('label').text() + " Other (Please Specify)</label> <input class='form-control' name='" + param + "Other'/> </div>";
    if ($(obj).children("option:selected").text() == 'Other') {
        $(obj).parent().append().after(element);
    } else {
        $(obj).parent().parent().find('.' + param + 'Other').remove();
    }
};
COMMON.GetAge = function (dob) {
    let age = ((new Date() - new Date(dob)) / (31556952000));
    return age.toFixed();
};
COMMON.CompareDates = function (date1, date2) {
    if (new Date(date1) <= new Date(date2)) {
        return true;
    } else {
        return false;
    }
};
COMMON.GetTotalCount = function (event, className) {
    if (event.keyCode != 13) {
        var sum = 0;
        $(className).each(function () {
            if ($(this).val() != '' && !isNaN($(this).val())) {
                sum += parseInt($(this).val());
            }
        });
        return sum
    }
}
COMMON.GetDateMMddyyyFormat = function (dateString) {
    if (dateString) {
        const date = new Date(dateString);
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const year = date.getFullYear();

        return month + '-' + day + '-' + year;
    }

    return '';
}