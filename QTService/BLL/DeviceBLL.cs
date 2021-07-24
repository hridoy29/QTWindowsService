using QTService.DAL;
using QTService.Entity;
using System.Collections.Generic;

namespace QTService.BLL
{
    public class DeviceBLL
    {
        DeviceDAO deviceDAO;
        public DeviceBLL()
        {
            deviceDAO = new DeviceDAO();
        }

        public List<DeviceEntity> GetDynamic(string whereCondition, string orderByExpression)
        {
            List<DeviceEntity> DeviceEntityLst = new List<DeviceEntity>();
            DeviceEntityLst = deviceDAO.GetDynamic(whereCondition, orderByExpression);
            return DeviceEntityLst;
        }
        public bool Add(DeviceEntity _DeviceEntity)
        {
            return deviceDAO.Add(_DeviceEntity);
        }
        public bool Update(DeviceEntity _DeviceEntity)
        {
            return deviceDAO.Update(_DeviceEntity);
        }
    }
}
