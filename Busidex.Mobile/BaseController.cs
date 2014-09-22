using System;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public class BaseController
	{
		public BaseController ()
		{
		}


		protected static string MakeRequest(string url, string method, string token, string data = null){
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;

			if(data != null){
				using (var writer = new StreamWriter (request.GetRequestStream ())) {
					writer.Write (data);
				}
			}
			request.Headers.Add ("X-Authorization-Token", token);

			if (method == "POST") {
				StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
				requestWriter.Write("{}");
				request.ContentType = "application/json";
				requestWriter.Close();
			}

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();

				responseReader.Close();

				return response;

			} catch (Exception e) {
				NewRelic.NRLogger.Log ((uint)NewRelic.NRLogLevels.Error, e.Source, 14, "MakeRequest", e.Message);
			}
			return string.Empty;
		}
	}
}

