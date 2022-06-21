define(function () {
   
    /// <summary>
    /// Helpers and utilities
    /// </summary>
    var utils = (function () {
        
        function iconoAjax(mostrar) {
            /// <summary>
            /// Luego de invocar este metodo, todas las acciones ajax que sean
            /// ejecutadas mostraran un icono de "loading" en pantalla
            /// </summary>
            /// <param name="mostrar">Indica si el icono debe mostrarse u ocultarse</param>
            /// <remarks>
            /// Solo funcionara para llamadas Ajax que sean ejecutadas con jQuery
            /// </remarks>
            var $icon = $('.ajax-icon');
            if (mostrar) {
                setTimeout(function () { $icon.fadeIn(500).removeClass('hide') }, 20);
            } else {
                setTimeout(function () { $icon.fadeOut(500).addClass('hide') }, 20);
            }
        }

        function loaders() {
            /// <summary>
            /// Luego de invocar este metodo, todas las acciones ajax que sean
            /// ejecutadas mostraran un icono de "loading" en pantalla
            /// </summary>
            /// <remarks>
            /// Solo funcionara para llamadas Ajax que sean ejecutadas con jQuery
            /// </remarks>
            var deferred = new $.Deferred();

            var $icon = $('.ajax-icon');
            setTimeout(function () { $icon.fadeIn(500).removeClass('hide') }, 20);

            deferred.done(function () {
                setTimeout(function () { $icon.fadeOut(500).addClass('hide') }, 20);
            });

            return deferred;
        }

        function ajax(url, data, resolverUrl, metodo) {
            /// <summary>
            /// Envia una peticion Ajax a servidor con los datos suministrados
            /// </summary>
            /// <param name="url" type="String">URI del servicio</param>
            /// <param name="data">Objeto JSON que sera enviado al cliente</param>
            /// <param name="metodo" type="String">Metodo HTTP que sera utilizado</param>
            /// <param name="resolverUrl" type="Boolean">Indica si la URI  debe ser convertida</param>
            /// <returns>
            /// Promesa que sera atendida cuando la operacion termine
            /// </returns>
            resolverUrl = typeof resolverUrl == "undefined" ? true : resolverUrl;
            var options = {
                url: resolverUrl ? obtenerUrl(url) : url,
                type: metodo || 'POST',
                contentType: 'application/json; charset=UTF-8',
                data: data ? JSON.stringify(data) : null
            };

            return $.ajax(options).fail(function (jqXhr) {
                notif.log('Error al ejecutar la petición Ajax', jqXhr);
            });
        }

        function obtenerUrl(urlRelativa) {
            /// <summary>
            /// Obtiene la URI absoluta a partir de su URI relativa
            /// </summary>
            /// <param name="urlRelativa" type="String">URI relativa que sera convertida</param>
            return finder.getAppFile(urlRelativa);
        }

        function redirigir(url, resolver) {
            /// <summary>
            /// Redirige el usuario a la pantalla especificada en 4 segundos
            /// </summary>
            /// <param name="url">Direccion URI destino</param>
            /// <param name="resolver">Indica si la direccion url debe ser resuelta</param>
            setTimeout(function () {
                window.location = (resolver ? obtenerUrl(url) : url);
            }, 3000);
        }

        function serializarFormulario($form) {
            /// <summary>
            /// Serializa un formulario en un objeto que puede ser recibido
            /// correctamente del lado del servidor
            /// </summary>
            /// <param name="$form">Formulario jQuery a ser serializado</param>
            var form = $form.serializeArray();

            var data = {};
            for (var i = 0; i < form.length; i++) {
                data[form[i].name] = form[i].value;
            }

            return data;
        }

        return {
            ajax: ajax,
            iconoAjax: iconoAjax,
            loaders: loaders,
            redirigir: redirigir,
            obtenerUrl: obtenerUrl,
            serializarFormulario: serializarFormulario,
        };

    })();

    var config = (function () {
   
        /// <summary>
        //  Funciones comunes de la app client-side
        /// </summary>
        function configurarTooltips() {
            $('.tooltip').tooltip({ placement: 'left' });
        }

        function popups() {
             /// <summary>
            /// Configura los Popup Ajax del sistema
            /// </summary>
            $('body').on('click', '.ajax-popup', function (e) {
                e.preventDefault();
                var $e = $(e.target),
                    uri = $e.attr('href'),
                    index = uri.indexOf('Entrenamiento') + 8,
                    target = uri.substring(index).replace(/[\%]/gi, '').replace(/[\/=\+\?\&]/gi, '-'),
                    $div = $('#' + target);
              
                if ($div.length == 0) {
                    $div = $('<div id="'+target+'" class="modal fade">')
                        .append('<div class="modal-header"><button type="button" class="close" data-dismiss="modal">&times;</button></div>')
                        .append('<div class="modal-body"></div>')
                        .append('<div class="modal-footer"></div>')
                        .appendTo($('body'));

                    $.get(uri).done(function (html) { cargarPopup(html, $div); });

                } else {
                    $div.modal('show');
                }
            });
        }

        function cargarPopup(html, $div) {
            var $body = $div.find('.modal-body'),
                $header = $div.find('.modal-header'),
                $footer = $div.find('.modal-footer'),
                $html = $('<div>').html(html),
                $h2 = $html.find('h2'),
                $controls = $html.find('.control-buttons');

            $body.html($html);
            if ($h2.length == 0) {
                $header.remove();
            } else {
                $header.append($h2);
            }
            if ($controls.length == 0) {
                $footer.remove();
            } else {
                $footer.append($controls);
            }

            $div.modal('show');
        }

        function botones() {
            $(':submit').click(function (e) {
                e.preventDefault();
                var $this = $(e.target),
                    form = $this.attr('form');
                if (!!form) {
                    $('#' + form).submit();
                } else {
                    $this.parent('form').submit();
                };
            });
        }

        function placeholders() {
            if (!Modernizr.input.placeholder) {
                var configPlugin = function () {
                    var $inputs = $('input, textarea');
                    if (typeof $inputs.placeholder == 'function') {                        
                        $inputs.placeholder();
                    }
                };
                if (pluginCargado('placeholder')) {
                    require(['jquery_placeholder'], configPlugin);
                } else {
                    configPlugin();
                }
            }
        }

        function dtTables() {

            require(['datatables'], function () {

                $('.dataTable').dataTable({
                    "bLengthChange": false,

                    language: {
                        processing: "Procesando",
                        search: "Buscar:",
                        lengthMenu: "Ver _MENU_ Filas",
                        info: "_START_ - _END_ de _TOTAL_ elementos",
                        infoEmpty: "0 - 0 de 0 elementos",
                        infoFiltered: "(Filtro de _MAX_ entradas en total)",
                        infoPostFix: "",
                        loadingRecords: "Cargando datos.",
                        zeroRecords: "No se encontraron datos",
                        emptyTable: "No hay datos disponibles",
                        paginate: {
                            first: "Primero",
                            previous: "Anterior",
                            next: "Siguiente",
                            last: "Ultimo"
                        },
                        aria: {
                            sortAscending: ": activer pour trier la colonne par ordre croissant",
                            sortDescending: ": activer pour trier la colonne par ordre décroissant"
                        }
                    }
                });

            });
        }


        function busqueda() {
            /// <summary>
            /// Configura la busqueda global del sistema
            /// </summary>
            $('.seach input').keydown(function (e) {
                if (e.keyCode == 13) {
                    var $text = $(e.target);
                    if ($text.val().length > 0) {
                        $text.parent('form').submit();
                    }
                } else if (e.keyCode == 27) {
                    $(e.target).val('');
                }
            });
        }

        function formulario(id, callback) {
            /// <summary>
            /// Crea un formulario que sera posteado via Ajax
            /// </summary>
            /// <param name="id">Id del formulario HTML</param>
            /// <param name="callback">Funcion que sera invocada</param>
            var $form = $('#' + id);

            if ($form.length != 1 || !$form.is('form')) {
                return false;
            }

            var isLoading = false;
            $form.on('submit', function (e) {
                e.preventDefault();

                if (isLoading || (pluginCargado('parsley') && !$form.parsley('validate'))) {
                    return false;
                }

                var url = $form.prop('action'),
                    $submit = $('input[type="submit"][form="' + id + '"]');

                $submit.data('loading-text', 'Procesando...');
                $submit.button('loading');

                var data = utils.serializarFormulario($form);

                isLoading = true;
                $.post(url, data).done(callback)
                    .fail(function (jqXhr) {
                        notif.log('Error al ejecutar el request Ajax', jqXhr);
                    })
                    .always(function () {
                        isLoading = false;
                        $submit.button('reset');
                    });
            });
        }

        function validacion(id, extra, trigger) {
            /// <summary>
            /// Configura las validaciones del lado del cliente de un formulario
            /// </summary>
            /// <param name="id">Id del formulario HTML</param>
            /// <param name="extra">Opciones adicionales de validacion</param>
            /// <param name="trigger">Evento que desencadenara la validacion</param>
            /// <remarks>
            /// Las opciones actuales de configuracion estan documentadas en:
            /// http://parsleyjs.org/documentation.html
            /// </remarks>
            var configPlugin = function () {
                var config = {
                    trigger: trigger || 'change',
                    errorClass: 'error',
                    successClass: 'success',
                    errors: {
                        errorsWrapper: '<ul class="validation-summary-errors"></ul>'
                    }
                };

                if (extra && 'validators' in extra) {
                    config.validators = extra.validators;
                    if ('messages' in extra) {
                        config.messages = extra.messages;
                    }
                }

                id = (!id) ? "form" : "#" + id;
                $(id).parsley(config);
            };
            if (!pluginCargado('parsley')) {
                require(["parsley", "parsley_brrd"], configPlugin);
            } else {
                configPlugin();
            }
        }

        function fechas() {
            /// <summary>
            /// Configura los selectores de fecha
            /// </summary>

            var configPlugin = function () {
                
                $('input[type="calendar"], .calendar').datepicker({
                    monthNamesShort: ['Enero', 'Feb.', 'Marzo', 'Abr', 'May.', 'Jun.', 'Jul.',
                       'Agosto', 'Sept.', 'Oct.', 'Nov.', 'Dic.'],
                    monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio',
                       'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
                    dayNamesMin: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
                    currentText: 'Hoy',
                    closeText: 'Cerrar',
                    dateFormat: 'dd/mm/yy',
                    firstDay: 1,
                    showMonthAfterYear: false,
                    changeMonth: true,
                    changeYear: true,
                    setDate: new Date(),
                    showButtonPanel: true,
                    gotoCurrent: true,
                }
               );
            };

            if (!pluginCargado('datepicker')) {
                require(['datepicker'], configPlugin);
            } else {
                configPlugin();
            }


          
        }

        function areasAsync(id) {
            /// <summary>
            /// Configura las areas cargadas de forma asincrona
            /// </summary>
            /// <param name="id">Id del area a crear o actualizar, nulo para tomar todas las areas</param>
            /// <returns>Promesa con una operacion que sera satisfecha en el futuro</returns>
            var deferreds = [];

            id = (!!id) ? '#' + id : '';
            id += '.async-area';

            $(id).each(function (i, e) {
                var $this = $(e).hide(),
                    uri = $this.data('uri');

                if (uri) {
                    deferreds.push(
                        $.get(uri).done(function (html) {
                            $this.html(html);
                            $this.fadeIn('slow');
                        }).fail(function (jqXhr) {
                            notif.log('Error al ejecutar la petición Ajax', jqXhr);
                        })
                    );
                }
            });
            return $.when.apply(window, deferreds);
        }

        function pluginCargado(nombre) {
            /// <summary>
            /// Verifica si un plugin de jQuery esta cargado
            /// </summary>
            /// <param name="nombre">Nombre del plugin</param>
            return (!!$.fn[nombre]);
        }

        function autocompletado() {
            /// <summary>
            /// Configura las funcionalidades de autocompletado
            /// </summary>
            var loadPlugin = function () {
                $('.autocomplete').each(function (k, e) {
                    var $this = $(e);
                    $this.select2({
                        allowClear: true,
                        placeholder: $this.data('placeholder'),
                        minimumInputLength: 3,
                        formatInputTooShort: function (input, min) { return "Digite " + min + " letras minimo."; },
                        formatSearching: function () { return "Buscando..."; },
                        formatNoMatches: function () { return "No se encontraron coincidencias."; },
                        width: "element",
                        formatSelection: function (data) { notif.log(data.text); return data.text },
                        formatResult: function (data) { return '<div class="select2-user-result">' + data.text + '</div>' },
                        ajax: {
                            url: $this.data('uri'),
                            dataType: 'json',
                            type: 'POST',
                            quietMillis: 1000,
                            data: function (term, page) {
                                return {
                                    search_term: term,
                                    page_limit: 10
                                };
                            },
                            results: function (data, page) {
                                return { results: data };
                            }
                        }
                    });

                });
            };
            if (!pluginCargado('select2')) {
                require(['select2'], loadPlugin);
            } else {
                loadPlugin();
            }
        }

        function listas() {
            /// <summary>
            /// Configura las funcionalidades de lista inteligente
            /// </summary>
            var loadPlugin = function () {
                
                $('.select-smart-list').each(function (k, e) {

                    var $this = $(e);
                    $this.select2({
                        placeholder: $this.data('placeholder'),
                        //minimumInputLength: 8,
                        width: "element"
                    });
                });
            };

            if (!pluginCargado('select2')) {
                require(['select2'], loadPlugin);
            } else {
                loadPlugin();
            }
        }

        function inicializar() {
            configurarTooltips();
            busqueda();
            popups();
            placeholders();
            fechas();

        }

        /// <summary>
        /// Configuraciones Generales
        /// </summary>
        return {
            inicializar: inicializar,
            fechas: fechas,
            listas: listas,
            popups: popups,
            botones: botones,
            formulario: formulario,
            validacion: validacion,
            areasAsync: areasAsync,
            autocompletado: autocompletado,
            placeholders: placeholders,
            pluginCargado: pluginCargado,
            dtTables: dtTables
        };

    })();

    var notif = (function () {        
        function mensaje(mensaje, tipo) {
            /// <summary>
            /// Muenstra un mensaje informativo al usuario
            /// </summary>
            /// <param name="mensaje" type="String">Mensaje a ser mostrado</param>
            /// <param name="tipo">Tipo del mensaje (error, success, warning, info)</param>
            require(['alertify'], function (alertify) {
                var tipos = ["error", "success", "warning", "info"];
                if (typeof tipo != "undefined" && $.inArray(tipo, tipos) == -1) {
                    tipo = undefined;
                }
                alertify.log(mensaje, tipo);
            });
        }

        function confirmacion(mensaje) {
            /// <summary>
            /// Muestra una ventana de confirmacion al usuario
            /// </summary>
            /// <param name="mensaje" type="String">Mensaje a ser mostrado</param>
            /// <returns>
            /// Promesa que sera atendida cuando la operacion termine
            /// </returns>
            var deferred = new $.Deferred();

            require(['alertify'], function (alertify) {
                alertify.set({ labels: { ok: "Aceptar", cancel: "Cancelar" } });
                alertify.confirm(mensaje, function (e) {
                    if (e) {
                        log(e);
                        deferred.resolve();
                        
                    } else {
                        deferred.reject();
                    }
                });
            });

            return deferred.promise();
        }

        function log(mensaje, data) {
            /// <summary>
            /// Escribe un mensaje en la Consola del explorador (si esta disponible)
            /// </summary>
            /// <param name="mensaje" type="String">Mensaje que sera guardado</param>
            /// <param name="data">Objeto que sera almacenado</param>
            if (typeof console != "undefined") {
                console.log(mensaje, data);
            }
        }

        function info(mensaje) {
            log(mensaje);
        }

        /// <summary>
        /// Modulo de notificaciones y alertas
        /// </summary>
        return {
            mensaje: mensaje,
            confirmacion: confirmacion,
            log: log,
            info: info
        };

    })();


    /// <summary>
    /// Objetos Publicos
    /// </summary>
    var app = {
        utils: utils,
        config: config,
        notif: notif
    };

    return app;

     

});