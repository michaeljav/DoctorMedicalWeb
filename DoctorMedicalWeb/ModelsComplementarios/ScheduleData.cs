using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoctorMedicalWeb.ModelsComplementarios
{
    // Define a class with all appointment fields
    public class ScheduleData
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Boolean AllDay { get; set; }
        public Boolean Recurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public string StartTimeZone { get; set; }
        public string EndTimeZone { get; set; }
        public string Description { get; set; }
    }
}