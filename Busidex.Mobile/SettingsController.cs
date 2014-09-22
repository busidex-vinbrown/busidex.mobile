using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class SettingsController : BaseController
	{
		private const string CHANGE_USERNAME_URL = "User/ChangeUserName";
		private const string CHANGE_PASSWORD_URL = "Password";
		private const string CHANGE_EMAIL_URL = "User/ChangeEmail";

		public SettingsController ()
		{
		}

		public static string ChangeUserName(string name, string userToken){

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_USERNAME_URL + "?userId=0&name=" + name;

			return MakeRequest (url, "PUT", userToken);
		}

		public static string ChangePassword(string password, string userToken){

			string oldPassword = "";
			string data = @"{'userId':0, 'OldPassword'" + oldPassword + "','NewPassword':'" + password + "','ConfirmPassword','" + password + "'}";

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_PASSWORD_URL;

			return MakeRequest (url, "PUT", userToken, data);
		}

		public static string ChangeEmail(string email, string userToken){

			string url = Busidex.Mobile.Resources.BASE_API_URL + CHANGE_EMAIL_URL + "?email=" + email;
			string data = "{}";
			return MakeRequest (url, "PUT", userToken, data);
		}
	}
}