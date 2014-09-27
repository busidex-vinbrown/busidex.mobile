using System;
using System.Threading.Tasks;

namespace Busidex.Mobile
{
	public class OrganizationController : BaseController
	{
	
		public OrganizationController ()
		{
		}

		public async Task<string> GetMyOrganizations(string userToken){

			const string url = Busidex.Mobile.Resources.BASE_API_URL + "Organization";
			return await MakeRequest (url, "GET", userToken);
		}
	}
}

