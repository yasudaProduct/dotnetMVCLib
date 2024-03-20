using Merino.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace Merino.Controller
{

    [CustomExceptionFilter]
    [TypeFilter(typeof(AccessLogFilter))]
    public class MerinoController : Microsoft.AspNetCore.Mvc.Controller
    {

        public MerinoController()
        {
        }

        protected string UserId
        {
            get
            {
                return User.Claims.Where(x => x.Type == "UserId").FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// 認証cookieをレスポンスに追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        async protected void AddCookie<T>(T dto)
        {
            List<Claim> claimList = new List<Claim>();

            var t = typeof(T);
            foreach ( var f in t.GetProperties())
            {
                var name = f.Name;
                var value = f.GetValue(dto);

                // TODO 他の型も対応するか
                if (value != null && (value is string))
                {
                    Claim claim = new Claim(name, (string)value);
                    claimList.Add(claim);
                };
            }

            //TODO cookieの有効期限

            var identity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            //認証cookieをレスポンスに追加
            await HttpContext.SignInAsync(principal);

        }

        /// <summary>
        /// セッションに保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        async protected void AddSettion(string key,string value)
        {
            //セッション保存
            HttpContext.Session.SetString(key, value);
        }
        /// <summary>
        /// セッションに保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="type"></param>
        async protected void AddSettion<T>(string key, T type)
        {
            //セッション保存
            HttpContext.Session.Set(key, type);
        }
        /// <summary>
        /// セッションから取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string or null</returns>
        protected string GetSettion(string key)
        {
            //セッション保存
            return HttpContext.Session.GetString(key);
        }
        /// <summary>
        /// セッションから取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T GetSession<T>(string key)
        {
            var type = typeof(T);
            //セッション取得
            return HttpContext.Session.Get<T> (key);
        }
    }
}
