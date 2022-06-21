using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class ConsultaMedicaAntecedentes
    {
        public int DoctSecuencia_fk { get; set; }
        public int PaisSecuencia_fk { get; set; }
        public int clinSecuencia_fk { get; set; }
        public int ConsSecuencia_fk { get; set; }
        public int PaciSecuencia_fk { get; set; }
        public int CMediSecuencia { get; set; }
        public Nullable<System.DateTime> CMediFecha { get; set; }
        public Nullable<System.TimeSpan> CMediHora { get; set; }
        public string EnfeComentario { get; set; }
        public string CMediUnidadesMedidaTalla { get; set; }
        public Nullable<decimal> CMediTalla { get; set; }
        public string CMediUnidadesMedidaPeso { get; set; }
        public Nullable<decimal> CMediPeso { get; set; }
        public int GSangSecuencia_fk { get; set; }
        public string Codigo { get; set; }
        public bool CMedEmbarazada { get; set; }
        public Nullable<System.DateTime> CMedEmbarazadaFecha { get; set; }
        public string CMedEmbarazadaFechaString
        {
            get
            {
                string fecha = "";
                if (CMedEmbarazadaFecha != null)
                {
                    fecha = this.CMedEmbarazadaFecha.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMedEmbarazadaFecha != null)
                {
                    fecha = this.CMedEmbarazadaFecha.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }

        }
        public Nullable<int> CMedEmbarazadaSemanas { get; set; }
        public Nullable<int> CMedEmbarazadaDias { get; set; }
        public Nullable<int> CMedEmbarazadaMeses { get; set; }
        public Nullable<int> CMedEmbarazadaMesActualDias { get; set; }
        public Nullable<System.DateTime> CMedEmbarazadaFechaProbableParto { get; set; }
        public string CMedEmbarazadaFechaProbablePartoString
        {
            get
            {
                string fecha = "";
                if (CMedEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMedEmbarazadaFechaProbableParto != null)
                {
                    fecha = this.CMedEmbarazadaFechaProbableParto.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }

        }
        public string CMediAntecedentePadre { get; set; }
        public string CMediAntecedenteMadre { get; set; }
        public string CMediAntecedenteHermanos { get; set; }
        public string CMediAntecedenteOtros { get; set; }
        public string CMediMenarquia { get; set; }
        public string CMediPatronMenstrual { get; set; }
        public string CMediMensutracionDuracion { get; set; }
        public bool CMediDismenorrea { get; set; }
        public Nullable<int> CMediPrimerCoito { get; set; }
        public bool CMediDispareunia { get; set; }
        public bool CMediVidaSexualActiva { get; set; }
        public Nullable<int> CMediNumeroParejasSexual { get; set; }
        public Nullable<System.DateTime> CMediFechaUltimoParto { get; set; }
        public string CMediFechaUltimoPartoString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoParto != null)
                {
                    fecha = this.CMediFechaUltimoParto.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoParto != null)
                {
                    fecha = this.CMediFechaUltimoParto.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }

        }
        public Nullable<System.DateTime> CMediFechaUltimoAborto { get; set; }
        public string CMediFechaUltimoAbortoString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoAborto != null)
                {
                    fecha = this.CMediFechaUltimoAborto.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoAborto != null)
                {
                    fecha = this.CMediFechaUltimoAborto.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }

        }
        public Nullable<System.DateTime> CMediFechaUltimaMenstruacion { get; set; }
        public string CMediFechaUltimaMenstruacionString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimaMenstruacion != null)
                {
                    fecha = this.CMediFechaUltimaMenstruacion.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimaMenstruacion != null)
                {
                    fecha = this.CMediFechaUltimaMenstruacion.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }

        }
        public Nullable<int> CMediMenopausia { get; set; }
        public Nullable<int> CMediGestacionVeces { get; set; }
        public Nullable<int> CMediPartosVeces { get; set; }
        public Nullable<int> CMediAbortosVeces { get; set; }
        public Nullable<int> CMediCesariasVeces { get; set; }
        public Nullable<int> CMediEctopico { get; set; }
        public string CMediTensionArterial { get; set; }
        public string CMediFrecuenciaCardiaca { get; set; }
        public string CMediTiroides { get; set; }
        public string CMediPulmones { get; set; }
        public string CMediCorazon { get; set; }
        public string CMediMamas { get; set; }
        public string CMediAbdomen { get; set; }
        public string CMediGenitalesExternos { get; set; }
        public string CMediTactoVaginal { get; set; }
        public string CMediExtremidadesInferiores { get; set; }
        public Nullable<System.DateTime> CMediFechaUltimoPapanicolau { get; set; }
        public string CMediFechaUltimoPapanicolauString
        {
            get
            {
                string fecha = "";
                if (CMediFechaUltimoPapanicolau != null)
                {
                    fecha = this.CMediFechaUltimoPapanicolau.Value.ToString("dd/MM/yyyy");

                }
                return fecha;
            }
            set
            {
                string fecha = "";
                if (CMediFechaUltimoPapanicolau != null)
                {
                    fecha = this.CMediFechaUltimoPapanicolau.Value.ToString("dd/MM/yyyy");

                }
                value = fecha;
            }
        }
        public Nullable<int> UsuaSecuenciaCreacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaCreacion { get; set; }
        public Nullable<int> UsuaSecuenciaModificacion { get; set; }
        public Nullable<System.DateTime> UsuaFechaModificacion { get; set; }
        public bool EstaDesabilitado { get; set; }
    }
}