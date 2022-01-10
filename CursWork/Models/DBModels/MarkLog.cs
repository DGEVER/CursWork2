using System;
using System.Collections.Generic;

#nullable disable

namespace CursWork.Models.DBModels
{
    public partial class MarkLog
    {
        public int IdMarkLog { get; set; }
        public int IdUspevaemost { get; set; }
        public DateTime DateTime { get; set; }
        public int IdTeacher { get; set; }
        public string OldMark { get; set; }
        public string NewMark { get; set; }

        public virtual Teacher IdTeacherNavigation { get; set; }
        public virtual Uspevaemost IdUspevaemostNavigation { get; set; }
    }
}
