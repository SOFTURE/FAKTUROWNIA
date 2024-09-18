using System.Runtime.Serialization;

namespace SOFTURE.Fakturownia.Models.Enums;

public enum Period
{
    [EnumMember(Value = "this_month")]
    ThisMonth,
    
    [EnumMember(Value = "last_30_days")]
    Last30Days,
    
    [EnumMember(Value = "last_month")]
    LastMonth,
    
    [EnumMember(Value = "this_year")]
    ThisYear,
    
    [EnumMember(Value = "last_year")]
    LastYear,
    
    [EnumMember(Value = "all")]
    All
}