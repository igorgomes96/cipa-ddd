using System.Collections.Generic;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Services.Interfaces
{
    public interface IFormatadorEmailService
    {
        ICollection<Email> FormatarEmails();
    }
}