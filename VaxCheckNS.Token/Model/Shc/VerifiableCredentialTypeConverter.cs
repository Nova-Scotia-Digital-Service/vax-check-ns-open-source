using Newtonsoft.Json;
using VaxCheckNS.Token.Enums;
using VaxCheckNS.Token.Exceptions;
using System;

namespace VaxCheckNS.Token.Model.Shc
{
  public class VerifiableCredentialTypeConverter : Newtonsoft.Json.JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
      if (value is VerifiableCredentialType VerifiableCredentialType)
      {
        writer.WriteValue(VerifiableCredentialType.GetLiteral());
      }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
      if (reader.Value is object)
      {
        string VerifiableCredentialTypeString = (string)reader.Value;
        if (VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary.ContainsKey(VerifiableCredentialTypeString))
        {
          return VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary[VerifiableCredentialTypeString];
        }
        else
        {
          return VerifiableCredentialType.Unknown;
        }
      }
      else
      {
        throw new SmartHealthCardPayloadException($"Must provide at least one VerifiableCredentialTypes (vc.type). The supported types are: {GetSupportedVerifiableCredentialTypeUriStringList()}");
      }
    }

    private static string GetSupportedVerifiableCredentialTypeUriStringList()
    {
      string AllowedTypesList = string.Empty;
      foreach (var item in VerifiableCredentialTypeSupport.VerifiableCredentialTypeDictionary)
      {
        AllowedTypesList += $"{item.Key}, ";
      }
      AllowedTypesList = AllowedTypesList.Trim().TrimEnd(',');
      return AllowedTypesList;
    }

    public override bool CanRead
    {
      get { return true; }
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(string);
    }
  }
}
