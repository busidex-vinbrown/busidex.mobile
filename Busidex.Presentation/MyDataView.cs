using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.IOS
{
	public class MyDataView : UIView {
		UserCard myData;

		public MyDataView (UserCard myData)
		{
			Update (myData);
		}

		// Public method, that allows the code to externally update
		// what we are rendering.   
		public void Update (UserCard myData)
		{
			this.myData = myData;

			SetNeedsDisplay ();
		}
	}
}

