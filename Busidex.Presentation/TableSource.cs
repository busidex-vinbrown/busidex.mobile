using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Busidex.Mobile.Models;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Busidex.Presentation.IOS
{
	public delegate void CardSelected();
	public delegate void EditNotesHandler();
	public delegate void CardAddedToMyBusidexHandler(UserCard card);

	public class TableSource : UITableViewSource {

		List<UserCard> tableItems;
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		public UserCard SelectedCard{ get; set; }
		private List<UITableViewCell> cellCache;
		private List<UserCard> Cards{ get; set; }
		private bool NoCards;
		public bool ShowNoCardMessage{ get; set; }
		public event CardAddedToMyBusidexHandler CardAddedToMyBusidex;
		private UIColor CELL_BACKGROUND_COLOR = UIColor.FromRGB (240, 236, 236);
		private const float SUB_LABEL_FONT_SIZE = 17f;
		private const float LABEL_HEIGHT = 30f;
		private const float LABEL_WIDTH = 180f;
		private const float FEATURE_BUTTON_HEIGHT = 42f;
		private const float FEATURE_BUTTON_WIDTH = 42f;
		private enum UIElements{
			CardImage = 1,
			NameLabel = 2,
			CompanyLabel = 3,
			EmailLabel = 4,
			WebsiteLabel = 5,
			PhoneNumberLabel = 6,
			AddToMyBusidexButton = 7,
			MapButton = 8,
			NotesButton = 9
		}

		public TableSource (List<UserCard> items)
		{
			if (items.Count () == 0) {
				NoCards = true;
				items.Add (new UserCard ());
			}
			tableItems = items;
			cellCache = new List<UITableViewCell> ();
			Cards = new List<UserCard> ();

			Cards.AddRange (items);

		}
		public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.ContentView.BackgroundColor = CELL_BACKGROUND_COLOR;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Count;
		}
			
		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			// In here you could customize how you want to get the height for row. Then   
			// just return it. 
			float baseHeight = 220f;
			return NoCards ? baseHeight * 3 : baseHeight;
		}

		public event CardSelected CardSelected;

		public event EditNotesHandler EditingNotes;

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var card = (UserCard)tableItems [indexPath.Row];

			var cell = tableView.DequeueReusableCell (MyBusidexController.BusidexCellId, indexPath);
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
			if (cellCache.All (c => c.Tag != indexPath.Row)) {

				cellCache.Add (cell);
			} 

			if (NoCards && ShowNoCardMessage) {
				LoadNoCardMessage (cell);
			} else {
				AddControls (cell, card, indexPath.Row);
			}
			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		private void GoToCard(int idx){
			this.SelectedCard = Cards[idx];
			if (this.CardSelected != null) {
				this.CardSelected ();
			}
		}

		private void EditNotes(int idx){
			this.SelectedCard = Cards [idx];
			if (this.EditingNotes != null){
				this.EditingNotes ();
			}
		}

		private void LoadNoCardMessage(UITableViewCell cell){

			const string NO_CARDS = "You Don't Have Any Cards In Your Collection. Search for some and add them!";

			float labelHeight = 61f * 3;
			float labelWidth = 280f;

			var frame = new RectangleF (10f, 10f, labelWidth, labelHeight);

			UILabel lbl = new UILabel (frame);
			lbl.Text = NO_CARDS;
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Font = UIFont.FromName ("Helvetica", 17f);
			lbl.Lines = 3;

			//cell.ImageView.Frame = frame;
			cell.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("appBackground.png"));

			cell.ContentView.AddSubview (lbl);

			cell.Frame = frame;

		}

		private void AddToMyBusidex(UserCard userCard){

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();
			if (cookie != null) {
				Busidex.Mobile.MyBusidexController ctrl = new Busidex.Mobile.MyBusidexController ();
				ctrl.AddToMyBusidex (userCard.Card.CardId, cookie.Value);
				if (CardAddedToMyBusidex != null) {
					CardAddedToMyBusidex (userCard);
				}
			}
		}

		private void AddMapButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons){
		
			var MapButton = UIButton.FromType (UIButtonType.System);

			if (card.Card.Addresses.Count () > 0) {
				string address = buildAddress (card.Card);

				if (!string.IsNullOrWhiteSpace (address)) {

					MapButton.TouchUpInside += delegate {

						this.InvokeOnMainThread( ()=>{
							var url = new NSUrl("http://www.maps.google.com/?saddr=" + System.Net.WebUtility.UrlEncode(address.Trim()));
							UIApplication.SharedApplication.OpenUrl(url);
						});	
					};
						
					MapButton.Tag = 8;
					MapButton.SetBackgroundImage (UIImage.FromBundle ("maps.png"), UIControlState.Normal);

					FeatureButtons.Add (MapButton);

				}
			}
		}

		private void AddNotesButton(UserCard card, UITableViewCell cell, ref List<UIButton> FeatureButtons, int idx){

			var NotesButton = UIButton.FromType (UIButtonType.System);
				
			NotesButton.SetBackgroundImage (UIImage.FromBundle ("notes.png"), UIControlState.Normal);
			NotesButton.Tag = (int)UIElements.NotesButton;

			FeatureButtons.Add (NotesButton);

			NotesButton.TouchUpInside += delegate {

				EditNotes(idx);
			};
		}

		private void AddAddToMyBusidexButton(UserCard card, UITableViewCell cell){
			bool needsAddButton = false;
			var AddToMyBusidexButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == (int)UIElements.AddToMyBusidexButton).SingleOrDefault () as UIButton;
			if (AddToMyBusidexButton == null) {
				AddToMyBusidexButton = UIButton.FromType (UIButtonType.System);
				needsAddButton = true;
			}

			if (!card.ExistsInMyBusidex) {
				AddToMyBusidexButton.SetTitle ("Add To My Busidex", UIControlState.Normal);
				AddToMyBusidexButton.Hidden = card.ExistsInMyBusidex;
				AddToMyBusidexButton.Tag = 7;
				AddToMyBusidexButton.Frame = new RectangleF (10f, 100f, 120f, 22f);
				AddToMyBusidexButton.Font = UIFont.FromName ("Helvetica", 12f);
				AddToMyBusidexButton.SetTitleColor (UIColor.Blue, UIControlState.Normal);

				var CheckMark = new UIImageView (new RectangleF (10f, 100f, 22f, 22f));
				CheckMark.Image = UIImage.FromBundle ("checkmark.png");
				CheckMark.Hidden = true;

				AddToMyBusidexButton.TouchUpInside += delegate {
					this.InvokeOnMainThread (() => {
						AddToMyBusidexButton.Hidden = true;
						CheckMark.Hidden = false;
					});

					AddToMyBusidex (card);
				};

				if (needsAddButton) {
					cell.ContentView.AddSubview (AddToMyBusidexButton);
					cell.ContentView.AddSubview (CheckMark);
				}
			} else {
				AddToMyBusidexButton.RemoveFromSuperview ();
			}
		}

		private void AddCardImageButton(UserCard card, UITableViewCell cell, int idx){

			bool needsCardImage = false;

			var CardImageButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == (int)UIElements.CardImage).SingleOrDefault () as UIButton;//?? new UIButton (UIButtonType.Custom);
			if (CardImageButton != null) {
				CardImageButton.RemoveFromSuperview ();
			}
			CardImageButton = new UIButton (UIButtonType.Custom);
			needsCardImage = CardImageButton.Tag <= 0;

			CardImageButton.Tag = 1;

			var fileName = System.IO.Path.Combine (documentsPath, card.Card.FrontFileId + "." + card.Card.FrontType);

			if (File.Exists (fileName)) {
				CardImageButton.SetBackgroundImage (UIImage.FromFile (fileName), UIControlState.Normal); 
			} else {
				CardImageButton.SetBackgroundImage (UIImage.FromBundle ("defaultUserImage.png"), UIControlState.Normal); 
			}

			CardImageButton.TouchUpInside += delegate {
				GoToCard (idx);
			};

			if (needsCardImage) {
				CardImageButton.Frame = new RectangleF (10f, 10f, 120f, 80f);

				cell.ContentView.AddSubview (CardImageButton);
			}
		}

		private void AddNameLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsNameLabel = false;
			var NameLabel = cell.ContentView.Subviews.Where(s=> s.Tag == (int)UIElements.NameLabel).SingleOrDefault() as UILabel;

			if (NameLabel == null) {
				NameLabel = new UILabel (frame);
				needsNameLabel = true;
			}
			NameLabel.Tag = 2;
			NameLabel.Text = string.IsNullOrEmpty(card.Card.Name) ? "(No Name)" : card.Card.Name;
			NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 16f);

			frame.Y += LABEL_HEIGHT;
			if (needsNameLabel) {
				cell.ContentView.AddSubview (NameLabel);
			}
		}

		private void AddCompanyLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsCompanyLabel = false;

			var CompanyLabel = cell.ContentView.Subviews.Where (s => s.Tag == (int)UIElements.CompanyLabel).SingleOrDefault () as UILabel;
			if (CompanyLabel == null) {
				CompanyLabel = new UILabel (frame);
				needsCompanyLabel = true;
			}

			if (!string.IsNullOrWhiteSpace (card.Card.CompanyName)) {

				CompanyLabel.Tag = 3;
				CompanyLabel.Text = card.Card.CompanyName;
				CompanyLabel.Hidden = false;
				CompanyLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);

				frame.Y += LABEL_HEIGHT;
				if (needsCompanyLabel) {
					cell.ContentView.AddSubview (CompanyLabel);
				}
			} else {
				if (CompanyLabel != null) {
					CompanyLabel.RemoveFromSuperview ();
				}
			}
		}

		private void AddEmailLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsEmailLabel = false;

			var EmailLabel = cell.ContentView.Subviews.Where (s => s.Tag == (int)UIElements.EmailLabel).SingleOrDefault () as UITextView;
			if (EmailLabel == null) {
				EmailLabel = new UITextView (frame);
				needsEmailLabel = true;
			}

			if (!string.IsNullOrWhiteSpace (card.Card.Email)) {

				EmailLabel.BackgroundColor = CELL_BACKGROUND_COLOR;
				EmailLabel.Tag = 4;
				EmailLabel.Editable = false;
				EmailLabel.DataDetectorTypes = UIDataDetectorType.Link | UIDataDetectorType.Address;

				EmailLabel.Text = card.Card.Email;
				EmailLabel.ScrollEnabled = false;
				EmailLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);
				EmailLabel.UserInteractionEnabled = true;
				EmailLabel.Hidden = false;
				EmailLabel.Frame = frame;
				frame.Y += LABEL_HEIGHT;

				if (needsEmailLabel) {
					cell.ContentView.AddSubview (EmailLabel);
				}

			} else {
				if (EmailLabel != null) {
					EmailLabel.RemoveFromSuperview ();
				}
			}
		}

		private void AddWebSiteLabel(UserCard card, UITableViewCell cell, ref RectangleF frame){
			var needsWebsiteLabel = false;
			var WebsiteLabel = cell.ContentView.Subviews.Where (s => s.Tag == (int)UIElements.WebsiteLabel).SingleOrDefault () as UITextView;

			if (WebsiteLabel == null) {
				WebsiteLabel = new UITextView (frame);
				needsWebsiteLabel = true;
			}

			if (!string.IsNullOrWhiteSpace (card.Card.Url) && !string.IsNullOrEmpty (card.Card.Url)) {

				WebsiteLabel.BackgroundColor = CELL_BACKGROUND_COLOR;
				WebsiteLabel.Tag = 5;
				WebsiteLabel.Text = card.Card.Url.Trim ().Replace ("http://", "");
				WebsiteLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);
				WebsiteLabel.UserInteractionEnabled = true;
				WebsiteLabel.Editable = false;
				WebsiteLabel.ScrollEnabled = false;
				WebsiteLabel.DataDetectorTypes = UIDataDetectorType.Link;


				WebsiteLabel.Frame = frame;
				frame.Y += LABEL_HEIGHT;
				WebsiteLabel.Hidden = false;
				if (needsWebsiteLabel) {
					cell.ContentView.AddSubview (WebsiteLabel);
				}

			}else{
				if (WebsiteLabel != null) {
					WebsiteLabel.RemoveFromSuperview ();
				}
			}
		}

		private void AddPhoneNumberLabels(UserCard card, UITableViewCell cell, ref RectangleF frame){
			foreach(var item in cell.ContentView.Subviews.Where(c=>c.Tag == (int)UIElements.PhoneNumberLabel)) {
				item.RemoveFromSuperview ();
			}

			if (card.Card.PhoneNumbers != null) {

				foreach (PhoneNumber number in card.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))){

					var newLabel = new UITextView (frame);
					newLabel.UserInteractionEnabled = true;
					newLabel.Tag = 6;
					newLabel.Text = number.Number;
					newLabel.Font = UIFont.FromName ("Helvetica", SUB_LABEL_FONT_SIZE);
					newLabel.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
					newLabel.UserInteractionEnabled = true;
					newLabel.ScrollEnabled = false;
					newLabel.Editable = false;
					newLabel.BackgroundColor = CELL_BACKGROUND_COLOR;

					frame.Y += LABEL_HEIGHT;

					cell.ContentView.AddSubview (newLabel);
				}
			}
		}

		private void AddFeatureButtons(UserCard card, UITableViewCell cell, List<UIButton> FeatureButtons){

			float buttonY = 100f;
			if (!card.ExistsInMyBusidex) {
				buttonY += LABEL_HEIGHT;
			}
		
			int gridCol = 0;
			float padding = 10f;
			var mapButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == (int)UIElements.MapButton).SingleOrDefault () as UIButton;
			if(mapButton != null){
				mapButton.RemoveFromSuperview ();
			}
			var notesButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == (int)UIElements.NotesButton).SingleOrDefault () as UIButton;
			if(notesButton != null){
				notesButton.RemoveFromSuperview ();
			}

			foreach(var button in FeatureButtons){
			
				float buttonX = padding + (gridCol * FEATURE_BUTTON_WIDTH);
				gridCol++;
				button.Frame = new RectangleF (buttonX, buttonY, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
				cell.ContentView.AddSubview (button);
			}

		}

		public void AddControls(UITableViewCell cell, UserCard card, int idx){

			if (card != null && card.Card != null) {

				AddCardImageButton (card, cell, idx);

				var FeatureButtonList = new List<UIButton> ();

				AddAddToMyBusidexButton (card, cell);

				AddMapButton (card, cell, ref FeatureButtonList);

				AddNotesButton (card, cell, ref FeatureButtonList, idx);

				var frame = new RectangleF (140f, 10f, LABEL_WIDTH, LABEL_HEIGHT);

				AddNameLabel (card, cell, ref frame);

				AddCompanyLabel (card, cell, ref frame);

				AddEmailLabel (card, cell, ref frame);

				AddWebSiteLabel (card, cell, ref frame);

				AddPhoneNumberLabels (card, cell, ref frame);

				AddFeatureButtons (card, cell, FeatureButtonList);
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}

		private string buildAddress(Card card){

			var address = string.Empty;
			address += string.IsNullOrEmpty(card.Addresses [0].Address1) ? string.Empty : card.Addresses [0].Address1;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].Address2) ? string.Empty : card.Addresses [0].Address2;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].City) ? string.Empty : card.Addresses [0].City;
			address += " ";
			address += card.Addresses [0].State != null ? string.Empty : card.Addresses [0].State.Code;
			address += " ";
			address += string.IsNullOrEmpty(card.Addresses [0].ZipCode) ? string.Empty : card.Addresses [0].ZipCode;

			return address;
		}
	}
}