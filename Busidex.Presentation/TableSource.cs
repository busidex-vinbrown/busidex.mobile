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
	public class TableSource : UITableViewSource {
		ObservableCollection<UserCard> tableItems;
		//NSString cellIdentifier = new NSString( "BusidexDataCell");
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		WebClient webClient;

		public TableSource (ObservableCollection<UserCard> items)
		{
			tableItems = items;
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Count;
		}
			
		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			// In here you could customize how you want to get the height for row. Then   
			// just return it. 

			return 150;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (MyBusidexController.BusidexCellId, indexPath);

			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			var card = (UserCard)tableItems [indexPath.Row];
		
			AddControls (cell, card);

			cell.SetNeedsDisplay ();

			return cell;
		}

		public void AddControls(UITableViewCell cell, UserCard card){


			var PhoneNumberLabels = new List<UITextView> ();

		    UIImageView	CardImage = null;
			UILabel NameLabel = null;
			UILabel CompanyLabel = null;
			UITextView WebsiteLabel = null;
			UITextView EmailLabel = null;
			for(var i=0; i<	PhoneNumberLabels.Count - 1; i++){
				PhoneNumberLabels [i] = null;
			}
			PhoneNumberLabels = new List<UITextView> ();

			if (card != null && card.Card != null) {

				bool needsCardImage = false;

				CardImage = cell.ContentView.Subviews.Where(s=> s.Tag == 1).SingleOrDefault() as UIImageView ?? new UIImageView ();
				 
				needsCardImage = CardImage.Tag <= 0;

				CardImage.Tag = 1;

				var fileName = System.IO.Path.Combine (documentsPath, card.Card.FrontFileId + "." + card.Card.FrontType);
				if (File.Exists (fileName)) {

					CardImage.Image = UIImage.FromFile (fileName); 

				} else {
					var imagePath = Busidex.Mobile.Utils.CARD_PATH + card.Card.FrontFileId + "." + card.Card.FrontType;
					var fName = card.Card.FrontFileId + "." + card.Card.FrontType;

					Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {
					

					 	fileName = t.Result;
						string jpgFilename = System.IO.Path.Combine (documentsPath, fileName);

						using(var f = File.Open(t.Result, FileMode.Open)){
							//var newFile = File.Create(jpgFilename);
							//newFile.Close();

							using (var data = NSData.FromFile (jpgFilename)) {
								CardImage.Image = UIImage.LoadFromData (data); 
							}
						}
					});
				}

				//CardImage.Image = GetImageFromUrl (Busidex.Mobile.Utils.CARD_PATH + card.Card.FrontFileId + "." + card.Card.FrontType);

				if (needsCardImage) {
					CardImage.Frame = new RectangleF (10, 10f, 120f, 80f);
					cell.ContentView.AddSubview (CardImage);
				}

				float labelHeight = 21f;
				float labelWidth = 140f;

				var frame = new RectangleF (140f, 10f, labelWidth, labelHeight);

				NameLabel = cell.ContentView.Subviews.Where(s=> s.Tag == 2).SingleOrDefault() as UILabel ?? new UILabel (frame);
			
				NameLabel.Tag = 2;
				NameLabel.Text = string.IsNullOrEmpty(card.Card.Name) ? "(No Name)" : card.Card.Name;
				NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 17f);
			
				frame.Y += 21;
				cell.ContentView.AddSubview (NameLabel);

				if (!string.IsNullOrWhiteSpace (card.Card.CompanyName)) {

					CompanyLabel = cell.ContentView.Subviews.Where (s => s.Tag == 3).SingleOrDefault () as UILabel ?? new UILabel (frame);

					CompanyLabel.Tag = 3;
					CompanyLabel.Text = card.Card.CompanyName;
					CompanyLabel.Hidden = false;
					CompanyLabel.Font = UIFont.FromName ("Helvetica", 12f);

					frame.Y += 21;
					cell.ContentView.AddSubview (CompanyLabel);
				} else {
					if (CompanyLabel != null) {
						CompanyLabel.Hidden = true;
					}
				}

				if (!string.IsNullOrWhiteSpace (card.Card.Email)) {
				
					EmailLabel = cell.ContentView.Subviews.Where (s => s.Tag == 4).SingleOrDefault () as UITextView ?? new UITextView (frame);

					EmailLabel.Tag = 4;
					EmailLabel.Editable = false;
					EmailLabel.DataDetectorTypes = UIDataDetectorType.Link | UIDataDetectorType.Address;

					EmailLabel.Text = card.Card.Email;
					EmailLabel.Font = UIFont.FromName ("Helvetica", 12f);
					EmailLabel.UserInteractionEnabled = true;
					EmailLabel.Hidden = false;
					frame.Y += 21;
					cell.ContentView.AddSubview (EmailLabel);

				} else {
					if (EmailLabel != null) {
						EmailLabel.Hidden = true;
					}
				}

				if (!string.IsNullOrWhiteSpace (card.Card.Url)) {

					WebsiteLabel = cell.ContentView.Subviews.Where (s => s.Tag == 5).SingleOrDefault () as UITextView ?? new UITextView (frame);

					WebsiteLabel.Tag = 5;

					WebsiteLabel.Text =  card.Card.Url.Trim ().Replace ("http://", "");
					WebsiteLabel.Font = UIFont.FromName ("Helvetica", 12f);
					WebsiteLabel.UserInteractionEnabled = true;
					WebsiteLabel.Editable = false;
					WebsiteLabel.DataDetectorTypes = UIDataDetectorType.Link;
		
					frame.Y += 21;
					WebsiteLabel.Hidden = false;
					cell.ContentView.AddSubview (WebsiteLabel);

				} else {
					if (WebsiteLabel != null) {
						WebsiteLabel.Hidden = true;
					}
				}

				if (card.Card.PhoneNumbers != null) {

					for (var i = 0; i < cell.ContentView.Subviews.Length - 1; i++) {
						if (cell.ContentView.Subviews [i].Tag == 6) {
							cell.ContentView.Subviews [i].RemoveFromSuperview ();
						}
					}

					foreach (PhoneNumber number in card.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))){
						var newLabel = new UITextView (frame);
						newLabel.UserInteractionEnabled = true;
						newLabel.Tag = 6;
						newLabel.Text = number.Number;
						newLabel.Font = UIFont.FromName ("Helvetica", 12f);
						newLabel.DataDetectorTypes = UIDataDetectorType.PhoneNumber;
						newLabel.UserInteractionEnabled = true;

						newLabel.Editable = false;

						frame.Y += 21;

//						var labelTap = new UITapGestureRecognizer(
//							tap => DoTap(number.Number)
//						);
//						newLabel.AddGestureRecognizer(labelTap);

						cell.ContentView.AddSubview (newLabel);

						PhoneNumberLabels.Add (newLabel);
					}
				}


			}

		}



//		private UIImage GetImageFromUrl (string uri)
//		{
//			try {
//				using (var url = new NSUrl (uri)) {
//					using (var data = NSData.FromUrl (url)) {
//						return UIImage.LoadFromData (data);
//					}
//				}
//			} catch (Exception ex) {
//
//			}
//			return null;
//		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			//WHAT TO DO HERE
		
//			var cell = ((UITableViewCell)tableView.CellAt (indexPath));//.getText().BecomeFirstResponder();
//			CardDisplay c = (CardDisplay)cell.ContentView.Subviews [0];
//			string control = c.SelectedControl;
//			switch (control) {
//			case "EMAIL":
//				{
//					c.EmailLabel.BecomeFirstResponder ();
//					UIApplication.SharedApplication.OpenUrl(new NSUrl("mailto:" + c.EmailLabel.TitleLabel.Text));
//					break;
//				}
//			case "URL":
//				{
//					c.WebsiteLabel.BecomeFirstResponder ();
//					UIApplication.SharedApplication.OpenUrl(new NSUrl(c.WebsiteLabel.Text));
//					break;
//				}
//			default:
//				{
//					UIApplication.SharedApplication.OpenUrl(new NSUrl("tel:" + c.SelectedControl));
//					break;
//				}
//			}
			//c.WebsiteLabel.BecomeFirstResponder ();
			//UIApplication.SharedApplication.OpenUrl(new NSUrl(c.WebsiteLabel.Text));
			tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
		}
	}
}

