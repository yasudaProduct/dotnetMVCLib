using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


public static class SessionExtensions
{
    /// <summary>
    /// セッションに値を設定します。
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="session">セッションオブジェクト</param>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// セッションから値を取得します。
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="session">セッションオブジェクト</param>
    /// <param name="key">キー</param>
    /// <returns>セッションから取得した値</returns>
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
