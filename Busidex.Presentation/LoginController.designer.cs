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
	[Register ("LoginController")]
	partial class LoginController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnLogin { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblLoginResult { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtUserName { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}
			if (lblLoginResult != null) {
				lblLoginResult.Dispose ();
				lblLoginResult = null;
			}
			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}
			if (txtUserName != null) {
				txtUserName.Dispose ();
				txtUserName = null;
			}
		}
	}
}
