
$(document).ready(function () {

    retryFileList = [];

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

        $('.ready').hide();
        $('.readyProgress').show();
    });

    $('#fileupload').bind('fileuploaddone', function (e, data) {
        alert('ACABOU!');
    });
}

function Cenas(sender, fileToAdd) {

    //var fileName = $('#fileupload').find(sender).closest('tr').find('.name').first().next().val();
    //console.log(fileName);


    //var indexOfFile = 0;

    //for (var i = 0; i < retryFileList.length; i++) {
    //    var file = retryFileList[i];
    //    if (file.name == fileName){
    //        indexOfFile = i;
    //        break;
    //    }
    //}

    //$('#fileupload').fileupload('add', { files: retryFileList[indexOfFile] });
    $('#fileupload').fileupload('add', {
        files: fileToAdd,
        formData: $(sender).closest('tr').find(':input').serializeArray().concat([{ name: 'IsSignature', value: 'true' }]),
        IsSignature: true,
        OriginalFileName: $(sender).closest('tr').children('td.name').text()
    });

    //retryFileList.splice(indexOfFile, 1);

    $('#fileupload').find(sender).closest('tr').remove();

}

function appendFile(file) {

    retryFileList.push(file);

}