using System;
using Cipa.Domain.Exceptions;
using Cipa.Domain.Helpers;

namespace Cipa.Application.Services.Models
{
    public enum DataType
    {
        String,
        Date,
        Int,
        Decimal,
        Email
    }

    public class DataColumnValidator
    {
        public string ColumnName { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }
        public bool Required { get; set; }
        public DataType DataType { get; set; }

        public string ValidarValor(object value)
        {
            if (Required && value == null)
            {
                return $"A coluna {ColumnName} é obrigatória.";
            }
            var stringValue = value.ToString();
            switch (DataType)
            {
                case DataType.String:
                case DataType.Email:
                    if (Required && string.IsNullOrWhiteSpace(stringValue))
                    {
                        return $"A coluna {ColumnName} é obrigatória.";
                    }
                    if (stringValue.Length > MaxLength)
                    {
                        return $"A {ColumnName} pode ter no máximo {MaxLength} caracteres.";
                    }
                    if (stringValue.Length < MinLength)
                    {
                        return $"A {ColumnName} deve ter no mínimo {MinLength} caracteres.";
                    }
                    if (DataType == DataType.Email)
                    {
                        var emailValido = Util.EmailEhValido(stringValue);
                        if (!emailValido) return "O email está em formato inválido.";
                    }
                    break;
                case DataType.Date:
                    DateTime dataValor = DateTime.MinValue;
                    if (!Required && string.IsNullOrWhiteSpace(stringValue)) break;
                    if (!(value is DateTime))
                    {
                        dataValor = Util.ConverteStringParaData(stringValue);
                    }
                    if (dataValor == DateTime.MinValue)
                    {
                        return $"A data {stringValue} está em formato inválido.";
                    }
                    break;
                case DataType.Decimal:
                    var valido = decimal.TryParse(stringValue, out decimal valor);
                    if (!valido)
                    {
                        return $"A coluna {ColumnName} deve ser um número.";
                    }
                    break;
                case DataType.Int:
                    var validoInt = int.TryParse(stringValue, out int valorInt);
                    if (!validoInt)
                    {
                        return $"A coluna {ColumnName} deve ser um número inteiro.";
                    }
                    break;
                default:
                    return "DataType inválido.";
            }
            return string.Empty;
        }

        public object ParseValor(object value)
        {
            switch (DataType)
            {
                case DataType.String:
                    return value.ToString();
                case DataType.Email:
                    return value.ToString().Trim().ToLower();
                case DataType.Decimal:
                    return decimal.Parse(value.ToString());
                case DataType.Int:
                    return int.Parse(value.ToString());
                case DataType.Date:
                    var valorConvertido = value is DateTime ? (DateTime)value : Util.ConverteStringParaData(value.ToString());
                    if (valorConvertido == DateTime.MinValue && !Required)
                        return null;
                    return valorConvertido;
                default:
                    throw new CustomException("DataType inválido");
            }
        }

    }
}