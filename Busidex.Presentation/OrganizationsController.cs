using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Linq;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.IO;

namespace Busidex.Presentation.IOS
{
	partial class OrganizationsController : UIViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");

		public OrganizationsController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		private void LoadMyOrganizations(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

			if (cookie != null) {
				var controller = new Busidex.Mobile.OrganizationController ();
				var response = controller.GetMyOrganizations (cookie.Value);
				if (!string.IsNullOrEmpty (response)) {
					OrganizationResponse MyOrganizationsResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OrganizationResponse> (response);
					string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					foreach(Organization org in MyOrganizationsResponse.Model){
						var fileName = org.LogoFileName + "." + org.LogoType;
						var fImagePath = Busidex.Mobile.Utils.CARD_PATH + fileName;
						if (!File.Exists (documentsPath + "/" + fileName)) {
							Busidex.Mobile.Utils.DownloadImage (fImagePath, documentsPath, fileName).ContinueWith (t => {	});
						} 
					}
					var src = new OrganizationTableSource (MyOrganizationsResponse.Model);
					src.ViewWebsite += delegate(string url) {
						UIApplication.SharedApplication.OpenUrl (new NSUrl ("http://" + url.Replace("http://", "")));
					};
					this.vwOrganizations.Source = src;

				}
			}
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.vwOrganizations.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			LoadMyOrganizations ();

		}
	}
}
