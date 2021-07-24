using BioMetrixCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace QTService.DAL
{
    public static class StringExtension
    {
        //Extension Method
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }
    public class AttandanceDAO
    {
        private static volatile AttandanceDAO instance;

        private static readonly object lockObj = new object();
        private SqlConnection osqlConnection;
        public static AttandanceDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new AttandanceDAO();
            }
            return instance;
        }
        public AttandanceDAO()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DbCon"];
            // If found, return the connection string.
            if (settings != null)
                osqlConnection = new SqlConnection(settings.ConnectionString);
        }
        public static AttandanceDAO GetInstanceThreadSafe
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new AttandanceDAO();
                        }
                    }
                }
                return instance;
            }
        }
        public string SaveData(ICollection<MachineInfo> listAttandance, string _IpAddress)
        {

            string Error = string.Empty;
            string FileLocation = ConfigurationManager.AppSettings["FileLocation"].ToString();
            string _Zid = ConfigurationManager.AppSettings["Zid"].ToString();
            SqlTransaction sqlTransaction = null;
            MachineInfo machineInfonew = new MachineInfo();
            machineInfonew = listAttandance.OrderByDescending(x => x.PunchTime).FirstOrDefault();
            try
            {
                //  @xemp VarChar(100), @xdate date,@xdesc datetime, @xtrnord VarChar(50),@zid int
                osqlConnection.Open();
                sqlTransaction = osqlConnection.BeginTransaction();
                foreach (MachineInfo machineInfo in listAttandance)
                {
                    SqlCommand osqlCommand = new SqlCommand();
                    osqlCommand.Connection = osqlConnection;
                    osqlCommand.Transaction = sqlTransaction;
                    osqlCommand.CommandType = CommandType.StoredProcedure;

                    SqlParameter EmployeeID = new SqlParameter();
                    EmployeeID.ParameterName = "@xemp"; // Defining Name  
                    EmployeeID.SqlDbType = SqlDbType.VarChar; // Defining DataType  
                    EmployeeID.Direction = ParameterDirection.Input; // Setting the direction  
                    EmployeeID.Value = machineInfo.EmployeeID;



                    SqlParameter PunchDate = new SqlParameter();
                    PunchDate.ParameterName = "@xdate"; // Defining Name  
                    PunchDate.SqlDbType = SqlDbType.Date; // Defining DataType  
                    PunchDate.Direction = ParameterDirection.Input; // Setting the direction  
                    PunchDate.Value = machineInfo.PunchDate;

                    SqlParameter PunchTime = new SqlParameter();
                    PunchTime.ParameterName = "@xdesc"; // Defining Name  
                    PunchTime.SqlDbType = SqlDbType.VarChar; // Defining DataType  
                    PunchTime.Direction = ParameterDirection.Input; // Setting the direction  
                    PunchTime.Value = machineInfo.PunchTime;

                    SqlParameter IpAddress = new SqlParameter();
                    IpAddress.ParameterName = "@xtrnord"; // Defining Name  
                    IpAddress.SqlDbType = SqlDbType.VarChar; // Defining DataType  
                    IpAddress.Direction = ParameterDirection.Input; // Setting the direction  
                    IpAddress.Value = _IpAddress;//.GetLast(4);


                    SqlParameter MachineId = new SqlParameter();
                    MachineId.ParameterName = "@zid"; // Defining Name  
                    MachineId.SqlDbType = SqlDbType.Int; // Defining DataType  
                    MachineId.Direction = ParameterDirection.Input; // Setting the direction  
                    MachineId.Value = int.Parse(_Zid);

                    osqlCommand.Parameters.Add(EmployeeID);
                    osqlCommand.Parameters.Add(PunchDate);
                    osqlCommand.Parameters.Add(PunchTime);
                    osqlCommand.Parameters.Add(IpAddress);
                    osqlCommand.Parameters.Add(MachineId);

                    osqlCommand.CommandText = "ws_proc_insert_attandancedata";
                    osqlCommand.ExecuteNonQuery();
                }
                sqlTransaction.Commit();
                Error = "Successfull";

                string filename = FileLocation + DateTime.Now.ToString("yyyyMMdd") + ".qat";


                using (TextWriter tw = new StreamWriter(filename, true))
                {
                    //   192.168.251.5 * 111794 * 02 - 24 - 2021 * 10:27:01

                    var myQuery = from p in listAttandance

                                  orderby p.PunchTime ascending
                                  select p;

                    if (myQuery.ToList<MachineInfo>().Count > 0)
                    {
                        listAttandance = myQuery.ToList<MachineInfo>();

                    }
                    else
                    {
                        listAttandance = null;
                    }

                    foreach (MachineInfo s in listAttandance)
                        tw.WriteLine(_IpAddress + "*" + s.EmployeeID + "*" + s.PunchDate.ToString("MM-dd-yyyy") + "*" + DateTime.Parse(s.PunchTime).ToString("HH:mm:ss"));
                }
            }
            catch (SqlException ex)
            {
                Error = ex.Message;
                sqlTransaction.Rollback();
            }
            finally
            {
                if (osqlConnection.State == ConnectionState.Open)
                {
                    osqlConnection.Close();
                }
            }
            try
            {
                osqlConnection.Open();
                SqlCommand osqlCommand = new SqlCommand();
                osqlCommand.Connection = osqlConnection;
                osqlCommand.CommandType = CommandType.Text;
                osqlCommand.CommandText = "UPDATE TRN_SM_TimeConfigSettings_t SET LastUpdateTime= '" + machineInfonew.PunchTime + "' WHERE IP='" + _IpAddress + "'";
                osqlCommand.ExecuteNonQuery();

            }
            catch (SqlException ex)
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
            return Error;
        }
    }
}
