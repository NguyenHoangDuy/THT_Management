using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace THT.Helpers
{
    public static class Helper
    {
        public static string ToVietnameseWithoutAccent(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = text.Normalize(System.Text.NormalizationForm.FormD);
                string result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                return result;
            }
            return text;
        }
    }
}