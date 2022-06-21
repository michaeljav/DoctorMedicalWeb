using DoctorMedicalWeb.App_Data;
using DoctorMedicalWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            
        }
        //[JsonProperty(PropertyName = "id")]
        //[JsonProperty()]
        //[JsonProperty(PropertyName = "respuesta")]
        public bool respuesta { get; set; }
        //[JsonProperty()]
        //[JsonProperty(PropertyName = "redirect")]
        public string redirect { get; set; }
        //valor devuelto cuando se guarda
        public int returnSaveChange { get; set; }
        public string menssage { get; set; }
        public string reloadLayout { get; set; }
        //[JsonProperty()]
        //[JsonProperty(PropertyName = "error")]
        public string error { get; set; }
        //error al crear un usuario
        public bool errorAlconsultarelusuarioParaCrear { get; set; }

        //si esta en mode debugiar
        public bool IsDebuggingMode { get; set; }

        //campos  lista con error
        public IEnumerable<string> fielsWithError;
        //por si deseo enviar un solo objeto
        public object obj;
        public Dictionary<string, object> dictionaryStringObjec;
        public Dictionary<string, List<object>> dictionaryStringListObjec;
        public Dictionary<string, List<Usar_HorarioTrabajoDestalle>> dictionaryStringListObjecIenume;
        public Dictionary<string, IEnumerable<object>> dictionaryStringIenumerableObjec;
        public IEnumerable<object> someCollection;
        public IEnumerable<object> ObjectGridList;
        
        public List<Usar_TipoFormulario> usarTipoformulario;
    }
}