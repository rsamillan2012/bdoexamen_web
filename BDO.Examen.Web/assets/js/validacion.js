
function Validacion() {
    this.cLogin = function (id) {
        $(id).validate({
            rules: {
                'correo': { required: true, minlength: 8 },
                'contrasena': { required: true, minlength: 6 }
            },
            messages: {
                'correo': '',
                'contrasena': ''
            },
            highlight: function (element, errorClass, validClass) {
                errorClass = 'has-error has-feedback', validClass = 'has-success has-feedback', $(element).closest('.formulario-campo').removeClass(validClass).addClass(errorClass);
            },
            unhighlight: function (element, errorClass, validClass) {
                errorClass = 'has-error has-feedback', validClass = 'has-success has-feedback', $(element).closest('.formulario-campo').removeClass(errorClass).addClass(validClass);
            }
        });
    };
}

var corre = new Validacion(), pagina = document.querySelector('html'), pClase = (pagina.classList.length > 0) ? (pagina.classList[0] + '-') : '';

console.log(pClase + pagina.id);

switch (pClase + pagina.id) {
    case 'inicial-login':
        corre.cLogin('#formulario-login');
        break;
}


