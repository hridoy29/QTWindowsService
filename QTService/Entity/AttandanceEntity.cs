using System;

namespace QTService.Entity
{
    public class AttandanceEntity
    {
        public int zid { get; set; }
        public int xemp { get; set; }
        public DateTime xdate { get; set; }
        public DateTime xdesc { get; set; }
        public int xtrnord { get; set; }

        public AttandanceEntity(int _zid, int _xemp, DateTime _xdate, DateTime _xdesc, int _xtrnord)
        {
            zid = _zid;
            xemp = _xemp;
            xdate = _xdate;
            xdesc = _xdesc;
            xtrnord = _xtrnord;
        }

        public AttandanceEntity()
        {

        }
    }
}
