using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScrapper.Extensions
{
    public static class StringMethods
    {
        private readonly static Dictionary<string, string> _forbiddenTags = new()
        {
            {"<style>","</style>" },
            {"<script>","</script>" }

        };
        public static string GetTextFromHtml(this string html)
        {
            var temp = "";
            html = html.RemoveForbiddenTags();
            while (html.Contains('<'))
            {
                html = html.Trim();
                if (html[0] == '<')
                {
                    html = html.Replace(html[..(html.IndexOf('>') + 1)], "");
                }
                else
                {
                    temp += $"{string.Join("", html.TakeWhile(c => c != '<'))}\n";
                    html = string.Join("", html.SkipWhile(c => c != '<'));
                }
            }
            return Regex.Replace(temp.Trim(), @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline); ;
        }
        public static void GetLinks(this string html, int numberOfLinks = 0)
        {
            var startTagIndex = html.IndexOf("<a ");
            if (startTagIndex < 0) return;
            var starTag = string.Join("", html[startTagIndex..].TakeWhile(c => c != '>')) + ">";
            var innerHtml = html.GetStrBetweenTags(starTag, "</a>");
            if (string.IsNullOrEmpty(innerHtml)) Console.WriteLine("No results");
            if (!innerHtml.Contains("h3"))
            {
                GetLinks(html.Remove(startTagIndex, innerHtml.Length + "</a>".Length + starTag.Length), numberOfLinks);
                return;
            }
            var indexOfHref = starTag.IndexOf("https:");
            if (indexOfHref < 0)
            {
                GetLinks(html.Remove(startTagIndex, innerHtml.Length + "</a>".Length + starTag.Length), numberOfLinks);
                return;
            }
            if (numberOfLinks > 9) return;
            var link = string.Join("", starTag[indexOfHref..].TakeWhile(c => c != '\"' && c != '&'));
            numberOfLinks++;
            Console.WriteLine($"{numberOfLinks}. {link}");
            GetLinks(html.Remove(startTagIndex, innerHtml.Length + "</a>".Length + starTag.Length), numberOfLinks);
        }
        private static string GetStrBetweenTags(this string value, string startTag, string endTag)
        {
            if (value.Contains(startTag) && value.Contains(endTag))
            {
                int index = value.IndexOf(startTag) + startTag.Length;
                return value.Substring(index, value.IndexOf(endTag) - index);
            }
            else
                return "";
        }
        private static string RemoveForbiddenTags(this string html)
        {
            foreach (KeyValuePair<string, string> entry in _forbiddenTags)
            {
                var openingTagIndex = html.IndexOf(entry.Key[..(entry.Value.Length - 2)]);
                if (openingTagIndex < 0) continue;
                var openingTag = $"{string.Join("", html[openingTagIndex..].TakeWhile(c => c != '>'))}>";
                html = html.Replace(html[openingTagIndex..(html.IndexOf(entry.Value) + entry.Value.Length)], "");
            }
            return _forbiddenTags.Keys.Any(k => html.Contains(k[..(k.Length - 2)])) ? RemoveForbiddenTags(html) : html;
        }
    }
}
