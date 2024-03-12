using static Merino.Const.FWConst;

namespace Merino.Settings
{

    /// <summary>
    /// merino.json
    /// </summary>
    public class MerinoSettings
    {

        public SettingFile? SettingFile { get; set; }

        public List<InjectionAssembly> InjectionAssembly { get; set; }

        public List<CustomSettingFile>? CustomSettingFile { get; set; }

        public string? Env { get; set; } = string.Empty;

        public List<EnvSetting>? EnvSetting { get; set; }

        public Web Web { get; set; }
    }

    public class SettingFile
    {
        public string Application { get; set; }
        public string Database { get; set; }
        public string Message { get; set; }
    }

    public class CustomSettingFile
    {
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string FileName { get; set; }
    }

    public class InjectionAssembly
    {
        public string AssemblyName { get; set; }
        public List<string> EndMatchNames { get; set; }
        public List<string> ExactMatchNames { get; set; }
    }

    //public class EndMatchNames
    //{
    //    public string EndMatchName { get; set; }
    //}

    //public class ExactMatchNames
    //{
    //    public string ExactMatchName { get; set; }
    //}

    public class EnvSetting
    {
        public string EnvCls { get; set; }

        public string EnvName { get; set; }
    }

    public class Web
    {
        public string CustomErrorPage { get; set; }

        public string RootUrlPattern { get; set; }
    }

    /// <summary>
    /// database.json
    /// </summary>
    public class DatabaseSetting
    {
        public DataSources DataSources { get; set; }

    }

    public class DataSources
    {
        public List<DataSource> DataSource { get; set; }

    }

    public class DataSource
    {
        public string ConnectionString { get; set; }

        public string CreateDateColum { get; set; }

        public string UpdateDateColum { get; set; }

        public EntityFramework EntityFramework { get; set; }

    }

    public class EntityFramework
    {
        public bool Valid { get; set; }

        public DbProvider UseDbProvider { get; set; }

        public string ContextAssemblyName { get; set; }

        public string ContextClassName { get; set; }

    }
}

