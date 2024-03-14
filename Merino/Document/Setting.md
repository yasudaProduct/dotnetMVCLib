### 設定ファイル

|・[概要](#概要)<br>・[merino.json](#merino.json)<br>・[database.json](#database.json)|
|:-|

#### 概要
resource/templateフォルダにある設定ファイルをWebプロジェクトのconfigフォルダへコピー

<br><br>
<h4 id="merino.json">merino.json</h4>
フレームワークに必要な設定を記載する

```json:merino.json
{
  "MerinoSettings": {
    "SettingFile": {
      "Database": "database.json"
    },
    "InjectionAssembly": [
      {
        "AssemblyName": "SampleApp.Application",
        "EndMatchNames": [
          "Service"
        ]
      },
      {
        "AssemblyName": "SampleApp.Infrastructure",
        "EndMatchNames": [
          "Repository"
        ],
        "ExactMatchNames": [
          "InMemoryUserRepository"
        ]
      },
    ],
    "CustomSettingFile": [
      {
        "AssemblyName": "WebMvcSampleLibrary",
        "ClassName": "WebMvcSampleLibrary.Util.Test1SettingFile",
        "FileName": "test1.json"
      }
    ],
    "Env": 1,
    "EnvSetting": [
      {
        "EnvCls": 1,
        "EnvName": "local"
      },
      {
        "EnvCls": 2,
        "EnvName": "develop"
      },
      {
        "EnvCls": 3,
        "EnvName": "st"
      },
      {
        "EnvCls": 4,
        "EnvName": "prod"
      }
    ]
  }
}
```

|項目|内容|必須|
| ---- | ---- | ---- |
|SettingFile|フレームワーク既定の設定ファイル名を記載|〇|
|SettingFile.Database |データベース関連設定|〇|
|SettingFile.CustomSettingFile |独自設定ファイル|×|
|Env |環境|×|
|EnvSetting |環境設定|×|

InjectionAssembly

|項目|内容|必須|
| ---- | ---- | ---- |
|AssemblyName |アセンブリ名|○|
|EndMatchNames|検索文字列(部分一致)||
|ExactMatchNames|検索文字列(完全一致)||


CustomSettingFile

|項目|内容|必須|
| ---- | ---- | ---- |
|AssemblyName |アセンブリ名|〇|
|ClassName |バインドするクラスの名前空間含めた名前|〇|
|FileName |ファイル名|〇|

EnvSetting

|項目|内容|必須|
| ---- | ---- | ---- |
|EnvCls |環境区分|〇|
|EnvName |環境名|〇|


<br><br>
<h4 id="database.json">database.json</h4>

```json:database.json
{
  "DatabaseSetting": {
    "DataSources": {
      "DataSource": [
        {
          "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=WebMvcSample.Data;Trusted_Connection=True;MultipleActiveResultSets=true",
          "EntityFramework": {
            "Valid": true,
            "UseDatabase": "SqlServer",
            "ContextAssemblyName": "WebMvcSampleLibrary",
            "ContextClassName": "WebMvcSampleLibrary.Data.WebMvcSampleContext"
          }
        },
        {
          "ConnectionString": "test",
          "EntityFramework": {
            "Valid": true,
            "UseDatabase": "PostgreSQL",
            "ContextAssemblyName": "WebMvcSampleLibrary",
            "ContextClassName": "WebMvcSampleLibrary.Data.WebMvcSampleContext"
          }
        }
      ]
    }
  }
}
```

|項目|内容|必須|
| ---- | ---- | ---- |
|DataSource | |〇|
|DataSources.ConnectionStrig|データベースへの接続文字列|〇|
|DataSources.EntityFramework|EntityFrameworkを使用する場合の設定|×|

EntityFramework

|項目|内容|必須|
| ---- | ---- | ---- |
|Valid|有効(true)/無効(false)|〇|
|UseDatabase|データベースプロパイダ※|〇|
|ContextAssemblyName|Contextクラスを含むアセンブリ名|〇|
|ContextClassName|Contextクラス名|〇|

※使用可能なDBプロパイダの種類はDbProvider列挙体を参照