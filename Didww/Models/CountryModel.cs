namespace DIDWW_Api.Model
{
    public class CountryModel
    {
        public class CountryApiResponse
        {
            public List<CountryData>? Data { get; set; }
            public ApiMeta? Meta { get; set; }
        }

        public class CountryData
        {
            public string? Id { get; set; }
            public string? Type { get; set; }
            public CountryAttributes? Attributes { get; set; }
        }

        public class CountryAttributes
        {
            public string? Name { get; set; }
            public string? Prefix { get; set; }
        }

        public class ApiMeta
        {
            public string? ApiVersion { get; set; }
        }

    }
}
