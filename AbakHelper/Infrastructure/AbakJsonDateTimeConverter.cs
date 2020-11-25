using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AbakHelper.Infrastructure
{
    public class AbakJsonDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException($"Unexpected token or value when parsing date. Token: {reader.TokenType}, Value: {reader.Value}");

            string value = reader.Value.ToString();

            var regexResult = Regex.Match(value, @"new\ Date\((?<year>\d{4}),(?<month>\d{2}),(?<day>\d{2}),00,00,00\)");
            var result = new DateTime(int.Parse(regexResult.Groups["year"].Value), int.Parse(regexResult.Groups["month"].Value) + 1, int.Parse(regexResult.Groups["day"].Value));

            return result;
        }
    }
}
