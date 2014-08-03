using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public class SearchController
	{
		public SearchController ()
		{
		}

		public string DoSearch(string criteria, string userToken){

			string url = "https://www.busidexapi.com/api/search/DoSearch";
			string data = 
				@"{" +
				"'Criteria': ''," + 
				"'SearchText':'" + criteria + "', " + 
				"'TagSearch': 'false'," + 
				"'SearchModel': { " + "'Results': []," + 
				"'UserId': null," + 
				"'NoResults': 'false'," +
				"'SearchResultsMessage': ''," +
				"'UserId': '0'}" +
				"}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";

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