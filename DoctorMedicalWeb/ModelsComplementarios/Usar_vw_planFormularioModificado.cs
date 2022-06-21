using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class Usar_vw_planFormularioModificado
    {
        public int PlanSecuencia { get; set; }
        public string PlanDescripcion { get; set; }
        public Nullable<decimal> Precio { get; set; }
        public int MoneSecuencia_fk { get; set; }
        public int PFSecuencia { get; set; }
        public int PlanSecuencia_fk { get; set; }
        public int FormSecuencia_fk { get; set; }
        public string FormDescripcion { get; set; }
        public string formPantalla { get; set; }
        public bool seleccionado { get; set; }
    }
}