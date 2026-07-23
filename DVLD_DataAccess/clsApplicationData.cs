using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public class clsApplicationData
    {
        public static bool GetApplicationInfoByApplicationID
            (int ApplicationID, ref int ApplicantPersonID,
            ref DateTime ApplicationDate, ref int ApplicationTypeID,
            ref byte ApplicationStatus, ref DateTime LastStatusDate,
            ref decimal PaidFees, ref int CreatedByUserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "SELECT [ApplicantPersonID]" +
                                    ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                    ",[CreatedByUserID]" +
                                   " FROM [Applications]" +
                                   " WHERE [ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ApplicantPersonID = Convert.ToInt32(reader["ApplicantPersonID"]);
                                ApplicationDate = (DateTime)reader["ApplicationDate"];
                                ApplicationTypeID = Convert.ToInt32(reader["ApplicationTypeID"]);
                                ApplicationStatus = Convert.ToByte(reader["ApplicationStatus"]);
                                LastStatusDate = (DateTime)reader["LastStatusDate"];
                                PaidFees = Convert.ToDecimal(reader["PaidFees"]);
                                CreatedByUserID = Convert.ToInt32(reader["CreatedByUserID"]);

                                IsFound = true;
                            }
                            else
                            {
                                IsFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed

            }
            return IsFound;
        }

        public static int AddNewApplication(int ApplicantPersonID,
            ref DateTime ApplicationDate, int ApplicationTypeID,
            ref byte ApplicationStatus, ref DateTime LastStatusDate,
            decimal PaidFees, int CreatedByUserID)
        {

            int ApplicationID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {


                    string Query = "INSERT INTO [Applications] " +
                        "([ApplicantPersonID], [ApplicationDate], [ApplicationTypeID], [ApplicationStatus], [LastStatusDate], [PaidFees], [CreatedByUserID]) " +
                        "OUTPUT INSERTED.ApplicationID, INSERTED.ApplicationDate, INSERTED.LastStatusDate, INSERTED.ApplicationStatus " +
                        "VALUES " +
                        "(@ApplicantPersonID, GETDATE(), @ApplicationTypeID, 1, GETDATE(), @PaidFees, @CreatedByUserID);";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ApplicationID = Convert.ToInt32(reader["ApplicationID"]);
                                ApplicationDate = (DateTime)reader["ApplicationDate"];
                                LastStatusDate = (DateTime)reader["LastStatusDate"];
                                ApplicationStatus = Convert.ToByte(reader["ApplicationStatus"]);
                            }
                            else
                            {
                                ApplicationID = -1;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {

            }

            return ApplicationID;

        }

        public static bool UpdateApplication(int ApplicationID,
            decimal PaidFees, int CreatedByUserID)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "UPDATE [Applications] " +
                        "SET[PaidFees] = @PaidFees, " +
                        "[CreatedByUserID] = @CreatedByUserID " +
                        "WHERE[ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {

                        command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);



                        connection.Open();
                        NumberOfEffectedRows = command.ExecuteNonQuery();
                    }
                }
               
            }
            catch (Exception ex)
            {

            }

            return NumberOfEffectedRows > 0;

        }


        public static DataTable GetApplicationsPersonList(int ApplicantPersonID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "SELECT [ApplicationID]" + ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                    ",[CreatedByUserID]" +
                                   " FROM [Applications]" +
                                   " WHERE [ApplicantPersonID] = @ApplicantPersonID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                   
            }
            catch (Exception ex)
            {

                
            }

            return dt;

        }

        public static DataTable GetApplicationsCreatedByUserList(int CreatedByUserID)
        {
            DataTable dt = new DataTable();
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "SELECT [ApplicationID]" +
                                    ",[ApplicantPersonID]" +
                                    ",[ApplicationDate]" +
                                    ",[ApplicationTypeID]" +
                                    ",[ApplicationStatus]" +
                                    ",[LastStatusDate]" +
                                    ",[PaidFees]" +
                                   " FROM [Applications]" +
                                   " WHERE [CreatedByUserID] = @CreatedByUserID";


                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return dt;

        }

        public static bool IsApplicationExist(int ApplicationID)
        {
            bool IsFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string Query = "SELECT TOP 1 1 FROM Applications " +
                                     "WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        connection.Open();
                        object Scalar = command.ExecuteScalar();
                        IsFound = (Scalar != null); 
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return IsFound;
        }

        public static bool DoesPersonHaveActiveApplication(int PersonID, int ApplicationTypeID)
        {

            //incase the ActiveApplication ID !=-1 return true.
            return (GetActiveApplicationID(PersonID, ApplicationTypeID) != -1);
        }

        public static int GetActiveApplicationID(int PersonID, int ApplicationTypeID)
        {
            int ActiveApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = "SELECT TOP 1 ActiveApplicationID=ApplicationID FROM Applications WHERE ApplicantPersonID = @ApplicantPersonID and ApplicationTypeID=@ApplicationTypeID and ApplicationStatus=1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


                        connection.Open();
                        object result = command.ExecuteScalar();


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return ActiveApplicationID;
        }

        public static int GetActiveApplicationIDForLicenseClass(int PersonID, int ApplicationTypeID, int LicenseClassID)
        {
            int ActiveApplicationID = -1;
            try
            {

                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = @"SELECT TOP 1 ActiveApplicationID=Applications.ApplicationID  
                            From
                            Applications INNER JOIN
                            LocalDrivingLicenseApplications ON Applications.ApplicationID = LocalDrivingLicenseApplications.ApplicationID
                            WHERE ApplicantPersonID = @ApplicantPersonID 
                            and ApplicationTypeID=@ApplicationTypeID 
							and LocalDrivingLicenseApplications.LicenseClassID = @LicenseClassID
                            and ApplicationStatus=1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicantPersonID", PersonID);
                        command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        connection.Open();
                        object result = command.ExecuteScalar();


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        } 
                    }
                    
                }

            }
            catch (Exception ex)
            {

            }
            return ActiveApplicationID;
        }

        public static bool DeleteApplication(int ApplicationID)
        {
            int NumberOfEffectedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "DELETE FROM Applications " +
                        "WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        connection.Open();
                        NumberOfEffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return NumberOfEffectedRows > 0;
        }

        public static DataTable GetAllApplications()
        {
            DataTable Table = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string Query = "select * from ApplicationsList_View order by ApplicationDate desc";

                    using (SqlCommand Command = new SqlCommand(Query, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = Command.ExecuteReader())
                        {
                            Table.Load(reader);
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Table;
        }

        public static bool UpdateStatus(int ApplicationID, short NewStatus, DateTime LastStatusDate)
        {

            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = @"Update  Applications  
                            set 
                                ApplicationStatus = @NewStatus, 
                                LastStatusDate = @LastStatusDate
                            where ApplicationID=@ApplicationID;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@NewStatus", NewStatus);
                        command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);



                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                    }

                }

            }
            catch (Exception ex)
            {

            }
            return (rowsAffected > 0);
        }

        public static bool CanApplicationBeEdited(int ApplicationID)
        {
            bool Result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT TOP (1) 1 " +
                                "FROM Applications " +
                                "WHERE ApplicationID = @ApplicationID " +
                                "AND ApplicationStatus NOT IN(2, 3); ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        connection.Open();
                        object result = command.ExecuteScalar();

                        Result = result != null; 
                    }

                }
               
            }
            catch (Exception ex)
            {
            }

            return Result;

        }

        public static int GetApplicationTypeID(int ApplicationID)
        {
            int ApplicationTypeID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = "SELECT TOP (1) ApplicationTypeID " +
                        "FROM " +
                        "Applications WHERE ApplicationID = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        connection.Open();
                        object result = command.ExecuteScalar();


                        if (result != null && int.TryParse(result.ToString(), out int AppTypeID))
                        {
                            ApplicationTypeID = AppTypeID;
                        } 
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return ApplicationTypeID;
        }

        public static int GetApplicationStatus(int ApplicationID)
        {
            int ActiveApplicationID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = @"SELECT [ApplicationStatus]
                             FROM [Applications]
                             WHERE [ApplicationID] = @ApplicationID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);


                        connection.Open();
                        object result = command.ExecuteScalar();


                        if (result != null && int.TryParse(result.ToString(), out int AppID))
                        {
                            ActiveApplicationID = AppID;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }
            return ActiveApplicationID;
        }


    }
}
