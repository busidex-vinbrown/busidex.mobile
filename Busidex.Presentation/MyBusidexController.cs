using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace Busidex.Presentation.IOS
{
	partial class MyBusidexController : UITableViewController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);


		public MyBusidexController (IntPtr handle) : base (handle)
		{
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		private void GoToCard(){
			var cardController = this.Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)this.TableView.Source).SelectedCard;

			if (cardController != null) {
				this.NavigationController.PushViewController (cardController, true);
			}
		}

		private void LoadMyBusidex(string data){
			MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);

			Application.MyBusidex = new List<UserCard> ();
			Application.MyBusidex.AddRange (MyBusidexResponse.MyBusidex.Busidex.Where (c => c.Card != null));

			var src = new TableSource (Application.MyBusidex);

			src.CardSelected += delegate {
				GoToCard();
			};
			if (this.TableView.Source == null) {
				this.TableView.Source = src;
			}
			this.TableView.AllowsSelection = true;
		}

		private void LoadMyBusidexFromFile(string fullFilePath){
		
			if(File.Exists(fullFilePath)){
				var myBusidexFile = File.OpenText (fullFilePath);
				var myBusidexJson = myBusidexFile.ReadToEnd ();
				LoadMyBusidex (myBusidexJson);
			}
		}

		private void SaveMyBusidexResponse(string response){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			File.WriteAllText (fullFilePath, response);
		}

		private void LoadMyBusidexAsync(){
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);

				if(!string.IsNullOrEmpty(response)){
					LoadMyBusidex (response);

					SaveMyBusidexResponse (response);
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			if (File.Exists (fullFilePath)) {
				LoadMyBusidexFromFile (fullFilePath);
			} else {
				LoadMyBusidexAsync ();
			}
		}


	}
}
