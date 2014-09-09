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
			txtSearch.SearchButtonClicked += delegate {
				StartSearch ();
					DoSearch();
					vwSearchResults.Hidden = false;
					txtSearch.ResignFirstResponder(); // hide keyboard
			};
			//txtSearch.SearchBarStyle = UISearchBarStyle.Minimal;
			txtSearch.CancelButtonClicked += delegate {
				txtSearch.ResignFirstResponder();
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
			src.ShowNoCardMessage = false;
			src.CardSelected += delegate {
				GoToCard();
			};
			src.CardAddedToMyBusidex += new CardAddedToMyBusidexHandler (AddCardToMyBusidex);

			this.vwSearchResults.Source = src; //new CollectionSource (cards);
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			Overlay.Hide ();

		}

		private void AddCardToMyBusidex(UserCard userCard){
			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			// we only need to update the file if they've gotten their busidex. If they haven't, the new card will
			// come along with all the others
			var file = string.Empty;
			if (File.Exists (fullFilePath)) {
				using (var myBusidexFile = File.OpenText (fullFilePath)) {
					var myBusidexJson = myBusidexFile.ReadToEnd ();
					MyBusidexResponse myBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (myBusidexJson);
					myBusidexResponse.MyBusidex.Busidex.Add (userCard);
					file = Newtonsoft.Json.JsonConvert.SerializeObject(myBusidexResponse);
				}

				File.WriteAllText (fullFilePath, file);
			}
		}

		private void StartSearch(){

			this.InvokeOnMainThread (() => {
				Overlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
				View.Add (Overlay);
			});

			var src = new TableSource (new List<UserCard>());
			this.vwSearchResults.Source = src;
			this.vwSearchResults.ReloadData ();
			this.vwSearchResults.AllowsSelection = true;
			this.vwSearchResults.SetNeedsDisplay ();

			this.View.SetNeedsDisplay ();

		}

		public async Task<int> DoSearch(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
			string token = string.Empty;

			if (cookie != null) {
				token = cookie.Value;
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
						userCard.ExistsInMyBusidex = item.ExistsInMyBusidex;
						userCard.Card = item;
						userCard.CardId = item.CardId;
						cards.Add (userCard);

						if (!File.Exists (System.IO.Path.Combine (documentsPath, item.FrontFileId + "." + item.FrontType))) {
							await Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {

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
