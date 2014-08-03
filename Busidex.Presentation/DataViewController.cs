using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Busidex.Presentation.IOS
{
	public partial class DataViewController : UIViewController
	{

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
				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
				if (userId > 0) {
					UserId = userId;
					SetLoginDisplay (false);
				} else {
					SetLoginDisplay (true);
				}
			} else {
				SetLoginDisplay (true);
			}

			btnLogin.TouchUpInside += delegate {
				GoToLogin();
			};

			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch();
			};

			btnGoToMyBusidex.TouchUpInside += delegate {
				GoToMyBusidex();
			};
				
			// Perform any additional setup after loading the view, typically from a nib.
		}

		private void SetLoginDisplay(bool visible){
			btnLogin.Hidden = !visible;
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

		private void GoToLogin ()
		{
			var loginController = this.Storyboard.InstantiateViewController ("LoginController") as LoginController;

			if (loginController != null) {
				this.NavigationController.PushViewController (loginController, true);

			}
		}

		private void GoToMyBusidex ()
		{

			var myBusidexController = this.Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null) {
				this.NavigationController.PushViewController (myBusidexController, true);
			}
		}

		private void GoToSearch ()
		{
			var searchController = this.Storyboard.InstantiateViewController ("SearchController") as SearchController;

			if (searchController != null) {
				this.NavigationController.PushViewController (searchController, true);
			}
		}

	}
}

