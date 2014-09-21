using System;

namespace Busidex.Mobile
{
	public class OrganizationController : BaseController
	{
	
		public OrganizationController ()
		{
		}

		public string GetMyOrganizations(string userToken){

			const string url = BASE_API_URL + "Organization";
			return MakeRequest (url, "GET", userToken);
		}
	}
}

