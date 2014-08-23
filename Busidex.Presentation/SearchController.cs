using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Busidex.Presentation.IOS
{
	partial class SearchController : BaseController
	{
		public static NSString cellID = new NSString ("cellId");
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		LoadingOverlay Overlay;

		public SearchController (IntPtr handle) : base (handle)
		{
			//TableView.RegisterClassForCell (typeof(SearchViewCell), cellID);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//vwSearchResults.RegisterClassForCell (typeof(SearchViewCell), cellID);
			vwSearchResults.RegisterClassForCellReuse (typeof(UITableViewCell), cellID);


			vwSearchResults.Hidden = true;
			btnSearch.TouchUpInside += delegate {
				StartSearch ();
					DoSearch();
					vwSearchResults.Hidden = false;
					txtSearch.ResignFirstResponder(); // hide keyboard
			};
		}
			
		private void GoToCard(){
			var cardController = this.Storyboard.InstantiateViewController ("CardViewController") as CardViewController;
			cardController.UserCard = ((TableSource)this.vwSearchResults.Source).SelectedCard;

			if (cardController != null) {
				this.NavigationController.PushViewController (cardController, true);
			}
		}

		public override void ViewWillAppear (bool animated)
		{

			base.ViewWillAppear (animated);
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}
		}

		private void LoadSearchResults(List<UserCard> cards){

			var src = new TableSource (cards);
			src.CardSelected += delegate {
				GoToCard();
			};
			this.vwSearchResults.Source = src; //new CollectionSource (cards);
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			Overlay.Hide ();

		}

		private void StartSearch(){

			this.InvokeOnMainThread (() => {
				Overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				View.Add (Overlay);
			});

			this.vwSearchResults.Source = new TableSource (new List<UserCard>());
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			this.View.SetNeedsDisplay ();

		}

		public async Task<int> DoSearch(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
			string token = string.Empty;

			if (cookie != null) {
				token = Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes (cookie.Value));
			}

			var ctrl = new Busidex.Mobile.SearchController ();
			string response = await ctrl.DoSearch (txtSearch.Text, token);

			if (!string.IsNullOrEmpty (response)) {
			
				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response);
				List<UserCard> cards = new List<UserCard> ();
				float total = Search.SearchModel.Results.Count;
				float processed = 0;

				foreach (var item in Search.SearchModel.Results) {
					if (item != null) {

						var imagePath = Busidex.Mobile.Utils.CARD_PATH + item.FrontFileId + "." + item.FrontType;
						var fName = item.FrontFileId + "." + item.FrontType;

						var userCard = new UserCard ();
						userCard.Card = item;
						userCard.CardId = item.CardId;
						cards.Add (userCard);

						if (!File.Exists (System.IO.Path.Combine (documentsPath, item.FrontFileId + "." + item.FrontType))) {
							Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

								if (++processed == total) {

									this.InvokeOnMainThread (() => {
										LoadSearchResults (cards);
									});

								} 
							});
						} else {

							if (++processed == total) {
								LoadSearchResults (cards);
							}
						}
					}
				}
			}
			return 1;
		}
	}
}
