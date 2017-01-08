
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ElusScraping
{
    internal class Storage
    {
        private const int CommissionMax = 3;
		private const int MandatMax = 2;
        private const string Separator = "\t";
		public string Filename { get; private set; }

		public Storage(string filename)
		{
			Filename = filename;
		}

		public void Persist(IEnumerable<Elu> elus)
		{
			using (var file = File.CreateText(Filename))
			{
				WriteHeader(file);

				foreach (var elu in elus)
				{
					WriteLine(file, elu);
				}
			}
		}

        private void WriteHeader(StreamWriter writer)
		{
			writer.WriteLine(
				string.Join(
					Separator,
					"nom", "age", "profession", "etiquette",
					"binome", "annee_election", "mois_election", "secteur", "titre",
					string.Join(
						Separator,
						Enumerable.Range(1, CommissionMax).Select(x => string.Format("commission_{0}", x))
					),
					string.Join(
						Separator,
						Enumerable.Range(1, MandatMax).Select(x => string.Format("mandat_{0}", x))
					),
					"url"
				)
			);
		}

		private void WriteLine(StreamWriter writer, Elu elu)
		{
			writer.WriteLine(
				string.Join(
					Separator,
					elu.Nom,
					elu.Infos.Age,
					elu.Infos.Profession,
					elu.Infos.Etiquette,
					elu.Infos.Binome,
					elu.Infos.DateElection.Year.ToString(),
					elu.Infos.DateElection.Month.ToString(),
					elu.Secteur,
					elu.Infos.Titre,
					string.Join(
						Separator,
						elu.Infos.Commissions.Take(CommissionMax)
						.Concat(Enumerable.Range(1, CommissionMax - elu.Infos.Commissions.Count()).Select(x => string.Empty))
					),
					string.Join(
						Separator,
						elu.Infos.Mandats.Take(MandatMax)
						.Concat(Enumerable.Range(1, MandatMax - elu.Infos.Mandats.Count()).Select(x => string.Empty))
					),
					elu.Url
				)
			);
		}
    }
}