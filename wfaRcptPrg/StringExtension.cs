using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace wfaRcptPrg {
   //===== StringExtension.ContainsWord(s,word);
   //===== StringExtension.RemoveDuplicatesChars(word)
   //===== StringExtension.CapitalizeFirstLetter(s)
   //===== StringExtension.CapitalizeFirstLetterAllWords(s)
   //===== StringExtension.NodupWord(s) 

   public static class StringExtension {
      public static bool ContainsWord(this string strLine, string word) {
         if (Regex.IsMatch(strLine, "\\b" + word + "\\b", RegexOptions.IgnoreCase)) {
            return true;
         } else {
            return false;
         }
      }

      public static string RemoveDuplicatesChars(this string word) {
         string result = string.Join(" ", word).Split(new char[] { ' ' }).Distinct().ToString();
         return result;
      }

      public static string CapitalizeFirstLetter(this string s) {
         switch (s) {
            case null: throw new System.ArgumentNullException(nameof(s));
            case "": throw new System.ArgumentException($"{nameof(s)} cannot be empty", nameof(s));
            default: return s.First().ToString().ToUpper() + s.Substring(1);
         }
      }

      public static string CapitalizeFirstLetterAllWords(this string s) {
         switch (s) {
            case null: throw new System.ArgumentNullException(nameof(s));
            case "": throw new System.ArgumentException($"{nameof(s)} cannot be empty", nameof(s));
            default: return Regex.Replace(s, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
         }
      }


      public static string NodupWord(string s) {
         // Keep track of words found in this Dictionary.
         Dictionary<string, bool> d = new Dictionary<string, bool>();

         // Build up string into this StringBuilder.
         StringBuilder b = new StringBuilder();

         // Split the input and handle spaces and punctuation.
         string[] a = s.Split(new char[] { ' ', ';', '.' }, System.StringSplitOptions.RemoveEmptyEntries);

         // Loop over each word
         foreach (string current in a) {
            string lower = current.ToLower();

            // If we haven't already encountered the word, append it to the result.
            if (!d.ContainsKey(lower)) {
               b.Append(current).Append(' ');
               d.Add(lower, true);
            }
         }
         return b.ToString().Trim();
      }
   }
}
