using Microsoft.Data.SqlClient;
using System.Data;

namespace Didww.Services
{
    public class PhoneNumbersService: IPhoneNumbersService
    {
        private readonly IConfiguration _configuration;
        public PhoneNumbersService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<int> VerifyNumber(string phoneNumber)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DIDDbConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("sp_verifyNumber", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                    // Add output parameter for PhoneNumberExists
                    var phoneNumberExistsParam = new SqlParameter("@VerificationStatus", SqlDbType.Int);
                    phoneNumberExistsParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(phoneNumberExistsParam);

                   await command.ExecuteNonQueryAsync(); // Execute the stored procedure

                    // Retrieve the value of PhoneNumberExists
                    int phoneNumberExists = (int)phoneNumberExistsParam.Value;

                    return phoneNumberExists; // Return the result
                }
            }
        }

    }

}
