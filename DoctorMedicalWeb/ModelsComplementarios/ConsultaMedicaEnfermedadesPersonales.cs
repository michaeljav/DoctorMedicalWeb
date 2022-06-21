using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class ConsultaMedicaEnfermedadesPersonales
    {
        public int DoctSecuencia_fk { get; set; }
        public int PaisSecuencia_fk { get; set; }
        public int ClinSecuencia_fk { get; set; }
        public int ConsSecuencia_fk { get; set; }
        public int PaciSecuencia_fk { get; set; }
        public int CMediSecuencia_fk { get; set; }
        public int EnfeSecuencia_fk { get; set; }
        public string EnfeComentario { get; set; }
        public bool EnfermedadActiva { get; set; }
        public Nullable<System.DateTime> FechaDeDiagnostico { get; set; }
        public bool EnfermedadInfecciosa { get; set; }
        public Nullable<int> EdadMomentoDiagnosticado { get; set; }
    }
}