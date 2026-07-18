using System.Runtime.Serialization;

public enum TmdbGenderType
{
    [EnumMember(Value = "Not set / not specified")]  // TODO: Currently shows as "NotSetNotSpecified", make this better
    NotSetNotSpecified = 0,
    [EnumMember(Value = "Female")]
    Female = 1,
    [EnumMember(Value = "Male")]
    Male = 2,
    [EnumMember(Value = "Non-binary")]

    NonBinary = 3
}