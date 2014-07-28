using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Busidex.Presentation.IOS
{
	public partial class DataViewController : UIViewController
	{
		LoadingOverlay loadingOverlay;

		public DataViewController (IntPtr handle) : base (handle)
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

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
			long userId;
			if (cookie != null) {
				userId = Busidex.Mobile.Utils.DecodeUserId(cookie.Value);
				if (userId > 0) {
					UserId = userId;
					GoToMyBusidex ();
				}
			}



			btnLogin.TouchUpInside += (o,s) => {

				loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				View.Add (loadingOverlay);

				string username = txtUserName.Text;
				string password = txtPassword.Text;


				UserId = Busidex.Mobile.LoginController.DoLogin(username, password);

				if(UserId > 0){

					var nCookie = new System.Net.Cookie();
					nCookie.Name = "UserId";
					DateTime expiration = DateTime.Now.AddYears(1);
					nCookie.Expires = expiration;
					nCookie.Value = EncodeUserId(UserId);
					cookie = new NSHttpCookie(nCookie);

					NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
					//NSHttpCookie test = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
					GoToMyBusidex();
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

			dataLabel.Text = DataObject;
		}

		private void GoToMyBusidex ()
		{

			// set the View Controller that’s powering the screen we’re
			// transitioning to

			var myBusidexController = this.Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;


			//set the Table View Controller’s list of phone numbers to the
			// list of dialed phone numbers

			if (myBusidexController != null) {
				this.NavigationController.PushViewController (myBusidexController, true);

			}
		}


	}
}

