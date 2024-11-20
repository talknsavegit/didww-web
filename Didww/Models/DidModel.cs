namespace Didww.Models
{
    public class DidModel
    {
        public class DidApiResponse
        {
            public List<DidData> data { get; set; }
            public DidApiResponseMeta meta { get; set; }
            public DidApiResponseLinks links { get; set; }
        }

        public class DidInfoResponse
        {
            public string? Id { get; set; }
            public bool? Active { get; set; }
            public string? Number { get; set; }
            public string? Status { get; set; }
            public string? didGroup { get; set; }
            public PhoneNumberInfo? phonenumberinfo { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public string? VoiceInTrunk { get; set; }
            public List<string>? Relationships { get; set; }
        }
        public class DidInfo
        {
            public string VoiceInTrunk { get; set; }
        }

        public class RootObject
        {
            public ValueContainer Value { get; set; }
        }

        public class ValueContainer
        {
            public DidInfo[] DidInfoResponse { get; set; }
        }

        //voice in trunk id 
        public class VoiceInTrunkResponse
        {
            public VoiceIn Data { get; set; }
        }

        public class VoiceIn
        {
            public string Type { get; set; }
            public string Id { get; set; }
        }




        public class PhoneNumberParts
        {
            public string? CountryCode { get; set; }
            public string? CountryName { get; set; }
            public string? AreaCode { get; set; }
            public string? MobileNumber { get; set; }
            public string? areaCodeName { get; set; }
        }
       
        public class getdidOutput
        {
            public List<DidInfoResponse>? DidInfoResponse { get; set; }
            public PhoneNumberParts? PhoneNumberParts { get; set; }
        }

        public class DidData
        {
            public string id { get; set; }
            public string type { get; set; }
            public DidAttributes attributes { get; set; }
            public DidRelationships relationships { get; set; }
        }

        public class DidAttributes
        {
            public bool blocked { get; set; }
            public object capacity_limit { get; set; }
            public string description { get; set; }
            public bool terminated { get; set; }
            public bool awaiting_registration { get; set; }
            public DateTime created_at { get; set; }
            public object billing_cycles_count { get; set; }
            public string number { get; set; }
            public DateTime expires_at { get; set; }
            public int channels_included_count { get; set; }
            public int dedicated_channels_count { get; set; }

            //public string ExpiresAt { get; set; }
        }


        public class DidRelationships
        {
            public DidGroupRelation did_group { get; set; }
            public OrderRelation order { get; set; }
            // ... other relationships you might have

            public CapacityPoolRelation capacity_pool { get; set; }
            public SharedCapacityGroupRelation shared_capacity_group { get; set; }
            public AddressVerificationRelation address_verification { get; set; }
            public VoiceInTrunkRelation voice_in_trunk { get; set; }
            public VoiceInTrunkGroupRelation voice_in_trunk_group { get; set; }
        }

        public class CapacityPoolRelation
        {
            public DidLink links { get; set; }
        }

        public class SharedCapacityGroupRelation
        {
            public DidLink links { get; set; }
        }

        public class AddressVerificationRelation
        {
            public DidLink links { get; set; }
        }

        public class VoiceInTrunkRelation
        {
            public DidLink links { get; set; }
        }

        public class VoiceInTrunkGroupRelation
        {
            public DidLink links { get; set; }
        }

        public class DidLink
        {
            public string self { get; set; }
            public string related { get; set; }
        }


        public class DidGroupRelation
        {
            public DidLink links { get; set; }
        }

        public class OrderRelation
        {
            public DidLink links { get; set; }
        }
        public class DidGroupLinkResponse
        {
            public DidGroupData data { get; set; }
        }

        public class DidGroupData
        {
            public string type { get; set; }
            public string id { get; set; }
            public DidGroupLinks links { get; set; }
        }

        public class DidGroupLinks
        {
            public string self { get; set; }
            public string related { get; set; }
        }


        public class DidApiResponseMeta
        {
            public int total_records { get; set; }
            public string api_version { get; set; }
        }

        public class DidApiResponseLinks
        {
            public string first { get; set; }
            public string last { get; set; }
        }
        public class DidGroupResponse
        {
            public Data data { get; set; }
            public Meta meta { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string type { get; set; }
            public Attributes attributes { get; set; }
            public Relationships relationships { get; set; }

        }

        public class Attributes
        {
            public string prefix { get; set; }
            public List<string> features { get; set; }
            public bool is_metered { get; set; }
            public string area_name { get; set; }
            public bool allow_additional_channels { get; set; }
        }

        public class Relationships
        {
            public RelationshipLinks country { get; set; }
            public RelationshipLinks city { get; set; }
            public RelationshipLinks did_group_type { get; set; }
            public RelationshipLinks region { get; set; }
            public RelationshipLinks stock_keeping_units { get; set; }
            public RelationshipLinks requirement { get; set; }
        }

        public class RelationshipLinks
        {
            public LinkData links { get; set; }
        }

        public class LinkData
        {
            public string self { get; set; }
            public string related { get; set; }
        }

        public class Meta
        {
            public bool available_dids_enabled { get; set; }
            public bool needs_registration { get; set; }
            public bool is_available { get; set; }
            public int total_count { get; set; }
        }

        public class PhoneNumberInfo
        {
            public string area_name { get; set; }
            public string prefix { get; set; }
            public List<string> features { get; set; }
            public bool is_metered { get; set; }
            public bool allow_additional_channels { get; set; }
            public bool available_dids_enabled { get; set; }
            public bool needs_registration { get; set; }
            public bool is_available { get; set; }
            public int total_count { get; set; }
        }


    }
}
