

function iniciosession() {

    var _validate1 = $("#formulario-login").valid();

    console.log('_validate1', _validate1);


    if (_validate1) {
        $("#btn-entrar").html('');
        $("#btn-entrar").append('<span>Autenticando...</span>');
        $('#btn-entrar').attr('disabled', true);

        var _correo_l = $("#login-correo").val();
        var _pwd_l = $("#login-contrasena").val();

        //console.log('jsForm.url: ', jsForm.url);

        axios.post(jsForm.url, { email: _correo_l, password: _pwd_l, keymaster: jsForm.key })
            .then(function (response) {
                _response = response;
                console.log('_response: ', _response);

                result = true;
            }).catch(error => {
                if (!error) {
                    mensaje = jsForm.mensaje_servicionodisponible;
                } else {
                    if (error.response) {
                        if (error.response.status == 400) {
                            mensaje = "No es un email válido";
                        } else if (error.response.status == 404) {
                            mensaje = "No existe el usuario o sus datos son incorrectos";
                        } else if (error.response.status == 500) {
                            mensaje = "Ha ocurrido un error interno";
                        } else {
                            mensaje = "Ha ocurrido un error interno";

                        }
                    } else {
                        mensaje = jsForm.mensaje_servicionodisponible;
                    }
                }
                result = false;

            }).finally(function (error) {

                if (result) {

                    var _correo_2 = $("#login-correo").val();

                    axios.post(jsForm.url_mvc,
                        {
                            email: _correo_2,
                            password: jsForm._passwd,
                            tknm: _response.data.token,
                            datos: _response.data.datos
                        })
                        .then(function (response) {
                            
                            window.location.href = response.data.form_current;

                        }).catch(function (error) {
                            console.log(error);
                        }).finally(function (error) {

                        });


                } else {
                    $("#dvError").show();

                    $("#mensage_error").text(mensaje);
                    $("#btn-entrar").html('');
                    $("#btn-entrar").append('<span>ENTRAR</span>');
                    $('#btn-entrar').attr('disabled', false);
                }
            });
    }
}