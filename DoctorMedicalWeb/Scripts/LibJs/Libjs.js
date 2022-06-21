

function isRightDateClient(url) {

    let dat = new Date();
    ///DashBoard/IsRightDate



    ////appoiment can't be before date current
    //convert JavaScript Date Output
    //Tue Mar 24 2015 20:00:00 GMT-0400 (SA Western Standard Time) to MM/dd/yyyy
    dat = changeFormatTime(dat);

    //var ur = '';
    //ur =@Url.Action("IsRightDate", "DashBoard");
    //you can make appointment into range time only                   
    var _isRightDate = AjaxSend(false, 'POST', url, JSON.stringify({ dateTimeclient: dat }), 'application/json; charset=utf-8');
    if (_isRightDate.dictionaryStringObjec["isDateDiference"] == true) {

        alertify.alert('Fecha y Hora', _isRightDate.error, function () {

            window.location = _isRightDate.redirect;
            alertify.success('Ok');
        });
       
     
    }
    if (_isRightDate.dictionaryStringObjec["isTimeIntoRange"] == false) {

        alertify.alert('Fecha y Hora', _isRightDate.error, function () {

            window.location = _isRightDate.redirect;
            alertify.success('Ok');
        });


    }

    //let _isHabilitadoDia = isInRange.dictionaryStringObjec["isHabilitadoDia"];

   // return _isRightDate
}

//change format from gmt to MM/dd/yyyy
function changeFormatTime(datetimeGMT) {
    //appoiment can't be before date current
    //JavaScript Date Output
    //Mon Aug 14 2017 11:00:00 GMT-0400 (SA Western Standard Time)
    var dat = datetimeGMT;
    var formatDate = "";
    var myDateend = new Date(dat);
    // date and time dd/MM/yyyy "15/8/2017 10:30:00"
    myDateend = myDateend.toLocaleString('es-ES');
    var chunks = myDateend.split('/');
    //date format MM/dd/yyyy and hour minu.... "8/15/2017 10:30:00"
    formatDate = [chunks[1], chunks[0], chunks[2]].join("/");

    return formatDate;

}


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

function convertObjectToArrayObject(object) {
    //convierto un objeto que tiene objetos dentro a 
    //array de objetos
    let arrayObject = [];

    $.each(object, function (key, value) {

        arrayObject.push(value);
    })

    return arrayObject;
}

function getAllItemsListbox(listboxName) {
    let items = undefined;
    //first select all
    $('#' + listboxName).ejListBox("selectAll");
    items = $('#' + listboxName).ejListBox("getSelectedItems");
    $('#' + listboxName).ejListBox("unselectAll");
    

    return items;
}

function getItemsListBoxSelected(listBox) {
    //get items selected
    var itemsSelected = undefined;

   itemsSelected= $('#' + listBox).ejListBox("getSelectedItems");
    return itemsSelected;
}

function removeItemsSelectedListBox(listBox) {
    //remove itemes lisbox selected

    var target = $('#'+listBox).data("ejListBox");
    target.removeSelectedItems();
}

function isEmptyTimePicker(timePick) {
    var isEmp = false;

    var t = $("#" + timePick).ejTimePicker("getValue")
    if (t == "") {
        isEmp = true;
    }
    return isEmp;
}

function mensaje(meng) {
    alertify.alert('', meng);
}

function AjaxSend(_async, _type, _url, _data, _contentType, _dataType, _cache) {
    // var form = $('#frmAplicacion').serializeArray()
    // var jform = JSON.stringify({ usuarioSSI: form });
    /*
    
    */
    _contentType = _contentType || 'application/x-www-form-urlencoded; charset=utf-8';
    _cache = _cache || false;
    _dataType = _dataType || 'json';

    var result;
    //El que utilizo y funciona
    $.ajax({
        async: _async,
        type: _type,
        url: _url,
        dataType: _dataType,
        //dataType: "json",       
        //  contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        //   contentType: "application/json; charset=utf-8",
        contentType: _contentType,
        data: _data,
        cache: _cache,
        success: function (_result) {

            result = _result;
        }

    });

    return result;

}

function getMaxValueArrayAssociative(list, value) {

    var maxCount = 0;

    var dataSourcSchec = list;
    if (Object.keys(dataSourcSchec).length > 0) {
        var count = $.map(dataSourcSchec, function (dataSourcSchec) { return dataSourcSchec.sec; }),
        maxCount = Math.max.apply(this, count);
        // minCount = Math.min.apply(this, count);
    }


    return maxCount + 1;

}

function AddlistHour(arrayAsso, newItem) {
    var exiRang = false;
    exiRang = existeRangoEnLista(arrayAsso, newItem)
    //si  no existe el rango en la lista entonces lo agrego
    if (exiRang == false) {

        arrayAsso[newItem.sec] = $.extend(true, {}, newItem);
    }
    else {

        arrayAsso['existe'] = true;
        //mensaje('Este rango de hora  ya lo ha introducido');
        //return;
    }



    return arrayAsso;

}

function existeRangoEnLista(listaRangos, RangoNuevo) {
    //si hay aunque sea una coincidencia de que exista el rango que voy a insertar
    //entonces para el loop y digo que no se puede entrar este rango

    /*
    
    +----------------------------------------+
| JavaScript              | PHP          |
+-------------------------+--------------+
|                         |              |
| return false;           | break;       |
|                         |              |
| return true; or return; | continue;    |
+-------------------------+--------------+
    */
    var _RangoExiste = false;
    $.each(listaRangos, function (key, value) {

        _RangoExiste = rangoBetweenTwoRango(RangoNuevo.hi, RangoNuevo.hf, value.hi, value.hf);
        if (_RangoExiste == true) {

            return false;

        }
    });



    return _RangoExiste;
}

function rangoBetweenTwoRango(_currentTimeInicio, _currentTimeFinal, _startTimeInicio, _endTimeFinal) {
    //valido que un rango de hora no este dentro de un rango 
    //valido que una fecha del rango no este dentro del rango
    var existHour = true;
    //existe la primera hora dentro del rango para agregar? o es igual una de sus fechas
    var hi = timeBetweenTwoHourandSame(_currentTimeInicio, _startTimeInicio, _endTimeFinal);
    //existe la segunda hora dentro  del rango para agregar? o es igual una de sus fechas
    var hf = timeBetweenTwoHourandSame(_currentTimeFinal, _startTimeInicio, _endTimeFinal);
    

    //validar la hora ya almacenada en la lista, no este tampo dentro del rango  nuevo para insertar ej:2:00-3:00  1:1-3:1
    // existe  la hora lamacenada  dentro del rango nuevo a agregar? 
    var hAlmacenadai = timeBetweenTwoHourandSame(_startTimeInicio,_currentTimeInicio, _currentTimeFinal);
    // existe  la hora lamacenada  dentro del rango nuevo a agregar? 
    var hAlmacenadaf = timeBetweenTwoHourandSame(_endTimeFinal, _currentTimeInicio, _currentTimeFinal);

    ////si el rango que se quiero insertar no existe entonces puedo agregarlo
    //if (hi === false && hf === false) {

    //    existHour = false;
    //}

    //si el rango que se quiero insertar no existe entonces puedo agregarlo
    if (hi === false && hf === false && hAlmacenadai === false && hAlmacenadaf === false) {

        existHour = false;
    }
    return existHour;

}

function timeBetweenTwoHourandSame(_currentTime, _startTime, _endTime) {
    //validar si una hora esta dentro de un rango de hora
    //o si la una de las horas del rango es igual

    let isBetween = false;

    if (_currentTime != '' && _startTime != '' && _endTime != '') {

        let currentTime = moment(_currentTime.trim(), "HH:mm A");    // e.g. 11:00 pm
        let startTime = moment(_startTime.trim(), "HH:mm A");
        let endTime = moment(_endTime.trim(), "HH:mm A");


        //incluye la misma fecha a evaluar entre los rangos []
        //isBetween = currentTime.isBetween(_startTime, _endTime, null, '[]');
        isBetween = currentTime.isBetween(startTime, endTime, null, '[]');
        //var isBetween = currentTime.isBetween(startTime, endTime, null, '[]');

    }
    return isBetween;


}

function ObjToAddTolist(objlistHorarios,textBoxIdInicio,textboxIdFinal) {

    var obj = { sec: undefined, hi: undefined, hf: undefined, ht: undefined };
    //to add in array associative
    obj.sec = '' + getMaxValueArrayAssociative(objlistHorarios);
    obj.hi = $("#" + textBoxIdInicio).ejTimePicker("getValue");
    obj.hf = $("#" + textboxIdFinal).ejTimePicker("getValue");
    obj.ht = obj.hi + '  /  ' + obj.hf;


    return obj;


}

function addItemsToLisbox(ListObject,nameListBox,nameValue,nameText) {
    //creo un array de objectos desde el listado de array asociativo
    //para agregarlo como data source al lisbox

    //create array associative to array 
    var nObj = [];
    
    $.each(ListObject, function (key, value) {
       
        nObj.push(value);       

    });

    $("#" + nameListBox).ejListBox({
        dataSource: nObj,
        fields: {
            value: nameValue,
            text: nameText
        }
    });


}

function cleanlistbox(nameListbox) {
    $('#' + nameListbox).ejListBox("selectAll");
    var target = $('#' + nameListbox).data("ejListBox");
    target.removeSelectedItems();

}

function removeList(lisboxName,listtIMES) {

    //  var itemsSelectr = getItemsListBoxSelected('listbox');
    var itemsSelect = getItemsListBoxSelected(lisboxName);
    if (itemsSelect.length < 1) {
        
        return itemsSelect.length;
    }
    //delete itemes form   list
    DeleteItem(listtIMES, itemsSelect);
    //removed items lisbox selected
    removeItemsSelectedListBox(lisboxName);

}

function DeleteItem(arrayAsso, newItem) {
    
    $.each(newItem, function (key, value) {

        //delete itemes form list array
        if (arrayAsso[value.value] !== undefined) {

            delete arrayAsso[value.value];
        }
    });
 

    return arrayAsso;

}

function cleanTimePicker(inputs) {

    //var inputs = $(":input");
    $.each(inputs, function (k, v) {
        //content this word
        if (v.className.indexOf("e-timepicker e-js e-input") != -1) {
            $("#" + v.name).ejTimePicker({ value: "" });
        }


    });
}

function sumOneHour(HourToSum) {
    //hh:mm AM
    //hh:mm PM
    var newHour;

    var end = HourToSum.indexOf(":");
    var valu = HourToSum.substring(0, end);
    var NewHour = (Number(valu) + 1);

    if (NewHour > 12)
        NewHour = 1;

    if (NewHour.toString().length < 2) {
        NewHour = "0" + NewHour.toString();
    }

    var firtIndex = HourToSum.indexOf(":");
    var newValue = HourToSum.substring(firtIndex);
    newValue = NewHour + newValue;

    //if new Value  is 12, and it come am change to pm or viceverse
    if (NewHour == 12) {

        if (newValue.indexOf("AM") != -1) {
            newValue = newValue.replace("AM", "PM");
        } else {
            newValue = newValue.replace("PM", "AM");
        }

    }

    newHour = newValue;

    return newHour;

}

function FillListAssociative(listObject) {
    //clean list
   let lhoursun = {};
    let secu = 1;
    $.each(listObject, function (key,value) {
             

        var obj = { sec: undefined, hi: undefined, hf: undefined, ht: undefined };
        //to add in array associative
        obj.sec = secu;
        obj.hi = value.hi;
        obj.hf = value.hf;
        obj.ht = value.ht;      

        lhoursun[secu] = obj;

        secu = secu + 1;

    })
    return lhoursun;
}