namespace Merino.Extensions
{
    public static class StringExtensions
    {

        public static bool IsFormatString(this string value)
        {
            if (value.Contains("{0}")) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
