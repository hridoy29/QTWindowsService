using QTService.BLL;
using QTService.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTService
{
    partial class Service1 : ServiceBase
    {
        DeviceBLL deviceBLL = new DeviceBLL();
        SendMachineData machineData = new SendMachineData();
        public Service1()
        {
            InitializeComponent();
        }
         
        private void StartAttandanceService(string _Zid)
        {
            try
            {
                IList<DeviceEntity> deviceEntities = deviceBLL.GetDynamic(" Where isActive=1 and Zid='" + _Zid + "'", "");
                IList<DeviceEntity> deviceOnline = new List<DeviceEntity>();
                string FileLocation = ConfigurationManager.AppSettings["FileLocation"].ToString();
                string Error = string.Empty;
                #region Variables
                string machineId = string.Empty;
                string status = string.Empty;
                string ipAddress = string.Empty;
                int port = 0;
                DateTime dtFrom = DateTime.Now;
                DateTime dtTo = DateTime.Now;
                #endregion
                foreach (DeviceEntity deviceEntity in deviceEntities)
                {
                    machineId = deviceEntity.MachineId.ToString();
                    ipAddress = deviceEntity.IpAddress.ToString();
                    port = int.Parse(deviceEntity.Port.ToString());
                    dtFrom = DateTime.Now;
                    dtTo = DateTime.Now;

                    status = machineData.CheckingConnectionStatus(ipAddress, port.ToString());
                    if (status != "Device Is Online")
                    {

                        using (TextWriter tw = new StreamWriter(FileLocation + "Error_" + DateTime.Now.ToString("yyyyMMdd") + ".dat", true))
                        {
                            tw.WriteLine(status);
                        }
                    }
                    else
                    {
                        deviceOnline.Add(deviceEntity);

                    }

                }
                foreach (DeviceEntity deviceEntity in deviceOnline)
                {
                    machineId = deviceEntity.MachineId.ToString();
                    ipAddress = deviceEntity.IpAddress.ToString();
                    port = int.Parse(deviceEntity.Port.ToString());
                    dtFrom = DateTime.Now;
                    dtTo = DateTime.Now;

                    Error = machineData.StartService(machineId, ipAddress, port, dtFrom, dtTo, "Device Is Online");
                    if (Error == "Successfull")
                    {
                        //Text File write
                    }
                    else
                    {
                        using (TextWriter tw = new StreamWriter(FileLocation + "Error_" + DateTime.Now.ToString("yyyyMMdd") + ".dat", true))
                        {
                            tw.WriteLine(Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string _FileLocation = ConfigurationManager.AppSettings["FileLocation"].ToString();
                string _Error = string.Empty;
                using (TextWriter tw = new StreamWriter(_FileLocation + "TaskSceduleError_" + DateTime.Now.ToString("yyyyMMdd") + ".dat", true))
                {
                    tw.WriteLine(_Error);
                }
            }
            finally
            {
                Application.Exit();
            }


        }
        protected override void OnStart(string[] args)
        {
            string FileLocation = ConfigurationManager.AppSettings["FileLocation"].ToString();
            string _Zid = ConfigurationManager.AppSettings["Zid"].ToString();         
            StartAttandanceService(_Zid);
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
