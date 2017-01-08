using System;
using System.Collections.Generic;

namespace ElusScraping
{
    internal class EluInfos
	{
        public string Titre { get; set; }
        public int Age { get; set; }
        public string Profession { get; set; }
        public string Etiquette { get; set; }
        public string Binome { get; set; }
        public IEnumerable<string> Mandats { get; private set; }
        public IEnumerable<string> Commissions { get; private set; }
        public DateTime DateElection { get; set; }

        public EluInfos(IEnumerable<string> mandats, IEnumerable<string> commissions)
        {
            Mandats = mandats;
            Commissions = commissions;
        }
    }
}
