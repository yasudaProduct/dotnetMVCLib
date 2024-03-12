namespace Merino.config
{

    public class AppSetteings
    {
        public string Env { get; set; }

        public SettingFile SettingFile { get; set; }
    }

    public class SettingFile
    {
        public string Database { get; set; }
    }

    public class DatabaseSetting
    {
        public DataSource DataSource { get; set; }
        
    }

    public class DataSource
    {
        public string ConnectionString { get; set; }

        public string CreateDateColum { get; set; }

        public string UpdateDateColum { get; set; }

    }



}
