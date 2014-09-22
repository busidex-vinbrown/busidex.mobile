using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Busidex.Mobile.Models;

namespace Busidex.Presentation.IOS
{
	public class BaseController : UIViewController
	{
		public BaseController (IntPtr handle) : base (handle)
		{
		}

		public BaseController ()
		{
		}
	}
}

