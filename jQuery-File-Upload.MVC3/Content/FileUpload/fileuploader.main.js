
$(document).ready(function () {

    InitializeFileUploader();

});

function GetChunkSize() {
    return $("#fileupload").fileupload('option').maxChunkSize;
}

function ToggleSignatureFiles() {
    $('.sign-col').toggle();
}

function AddToUploadList(sender) {

    //set certificateFileName
    $(sender).next().attr('value', $(sender).prop("files")[0].name);

    //disable file selector
    $(sender).attr('disabled', 'disabled');

    //send signature file
    $('#fileupload').fileupload('send', { files: $(sender).prop("files") });

}

function InitializeFileUploader() {

    'use strict';
    $('#fileupload').fileupload();

    $('#fileupload').fileupload('option', {
        maxFileSize: 9999999999,
        maxChunkSize: 1024 * 2048, //512 KB
        autoUpload: false,
        maxNumberOfFiles: 10
    });

    //Change form context to row instead of entire form to submit event
    $('#fileupload').bind('fileuploadsubmit', function (e, data) {
        var inputs = data.context.find(':input');
        data.formData = inputs.serializeArray();
    });
    
}