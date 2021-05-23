using System.Threading.Tasks;
using Cipa.Domain.Entities;

namespace Cipa.Domain.Services.Interfaces {
    public interface IEmailSender {
        Task Send(Email email);
    }
}