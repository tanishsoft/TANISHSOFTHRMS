function ConfirmBoxClickAlerts() {
    $.alert({
        title: 'Hey!',
        content: 'This is a simple alert to the user. <br> with some <strong>HTML</strong> <em>contents</em>',
        confirmButton: 'Okay',
        confirmButtonClass: 'btn-primary',
        icon: 'fa fa-info',
        animation: 'zoom',
        confirm: function () {
            $.alert('Okay action clicked.');
        }
    });
}

function ConfirmBoxClickConfirmation() {
    $.confirm({
        title: 'A secure action',
        content: 'Its smooth to do multiple confirms at a time. <br> Click proceed for another modal',
        confirmButton: 'Proceed',
        confirmButtonClass: 'btn-info',
        icon: 'fa fa-question-circle',
        animation: 'scale',
        animationClose: 'top',
        opacity: 0.5,
        confirm: function () {
            $.confirm({
                title: 'This maybe critical',
                content: 'Critical actions can have multiple confirmations like this one.',
                confirmButton: 'Yes, sure!',
                icon: 'fa fa-warning',
                confirmButtonClass: 'btn-warning',
                animation: 'zoom',
                confirm: function () {
                    $.alert('A very critical action triggered!');
                }
            });
        }
    });
}


function ConfirmBoxClickActLikePrompt() {
    $.confirm({
        title: 'A simple form',
        content: 'url:form.txt',
        confirm: function () {
            var input = this.$b.find('input#input-name');
            var errorText = this.$b.find('.text-danger');
            if (input.val() == '') {
                errorText.show();
                return false;
            } else {
                $.alert('Hello ' + input.val());
            }
        }
    });
}

function ConfirmBoxClickBackGroundDismiss() {
    $.alert({
        title: 'Background dismiss',
        content: 'By default the user is not allowed to click outside the modal. Click outside the modal to close.',
        confirmButton: 'okay',
        confirmButtonClass: 'btn-info',
        animation: 'bottom',
        icon: 'fa fa-check',
        backgroundDismiss: true
    });
}

function ConfirmBoxClickUsingAsDialogModels() {
    $.confirm({
        title: 'Title here',
        content: 'Need a popup modal?, no problem!<br>disable the buttons, and get a full functional modal. <br><h3>HTML inside modals</h3><h4><strong>centered on the screen</strong></h4><h5><em>Like a boss</em></h5>' +
        '<button type="button">Interactive too</button>',
        confirmButton: false,
        cancelButton: false,
        animation: 'scale',
        onOpen: function () {
            var that = this;
            this.$content.find('button').click(function () {
                that.$content.html('As simple as that !');
            })
        }
    });
}


function ConfirmBoxClickAsynchronousContent() {
    $.confirm({
        title: 'Asynchronous content',
        content: 'url:table.html',
        animation: 'top',
        columnClass: 'col-md-6 col-md-offset-3',
        closeAnimation: 'bottom',
        backgroundDismiss: true,
    });
}

function ConfirmBoxClickAutoClose() {
    $.confirm({
        title: 'Auto close',
        content: 'Some actions maybe critical, prevent it with the Auto close feature. This dialog will automatically trigger cancel after the timer runs out.',
        autoClose: 'cancel|10000',
        confirmButtonClass: 'btn-danger',
        confirmButton: 'Delete Ben\'s account',
        cancelButton: 'Close',
        confirm: function () {
            $.alert('You deleted Ben\'s account!');
        },
        cancel: function () {
            $.alert('Ben just got saved!');
        }
    });
}

function ConfirmBoxClickKeyStrokes() {
    $.confirm({
        title: 'Keystrokes Enabled!',
        keyboardEnabled: true,
        content: 'Press ENTER to confirm, ESC to cancel.<br> Use <code>keyboardEnabled:true</code> to turn it on.',
        backgroundDismiss: true,
        confirm: function () {
            $.alert('confirm triggered!');
        },
        cancel: function () {
            $.alert('cancel triggered!');
        }
    });
}

function ConfirmBoxClickAlignment() {
    $.confirm({
        title: 'This is smoooth',
        content: '<p>You can add content and not worry about the alignment. The goal is to make a Interactive dialog!.</p>' +
        '<button type="button">Click me to add content</button> <span> <br></span> ',
        confirmButtonClass: 'btn-primary',
        animation: 'zoom',
        onOpen: function () {
            var that = this;
            this.$content.find('button').click(function () {
                that.$content.find('span').append('<br>This is awesome!!!!');
            });
        },
        confirmButton: 'Say "Wowww"',
        confirm: function () {
            this.$content.find('span').append('<br>Wowww');
            return false; // prevent dialog from closing.
        }
    });

}

function ConfirmBoxClickImages() {
    window.b = $.confirm({
        title: 'Adding images',
        content: 'Images from flickr <br><img src="https://c2.staticflickr.com/4/3891/14354989289_2eec0ba724_b.jpg">',
        confirmButtonClass: 'btn-primary',
        animation: 'zoom',
        animationClose: 'top',
        confirmButton: 'Add more',
        confirm: function () {
            this.$content.append('<img src="https://c2.staticflickr.com/6/5248/5240523362_8d6d315391_b.jpg">');
            return false; // prevent dialog from closing.
        }
    });
}

function ConfirmBoxClickAnimations() {
    $.alert({
        title: 'Animations',
        content: 'jquery-confirm provides 12 animations to choose from.',
        animation: 'rotate',
        closeAnimation: 'right',
        opacity: 0.5
    });
}

function ConfirmBoxClickExampleAlert() {
    $.alert({
        title: 'Alert!',
        content: 'Simple alert!',
        confirm: function () {
            $.alert('Confirmed!');
        }
    });
}

function ConfirmBoxClickExampleConfirm() {
    $.confirm({
        title: 'Confirm!',
        content: 'Simple confirm!',
        confirm: function () {
            $.alert('Confirmed!');
        },
        cancel: function () {
            $.alert('Canceled!');
        }
    });
}

function ConfirmBoxClickGotoTwitter() {
    $.confirm({
        content: "This will take you to my twitter <br> You can access the clicked element by <code>this.$target</code>",
    });
}

function ConfirmBoxClickExampleDialog2() {
    $.dialog({
        title: 'Text content!',
        content: 'Simple modal!'
    });
}


function ConfirmBoxClickClickMe() {
    $.confirm({
        confirmButton: 'Yes i agree',
        cancelButton: 'NO never !'
    })
}

function ConfirmBoxClickInfoAndDanger() {
    $.confirm({
        confirmButtonClass: 'btn-info',
        cancelButtonClass: 'btn-danger'
    });
}

function ConfirmBoxClickWarningAndSuccess() {
    $.confirm({
        confirmButtonClass: 'btn-warning',
        cancelButtonClass: 'btn-success'
    });
}

function ConfirmBoxClickNoTitle() {
    $.confirm({
        title: false,
        content: 'Hide the title.'
    });
}

function ConfirmBoxClickNoContent() {
    $.confirm({
        content: false
    });
}

function ConfirmBoxClickNoTitleCancelButton() {
    $.confirm({
        title: false,
        content: 'Hide the cancel button &amp; title.',
        cancelButton: false
    });
}

function ConfirmBoxClickNoTitleCancelConfirmButton() {
    $.confirm({
        title: false,
        content: 'Hide both the buttons &amp; title.',
        cancelButton: false,
        confirmButton: false
    });
}

function ConfirmBoxClickNoTitleCancelConfirmButtonNoCloseOIon() {
    $.confirm({
        title: false,
        content: 'If you hide everything, what can do you to close the modal?. <br>"backgroundDismiss" is active on this one, click outside to close.',
        cancelButton: false,
        confirmButton: false,
        backgroundDismiss: true,
        closeIcon: false
    });
}

function ConfirmBoxClickCl_md_6Cl_md_offset_3() {
    $.confirm({
        columnClass: 'col-md-6 col-md-offset-3'
    });
}

function ConfirmBoxClickcol_md_4() {
    $.confirm({
        columnClass: 'col-md-4'
    });
}

function ConfirmBoxClickcol_md_4Cl_md_offset_8() {
    $.confirm({
        columnClass: 'col-md-4 col-md-offset-8'
    });
}

function ConfirmBoxCliCo_md_12() {
    $.confirm({
        columnClass: 'col-md-12'
    });
}

function ConfirmBoxClickCol_md_4Col_md_offset_4() {
    $.confirm({
        columnClass: 'col-md-4 col-md-offset-4'
    });
}


function ConfirmBoxClickUsing_bootstrap_glyphicon() {
    $.confirm({
        icon: 'glyphicon glyphicon-heart',
        title: 'glyphicon'
    });
}

function ConfirmBoxClickUsing_font_awesome() {
    $.confirm({
        icon: 'fa fa-warning',
        title: 'font-awesome'
    });
}

function ConfirmBoxClickAnimated_font_awesome() {
    $.alert({
        icon: 'fa fa-spinner fa-spin',
        title: 'Working!',
        content: 'Sit back, we are processing your request. <br>The animated icon is provided by Font-Awesome!'
    });
}

function ConfirmBoxClickWith_closeIcon() {
    $.alert({
        closeIcon: true,
    });
}

function ConfirmBoxClickcloseIcon_Using_glyphicon() {
    $.alert({
        closeIcon: true,
        closeIconClass: 'glyphicon glyphicon-remove',
    });
}

function ConfirmBoxClickcloseIcon_Using_font_awesome() {
    $.alert({
        closeIcon: true,
        closeIconClass: 'fa fa-close'
    })
}




function ConfirmBoxClick_Animations_Right() {
    $.confirm({
        animation: 'right',
        closeAnimation: 'left',
    });
}

function ConfirmBoxClick_Animations_Left() {
    $.confirm({
        animation: 'left',
        closeAnimation: 'right',
    });
}


function ConfirmBoxClick_Animations_Bottom() {
    $.confirm({
        animation: 'bottom',
        closeAnimation: 'top'
    });
}

function ConfirmBoxClick_Animations_Top() {
    $.confirm({
        animation: 'top',
        closeAnimation: 'bottom'
    });
}

function ConfirmBoxClick_Animations_Rotate() {
    $.confirm({
        animation: 'Rotate',
        closeAnimation: 'rotate'
    });
}

function ConfirmBoxClick_Animations_None() {
    $.confirm({
        animation: 'none',
    });
}

function ConfirmBoxClick_Animations_Opacity() {
    $.confirm({
        animation: 'opacity'
    });
}

function ConfirmBoxClick_Animations_ScaleDefault() {
    $.confirm({
        animation: 'scale',
        closeAnimation: 'zoom'
    });
}

function ConfirmBoxClick_Animations_ZoomDefault() {
    $.confirm({
        animation: 'zoom',
        closeAnimation: 'scale'
    });
}

function ConfirmBoxClick_Animations_ScaleY() {
    $.confirm({
        animation: 'scaleY',
        closeAnimation: 'scaleX'
    })
}

function ConfirmBoxClick_Animations_scaleX() {
    $.confirm({
        animation: 'scaleX',
        closeAnimation: 'scaleY'
    })
}

function ConfirmBoxClick_Animations_RotateY() {
    $.confirm({
        animation: 'rotateY',
        closeAnimation: 'rotateYR'
    });
}

function ConfirmBoxClick_Animations_RotateYR() {
    $.confirm({
        animation: 'rotateYR',
        closeAnimation: 'rotateY'
    });
}

function ConfirmBoxClick_Animations_RotateX() {
    $.confirm({
        animation: 'rotateX',
        closeAnimation: 'rotateXR'
    });
}
function ConfirmBoxClick_Animations_RotateXRR() {
    $.confirm({
        animation: 'rotateXR',
        closeAnimation: 'rotateX'
    });
}

function ConfirmBoxClick_AnimationsBounce_NoBounce() {
    $.confirm({
        animationBounce: 1
    });
}

function ConfirmBoxClick_AnimationsBounce_1_5Bounce() {
    $.confirm({
        animationBounce: 1.5
    });
}

function ConfirmBoxClick_AnimationsBounce_2Bounce() {
    $.confirm({
        animationBounce: 2
    });
}

function ConfirmBoxClick_AnimationsBounce_2_5Bounce() {
    $.confirm({
        animationBounce: 2.5
    });
}

function ConfirmBoxClick_Animations_TooLate() {
    $.confirm({
        animationSpeed: 2000
    });
}

function ConfirmBoxClick_Animations_TooQuick() {
    $.confirm({
        animationSpeed: 200
    });
}

function ConfirmBoxClick_Animations_Light_theme() {
    $.confirm({
        theme: 'white'
    });
}

function ConfirmBoxClick_Animations_Dark_theme() {
    $.confirm({
        theme: 'black'
    });
}


function ConfirmBoxClick_Animations_Supervan_theme() {
    $.confirm({
        theme: 'supervan'
    });
}

function ConfirmBoxClick_Animations_Material_theme() {
    $.confirm({
        theme: 'material'
    });
}
function ConfirmBoxClick_Animations_Botstarp_theme() {
    $.confirm({
        theme: 'bootstrap',
    });
}

function ConfirmBoxClick_LoadFromTXT() {
    $.confirm({
        title: 'Title',
        content: 'url:text.txt',
        animation: 'top',
        columnClass: 'col-md-6 col-md-offset-3',
        closeAnimation: 'top'
    });
}
function ConfirmBoxClick_ContentLoadCallBack() {
    $.confirm({
        title: 'Title',
        content: 'url:text.txt',
        contentLoaded: function (data, status, xhr) {
            var self = this;
            setTimeout(function () {
                self.setContent('<h1>OK! the status is: ' + status + '</h1><br>' + self.content); // concat content
                self.setTitle('Stuff is loaded');
            }, 2000);
        },
        animation: 'top',
        columnClass: 'col-md-6 col-md-offset-3',
        closeAnimation: 'top'
    });
}

function ConfirmBoxClick_ContentLoadFromBrowser() {
    $.confirm({
        title: 'Title',
        content: function () {
            // because `this` is unreachable in jquery ajax callbacks
            var jc = this;

            return $.ajax({
                url: 'bower.json',
                dataType: 'json',
                method: 'get'
            }).done(function (response) {
                jc.setContent('Description: ' + response.description);
                jc.setContent(jc.content + '<br>Version: ' + response.version); // appending
                jc.setTitle(response.name);
            }).fail(function () {
                jc.setContent('Something went wrong.');
            });
        },
        confirmButton: 'Add sentence',
        confirm: function () {
            this.setContent(this.content + '<h4>Adding a new sentence.</h4>');
            return false;
        }
    });
}
function ConfirmBoxClick_AutoCancel() {
    $.confirm({
        title: 'Delete user?',
        content: 'This dialog will automatically trigger \'cancel\' in 6 seconds if you don\'t respond.',
        autoClose: 'cancel|6000',
        confirm: function () {
            $.alert('confirmed');
            return false;
        },
        cancel: function () {
            $.alert('canceled');
        }
    });
}

function ConfirmBoxClick_AutoConfirm() {
    $.confirm({
        title: 'Logout?',
        content: 'Your time is out, you will be automatically logged out in 10 seconds.',
        autoClose: 'confirm|10000',
        confirm: function () {
            $.alert('confirmed');
        },
        cancel: function () {
            $.alert('canceled');
        }
    });
}
function ConfirmBoxClick_AllowBGDismiss() {
    $.confirm({
        backgroundDismiss: true,
        content: 'Click outside the dialog, and i shall close!'
    });
}

function ConfirmBoxClick_DisAllowBGDismiss() {
    $.confirm({
        backgroundDismiss: false,
        content: 'Click outside the dialog, and i will shake it off like taylor swift.'
    });
}
function ConfirmBoxClick_Keyboard_Enabled() {
    $.confirm({
        keyboardEnabled: true,
        content: 'Press ESC or ENTER to see it in action.',
        cancel: function () {
            $.alert('canceled');
        },
        confirm: function () {
            $.alert('confirmed');
        }
    });
}

function ConfirmBoxClick_WithCustomKeys() {
    $.confirm({
        keyboardEnabled: true,
        content: 'Press "A" to confirm or "B" to cancel. <input type="text" class="form-control" placeholder="typing a or b will close the modal"/>',
        confirmKeys: [65],
        cancelKeys: [66],
        cancel: function () {
            $.alert('canceled');
        },
        confirm: function () {
            $.alert('confirmed');
        }
    });
}
function ConfirmBoxClick_TryRTL() {
    $.alert({
        title: 'پیغام',
        content: 'این یک متن به زبان شیرین فارسی است',
        confirmButton: 'تایید',
        cancelButton: 'انصراف',
        confirmButtonClass: 'btn-primary',
        rtl: true,
        closeIcon: true,
        confirm: function () {
            alert('تایید شد.');
        }
    });
}

function ConfirmBoxClick_With_Callbacks() {
    $.confirm({
        content: 'Imagine this is a complex form and you have to attach events all over the form or any element <br>' +
        '<button type="button" class="examplebutton">I\'ve events attached!</button>',
        onOpen: function () {
            alert('after the modal is opened/rendered');
            // find the input element and attach events to it.
            // NOTE: `this.$content` is the jquery object for content.
            this.$content.find('button.examplebutton').click(function () {
                alert('I\'ve powers!');
            });
        },
        onClose: function () {
            alert('before the modal is closed');
        },
        onAction: function (action) {
            // action is either 'confirm', 'cancel' or 'close'
            alert(action + ' was clicked');
        }
    });
}


var AnimationTypesArray = ['right', 'left', 'bottom', 'top', 'Rotate', 'none', 'opacity', 'scale', 'zoom', 'scaleY', 'scaleX', 'rotateY', 'rotateYR', 'rotateX', 'rotateXR'];

var ButtonClassArray = ['btn-success', 'btn-info', 'btn-danger', 'btn-warning', 'btn-primary', 'btn-default', 'btn-link'];

var ButtonSizeArray = ['btn-lg', 'btn-sm', 'btn-xs'];

var PopUpThemeArray = ['white', 'black', 'supervan', 'material', 'bootstrap'];

var ColumnClassArray = ['col-md-offset-4 col-md-4 col-md-offset-4', 'col-md-offset-3 col-md-6 col-md-offset-3', 'col-md-offset-2 col-md-8 col-md-offset-2', 'col-md-offset-1 col-md-10 col-md-offset-1', 'col-md-12'];

/************************Model PopUp With Dynamic Title & Content[Style-1]**************************************/
function BSAlertPopUpWithTitleAndContent(Title, Content) {
    var Num = Math.floor(Math.random() * (14 - 0 + 1)) + 0;
    $.alert({
        title: Title,
        content: Content,
        cancelButton: 'Okay',
        cancelButtonClass: 'btn-info',
        animation: AnimationTypesArray[Num],
        icon: 'fa fa-ban',
        opacity: 1,
        backgroundDismiss: true,
        columnClass: ColumnClassArray[0]
    });
}


/////////**********************************************************************************************///////////////////////
/////////*****************Please Use The Below Methods And Do Not Use The Above Methods.//////////////////////////////////////



function OpenSuccessAlertPopUpBox_ConfirmPopUpJS(Content) {
    $.confirm({
        title: 'Alert',
        content: Content,
        //icon: 'fa fa-check',
        backgroundDismiss: false,
        confirmButton: 'okay',
        confirmButtonClass: 'btn-info',
        cancelButton: false,
        opacity: 1,
    });
}