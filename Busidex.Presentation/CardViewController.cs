using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.IO;
using Busidex.Mobile.Models;

namespace Busidex.Presentation
{
	partial class CardViewController : UIViewController
	{
		private readonly string documentsPath;
		public UserCard UserCard{ get; set; }
		private string FileName{ get; set; }

		public CardViewController (IntPtr handle) : base (handle)
		{
			documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		}

		public void LoadCard(){

			if (UserCard != null && UserCard.Card != null) {
				FileName = System.IO.Path.Combine (documentsPath, UserCard.Card.FrontFileId + "." + UserCard.Card.FrontType);
				if (File.Exists (FileName)) {

					imgCard.Image = UIImage.FromFile (FileName);

				}
			}
		}

		public override void DidRotate(UIInterfaceOrientation orientation){


			base.DidRotate (orientation);

		}

		public override void ViewWillAppear (bool animated)
		{

			base.ViewWillAppear (animated);

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
			LoadCard ();
		}
	}
}
