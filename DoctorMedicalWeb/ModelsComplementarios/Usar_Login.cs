using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class Usar_Login
    {

        public Usar_Login()
        {
            
        }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo", Prompt = "NO FUNCIONA")]        
        [Required(ErrorMessage = "Favor introduzca correo.")]
        [RegularExpression(@"(\S)+", ErrorMessage = "Favor no introducir espacio.")]
        public string Email { get; set; }

   
        [Display(Name = "Clave", Prompt = "Clave")]
        [Required(ErrorMessage = "Favor introduzca Clave")]
        public string Clave { get; set; }




    }
}