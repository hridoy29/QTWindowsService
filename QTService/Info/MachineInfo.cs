using System;

namespace BioMetrixCore
{
    public class MachineInfo
    {
        public int MachineNumber { get; set; }
        public int EmployeeID { get; set; }
        public string PunchTime { get; set; }


        public DateTime PunchDate
        {
            get { return DateTime.Parse(DateTime.Parse(PunchTime).ToString("yyyy-MM-dd HH:mm:ss")); }
        }


        //        public DateTime DateOnlyRecord  
        //        {
        //            get { return DateTimeRecord.ToLocalTime() ; }
        //}
        //public DateTime TimeOnlyRecord
        //        {
        //            get { return DateTime.Parse(DateTime.Parse(DateTimeRecord).ToString("hh:mm:ss tt")); }
        //        }

    }
}
