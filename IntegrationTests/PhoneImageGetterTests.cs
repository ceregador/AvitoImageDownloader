using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhoneImageGetter;

namespace IntegrationTests
{
	[TestClass]
	public class PhoneImageGetterTests
	{
		[TestMethod]
		public async Task TheImageIsSavedToDisk()
		{
			string pageUrl = "https://www.avito.ru/moskva/zapchasti_i_aksessuary/reshetka_bampera_porsche_cayenne_958_1690129542";
			string filePath = @"c:\Avito\phone.jpg";

			if (File.Exists(filePath))
				File.Delete(filePath);

			using (var imageGetter = new ImageDownloader())
				await imageGetter.SavePhoneImageFromPage(pageUrl, filePath);

			Assert.IsTrue(File.Exists(filePath));
		}
	}
}
