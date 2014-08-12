using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Linq;

namespace Busidex.Presentation.IOS
{
	partial class StartupController : UIViewController
	{
		const string TEST_ACCOUNT_ID = "45ef6a4e-3c9d-452c-9409-9e4d7b6e532f";
		const bool DEVELOPMENT_MODE = true;

		public StartupController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies
				.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName)
				.SingleOrDefault();

			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId <= 0) {
					DoLogin ();
				} 
				GoToMain ();
			}

			btnStart.TouchUpInside += delegate {
				GoToMain();
			};

			btnConnect.TouchUpInside += delegate {
				GoToLogin();
			};

		}


		private void GoToLogin ()
		{
			var loginController = this.Storyboard.InstantiateViewController ("LoginController") as LoginController;

			if (loginController != null) {

				this.NavigationController.PushViewController (loginController, true);
			}
		}

		private void GoToMain ()
		{
			this.NavigationController.SetNavigationBarHidden (true, true);

			var dataViewController = this.Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				this.NavigationController.PushViewController (dataViewController, true);
			}
		}

		private string EncodeUserId(long userId){
			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(userId.ToString());
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		private string GetDeviceId(){
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}

		private void DoLogin(){
			string uidId = GetDeviceId ();
			var userId = Busidex.Mobile.LoginController.AutoLogin (uidId);
			if (userId > 0) {
				var nCookie = new System.Net.Cookie();

				nCookie.Name = "UserId";
				DateTime expiration = DateTime.Now.AddYears(1);
				nCookie.Expires = expiration;
				nCookie.Value = EncodeUserId(userId);
				var cookie = new NSHttpCookie(nCookie);

				NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
			}
		}
	}
}
