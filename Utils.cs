using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;

namespace ElusScraping
{
    internal static class Utils
    {
        public static string GetHtmlContent(string uri)
		{
			Debug.WriteLine("Loading " + uri);

			var client = new HttpClient();
			var task = client.GetStringAsync(uri);
			task.Wait(TimeSpan.FromMinutes(1));

			Debug.WriteLine("Loaded " + uri);

			return task.Result;
		}

		public static HtmlNode GetElement(this HtmlNode me, string element, string className, bool recursive = false)
		{
			return
				(recursive ? me.Descendants(element) : me.Elements(element))
				.Where(n => n.GetAttributeValue("class", string.Empty) == className)
				.FirstOrDefault();
		}

		public const string ClassNameRecursivePrefix = "r>";

		public static HtmlNode Div(this HtmlNode me, params string[] classPath)
		{
			var node = me;
			foreach (string className in classPath)
			{
				if (className.StartsWith(ClassNameRecursivePrefix))
				{
					node = node.GetElement("div", className.Substring(ClassNameRecursivePrefix.Length), true);
				}
				else
				{
					node = node.GetElement("div", className);
				}

				if (node == null)
				{
					break;
				}
			}

			return node;
		}

		public static string GetInnerText(this HtmlNode me)
		{
			return WebUtility.HtmlDecode(me.InnerText.Trim());
		}
    }
}
