using System.Collections.Generic;

using System.Linq;

namespace Cipa.Domain.Entities
{
    public class Email
    {
        public long Id { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Sent { get; set; }
        public string Error { get; set; }

        private IEnumerable<string> MailAddresses(string addressesString)
        {
            List<string> addresses = new List<string>();
            if (string.IsNullOrWhiteSpace(addressesString)) return addresses;
            foreach (var mail in addressesString.Split(',').Where(add => !string.IsNullOrWhiteSpace(add)))
                addresses.Add(mail);
            return addresses;
        }

        public IEnumerable<string> ToMailAdresses()
        {
            return MailAddresses(To);
        }

        public IEnumerable<string> CCMailAdresses()
        {
            return MailAddresses(CC);
        }
    }
}
