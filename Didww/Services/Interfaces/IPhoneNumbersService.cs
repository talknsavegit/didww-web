using Microsoft.Data.SqlClient;
using System.Data;

namespace Didww.Services
{
    public interface IPhoneNumbersService
    {
        Task<int> VerifyNumber(string phoneNumber);

    }

}
