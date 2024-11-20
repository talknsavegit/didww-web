using System.ServiceModel;

namespace Didww.Services.Interfaces
{
    [ServiceContract]

    public interface IForwardService
    {
        [OperationContract]
        Task<string> AddForwardRule(string phoneNumber, string did, string countryCode);
    }
}
