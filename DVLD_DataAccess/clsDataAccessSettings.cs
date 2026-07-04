namespace DVLD_DataAccess
{
    public class clsDataAccessSettings
    {
        public static string LocalDataBaseName { get; } = "DVLD_Database.mdf";
        public static string ServerDataBaseName { get; } = "DVLD";
        public static string SQLServerConnectionString { get; } = $"Server=.;Database={ServerDataBaseName};User=sa;Password=123456";
        public static string SQLLocalDBConnectionString { get; } = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database\{LocalDataBaseName};Integrated Security=True;Connect Timeout=30;";
        public static string ConnectionString { get; } = SQLServerConnectionString;
    }
}
