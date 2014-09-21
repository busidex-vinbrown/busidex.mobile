using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Mobile
{
	public class AccountController : BaseController
	{
		public AccountController ()
		{
		}

		public string UpdateDisplayName(string name, string userToken){
			string encodedName = System.Net.WebUtility.HtmlEncode (name);
			string data = @"{'name':'" + name + "'}";

			string url = BASE_API_URL + "Account/UpdateDisplayName?name=" + encodedName;

			return MakeRequest (url, "PUT", userToken, data);
		}
	}
}

