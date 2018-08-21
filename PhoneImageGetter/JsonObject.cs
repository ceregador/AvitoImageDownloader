using Newtonsoft.Json;

namespace PhoneImageGetter
{
	public class JsonResponse
	{
		[JsonProperty("image64")]
		public string Image { get; set; }
	}
}
