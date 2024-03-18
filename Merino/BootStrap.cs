using Merino.Resources;
using Merino.Settings;
using Merino.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.Reflection;
using static Merino.Const.FWConst;

//TODO staticとインスタンスパフォーマンスどちらが良いか
//TODO 環境ごとの設定ファイルの切替えを起動時とビルド時どちらが良いか検討
//     →ビルド変数に合わせた方がよさそう。
//FW側でDBプロパイダに対応しようとすると不要なdllが配置される
// ⇒Useメソッドもリファレンス呼び出しにすることで、ContextがあるプロジェクトにプロパイダごとのEntityFrameworkを追加する


namespace Merino
{
    public class BootStrap
    {
        #region 定数
        /// <summary>
        /// 設定ファイル配置フォルダ名
        /// </summary>
        private const string CONF_FOLDER_NAME = "config";

        /// <summary>
        /// 設定ファイル名
        /// </summary>
        private const string CONF_FILE_NAME = "merino.json";

        /// <summary>
        /// Nlog conf filename
        /// </summary>
        private const string NLOG_CONF_FILE_NAME = "nlog.config";

        #endregion

        private static NLog.Logger _logger;

        private static MerinoSettings _setting;

        /// <summary>
        /// BuildWebApplication
        /// </summary>
        /// <param name="args"></param>
        /// <returns>WebApplicationBuilder</returns>
        public static WebApplicationBuilder BuildWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //NLog取得 当クラスのみで使用するインスタンス
            if (!File.Exists(Path.Combine(CONF_FOLDER_NAME, NLOG_CONF_FILE_NAME))) throw new DirectoryNotFoundException("設定ファイルが見つかりません。:" + Path.Combine(CONF_FOLDER_NAME, NLOG_CONF_FILE_NAME));
            _logger = NLogBuilder.ConfigureNLog(Path.Combine(CONF_FOLDER_NAME, NLOG_CONF_FILE_NAME)).GetCurrentClassLogger();
            _logger.Trace("▽MerinoWebApplication InitWebApplication▽");

            //Nlog登録
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.Host.UseNLog();

            //設定ファイル読み込み
            InitAppSettings(ref builder);

            //FW設定
            MerinoSettings setting = builder.Configuration.GetSection(nameof(MerinoSettings)).Get<MerinoSettings>() ?? throw new ArgumentNullException(nameof(MerinoSettings));

            //独自の環境を設定
            //foreach(EnvSetting env in _setting.EnvSetting)
            //{
            //    if (_setting.Env == env.EnvCls)
            //    {
            //        builder.Environment.EnvironmentName = env.EnvName;
            //        break;
            //    }
            //}            

            //Database設定
            DatabaseSetting dbSetting = builder.Configuration.GetSection(nameof(DatabaseSetting)).Get<DatabaseSetting>();

            if (dbSetting != null)
            {

                //EntityFrameworkが有効なDataSource設定を取得
                var dataSourceList = dbSetting.DataSources.DataSource.Where(m => m.EntityFramework != null).ToList();
                if (dataSourceList != null && dataSourceList.Count >= 0)
                {
                    //EntityFramework設定
                    InitDbContext(dataSourceList, ref builder);
                }
            }

            //依存注入
            if (setting.InjectionAssembly != null)
            {
                InjectionClass(setting.InjectionAssembly, ref builder);
            }

            //セッション
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "MerinoSession";
            });

            builder.Services.AddControllersWithViews(op =>
            {

                op.ModelMetadataDetailsProviders.Add(new CustomValidationMetadataProvider());

                //モデルバインディングメッセージの日本語化
                Func<string, string, string> f1 = (f, a1) => string.Format(f, a1);
                Func<string, string, string, string> f2 = (f, a1, a2) => string.Format(f, a1, a2);

                var mp = op.ModelBindingMessageProvider;
                mp.SetAttemptedValueIsInvalidAccessor((x, y) =>
                    f2(Resource.ModelBinding_AttemptedValueIsInvalid, x, y));
                mp.SetMissingBindRequiredValueAccessor((x) =>
                    f1(Resource.ModelBinding_MissingBindRequiredValue, x));
                mp.SetMissingKeyOrValueAccessor(() =>
                    Resource.ModelBinding_MissingKeyOrValue);
                mp.SetMissingRequestBodyRequiredValueAccessor(() =>
                    Resource.ModelBinding_MissingRequestBodyRequiredValue);
                mp.SetNonPropertyAttemptedValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_NonPropertyAttemptedValueIsInvalid, x));
                mp.SetNonPropertyUnknownValueIsInvalidAccessor(() =>
                    Resource.ModelBinding_NonPropertyUnknownValueIsInvalid);
                mp.SetNonPropertyValueMustBeANumberAccessor(() =>
                    Resource.ModelBinding_NonPropertyValueMustBeANumber);
                mp.SetUnknownValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_UnknownValueIsInvalid, x));
                mp.SetValueIsInvalidAccessor((x) =>
                    f1(Resource.ModelBinding_ValueIsInvalid, x));
                mp.SetValueMustBeANumberAccessor((x) =>
                    f1(Resource.ModelBinding_ValueMustBeANumber, x));
                mp.SetValueMustNotBeNullAccessor((x) =>
                    f1(Resource.ModelBinding_ValueMustNotBeNull, x));
            });

            return builder;
        }

        /// <summary>
        /// CreateWebApplication
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>WebApplication</returns>
        public static WebApplication CreateWebApplication(WebApplicationBuilder builder)
        {
            _logger.Trace("▽MerinoWebApplication CreateWebApplication▽");

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(_setting.Web.CustomErrorPage);
            }

            var test = app.Environment.EnvironmentName;
            //var environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: _setting.Web.RootUrlPattern
                );

            _logger.Trace("△MerinoWebApplication CreateWebApplication△");
            return app;
        }

        /// <summary>
        /// RunWebApplication
        /// </summary>
        /// <param name="app"></param>
        public static void RunWebApplication(WebApplication app)
        {
            try
            {
                app.Run();
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        #region private methods

        #region 設定ファイル初期化
        /// <summary>
        /// 設定ファイル初期化
        /// </summary>
        private static void InitAppSettings(ref WebApplicationBuilder builder)
        {
            _logger.Trace("▽MerinoWebApplication InitAppSettings▽");

            //merino.json
            if (!File.Exists(Path.Combine(CONF_FOLDER_NAME, CONF_FILE_NAME))) throw new DirectoryNotFoundException("設定ファイルが見つかりません。:" + Path.Combine(CONF_FOLDER_NAME, CONF_FILE_NAME));
            builder.Configuration.AddJsonFile(Path.Combine(CONF_FOLDER_NAME, CONF_FILE_NAME), optional: false, reloadOnChange: true);
            builder.Services.Configure<MerinoSettings>(builder.Configuration.GetSection(nameof(MerinoSettings)));
            _setting = builder.Configuration.GetSection(nameof(MerinoSettings)).Get<MerinoSettings>() ?? throw new ArgumentNullException(nameof(MerinoSettings));
            SettingFile settingFile = _setting.SettingFile;

            //環境取得
            string? env = _setting.Env;
            List<EnvSetting>? envSetting = _setting.EnvSetting;
            string? envName = null;
            if (env != null && envSetting != null) envName = envSetting.Find(v => v.EnvCls == env).EnvName;

            //database.json
            string databaseFileName = "";
            if (settingFile != null && !string.IsNullOrEmpty(settingFile.Database))
            {
                databaseFileName = Path.Combine(CONF_FOLDER_NAME, EnvSettingFileName(envName, settingFile.Database));
                builder.Configuration.AddJsonFile(databaseFileName, optional: false, reloadOnChange: true);
                builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection(nameof(DatabaseSetting)));
            }

            //アプリケーション固有設定ファイル読込
            List<CustomSettingFile> customSettingFiles = builder.Configuration.GetSection(nameof(MerinoSettings) + ":" + nameof(CustomSettingFile)).Get<List<CustomSettingFile>>();
            if (customSettingFiles != null)
            {
                foreach (CustomSettingFile item in customSettingFiles)
                {
                    //アセンブリ取得
                    Assembly assembly = Assembly.Load(item.AssemblyName);

                    //対象のクラスを取得
                    Type classType = assembly.GetType(item.ClassName);

                    if (classType == null)
                    {
                        // クラスが見つからない場合のエラー処理
                        throw new InvalidOperationException();
                    }

                    string fileName = Path.Combine(CONF_FOLDER_NAME, EnvSettingFileName(envName, item.FileName));

                    var instance = Activator.CreateInstance(classType);
                    builder.Configuration.AddJsonFile(fileName, optional: true, reloadOnChange: true);
                    builder.Configuration.GetSection(classType.Name).Bind(instance);
                    builder.Services.AddSingleton(classType, instance);

                }
            }

            _logger.Trace("△MerinoWebApplication InitAppSettings△");
        }
        #endregion

        #region EntityFrameWork設定
        /// <summary>
        /// EntityFrameWork設定
        /// </summary>
        private static void InitDbContext(List<DataSource> settingList, ref WebApplicationBuilder builder)
        {
            _logger.Info("▽MerinoWebApplication InitDbContext▽");

            #region リファレンス定数
            const string ADD_DB_CONTEXT_METHOD_NAME = "AddDbContext";

            // TODO 定数を切り替えて共通化する
            //SqlServer
            const string SQLSERVER_CLASS_NAME = "Microsoft.EntityFrameworkCore.SqlServer";
            const string SQLSERVER_EXTENSIONS_CLASS_NAME = "SqlServerDbContextOptionsExtensions";
            const string SQLSERVER_USE_METHOD_NAME = "UseSqlServer";

            //PostgreSQL
            const string NPGSQL_CLASS_NAME = "Npgsql.EntityFrameworkCore.PostgreSQL";
            const string NPGSQL_EXTENSIONS_CLASS_NAME = "NpgsqlDbContextOptionsBuilderExtensions";
            const string NPGSQL_USE_METHOD_NAME = "UseNpgsql";

            //PostgreSQL
            const string INMEMORY_CLASS_NAME = "Microsoft.EntityFrameworkCore.InMemory";
            const string INMEMORY_EXTENSIONS_CLASS_NAME = "InMemoryDbContextOptionsExtensions";
            const string INMEMORY_USE_METHOD_NAME = "UseInMemoryDatabase";
            #endregion


            foreach (DataSource setting in settingList)
            {

                if (setting.EntityFramework == null || !setting.EntityFramework.Valid) throw new ArgumentNullException(nameof(setting.EntityFramework));

                //dbContextアセンブリ取得
                Assembly assembly = Assembly.Load(setting.EntityFramework.ContextAssemblyName);
                var dbContextType = assembly.GetType(setting.EntityFramework.ContextClassName);
                if (dbContextType == null)
                {
                    throw new InvalidOperationException();
                }

                //DbContextOptionsBuilder<>インスタンスを取得
                var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
                var optionsBuilder = Activator.CreateInstance(optionsBuilderType);

                //DBプロパイダに合わせてアクションを作成
                Action<DbContextOptionsBuilder>? action = null;
                switch (setting.EntityFramework.UseDbProvider)
                {
                    case DbProvider.SqlServer:
                        //Assemblyから拡張メソッドを取得
                        //UseSqlServer実行Action作成 AddDbContextの引数用
                        action = delegate (DbContextOptionsBuilder op) { op.UseSqlServer(setting.ConnectionString); };
                        break;

                    case DbProvider.PostgreSQL:
                        //Assemblyから拡張メソッドを取得
                        Assembly NqgSqlAsmby = Assembly.Load(NPGSQL_CLASS_NAME);
                        var useNpgsqlMethod = NqgSqlAsmby.GetTypes().FirstOrDefault(t => t.Name == NPGSQL_EXTENSIONS_CLASS_NAME)
                            .GetMethods().FirstOrDefault(m =>
                            m.Name == NPGSQL_USE_METHOD_NAME
                            && m.GetParameters().Length == 3
                            && m.GetParameters()[1].ParameterType == typeof(string));

                        //PostgreSQL実行Action作成 AddDbContextの引数用
                        //action = delegate (DbContextOptionsBuilder op) { op.UseNpgsql(setting.ConnectionString); };
                        action = delegate (DbContextOptionsBuilder op)
                        {
                            useNpgsqlMethod.Invoke(op, new object[] { op, setting.ConnectionString, null });
                        };

                        break;
                    case DbProvider.UseInMemoryDatabase:
                        Assembly InMemoryasmby = Assembly.Load(INMEMORY_CLASS_NAME);
                        var useInMemoryMethod = InMemoryasmby.GetTypes().FirstOrDefault(t => t.Name == INMEMORY_EXTENSIONS_CLASS_NAME)
                            .GetMethods().FirstOrDefault(m =>
                            m.Name == INMEMORY_USE_METHOD_NAME
                            && m.GetParameters().Length == 3
                            && m.GetParameters()[1].ParameterType == typeof(string));

                        action = delegate (DbContextOptionsBuilder op) { op.UseInMemoryDatabase("InMemory"); };
                        //action = delegate (DbContextOptionsBuilder op) {
                        //    useInMemoryMethod.Invoke(op, new object[] { op, "InMemory", null });
                        //};
                        break;
                    default: throw new InvalidOperationException();
                }

                //AddDbContextメソッドを取得
                var addDbContextMethod = typeof(EntityFrameworkServiceCollectionExtensions)
                     .GetMethods().FirstOrDefault(m => m.Name == ADD_DB_CONTEXT_METHOD_NAME && m.IsGenericMethod)
                     .MakeGenericMethod(dbContextType);
                //AddDbContextを実行
                addDbContextMethod.Invoke(builder.Services, new object[] { builder.Services, action, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
            }

            _logger.Info("△MerinoWebApplication InitDbContext△");
        }
        #endregion

        #region Injection処理
        /// <summary>
        /// Injection処理
        /// </summary>
        /// <param name="settingList"></param>
        /// <param name="builder"></param>
        private static void InjectionClass(List<InjectionAssembly> settingList, ref WebApplicationBuilder builder)
        {
            _logger.Info("▽MerinoWebApplication InjectionClass▽");

            // interfaceがclassesにないので、おそらくinterfaceではなく実装クラス名を指定する必要がある？
            // AsMatchingInterfaceとしてるので、それに紐づくインターフェイスがinjectionされるのか。
            // IUserTestRepositoryに変更したらinjectionされてない模様。
            // ⇒interface名と実装クラス名を合わせる必要がある？
            // 　命名規則が必要だったら実装切り替えられないじゃん..
            //   ⇒AsImplementedInterfacesにしたら行けた！

            // migrationしたら起動時にエラーになる。
            // ⇒指定したアセンブリ内の依存関係全て検索していたので、どこかのクラス名にRepositoryが含むのがヒットし
            // 　AsMatchingInterfaceとしているので、マッチするinterfaceが見つからず。生成出来なかった

            //　命名規則を設けてEndsWithでDIしようとすると、
            //　IUserRepositoryの実装でPostgresUserRepositoryとInMemoryRepositoryを作成している場合、
            //　両方DIされているか、デフォルトで後勝ち。 実装の切り替えが出来ない。
            //　実装するクラスをフルで設定ファイルに記載するようにしたほうがよさそう。

            //①アセンブリ指定スキャン
            //②EndWithNameで検索 (Repository) ⇒ PostgresSqlUserRepositoryとInMemoryUserRepositoryが登録される
            //③exact matchで検索 (実装にしたいInMemoryUserRepository) ⇒ InMemoryUserRepositoryが登録される
            //④Replace(Scrutor.ReplacementBehavior.ServiceType)でサービスタイプを置き換える ⇒ PostgresSqlUserRepositoryが置換される

            //依存注入 https://github.com/khellang/Scrutor/tree/master
            //https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/#registering-services-which-match-a-standard-naming-convention

            foreach (InjectionAssembly injectionSetting in settingList)
            {

                List<string> conditions = new List<string>();
                if (injectionSetting.EndMatchNames != null) conditions = injectionSetting.EndMatchNames.ToList<string>();
                if (injectionSetting.ExactMatchNames != null) conditions.AddRange(injectionSetting.ExactMatchNames.ToList<string>());

                if (conditions.Count == 0) return;

                Assembly assembly = Assembly.Load(injectionSetting.AssemblyName);

                foreach (string cond in conditions)
                {
                    builder.Services.Scan(scan =>
                    scan.FromAssemblyDependencies(assembly)
                    .AddClasses(classes => classes.Where(type =>
                    type.Assembly == assembly && type.Name.EndsWith(cond)))
                    .UsingRegistrationStrategy(Scrutor.RegistrationStrategy.Replace(Scrutor.ReplacementBehavior.ServiceType))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
                }
            }

            _logger.Info("△MerinoWebApplication InjectionClass△");
        }
        #endregion

        #region 設定ファイル名の組み立て
        /// <summary>
        /// 設定ファイル名の組み立て
        /// </summary>
        /// <remarks>
        /// 環境名に合わせて設定ファイル名を作成して返す
        /// </remarks>
        /// <param name="envName">環境名</param>
        /// <param name="baseFileName">設定ファイル名</param>
        /// <returns>設定ファイル名</returns>
        private static string EnvSettingFileName(string? envName, string baseFileName)
        {
            return string.IsNullOrEmpty(envName)
                ? baseFileName
                : string.Join(".", baseFileName.Split(".").First(), envName, baseFileName.Split(".").Last());
        }
        #endregion

        /// <summary>
        /// クラス名一致チェック
        /// </summary>
        /// <param name="type"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private static bool CheckTypeCondition(Type type, string[] conditions)
        {
            bool result = false;
            foreach (string cond in conditions)
            {
                if (type.Name.EndsWith(cond))
                {
                    result = true;
                    break;
                };
            }
            return result;
        }

        #endregion
    }
}
