using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net.Http;
//using System.Web;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public class LoginController
	{
		public LoginController ()
		{

		}

		public static long DoLogin(string username, string password){
			string URL = "https://www.busidexapi.com/api/account/Login?name=" + username + "&pswd=" + password;
			//string DATA = @"{name:" + username + ", pswd: " + password + "}";
			long userId = 0;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			request.Method = "GET";
			//request.ContentType = "application/json"; 
			//request.ContentLength = DATA.Length;
			//StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
			//requestWriter.Write(DATA);
			//requestWriter.Close();

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				//Console.Out.WriteLine(response);
				long.TryParse(response, out userId);

				responseReader.Close();


			} catch (Exception e) {
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}

			return userId;
		}
	}
}

