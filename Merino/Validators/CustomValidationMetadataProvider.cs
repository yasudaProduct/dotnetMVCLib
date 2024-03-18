using Merino.Resources;
using System.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

//https://www.nextdoorwith.info/wp/se/asp-net-core-validation-default-japanese-message/
namespace Merino.Validators
{

    public class CustomValidationMetadataProvider : IValidationMetadataProvider
    {
        private const string RESOURCE_KEY_PREFIX = "Validator_";

        private ResourceManager resourceManager;

        private Type resourceType;

        private Dictionary<Type, string> defaultMessageDic;

        public CustomValidationMetadataProvider()
        {

            resourceType = typeof(Resource);
            string baseName = resourceType.FullName ?? string.Empty;
            Assembly ass = resourceType.GetTypeInfo().Assembly;
            resourceManager = new ResourceManager(baseName, ass);

            var dic = new Dictionary<Type, string>();
            dic.Add(typeof(CreditCardAttribute), @"The {0} field is not a valid credit card number.");
            dic.Add(typeof(EmailAddressAttribute), @"The {0} field is not a valid e-mail address.");
            dic.Add(typeof(PhoneAttribute), @"The {0} field is not a valid phone number.");
            dic.Add(typeof(UrlAttribute), @"The {0} field is not a valid fully-qualified http, https, or ftp URL.");
            this.defaultMessageDic = dic;
        }

        public void CreateValidationMetadata(
        ValidationMetadataProviderContext context)
        {
            var metaData = context.ValidationMetadata.ValidatorMetadata;

            // int/Decimal/DateTime等の値型の場合、
            // 暗黙的に必須属性が追加されるので、そのメッセージも置き換え
            if (context.Key.ModelType.GetTypeInfo().IsValueType &&
            metaData.Where(m => m.GetType() == typeof(RequiredAttribute)).Count() == 0)
            {
                metaData.Add(new RequiredAttribute());
            }

            foreach (var obj in metaData)
            {
                if (!(obj is ValidationAttribute attr))
                {
                    continue;
                }

                // メッセージが変更されている場合はそれを優先
                if (attr.ErrorMessageResourceName != null)
                {
                    continue;
                }
                Type type = attr.GetType();
                string message = attr.ErrorMessage;
                string? defaultMessage = this.defaultMessageDic.GetValueOrDefault(type);
                if (!string.Equals(message, defaultMessage))
                {
                    continue;
                }

                // メッセージが既定から変更されておらず、
                // 対応するメッセージが未定義の場合は既定の動作に任せる
                string name = RESOURCE_KEY_PREFIX + type.Name;
                string? newMessage = resourceManager.GetString(name);
                if (string.IsNullOrEmpty(newMessage))
                {
                    continue;
                }

                // メッセージが既定から変更されておらず、
                // 対応するメッセージが定義されている場合、それで上書き
                attr.ErrorMessageResourceType = resourceType;
                attr.ErrorMessageResourceName = name;
                attr.ErrorMessage = null;
            }
        }

    }
}
