using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.Libreria
{
    
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CannotBeEmptyAttribute : ValidationAttribute
    {
        private const string defaultError = "'{0}' debe al menos elegir un elemento.";
        public CannotBeEmptyAttribute()
            : base(defaultError) //
        {
        }
        public static List<string> idCamposError = new List<string>();
        public override bool IsValid(object value)
        {

            bool estavalidados = false;
            IList list = value as IList;
                      
            if (list != null && list.Count > 0)
            {
                //si los elementos son mayores a 0
                foreach (var item in list)
                {
                    //si es diferente de null
                    if (item != null)
                    {
                        //si es mayor de 0 quiere decir que tiene  por lo menos un consultorio.
                        if (((int)item) > 0)
                        {
                            estavalidados = true;

                        }
                    }

                }
            }
            return estavalidados;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(this.ErrorMessageString, name);
        }
    }
}