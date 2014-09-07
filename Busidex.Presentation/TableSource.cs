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
		private const float SUB_LABLE_FONT_SIZE = 14f;

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
			float baseHeight = 190f;
			return NoCards ? baseHeight * 3 : baseHeight;
		}

		public event CardSelected CardSelected;

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

		public void AddControls(UITableViewCell cell, UserCard card, int idx){
		
			//var PhoneNumberLabels = new List<UITextView> ();

			UIButton CardImageButton = null;
			UIButton MapButton = null;
			UIButton AddToMyBusidexButton = null;
			UIImageView CheckMark = null;
			UILabel NameLabel = null;
			UILabel CompanyLabel = null;
			UITextView WebsiteLabel = null;
			UITextView EmailLabel = null;

			if (card != null && card.Card != null) {

				bool needsCardImage = false;

				CardImageButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == 1).SingleOrDefault () as UIButton;//?? new UIButton (UIButtonType.Custom);
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

				bool needsAddButton = false;
				AddToMyBusidexButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == 7).SingleOrDefault () as UIButton;
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

					CheckMark = new UIImageView (new RectangleF (10f, 100f, 22f, 22f));
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

				bool needsMapButton = false;
				MapButton = cell.ContentView.Subviews.Where (s => s is UIButton && s.Tag == 8).SingleOrDefault () as UIButton;
				if (MapButton == null) {
					MapButton = UIButton.FromType (UIButtonType.System);
					needsMapButton = true;
				}

				if (card.Card.Addresses.Count () > 0) {
					string address = buildAddress (card.Card);

					if (!string.IsNullOrWhiteSpace (address)) {

						MapButton.TouchUpInside += delegate {
						
							this.InvokeOnMainThread( ()=>{
								var url = new NSUrl("http://www.maps.google.com/?saddr=" + System.Net.WebUtility.UrlEncode(address.Trim()));
								UIApplication.SharedApplication.OpenUrl(url);
							});	
						};

						float mapButtonTop = 100f;
						if (!card.ExistsInMyBusidex) {
							mapButtonTop += 25f;
						}
						MapButton.Frame = new RectangleF (10f, mapButtonTop, 35f, 35f);
						MapButton.Tag = 8;
						MapButton.SetBackgroundImage (UIImage.FromBundle ("maps.png"), UIControlState.Normal);

						if (needsMapButton) {
							cell.ContentView.AddSubview (MapButton);
						}
					}
				} else {
					if (MapButton != null) {
						MapButton.Hidden = true;
					}
				}

				float labelHeight = 21f;
				float labelWidth = 180f;

				var frame = new RectangleF (140f, 10f, labelWidth, labelHeight);

				var needsNameLabel = false;
				NameLabel = cell.ContentView.Subviews.Where(s=> s.Tag == 2).SingleOrDefault() as UILabel;

				if (NameLabel == null) {
					NameLabel = new UILabel (frame);
					needsNameLabel = true;
				}
				NameLabel.Tag = 2;
				NameLabel.Text = string.IsNullOrEmpty(card.Card.Name) ? "(No Name)" : card.Card.Name;
				NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 16f);
			
				frame.Y += 21;
				if (needsNameLabel) {
					cell.ContentView.AddSubview (NameLabel);
				}

				var needsCompanyLabel = false;

				CompanyLabel = cell.ContentView.Subviews.Where (s => s.Tag == 3).SingleOrDefault () as UILabel;
				if (CompanyLabel == null) {
					CompanyLabel = new UILabel (frame);
					needsCompanyLabel = true;
				}

				if (!string.IsNullOrWhiteSpace (card.Card.CompanyName)) {

					CompanyLabel.Tag = 3;
					CompanyLabel.Text = card.Card.CompanyName;
					CompanyLabel.Hidden = false;
					CompanyLabel.Font = UIFont.FromName ("Helvetica", SUB_LABLE_FONT_SIZE);

					frame.Y += 25;
					if (needsCompanyLabel) {
						cell.ContentView.AddSubview (CompanyLabel);
					}
				} else {
					if (CompanyLabel != null) {
						CompanyLabel.RemoveFromSuperview ();
					}
				}

				var needsEmailLabel = false;

				EmailLabel = cell.ContentView.Subviews.Where (s => s.Tag == 4).SingleOrDefault () as UITextView;
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
					EmailLabel.Font = UIFont.FromName ("Helvetica", SUB_LABLE_FONT_SIZE);
					EmailLabel.UserInteractionEnabled = true;
					EmailLabel.Hidden = false;
					frame.Y += 25;
					if (needsEmailLabel) {
						cell.ContentView.AddSubview (EmailLabel);
					}

				} else {
					if (EmailLabel != null) {
						EmailLabel.RemoveFromSuperview ();
					}
				}

				var needsWebsiteLabel = false;
				WebsiteLabel = cell.ContentView.Subviews.Where (s => s.Tag == 5).SingleOrDefault () as UITextView;

				if (WebsiteLabel == null) {
					WebsiteLabel = new UITextView (new RectangleF (140f, frame.Y, labelWidth, labelHeight));
					needsWebsiteLabel = true;
				}
					
				if (!string.IsNullOrWhiteSpace (card.Card.Url) && string.IsNullOrEmpty (card.Card.Url)) {

					WebsiteLabel.BackgroundColor = CELL_BACKGROUND_COLOR;
					WebsiteLabel.Tag = 5;
					WebsiteLabel.Text = card.Card.Url.Trim ().Replace ("http://", "");
					WebsiteLabel.Font = UIFont.FromName ("Helvetica", SUB_LABLE_FONT_SIZE);
					WebsiteLabel.UserInteractionEnabled = true;
					WebsiteLabel.Editable = false;
					WebsiteLabel.ScrollEnabled = false;
					WebsiteLabel.DataDetectorTypes = UIDataDetectorType.Link;
		
					frame.Y += 25;
					WebsiteLabel.Hidden = false;
					if (needsWebsiteLabel) {
						cell.ContentView.AddSubview (WebsiteLabel);
					}

				}else{
					if (WebsiteLabel != null) {
						WebsiteLabel.RemoveFromSuperview ();
					}
				}

				foreach(var item in cell.ContentView.Subviews.Where(c=>c.Tag == 6)) {
					//if (cell.ContentView.Subviews [i].Tag == 6) {
						item.RemoveFromSuperview ();
					//}
				}
					
				if (card.Card.PhoneNumbers != null) {

					foreach (PhoneNumber number in card.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))){
						var newLabel = new UITextView (frame);
						newLabel.UserInteractionEnabled = true;
						newLabel.Tag = 6;
						newLabel.Text = number.Number;
						newLabel.Font = UIFont.FromName ("Helvetica", SUB_LABLE_FONT_SIZE);
						newLabel.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
						newLabel.UserInteractionEnabled = true;
						newLabel.ScrollEnabled = false;
						newLabel.Editable = false;
						newLabel.BackgroundColor = CELL_BACKGROUND_COLOR;

						frame.Y += 25;

						cell.ContentView.AddSubview (newLabel);

						//PhoneNumberLabels.Add (newLabel);
					}
				}
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