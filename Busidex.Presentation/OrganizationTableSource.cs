using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using Busidex.Mobile.Models;
using MonoTouch.Foundation;
using System.Linq;
using System.Drawing;
using System.IO;

namespace Busidex.Presentation.IOS
{
	public class OrganizationTableSource : UITableViewSource
	{

		public event ViewWebsiteHandler ViewWebsite;

		private List<UITableViewCell> cellCache;
		private List<Organization> Organizations;
		private enum UIElements{
			OrganizationImage = 1,
			NameLabel = 2,
			WebsiteButton = 3,
			TwitterButton = 4,
			FacebookButton = 5
		}
		private const float LEFT_MARGIN = 5F;
		private const float LABEL_HEIGHT = 30f;
		private const float LABEL_WIDTH = 170f;
		private const float FEATURE_BUTTON_HEIGHT = 40f;
		private const float FEATURE_BUTTON_WIDTH = 40f;
		private const float FEATURE_BUTTON_MARGIN = 15f;

		public OrganizationTableSource (List<Organization> organizations)
		{
			this.Organizations = organizations;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return Organizations.Count;
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			float baseHeight = 180f;
			return Organizations.Count() == 0 ? baseHeight * 3 : baseHeight;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var organization = (Organization)Organizations [indexPath.Row];

			var cell = tableView.DequeueReusableCell (OrganizationsController.BusidexCellId, indexPath);
			cellCache = new List<UITableViewCell> ();
			cell.SelectionStyle = UITableViewCellSelectionStyle.Default;

			cell.Tag = indexPath.Row;
			if (cellCache.All (c => c.Tag != indexPath.Row)) {
				cellCache.Add (cell);
			} 

			// add controls here
			AddControls (cell, organization);

			cell.SetNeedsLayout ();
			cell.SetNeedsDisplay ();

			return cell;
		}

		private void AddControls(UITableViewCell cell, Organization org){

			//string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			List<UIButton> FeatureButtons = new List<UIButton> ();

//			var fileName = System.IO.Path.Combine (documentsPath, org.LogoFileName);
//			var OrgImage = cell.ContentView.Subviews.Where (s => s is UIImageView && s.Tag == (int)UIElements.OrganizationImage).SingleOrDefault () as UIImageView;
//			if (OrgImage != null) {
//				OrgImage.RemoveFromSuperview ();
//			}
			var frame = new RectangleF (10f, 10f, 300f, 50f);

//			OrgImage = new UIImageView(frame);
//			var imageFile = fileName + "." + org.LogoType;
//			if (File.Exists (imageFile)) {
//				var data = NSData.FromFile (imageFile);
//				if (data != null) {
//					OrgImage.Image = new UIImage (data);
//					cell.ContentView.AddSubview (OrgImage);
//				}
//			}

			var NameLabel = cell.ContentView.Subviews.Where(s=> s.Tag == (int)UIElements.NameLabel).SingleOrDefault() as UILabel;
			if (NameLabel != null) {
				NameLabel.RemoveFromSuperview ();
			}

			NameLabel = new UILabel (frame);
			NameLabel.Tag = (int)UIElements.NameLabel;
			NameLabel.Text = org.Name;
			NameLabel.Font = UIFont.FromName ("Helvetica-Bold", 18f);

			cell.ContentView.AddSubview (NameLabel);

			frame.Y += LABEL_HEIGHT + 8f;


			if (!string.IsNullOrEmpty (org.Url)) {
				var WebsiteButton = UIButton.FromType (UIButtonType.System);

				WebsiteButton.SetBackgroundImage (UIImage.FromBundle ("browser.png"), UIControlState.Normal);
				WebsiteButton.Tag = (int)UIElements.WebsiteButton;

				WebsiteButton.TouchUpInside += delegate {
					ShowBrowser(org.Url);
				};

				FeatureButtons.Add (WebsiteButton);
			}

			if (!string.IsNullOrEmpty (org.Twitter)) {
				var TwitterButton = UIButton.FromType (UIButtonType.System);

				TwitterButton.SetBackgroundImage (UIImage.FromBundle ("twitter.png"), UIControlState.Normal);
				TwitterButton.Tag = (int)UIElements.TwitterButton;

				TwitterButton.TouchUpInside += delegate {
					ShowBrowser(org.Twitter);
				};

				FeatureButtons.Add (TwitterButton);
			}

			if (!string.IsNullOrEmpty (org.Facebook)) {
				var FacebookButton = UIButton.FromType (UIButtonType.System);

				FacebookButton.SetBackgroundImage (UIImage.FromBundle ("fb.png"), UIControlState.Normal);
				FacebookButton.Tag = (int)UIElements.FacebookButton;

				FacebookButton.TouchUpInside += delegate {
					ShowBrowser(org.Facebook);
				};

				FeatureButtons.Add (FacebookButton);
			}

			AddFeatureButtons (cell, FeatureButtons);
		}

		private void AddFeatureButtons(UITableViewCell cell, List<UIButton> FeatureButtons){
		
			float buttonY = 40f + LABEL_HEIGHT;
			float buttonX =  LEFT_MARGIN;

			var cellButtons = cell.ContentView.Subviews.Where (s => s is UIButton).ToList ();
			foreach(var button in cellButtons){
				button.RemoveFromSuperview ();
			}

			var frame = new RectangleF (buttonX, buttonY, FEATURE_BUTTON_WIDTH, FEATURE_BUTTON_HEIGHT);
			float buttonXOriginal = buttonX;
			int idx = 0;
			foreach(var button in FeatureButtons.OrderBy(b=>b.Tag)){

				button.Frame = frame;
				cell.ContentView.AddSubview (button);

				idx++;
				if (idx % 3 == 0) { 
					buttonX = buttonXOriginal;
					frame.Y += FEATURE_BUTTON_HEIGHT + 10f;
				} else {
					buttonX += FEATURE_BUTTON_WIDTH + FEATURE_BUTTON_MARGIN;
				}
				frame.X = buttonX;
			}
		}

		private void ShowBrowser(string url){

			if (this.ViewWebsite != null){
				this.ViewWebsite (url);
			}
		}
	}
}

