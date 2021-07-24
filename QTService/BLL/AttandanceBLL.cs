using QTService.Entity;
using System.Collections.Generic;

namespace QTService.BLL
{
    public class AttandanceBLL
    {
        DeviceBLL DeviceBLL;
        public AttandanceBLL()
        {
            DeviceBLL = new DeviceBLL();
        }

        public List<DeviceEntity> GetAllDevices()
        {
            return DeviceBLL.GetDynamic(" Where IsActive=1", "");

        }

    }
}
