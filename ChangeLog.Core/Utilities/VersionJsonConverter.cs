using System;
using Newtonsoft.Json;
using SemVer;

namespace ChangeLog.Core.Utilities
{
	public class VersionJsonConvertor : JsonConverter<SemVer.Version>
	{
		public override void WriteJson(JsonWriter writer, SemVer.Version value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}

		public override SemVer.Version ReadJson(JsonReader reader, Type objectType, SemVer.Version existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string s = (string)reader.Value;

			return new SemVer.Version(s);
		}
	}
}