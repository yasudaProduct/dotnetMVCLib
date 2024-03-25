using Merino.Extensions;
using Merino.Resources;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Merino.Attribute
{
    /// <summary>
    /// データタイプ属性クラス
    /// </summary>
    public class MerinoDataTypeAttribute : DataTypeAttribute
    {
        DataType _type;

        /// <summary>
        /// MerinoDataTypeAttribute クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">データタイプ</param>
        public MerinoDataTypeAttribute(DataType type)
        : base(type)
        { _type = type; }

        /// <summary>
        /// リソースメッセージを取得します。
        /// </summary>
        /// <param name="resouceName">リソース名</param>
        /// <returns>リソースメッセージ</returns>
        protected string GetResouceMesseage(string resouceName)
        {
            if (resouceName == null)
            {
                //nullの場合は既定のmessageを返す
                switch (_type)
                {
                    case DataType.EmailAddress:
                        return Resource.Validator_EmailAddressAttribute;

                    default:
                        return null;
                }

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
        /// メッセージを取得します。
        /// </summary>
        /// <param name="messeage">メッセージ</param>
        /// <param name="validationContext">検証コンテキスト</param>
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
