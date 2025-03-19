using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace BookHotel.DTOs
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy/MM/dd"; // Định dạng mong muốn

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string dateString = reader.GetString();

            if (DateTime.TryParseExact(dateString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            throw new JsonException($"Invalid date format: {dateString}. Expected format: {Format}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
