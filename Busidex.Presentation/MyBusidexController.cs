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
	partial class MyBusidexController : BaseController
	{
		public static NSString BusidexCellId = new NSString ("cellId");
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		//UISearchBar SearchBar;
		List<UserCard> FilterResults;

		public MyBusidexController (IntPtr handle) : base (handle)
		{


		}

		private void SetFilter(string filter){
			FilterResults = new List<UserCard> ();
			FilterResults.AddRange (
				Application.MyBusidex.Where (c => 
					(!string.IsNullOrEmpty (c.Card.Name) && c.Card.Name.Contains (filter)) ||
				(!string.IsNullOrEmpty (c.Card.CompanyName) && c.Card.CompanyName.Contains (filter)) ||
				(!string.IsNullOrEmpty (c.Card.Email) && c.Card.Email.Contains (filter)) ||
				(!string.IsNullOrEmpty (c.Card.Url) && c.Card.Url.Contains (filter)) ||
				(c.Card.PhoneNumbers != null && c.Card.PhoneNumbers.Any (p => p.Number.Contains (filter)))
				));

			var src = new TableSource (FilterResults);
			src.ShowNoCardMessage = true;
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
			};
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		private void ResetFilter(){

			var src = new TableSource (Application.MyBusidex);
			SearchBar.Text = string.Empty;
			src.ShowNoCardMessage = true;
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
			};	
			TableView.Source = src;
			TableView.ReloadData ();
			TableView.AllowsSelection = true;
			TableView.SetNeedsDisplay ();
		}

		private void ConfigureSearchBar(){
			//SearchBar = new UISearchBar ();
			SearchBar.Placeholder = "Filter";
			SearchBar.BarStyle = UIBarStyle.Default;
			//SearchBar.ShowsSearchResultsButton = true;
			SearchBar.ShowsCancelButton = true;
			//SearchBar.Frame = new System.Drawing.RectangleF (0, 0, UIScreen.MainScreen.Bounds.Width, 40);


			SearchBar.SearchButtonClicked += delegate {
				SetFilter(SearchBar.Text);
				SearchBar.ResignFirstResponder();
			};
			SearchBar.CancelButtonClicked += delegate {
				ResetFilter();
				SearchBar.ResignFirstResponder();
			};

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

		private void EditNotes(){

			var notesController = this.Storyboard.InstantiateViewController ("NotesController") as NotesController;
			notesController.UserCard = ((TableSource)this.TableView.Source).SelectedCard;

			if (notesController != null) {
				this.NavigationController.PushViewController (notesController, true);
			}
		}
		private void LoadMyBusidex(string data){
			MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (data);

			Application.MyBusidex = new List<UserCard> ();
			MyBusidexResponse.MyBusidex.Busidex.ForEach (c => c.ExistsInMyBusidex = true);
			Application.MyBusidex.AddRange (MyBusidexResponse.MyBusidex.Busidex.Where (c => c.Card != null));

			var src = new TableSource (Application.MyBusidex);
			src.ShowNoCardMessage = true;
			src.CardSelected += delegate {
				GoToCard();
			};
			src.EditingNotes += delegate {
				EditNotes();
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

			//if (this.TableView == null) {
			//	this.TableView = new UITableView();
			//}
			//if (this.SearchBar == null) {
			//	SearchBar = new UISearchBar ();
			//}

			ConfigureSearchBar ();

			var fullFilePath = Path.Combine (documentsPath, Application.MY_BUSIDEX_FILE);
			this.TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);
			if (File.Exists (fullFilePath)) {
				LoadMyBusidexFromFile (fullFilePath);
			} else {
				LoadMyBusidexAsync ();
			}
		}


	}
}
