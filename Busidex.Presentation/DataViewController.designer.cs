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
	[Register ("DataViewController")]
	partial class DataViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnLogin { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel dataLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spnWait { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtUserName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView vwLogin { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblLoginResult { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (lblLoginResult != null) {
				lblLoginResult.Dispose ();
				lblLoginResult = null;
			}
		}
	}
}
