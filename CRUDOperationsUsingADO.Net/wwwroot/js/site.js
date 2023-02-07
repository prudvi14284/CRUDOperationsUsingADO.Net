//Javascript code

showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            
            //// to make popup draggable
            //$('.modal-dialog').draggable({
            //    handle: ".modal-header"
            //});
        }
    })
    $('#form-modal').modal({
        backdrop: 'static', // This is disable for click outside event
        keyword: true      // This is for keybord event
    });
   
}

//This is for colse the modal pop-up
function CloseModalPopup() {
    $('#form-modal').modal('hide');
}


//Ajax Post Action
jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (response) {
                window.location.href = response.redirectToUrl;
            },
            success: function (res) {
                
                if (res.isValid) {
                    window.location.href = res.redirectToUrl;
                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

//Ajax Delete Post
jQueryAjaxDelete = form => {
    if (confirm('Are you sure to delete this record ?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    window.location.href = res.redirectToUrl;
                },
                error: function (err) {
                    console.log(err)
                }
            })
        } catch (ex) {
            console.log(ex)
        }
    }

    //prevent default form submit event
    return false;
}


