using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using HtmlAgilityPack;

namespace ElusScraping
{
    internal static class Scrape
    {
        [Flags]
        public enum Options
        {
            None = 0x0,
			/// <summary>
			/// Load pages to scrape from local storage. 
			/// </summary>
            LoadLocal = 0x1,
			/// <summary>
			/// Save pages to scrape to local storage (for future use with option LoadLocal). 
			/// </summary>
            SaveLocal = 0x2
        }

        private const string BaseUrl = "http://www.illeetvilaine.fr";
		public const string ElusUrl = BaseUrl + "/fr/elus";

        public static IEnumerable<Elu> ScrapeElus(this HtmlDocument doc, Options options)
        {
            var elus
				= doc.DocumentNode
				.Div("r>media-list media-list-general elu")
				.Descendants("div")
				.Where(n => n.GetAttributeValue("class", string.Empty) == "media-text");

			foreach (var elu in elus)
			{
				string secteur = elu.Div("tag").GetInnerText();
				string url = BaseUrl + elu.Element("h3").Element("a").GetAttributeValue("href", string.Empty);
				string nom = elu.Element("h3").Element("a").GetInnerText();

				var docElu = new HtmlDocument();
				if (options.HasFlag(Options.LoadLocal))
				{
					docElu.Load(File.OpenRead(Path.Combine("pages", nom + ".html")));
				}
				else
				{
					docElu.LoadHtml(Utils.GetHtmlContent(url));
					if (options.HasFlag(Options.SaveLocal))
					{
						using (var stream = File.Create(Path.Combine("pages", nom + ".html")))
						{
							docElu.Save(stream);
						}
					}
				}

				var infos = docElu.ScrapeElu(options);
				
				yield return new Elu()
				{
					Nom = nom,
					Secteur = secteur,
					Url = url,
					Infos = infos
				};
			}
        }

        public static EluInfos ScrapeElu(this HtmlDocument doc, Options options)
        {
            var article
				= doc.DocumentNode
				.Div("r>block-system", "content", "page-body", "block block-rub-sel");

			var body = article.Div("block-body", "block-pad article elu");

			var mandats
				= body.Div(
					"field field-name-field-autres-mandats field-type-text field-label-above",
					"field-items"
				)
				?.Elements("div")
				?.Select(n => n.GetInnerText())
				?? Enumerable.Empty<string>();

			var commissions
				= body.Div(
					"field field-name-field-commissions-de-travail field-type-taxonomy-term-reference field-label-above",
					"field-items",
					"field-item even",
					"pagination"
				)
				?.GetElement("ul", "textformatter-list")
				?.Elements("li")
				?.Select(n => n.GetInnerText())
				?? Enumerable.Empty<string>();

			var infos = new EluInfos(mandats, commissions);

			infos.Titre
				= body.Div(
					"field field-name-field-titre-elu field-type-text field-label-hidden",
					"field-items",
					"field-item even"
				)
				?.GetInnerText()
				?? string.Empty;

			infos.Binome
				= article.Div("block-header elu")
				.GetElement("h2", "binome-elu")
				.Element("a")
				.GetInnerText();

			var etiquette = body.Div("field field-name-field-majorite-departementale field-type-list-text field-label-hidden");
			if (etiquette == null)
			{
				etiquette = body.Div("field field-name-field-groupe-politique field-type-taxonomy-term-reference field-label-inline clearfix");
			}
			infos.Etiquette
				= (etiquette == null
					? string.Empty
					: etiquette.Div("field-items", "field-item even").GetInnerText()
				);

			string dateElection
				= body.Div(
					"field field-name-field-elu-en field-type-text field-label-inline clearfix",
					"field-items",
					"field-item even"
				)
				.GetInnerText();
			infos.DateElection = DateTime.ParseExact(dateElection, "MMMM yyyy", new CultureInfo("fr"));

			string age
				= body.Elements("div")
				.Where(n => !n.Attributes.Contains("class"))
				.FirstOrDefault()
				.GetInnerText();
			var regex = new Regex("[0-9]+");
			var match = regex.Match(age);
			infos.Age = Int32.Parse(match.Groups[0].Value);

			infos.Profession
				= body.Div(
					"field field-name-field-profession field-type-text field-label-inline clearfix",
					"field-items",
					"field-item even"
				)
				?.GetInnerText()
				?? string.Empty;

			return infos;
        }
    } 
}