
public class Attributes
{
    public int priority { get; set; }
    //public int capacity_limit { get; set; }
    public int weight { get; set; }
    public string name { get; set; }
    public string cli_format { get; set; }
    public object cli_prefix { get; set; }
    public object description { get; set; }
    //public object ringing_timeout { get; set; }
    public Configuration configuration { get; set; }
    public DateTime created_at { get; set; }
    public string dst { get; set; }
    public string username { get; set; }
    public string host { get; set; }
    //public int port { get; set; }
}

public class Configuration
{
    public string type { get; set; }
    public Attributes attributes { get; set; }
}

public class Data
{
    public string id { get; set; }
    public string type { get; set; }
    public Attributes attributes { get; set; }
    public Relationships relationships { get; set; }
}

public class Links
{
    public string self { get; set; }
    public string related { get; set; }
}

public class Meta
{
    public string api_version { get; set; }
}

public class Pop
{
    public Links links { get; set; }
}

public class Relationships
{
    public VoiceInTrunkGroup voice_in_trunk_group { get; set; }
    public Pop pop { get; set; }
}

public class getvoiceTrunkDestDTO
{
    public Data data { get; set; }
    public Meta meta { get; set; }
}

public class VoiceInTrunkGroup
{
    public Links links { get; set; }
}
public class VoiceInTrunk
{
    public Links links { get; set; }
    public Data data { get; set; }
}

public class SharedCapacityGroup
{
    public Links links { get; set; }
}
public class DestNoResponse
{
    //public Data data { get; set; }
    public List<Included> included { get; set; }
    //public Meta meta { get; set; }
}
public class Included
{
    public string id { get; set; }
    public string type { get; set; }
    public Attributes attributes { get; set; }
    public Relationships relationships { get; set; }
}


