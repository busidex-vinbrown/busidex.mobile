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
	[Register ("CardOptionsController")]
	partial class CardOptionsController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnAdd { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnDismiss { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnAdd != null) {
				btnAdd.Dispose ();
				btnAdd = null;
			}
			if (btnDismiss != null) {
				btnDismiss.Dispose ();
				btnDismiss = null;
			}
		}
	}
}
