using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Linq;

namespace Busidex.Presentation.IOS
{
	partial class SettingsController : UIViewController
	{
		public SettingsController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			btnSave.TouchUpInside += delegate {

				NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName).SingleOrDefault();
				if(cookie != null){
					string token = cookie.Value;

					var user = NSUserDefaults.StandardUserDefaults;
					string oldUserName = user.StringForKey("UserName");
					string oldPassword = user.StringForKey("Password");
					string oldEmail = user.StringForKey("Email");

					string newUsername = txtUserName.Text;
					string newPassword = txtPassword.Text;
					string newEmail = txtEmail.Text;

					if(!oldUserName.Equals(newUsername)){
						var userNameResponse = Busidex.Mobile.SettingsController.ChangeUserName(newUsername, token);
						user.SetString(newUsername, Busidex.Mobile.Resources.USER_SETTING_USERNAME);
					}
					if(!oldPassword.Equals(newPassword)){
						var passwordResponse = Busidex.Mobile.SettingsController.ChangePassword(newPassword, token);
						user.SetString(newPassword, Busidex.Mobile.Resources.USER_SETTING_PASSWORD);
					}
					if(!oldEmail.Equals(newEmail)){
						var emailResponse = Busidex.Mobile.SettingsController.ChangeEmail(newEmail, token);
						user.SetString(newEmail, Busidex.Mobile.Resources.USER_SETTING_EMAIL);
					}
				}
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}

			var user = NSUserDefaults.StandardUserDefaults;
			string oldUserName = user.StringForKey("UserName");
			string oldPassword = user.StringForKey("Password");
			string oldEmail = user.StringForKey("Email");

			txtUserName.Text = oldUserName;
			txtPassword.Text = oldPassword;
			txtEmail.Text = oldEmail;
		}
	}
}
