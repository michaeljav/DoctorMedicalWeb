
//convert type string  date dd/MM/yyyy on MM/dd/yyyy
function convertDate(dateDDMMYY) {
    var myDate = dateDDMMYY;
    var formattedDate = undefined;

    if (myDate != "" && myDate != undefined) {
        var chunks = myDate.split('/');
        formattedDate = [chunks[1], chunks[0], chunks[2]].join("/");
    }

    return formattedDate;
}




//Limpiar textbox
function Nuevo( todosbtn) {
    alert("Entro al metodo nuevo");
     //todos los botones limpiar
    $(todosbtn).each(function () {
        $(todosbtn).val(" ");
    });

    $(':input').val('');
    return true;
}




//Bloquear boton
function doct_DisabledButtonAndEventClick(ctl, msg, todosbtn) {

    //$(ctl).prop("disabled", true).text(msg);

    //todos los botones desabilitar
    //$(todosbtn).each(function () {
    //    $(todosbtn).prop("disabled", true);
    //});

    $(todosbtn).each(function (index, items) {
        //todos los botones habilitar
        // $(items).prop("disabled", false);
        if (items.type == "button") {
            items.disabled = true;
        }
        //$(items.disabled).val(false);

    });


    ////buton de accion en cuestion ponerle nombre
    $(ctl).text(msg);

    //$(".submit-progress").removeClass("hidden");
    //$(".submit-progress").removeClass("hidden");           
    return true;
}
//desBloquear boton
function doct_EnableButtonAndEventClick(ctl, msg, todosbtn) {

    //   $(ctl).prop("disabled", false).text(msg);

    

    //$(todosbtn).each(function () {
    //    //todos los botones habilitar
    //    $(todosbtn).prop("disabled", false);
        
    //});

   // $(todosbtn).prop("disabled", false);



    $(todosbtn).each(function (index, items) {
        //todos los botones habilitar
        // $(items).prop("disabled", false);
        if (items.type == "button") {
            items.disabled = false;
        }
        //$(items.disabled).val(false);

    });

    ////buton de accion en cuestion ponerle nombre
    $(ctl).text(msg);



    //$(".submit-progress").removeClass("hidden");
    //$(".submit-progress").removeClass("hidden");           


  return true;
}


//verificar si la cookie esta habilitada
function cookiesEnabled() {
    var cookiesEnabled = (navigator.cookieEnabled) ? true : false;

    if (typeof navigator.cookieEnabled == "undefined" && !cookiesEnabled) {
        document.cookie = "mytestcookie";
        cookiesEnabled = (document.cookie.indexOf("mytestcookie") != -1) ? true : false;
    }

    return cookiesEnabled;
}

if (cookiesEnabled()) {
    //  document.getElementById("msg").innerHTML = "Cookies enabled";
}
else {
    document.getElementById("msg").innerHTML = "Cookies Esta desactivada, favor de activarla o contacte a la administración. ";
    //document.getElementById("msg").style.fontFamily = "Impact,Charcoal,sans-serif";
    //document.getElementById('msg').setAttribute("style", "width:5000px");
    //document.getElementById('msg').style.height = '200px'
    document.getElementById("msg").style.fontSize = "40px";
}




/*to collapse panel*/
//$(document).on('click', '.panel-heading span.clickable', function (e) {
//    var $this = $(this);
//    if (!$this.hasClass('panel-collapsed')) {
//        $this.parents('.panel').find('.panel-body').slideUp();
//        $this.addClass('panel-collapsed');
//        $this.find('i').removeClass('glyphicon-chevron-up').addClass('glyphicon-chevron-down');
//    } else {
//        $this.parents('.panel').find('.panel-body').slideDown();
//        $this.removeClass('panel-collapsed');
//        $this.find('i').removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-up');
//    }
//})

/*end to collapse panel*/