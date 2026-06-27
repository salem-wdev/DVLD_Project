namespace DVLD_DataAccess
{
    public class clsDataAccessSettings
    {
        public static string DataBaseName { get; } = "DVLD_Test";
        public static string ConnectionString { get; } = $"Server=.;Database={DataBaseName};User=sa;Password=123456";
    }
}
