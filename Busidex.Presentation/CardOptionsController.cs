using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Busidex.Presentation.IOS
{
	partial class CardOptionsController : BaseController
	{
		public CardOptionsController (IntPtr handle) : base (handle)
		{
		}
			
		public CardOptionsController ()
		{
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.Clear;

			btnDismiss.TouchUpInside += delegate {
				this.DismissViewController(true, null);
			};

			btnAdd.TouchUpInside += delegate {
				this.DismissViewController(true, null);
			};
		}
	}
}
