using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Busidex.Presentation.IOS
{
	partial class SearchController : UIViewController
	{
		public static NSString cellID = new NSString ("cellId");

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

		public void DoSearch(){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
			string token = string.Empty;


			this.vwSearchResults.Source = new  TableSource (new List<UserCard> ());
			this.vwSearchResults.SetNeedsDisplay ();

			if (cookie != null) {
				token = Convert.ToBase64String (System.Text.Encoding.ASCII.GetBytes (cookie.Value));
			}

			var ctrl = new Busidex.Mobile.SearchController ();
			var response = ctrl.DoSearch (txtSearch.Text, token);

			if (!string.IsNullOrEmpty (response)) {
			
				SearchResponse Search = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResponse> (response);
				List<UserCard> cards = new List<UserCard> ();

				foreach (var item in Search.SearchModel.Results) {
					if (item != null) {

						var userCard = new UserCard ();
						userCard.Card = item;
						userCard.CardId = item.CardId;
						cards.Add (userCard);

					}
				}

				var src = new TableSource (cards);
				src.CardSelected += delegate {
					GoToCard();
				};
				this.vwSearchResults.Source = src; //new CollectionSource (cards);
				this.vwSearchResults.ReloadData ();
				this.vwSearchResults.AllowsSelection = true;
				this.vwSearchResults.SetNeedsDisplay ();
			}
		}
	}
}
