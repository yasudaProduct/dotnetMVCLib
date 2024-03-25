using Merino.Extensions;
using Merino.Resources;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Merino.Attribute
{
    /// <summary>
    /// バリデーション属性を表すクラスです。
    /// </summary>
    public class MerinoValidationAttribute : ValidationAttribute
    {

        /// <summary>
        /// MerinoValidationAttribute クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MerinoValidationAttribute()
        : base()
        { }

        /// <summary>
        /// リソース名に基づいてメッセージを取得します。
        /// </summary>
        /// <param name="resouceName">リソース名</param>
        /// <returns>メッセージ</returns>
        protected string GetResouceMesseage(string resouceName)
        {
            if (resouceName == null)
            {
                //nullの場合は既定のmessageを返す
                return Resource.Validator_RequiredAttribute;
            }
            else
            {


                Type[] types = Assembly.GetEntryAssembly().GetTypes().Where(type => type.Name.EndsWith("MessageResource")).ToArray();

                foreach (Type type in types)
                {
                    //TODO キーが被っている場合どうするか
                    var method = type.GetMethods().Where(method => method.Name.Equals("get_" + resouceName) && method.IsStatic).FirstOrDefault();

                    if (method != null)
                    {
                        return (string)method.Invoke(null, null);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// メッセージとバリデーションコンテキストに基づいてメッセージを取得します。
        /// </summary>
        /// <param name="messeage">メッセージ</param>
        /// <param name="validationContext">バリデーションコンテキスト</param>
        /// <returns>メッセージ</returns>
        protected string GetMesseage(string messeage, ValidationContext validationContext)
        {
            if (messeage.IsFormatString() && validationContext.DisplayName != null)
            {
                return String.Format(messeage, validationContext.DisplayName);
            }
            else
            {
                return messeage;
            }
        }
    }
}
