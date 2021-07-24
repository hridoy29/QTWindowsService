using BioMetrixCore;
using QTService.DAL;
using QTService.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QTService.BLL
{
    public class SendMachineData
    {
        private SqlConnection osqlConnection;
        DeviceManipulator manipulator = new DeviceManipulator();
        AttandanceDAO attandanceDAO;
        public ZkemClient objZkeeper;
        public SendMachineData()
        {
            attandanceDAO = AttandanceDAO.GetInstanceThreadSafe;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DbCon"];
            // If found, return the connection string.
            if (settings != null)
                osqlConnection = new SqlConnection(settings.ConnectionString);
        }
        public string CheckingConnectionStatus(string ipAddress, string port)
        {
            string status = string.Empty;
            try
            {


                #region Validating 
                if (ipAddress == string.Empty || port == string.Empty)
                {
                    status = "The Device IP Address and Port is mandotory !!";

                }


                int portNumber = 0;
                if (!int.TryParse(port, out portNumber))

                {
                    status = "Not a valid port number";

                }
                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                {
                    status = "The Device IP is invalid !!";

                }
                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                {
                    status = "The device at " + ipAddress + ":" + port + " did not respond!!";

                }
                #endregion
                ZkemClient objZkeeper = new ZkemClient();
                bool IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    status = "Device Is Online";
                }

            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();
                //ShowStatusBar(ex.Message, false);
            }
            return status;
        }

        public string StartService(string machineId, string ipAddress, int port, DateTime fromDate, DateTime toDate, string status)
        {
            string ErrorStatus = string.Empty;
            ICollection<MachineInfo> lstMachineInfo = ReadMachineData(machineId, status, ipAddress, port, fromDate, toDate);

            //    ErrorStatus = ErrorStatus + "  mycount "+ lstMachineInfo.Count.ToString();
            if (lstMachineInfo != null && lstMachineInfo.Count > 0)
            {
                ErrorStatus = SaveData(lstMachineInfo, ipAddress);
            }
            else
            {
                ErrorStatus = "Record not found " + ipAddress;
            }

            return ErrorStatus;
        }
        public ICollection<MachineInfo> ReadMachineData(string machineId, string status, string ipAddress, int port, DateTime fromDate, DateTime toDate)
        {
            IList<MachineInfo> listAttandance = new List<MachineInfo>();
            {
                try
                {
                    List<TimeConfigSettings> timeConfigSettings = new List<TimeConfigSettings>();
                    TimeConfigSettings _timeConfigSettings = new TimeConfigSettings();
                    timeConfigSettings = GetDynamic(" where IP = " + "'"+ ipAddress + "'");
                   _timeConfigSettings = timeConfigSettings[0];
                    DateTime fromDate2 = _timeConfigSettings.LastUpdateTime;
                    if (status.Trim() == "Device Is Online")
                    {
                        objZkeeper = new ZkemClient();
                        bool IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, port);

                        if (IsDeviceConnected)
                        {
                            ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(machineId.Trim()));

                            if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                            {

                                DateTime fromDate1 = DateTime.Parse(fromDate.ToString("yyyy-MM-dd"));
                                DateTime fromDate3 = DateTime.Parse(fromDate2.ToString("yyyy-MM-dd"));
                                DateTime toDate1 = Convert.ToDateTime(toDate.ToString("yyyy-MM-dd"));

                                var myQuery = from p in lstMachineInfo
                                              where

                                              p.PunchDate > fromDate2
                                              //&& p.PunchDate <= toDate1
                                              orderby p.EmployeeID
                                              select p;

                                if (myQuery.ToList<MachineInfo>().Count > 0)
                                {
                                    listAttandance = myQuery.ToList<MachineInfo>();

                                }
                                else
                                {
                                    listAttandance = null;
                                }
                            }
                            else
                            {
                                listAttandance = null;
                            }
                        }
                        else
                        {
                            listAttandance = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }

            }
            return listAttandance;


        }
        public List<TimeConfigSettings> GetDynamic(string whereCondition)
        {
            try
            {
                List<TimeConfigSettings> _TimeConfigSettings = new List<TimeConfigSettings>();


                osqlConnection.Open();
                string osqlCommandText = "SELECT * FROM TRN_SM_TimeConfigSettings_t " + whereCondition;
                SqlCommand osqlCommand = new SqlCommand(osqlCommandText, osqlConnection);
                SqlDataReader osqlDataReader = osqlCommand.ExecuteReader();

                while (osqlDataReader.Read())
                {
                   
                    TimeConfigSettings timeConfigSettings = new TimeConfigSettings();
                    timeConfigSettings.Id = osqlDataReader.GetInt32(0);
                    timeConfigSettings.IP = osqlDataReader.GetString(1);
                    timeConfigSettings.LastUpdateTime = osqlDataReader.GetDateTime(2);
                   
                    _TimeConfigSettings.Add(timeConfigSettings);
                }

                return _TimeConfigSettings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (osqlConnection.State == ConnectionState.Open)
                {
                    osqlConnection.Close();
                }

            }

        }
        private string SaveData(ICollection<MachineInfo> listAttandance, string IpAddress)
        {
            string Error = string.Empty;
            Error = attandanceDAO.SaveData(listAttandance, IpAddress);
            return Error;
        }
    }
}
