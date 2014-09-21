using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile.Models;
using System.Linq;
using System.Drawing;
using System.IO;
using Busidex.Mobile;

namespace Busidex.Presentation.IOS
{
	partial class PhoneViewController : UIViewController
	{
		public UserCard UserCard{ get; set; }
		private string documentsPath;
		private string userToken;

		public PhoneViewController (IntPtr handle) : base (handle)
		{

		}

		private void LoadCard(){
			if (this.NavigationController != null) {
				this.NavigationController.SetNavigationBarHidden (false, true);
			}

			if (UserCard != null && UserCard.Card != null) {
				var FrontFileName = System.IO.Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
				if (File.Exists (FrontFileName)) {

					imgCard.Image = UIImage.FromFile (FrontFileName);

				}
			}

			var labelX = 25f;
			var labelY = 280f;
			var labelHeight = 30f;
			var labelWidth = 100f;

			var labelFrame = new RectangleF (labelX, labelY, labelWidth, labelHeight);
			var phoneFrame = new RectangleF (labelX + labelWidth + 10, labelY, labelWidth * 2, labelHeight);

			if (UserCard != null && UserCard.Card != null && UserCard.Card.PhoneNumbers != null) {

				foreach (PhoneNumber number in UserCard.Card.PhoneNumbers.Where(p=> !string.IsNullOrWhiteSpace(p.Number))) {

					var newLabel = new UILabel (labelFrame);
					var newNumber = UIButton.FromType (UIButtonType.System); //new UITextView (phoneFrame);
					if (number.PhoneNumberType != null) {
						newLabel.Text = Enum.GetName (typeof(PhoneNumberTypes), number.PhoneNumberType.PhoneNumberTypeId);

						newLabel.Font = UIFont.FromName ("Helvetica", 20f);
						newLabel.UserInteractionEnabled = true;
						newLabel.TextColor = UIColor.FromRGB(66,69,76);

						var textAttributed = new NSMutableAttributedString (
							                     number.Number, 
							                     new UIStringAttributes () {
								ForegroundColor = UIColor.Blue, 
								Font = UIFont.FromName ("Helvetica", 22f),
								UnderlineStyle = NSUnderlineStyle.Single 
							}
						);
						newNumber.SetAttributedTitle(textAttributed, UIControlState.Normal);
						newNumber.UserInteractionEnabled = true;
						newNumber.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
						newNumber.UserInteractionEnabled = true;
						newNumber.Frame = phoneFrame;
						newNumber.TouchUpInside += delegate {
							UIApplication.SharedApplication.OpenUrl (new NSUrl ("tel:" + number.Number));
							NewRelic.NewRelic.RecordMetricWithName (UIMetrics.WEBSITE_VISIT, UIMetrics.METRICS_CATEGORY, new NSNumber (1));
							ActivityController.SaveActivity ((long)EventSources.Call, UserCard.Card.CardId, userToken);
						};
						labelFrame.Y = phoneFrame.Y += 35;

						View.Add (newLabel);
						View.Add (newNumber);
					}
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			try{
				documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				LoadCard ();

				NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

				if (cookie != null) {
					userToken = cookie.Value;
				}

			}catch(Exception ex){

			}
		}
	}
}
