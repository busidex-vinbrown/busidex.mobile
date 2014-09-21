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
	[Register ("OrganizationsController")]
	partial class OrganizationsController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView vwOrganizations { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (vwOrganizations != null) {
				vwOrganizations.Dispose ();
				vwOrganizations = null;
			}
		}
	}
}
