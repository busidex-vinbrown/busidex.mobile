using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class NotesController
	{
		public NotesController ()
		{
		}

		public async Task<string> SaveNotes(long id, string notes, string userToken){

			string encodedNotes = System.Net.WebUtility.HtmlEncode (notes);

			string url = "https://www.busidexapi.com/api/Notes?id=" + id + "&notes=" + encodedNotes;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "PUT";
			request.ContentType = "application/json";

			string data = @"{'id':'" + id + "','notes':'" + encodedNotes + "'}";

			using (var writer = new StreamWriter (request.GetRequestStream ())) {
				writer.Write (data);
			}

			request.Headers.Add ("X-Authorization-Token", userToken);

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();

				responseReader.Close();

				return response;

			} catch (Exception e) {
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}
			return string.Empty;
		}
	}
}
