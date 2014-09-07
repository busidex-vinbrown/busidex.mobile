using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public class MyBusidexController
	{
		public MyBusidexController ()
		{
		}

		public string GetMyBusidex(string userToken){

			const string url = "https://www.busidexapi.com/api/busidex?all=true";
			//string DATA = @"{name:" + username + ", pswd: " + password + "}";
			return makeRequest (url, "GET", userToken);
		}

		public string AddToMyBusidex(long cardId, string userToken){
			string url = "https://www.busidexapi.com/api/busidex?userId=0&cId=" + cardId;

			return makeRequest (url, "POST", userToken);
		}

		private string makeRequest(string url, string method, string token){
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method;
			request.Headers.Add ("X-Authorization-Token", token);

			if (method == "POST") {
				StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
				string data = "{}";
				requestWriter.Write(data);
				//request.ContentLength = data.Length;
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
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}
			return string.Empty;
		}
	}
}

