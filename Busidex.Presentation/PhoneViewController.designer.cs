// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Busidex.Presentation.IOS
{
	[Register ("PhoneViewController")]
	partial class PhoneViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgCard { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView PhoneView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (imgCard != null) {
				imgCard.Dispose ();
				imgCard = null;
			}
			if (PhoneView != null) {
				PhoneView.Dispose ();
				PhoneView = null;
			}
		}
	}
}
