# Merino

|・[概要](#概要)<br>・[アーキテクチャ](#アーキテクチャ)<br>・[WEBアプリケーション初期化](#WEBアプリケーション初期化)<br>・[依存注入](#依存注入)<br>・[ログ](#ログ)<br>・[例外](#例外)<br>・[セッション管理](#セッション管理)<br>|
|:-|

## 概要
一旦ASP.NET Core MVCの利用を前提にする。(ASP.NET Coreでもできるかも)
基本的な機能を提供し、アーキテクチャはプロジェクトごとに選定する。
当FWの方針としては前フレームフレームからの高低差を少なくするため、
各レイヤー、クラスの命名規則は寄せる。
・レイヤードアーキテクチャ
・クリーンアーキテクチャ
を参考に（詳しくは[アーキテクチャ](#アーキテクチャ)参照）

## アーキテクチャ
当フレームワークでは主にMVCモデルでのV(View)C(Controller)部分の機能を提供し、
M(Model)以降のアーキテクチャ構成についてはプロジェクトごとに選定する必要がある。
・トランザクションスクリプト
・ドメイン駆動開発
保守性、テスト容易性を高めることを目的とし、
完璧なDDDパターンよりも簡易かつ前フレームフレームとの高低差を


## Webアプリケーションの初期化

1. [設定ファイル](/Document/Setting.md)の準備

2. WEBプロジェクトの参照に追加
3. Program.csを修正

```C#:Program.cs
using Merino;

//アプリケーション初期化
WebApplicationBuilder builder = BootStrap.BuildWebApplication(args);

WebApplication app = BootStrap.CreateWebApplication(builder);

BootStrap.RunWebApplication(app);

```

## 依存注入
インジェクション拡張暮らすライブラリのScrutorを使する。 </br>
(Scrutor https://github.com/khellang/Scrutor/tree/master)

</br>
設定ファイルに記載されたアセンブリ内のクラス名から自動的に判断し、</br>
ScopedでInjectionを行う。</br>


## ログ
Nolgを使用


## Entity Framework

1-1.Contextを配置するプロジェクトにNuGetよりEntityFramework.Core、プロパイダごとのパッケージをインストール</br>
1-2.スタートアッププロジェクトにEntityFramework.Toolをインストール</br>
2.dbContextを継承したクラスを作成</br>
3.設定ファイルのEntityFrameworkの項目を記載</br>
4.マイグレーションの実行</br>

#### ・マイグレーションの実行

```
// マイグレーションファイル作成
cd [スタートアッププロジェクト]

dotnet ef migrations add [マイグレーションファイル名] --project [Contextを含むプロジェクト] --startup-project [実行プロジェクト] --context [実行するContext]

// DBを更新する
dotnet ef database update --context [実行するContext]

// データベースを削除する
dotnet ef database drop -f -v --context [実行するContext]
```

```
>dotnet ef migrations add Initial --project ..\WebMvcSampleLibrary\WebMvcSampleLibrary.csproj --startup-project .\WebMvcSample.csproj --context PostgresContext
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'

>dotnet ef database update --context PostgresContext
Build started...
Build succeeded.
Applying migration '20240119070959_Initial'.
Done.
```

## 例外

## メッセージ

## セッション管理

## キャッシュ管理

## 利用ライブラリ