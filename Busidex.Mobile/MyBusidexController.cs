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

			string URL = "https://www.busidexapi.com/api/busidex?all=true";
			//string DATA = @"{name:" + username + ", pswd: " + password + "}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			request.Method = "GET";
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

