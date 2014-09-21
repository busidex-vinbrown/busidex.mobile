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
		MonoTouch.UIKit.UILabel dataLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView spnWait { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtUserName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGoToMyBusidex { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGoToSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnMyOrganizations { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblLoading { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIActivityIndicatorView spnLoading { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnGoToMyBusidex != null) {
				btnGoToMyBusidex.Dispose ();
				btnGoToMyBusidex = null;
			}
			if (btnGoToSearch != null) {
				btnGoToSearch.Dispose ();
				btnGoToSearch = null;
			}
			if (btnMyOrganizations != null) {
				btnMyOrganizations.Dispose ();
				btnMyOrganizations = null;
			}
			if (lblLoading != null) {
				lblLoading.Dispose ();
				lblLoading = null;
			}
			if (spnLoading != null) {
				spnLoading.Dispose ();
				spnLoading = null;
			}
		}
	}
}
