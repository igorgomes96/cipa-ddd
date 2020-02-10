using System;
using System.Collections.Generic;

namespace Cipa.Domain.Helpers
{

    public static class Util
    {
        public static DateTime ConverteStringParaData(string dataString)
        {
            var numbers = dataString.Split(' ')[0].Split('/');
            if (numbers.Length != 3) return DateTime.MinValue;
            try
            {
                return new DateTime(int.Parse(numbers[2]), int.Parse(numbers[1]), int.Parse(numbers[0]));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static bool EmailEhValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static string NomeMes(int mes) =>
            new Dictionary<int, string>{
                { 1, "Janeiro" },
                { 2, "Fevereiro" },
                { 3, "Março" },
                { 4, "Abril" },
                { 5, "Maio" },
                { 6, "Junho" },
                { 7, "Julho" },
                { 8, "Agosto" },
                { 9, "Setembro" },
                { 10, "Outubro" },
                { 11, "Novembro" },
                { 12, "Dezembro" }
            }[mes];

        public static string FormataCNPJ(string CNPJ) =>
            Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");


        public static DateTime HorarioBrasilia(this DateTime data) =>
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(data, TimeZoneInfo.Local.Id, FusosHorarios.Brasilia);


    }
}