using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;

namespace PhoneImageGetter
{
	public class ImageDownloader : IDisposable
	{
		const string hostUrl = "http://www.avito.ru";

		private readonly HttpClient _httpClient;

		public ImageDownloader()
		{
			_httpClient = new HttpClient();
		}

		public async Task SavePhoneImageFromPage(string pageUrl, string filePath)
		{
			var pageData = await ParsePage(pageUrl);

			var phoneHash = CalculateHash(pageData.ItemId, pageData.PhoneKey);

			var imageUrl = BuildImageUrl(pageData.ItemId, phoneHash);

			var image64String = await DownloadImage64String(pageUrl, imageUrl);

			SaveImage(image64String, filePath);
		}

		private async Task<PhoneData> ParsePage(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentException("The url is not valid.");

			var content = await _httpClient.GetStringAsync(url);
			var phoneKey = Regex.Match(content, "avito.item.phone =\\s+'(\\w+)'")
				.Groups[1].Value;

			var itemId = long.Parse(url.Split('_').Last());

			return new PhoneData(itemId, phoneKey);
		}

		private string CalculateHash(long itemId, string phoneKey)
		{
			var matches = Regex.Matches(phoneKey, "[0-9a-f]+")
				.OfType<Match>()
				.Select(m => m.Value);

			if (itemId % 2 == 0)
				matches = matches.Reverse();

			var joined = string.Join(string.Empty, matches);
			var strBuilder = new StringBuilder();

			for (int i = 0; i < joined.Length; i++)
			{
				if (i % 3 == 0)
					strBuilder.Append(joined.Substring(i, 1));
			}

			return strBuilder.ToString();
		}

		private string BuildImageUrl(long itemId, string phoneHash) =>
			$"{hostUrl}/items/phone/{itemId}?pkey={phoneHash}";

		private async Task<string> DownloadImage64String(string referrer, string imageUrl)
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(imageUrl),
				Method = HttpMethod.Get
			};
			request.Headers.Referrer = new Uri(referrer);

			var httpResponse = await _httpClient.SendAsync(request);
			var json = await httpResponse.Content.ReadAsStringAsync();
			var responseObject = JsonConvert.DeserializeObject<JsonResponse>(json);

			return responseObject.Image.Split(',')[1];
		}

		private void SaveImage(string image64String, string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("The filePath is invalid.");

			var bytes = Convert.FromBase64String(image64String);
			using (var imageFile = new FileStream(filePath, FileMode.Create))
			{
				imageFile.Write(bytes, 0, bytes.Length);
				imageFile.Flush();
			}
		}

		public void Dispose()
		{
			_httpClient.Dispose();
		}
	}
}
