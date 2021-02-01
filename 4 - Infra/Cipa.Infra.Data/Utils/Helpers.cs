using System.Text.RegularExpressions;

namespace Cipa.Infra.Data {
    public static class Helpers {
        public static string RemoverCaracteresEspeciais(string str)
        {
            return Regex.Replace(str, @"[^\/a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}