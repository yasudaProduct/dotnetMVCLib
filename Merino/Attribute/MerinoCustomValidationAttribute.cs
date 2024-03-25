using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Merino.Attribute
{

    /// <summary>
    /// メールアドレス属性を表すカスタムバリデーション属性です。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class MerinoEmailAddressAttribute : MerinoDataTypeAttribute
    {

        static string ErrorMageage;

        /// <summary>
        /// MerinoEmailAddressAttribute クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="resouceName">リソース名。</param>
        public MerinoEmailAddressAttribute(string resouceName = null)
            : base(DataType.EmailAddress)
        {
            ErrorMageage = GetResouceMesseage(resouceName);
        }

        /// <summary>
        /// 指定された値が有効かどうかを検証します。
        /// </summary>
        /// <param name="value">検証する値。</param>
        /// <param name="validationContext">検証コンテキスト。</param>
        /// <returns>検証結果。</returns>
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var email = "";

            try
            {
                if (value != null && value.GetType() == typeof(string))
                {
                    email = Regex.Replace(value.ToString(), @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));


                    string DomainMapper(Match match)
                    {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        string domainName = idn.GetAscii(match.Groups[2].Value);

                        return match.Groups[1].Value + domainName;
                    }
                }

            }
            catch (RegexMatchTimeoutException e)
            {

                return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
            }
            catch (ArgumentException e)
            {
                return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
            }

            try
            {
                var result = Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                if (result) return ValidationResult.Success;

            }
            catch (RegexMatchTimeoutException)
            {
                return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
            }

            return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
        }
    }

    /// <summary>
    /// 必須属性を表すカスタムバリデーション属性です。
    /// </summary>
    public sealed class MerinoRequiredAttribute : MerinoValidationAttribute
    {

        static string ErrorMageage = null;

        /// <summary>
        /// MerinoRequiredAttribute クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="resouceName">リソース名。</param>
        public MerinoRequiredAttribute(string resouceName = null)
            : base()
        {
            ErrorMageage = GetResouceMesseage(resouceName);
        }

        /// <summary>
        /// 指定された値が有効かどうかを検証します。
        /// </summary>
        /// <param name="value">検証する値。</param>
        /// <param name="validationContext">検証コンテキスト。</param>
        /// <returns>検証結果。</returns>
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
            }

            if (value.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return new ValidationResult(GetMesseage(ErrorMageage, validationContext));
                }
            }

            return ValidationResult.Success;
        }
    }

}

