using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Busidex.Presentation.IOS
{
	partial class LoginController : BaseController
	{
		LoadingOverlay loadingOverlay;
		NSObject _notificationHandle;

		public LoginController (IntPtr handle) : base (handle)
		{
		}

		public string DataObject {
			get;
			set;
		}

		private static long UserId{ get; set; }

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName).SingleOrDefault();
			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId > 0) {
					UserId = userId;
				} 
			} 

			btnLogin.TouchUpInside += (o,s) => {

				lblLoginResult.Text = string.Empty;

				loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				View.Add (loadingOverlay);

				string username = txtUserName.Text;
				string password = txtPassword.Text;


				UserId = Busidex.Mobile.LoginController.DoLogin(username, password);

				if(UserId > 0){

					var nCookie = new System.Net.Cookie();
					nCookie.Name = Busidex.Mobile.Resources.AuthenticationCookieName;
					DateTime expiration = DateTime.Now.AddYears(1);
					nCookie.Expires = expiration;
					nCookie.Value = EncodeUserId(UserId);
					cookie = new NSHttpCookie(nCookie);

					NSHttpCookieStorage.SharedStorage.SetCookie(cookie);

					var user = NSUserDefaults.StandardUserDefaults;

					user.SetString(username, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
					user.SetString(password, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
					user.SetString(username + "@busidex.com", Busidex.Mobile.Resources.USER_SETTING_EMAIL);
					user.SetBool(true, Busidex.Mobile.Resources.USER_SETTING_AUTOSYNC);
					user.Synchronize();

					GoToHome();
				}else{
					lblLoginResult.Text = "Login Failed";
					lblLoginResult.TextColor = UIColor.Red;
				}
				loadingOverlay.Hide();


			};
			// Perform any additional setup after loading the view, typically from a nib.
		}

		private string EncodeUserId(long userId){

			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(userId.ToString());
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}



		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		private void GoToHome ()
		{

			var dataViewController = this.Storyboard.InstantiateViewController ("DataViewController") as DataViewController;

			if (dataViewController != null) {
				this.NavigationController.PushViewController (dataViewController, true);

			}
		}
	}
}
