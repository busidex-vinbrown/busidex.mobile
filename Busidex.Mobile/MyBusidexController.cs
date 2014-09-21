using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Busidex.Mobile
{
	public class MyBusidexController : BaseController
	{
		public MyBusidexController ()
		{
		}

		public string GetMyBusidex(string userToken){

			const string url = BASE_API_URL + "busidex?all=true";
			return MakeRequest (url, "GET", userToken);
		}

		public string AddToMyBusidex(long cardId, string userToken){
			string url = BASE_API_URL + "busidex?userId=0&cId=" + cardId;

			return MakeRequest (url, "POST", userToken);
		}


	}
}

