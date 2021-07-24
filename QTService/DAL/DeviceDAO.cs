
using QTService.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QTService.DAL
{
    public class DeviceDAO
    {

        private static volatile DeviceDAO instance;
        private static readonly object lockObj = new object();
        private SqlConnection osqlConnection;
        public static DeviceDAO GetInstance()
        {
            if (instance == null)
            {
                instance = new DeviceDAO();
            }
            return instance;
        }
        public static DeviceDAO GetInstanceThreadSafe
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new DeviceDAO();
                        }
                    }
                }
                return instance;
            }
        }


        public DeviceDAO()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["DbCon"];
            // If found, return the connection string.
            if (settings != null)
                osqlConnection = new SqlConnection(settings.ConnectionString);
        }
        public List<DeviceEntity> GetDynamic(string whereCondition, string orderByExpression)
        {
            try
            {
                List<DeviceEntity> DeviceEntityLst = new List<DeviceEntity>();

                osqlConnection.Open();
                string osqlCommandText = "SELECT * FROM TRN_SMkneet_t " + whereCondition;
                SqlCommand osqlCommand = new SqlCommand(osqlCommandText, osqlConnection);
                SqlDataReader osqlDataReader = osqlCommand.ExecuteReader();

                while (osqlDataReader.Read())
                {
                    DeviceEntity deviceEntity = new DeviceEntity();
                    deviceEntity.ID = osqlDataReader.GetInt32(0);
                    deviceEntity.MachineId = osqlDataReader.GetString(1);
                    deviceEntity.IpAddress = osqlDataReader.GetString(2);
                    deviceEntity.Port = osqlDataReader.GetString(3);
                    deviceEntity.Address = osqlDataReader.GetString(4);
                    deviceEntity.IsActive = osqlDataReader.GetBoolean(5);

                    DeviceEntityLst.Add(deviceEntity);
                }

                return DeviceEntityLst;
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
        public bool Add(DeviceEntity _DeviceEntity)
        {
            bool isSaved = false;
            try
            {
                osqlConnection.Open();
                SqlCommand osqlCommand = new SqlCommand();
                osqlCommand.Connection = osqlConnection;
                osqlCommand.CommandType = CommandType.Text;
                osqlCommand.CommandText = "INSERT INTO TRN_SMkneet_t VALUES('" + _DeviceEntity.MachineId + "', '" + _DeviceEntity.IpAddress + "', '" + _DeviceEntity.Port + "','" + _DeviceEntity.Address + "','" + (_DeviceEntity.IsActive) + "') ";
                osqlCommand.ExecuteNonQuery();
                isSaved = true;
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
            return isSaved;
        }
        public bool Update(DeviceEntity _DeviceEntity)
        {

            bool isSaved = false;
            try
            {
                osqlConnection.Open();
                SqlCommand osqlCommand = new SqlCommand();
                osqlCommand.Connection = osqlConnection;
                osqlCommand.CommandType = CommandType.Text;
                osqlCommand.CommandText = "UPDATE  TRN_SMkneet_t SET IpAddress= '" + _DeviceEntity.IpAddress + "',port= '" + _DeviceEntity.Port + "',Address='" + _DeviceEntity.Address + "',IsActive='" + (_DeviceEntity.IsActive) + "' WHERE MachineId='" + _DeviceEntity.MachineId + "'";
                osqlCommand.ExecuteNonQuery();
                isSaved = true;
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
            return isSaved;

        }
    }

}
