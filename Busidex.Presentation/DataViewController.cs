using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Busidex.Mobile.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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

		private enum LoginVisibleSetting{
			Show = 1,
			Hide = 2
		}

		//private static long UserId{ get; set; }
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		private string GetDeviceId(){
			var thisDeviceId = UIDevice.CurrentDevice.IdentifierForVendor;
			if (thisDeviceId != null) {
				var dIdString = thisDeviceId.AsString ();
				return dIdString;
			}
			return string.Empty;
		}

//		private void DoLogin(){
//			string uidId = GetDeviceId ();
//			var userId = Busidex.Mobile.LoginController.DoLogin (uidId);
//			if (userId > 0) {
//				var nCookie = new System.Net.Cookie();
//
//				nCookie.Name = "UserId";
//				DateTime expiration = DateTime.Now.AddYears(1);
//				nCookie.Expires = expiration;
//				nCookie.Value = EncodeUserId(userId);
//				var cookie = new NSHttpCookie(nCookie);
//
//				NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
//			}
//		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			/*
			 * Check the authentication cookie.
			 * If it is null, use the device id to AutoRegister. This will
			 * use the device ID as the username and password and create the
			 * user account if one does not already exist. Then it will return 
			 * the userId. Set the authentication cookie and continue.
			 */

//			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies
//									.Where(c=>c.Name == Busidex.Mobile.Resources.AuthenticationCookieName)
//									.SingleOrDefault();
//			long userId;
//			if (cookie != null) {
//				userId = Busidex.Mobile.Utils.DecodeUserId (cookie.Value);
//				if (userId > 0) {
//					//UserId = userId;
//					SetLoginDisplay (LoginVisibleSetting.Hide);
//				} else {
//					DoLogin ();
//					SetLoginDisplay (LoginVisibleSetting.Hide);
//				}
//			} else {
//				DoLogin ();
//
//				SetLoginDisplay (LoginVisibleSetting.Hide);
//			}


			btnGoToSearch.TouchUpInside += delegate {
				GoToSearch();
			};

			btnGoToMyBusidex.TouchUpInside += delegate {
				LoadMyBusidexAsync();
			};

			btnGoToMyBusidex.TouchDown += delegate {
//				prgDownload.Hidden = false;
//				prgDownload.SetNeedsDisplay ();
				lblLoading.Hidden = false;
				spnLoading.Hidden = false;
			};
			lblLoading.Hidden = true;
			spnLoading.Hidden = true;

//			prgDownload.SetProgress (0f, false);

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
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (true, true);
			}
		}

//		private void GoToLogin ()
//		{
//			var loginController = this.Storyboard.InstantiateViewController ("LoginController") as LoginController;
//
//			if (loginController != null) {
//				this.NavigationController.PushViewController (loginController, true);
//
//			}
//		}

		private void GoToMyBusidex ()
		{
			//prgDownload.Hidden = true;
			var myBusidexController = this.Storyboard.InstantiateViewController ("MyBusidexController") as MyBusidexController;

			if (myBusidexController != null && this.NavigationController.ChildViewControllers.Where(c=> c is MyBusidexController).Count() == 0){
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

		private void LoadMyBusidexAsync(){
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);


				if(!string.IsNullOrEmpty(response)){
					MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (response);
					//float total = MyBusidexResponse.MyBusidex.Busidex.Count ();

					List<UserCard> cards = new List<UserCard> ();

					//float processed = 0;
					foreach (var item in MyBusidexResponse.MyBusidex.Busidex) {
						if (item.Card != null) {

							var imagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.FrontFileId + "." + item.Card.FrontType;
							var fName = item.Card.FrontFileId + "." + item.Card.FrontType;

							cards.Add (item);

							//float currentItem = ++processed;
						
							//var pct = Math.Round ((currentItem / total), 2) * 100;

							if (!File.Exists (documentsPath + "/" + fName)) {
								 	Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

//									if (pct >= 100) {
//										InvokeOnMainThread (() => {	
//											GoToMyBusidex ();
//										});
//									}  

								});
							} 

						}
					}
//					if (processed == total) {
//						InvokeOnMainThread (() => {	
							lblLoading.Hidden = true;
							spnLoading.Hidden = true;
							GoToMyBusidex ();
					//	});
					//} 

				}
			}
		}

	}
}

