using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net.Http;
//using System.Web;
using System.Net;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class LoginController
	{
		const string TEST_ACCOUNT_ID = "45ef6a4e-3c9d-452c-9409-9e4d7b6e532f";

		const bool DEVELOPMENT_MODE = true;

		public LoginController ()
		{

		}

		public static long DoLogin(string username, string password){
			string URL = "https://www.busidexapi.com/api/Account/Login";
			string DATA = "{'UserName':'" + username + "','Password':'" + password + "','Token':'','RememberMe':'true'}";
			long userId = 0;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			request.Method = "POST";
			request.ContentType = "application/json"; 
			request.ContentLength = DATA.Length;
			StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
			requestWriter.Write(DATA);
			requestWriter.Close();

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);

				userId = loginResponse != null ? loginResponse.UserId : 0;

				responseReader.Close();
				//var response = (new WebClient()).UploadString(URL, DATA);
				//long.TryParse(response, out userId);

			} catch (Exception e) {
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}

			return userId;
		}

		public static long AutoLogin(string uidId){
			string URL = "https://www.busidexapi.com/api/Registration/CheckAccount";
			string DATA = "uidId=" +  (DEVELOPMENT_MODE ? TEST_ACCOUNT_ID : uidId);
			long userId = 0;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";// "application/json"; 
			request.ContentLength = DATA.Length;
			StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
			requestWriter.Write(DATA);
			requestWriter.Close();

			try {
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader responseReader = new StreamReader(webStream);
				string response = responseReader.ReadToEnd();
				var loginResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse> (response);

				userId = loginResponse != null ? loginResponse.UserId : 0;

				responseReader.Close();
				//var response = (new WebClient()).UploadString(URL, DATA);
				//long.TryParse(response, out userId);

			} catch (Exception e) {
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
			}

			return userId;
		}
	}
}

