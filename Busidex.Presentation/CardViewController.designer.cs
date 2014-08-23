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
	[Register ("CardViewController")]
	partial class CardViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnCard { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnCard != null) {
				btnCard.Dispose ();
				btnCard = null;
			}
		}
	}
}
