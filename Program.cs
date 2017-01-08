using System.IO;
using ElusScraping;
using HtmlAgilityPack;

namespace ConsoleApplication
{
    public class Program
    {
		private const Scrape.Options options = Scrape.Options.None;

		public static void Main(string[] args)
		{
			var doc = new HtmlDocument();
			if (options.HasFlag(Scrape.Options.LoadLocal))
			{
				doc.Load(File.OpenRead(Path.Combine("pages", "elus.html")));
			}
			else
			{
				doc.LoadHtml(Utils.GetHtmlContent(Scrape.ElusUrl));
				if (options.HasFlag(Scrape.Options.SaveLocal))
				{
					using (var stream = File.Create(Path.Combine("pages", "elus.html")))
					{
						doc.Save(stream);
					}
				}
			}

			new Storage("elus.tsv").Persist(Scrape.ScrapeElus(doc, options));
		}
    }
}
