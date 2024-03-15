using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Merino.Adapter
{
    internal class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private const string RESOURCE_KEY_PREFIX = "Validator_";

        /// <summary>
        /// 指定された ValidationAttribute の IAttributeAdapter を返します。
        /// </summary>
        /// <param name="attribute">IAttributeAdapter を作成するための ValidationAttribute。</param>
        /// <param name="stringLocalizer">メッセージの作成に使用される IStringLocalizer。</param>
        /// <returns>指定された属性の IAttributeAdapter。</returns>
        IAttributeAdapter IValidationAttributeAdapterProvider.GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            IValidationAttributeAdapterProvider _fallback = new ValidationAttributeAdapterProvider();

            if (attribute.ErrorMessageResourceName != null) return _fallback.GetAttributeAdapter(attribute, stringLocalizer);

            Type attrType = attribute.GetType();

            var key = RESOURCE_KEY_PREFIX + attrType.Name;

            var getString = stringLocalizer[key];

            if (key != getString && attribute.ErrorMessage != getString)
            {
                attribute.ErrorMessage = getString;
            }

            return _fallback.GetAttributeAdapter(attribute, stringLocalizer);
        }
    }
}
