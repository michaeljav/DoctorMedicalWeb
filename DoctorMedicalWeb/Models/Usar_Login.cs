using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DoctorMedicalWeb.ModelsUsar
{
    public class Usar_Login
    {
    
    public bool EstaDesabilitado { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo", Prompt = "Correo")]
        [Required(ErrorMessage = "Introduzca el Correo")]
        [RegularExpression(@"(\S)+", ErrorMessage = "Espacio No Permitido.")]
        public string Email { get; set; }

        [Display(Name = "Contrasenia", Prompt = "Contrasenia")]
        [Required(ErrorMessage = "Introduzca Contraseña")]
        public string Clave { get; set; }




    }
}