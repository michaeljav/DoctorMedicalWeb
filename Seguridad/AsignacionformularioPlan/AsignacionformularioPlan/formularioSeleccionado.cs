using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsignacionformularioPlan
{
  public  class formularioSeleccionado
    {
      public string PlanDescripcion { get; set; }
        public int PlanSecuencia_fk { get; set; }
        public int FormSecuencia_fk { get; set; }
        public string FormDescripcion { get; set; }
        public bool seleccionar { get; set; }
    }
}
