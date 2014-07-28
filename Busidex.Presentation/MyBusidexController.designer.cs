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
	[Register ("MyBusidexController")]
	partial class MyBusidexController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView MyBusidexCtrl { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (MyBusidexCtrl != null) {
				MyBusidexCtrl.Dispose ();
				MyBusidexCtrl = null;
			}
		}
	}
}
