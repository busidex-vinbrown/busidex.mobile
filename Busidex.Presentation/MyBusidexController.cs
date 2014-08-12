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

		private void LoadMyBusidexAsync(){
			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);

				if(!string.IsNullOrEmpty(response)){
					MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (response);

					List<UserCard> cards = new List<UserCard> ();
					//prgDownload.Progress = 0f;

					//Progress<DownloadProgress> progressReporter = new Progress<DownloadProgress> ();
					//progressReporter.ProgressChanged += (s, args) =>  prgDownload.Progress = args.PercentComplete;

					int processed = 0;
					foreach (var item in MyBusidexResponse.MyBusidex.Busidex) {
						if (item.Card != null) {

//							var imagePath = Busidex.Mobile.Utils.CARD_PATH + item.Card.FrontFileId + "." + item.Card.FrontType;
//							var fName = item.Card.FrontFileId + item.Card.FrontType;

//							if (!File.Exists (fName)) {
//								Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {
//
////									IProgress<DownloadProgress> reporter = new Progress<DownloadProgress> ();
////
////									float currentItem = ++processed;
////									float total = MyBusidexResponse.MyBusidex.Busidex.Count ();
////									var pct = Math.Round ((currentItem / total), 2) * 100;
////
////									if (pct >= 100) {
////
////									} else {
////										DownloadProgress args = new DownloadProgress(fName, currentItem, total);
////										reporter.Report(args);
////
////									}
//								});
//							}
							cards.Add (item);
						}
					}

					var src = new TableSource (cards);

					src.CardSelected += delegate {
						GoToCard();
					};
					this.TableView.Source = src;
					this.TableView.AllowsSelection = true;

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

			LoadMyBusidexAsync ();




		}
	}
}
