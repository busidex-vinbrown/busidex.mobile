using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using Busidex.Mobile;
using Busidex.Mobile.Models;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Busidex.Presentation.IOS
{
	partial class MyBusidexController : UITableViewController
	{
		public static NSString BusidexCellId = new NSString ("BusidexCell");

		public MyBusidexController (IntPtr handle) : base (handle)
		{
			TableView.RegisterClassForCellReuse (typeof(UITableViewCell), BusidexCellId);

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NSHttpCookie cookie = NSHttpCookieStorage.SharedStorage.Cookies.Where(c=>c.Name == "UserId").SingleOrDefault();

			if (cookie != null) {

				var ctrl = new Busidex.Mobile.MyBusidexController ();
				var response = ctrl.GetMyBusidex (cookie.Value);

				if(!string.IsNullOrEmpty(response)){
					MyBusidexResponse MyBusidexResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<MyBusidexResponse> (response);

					ObservableCollection<UserCard> cards = new ObservableCollection<UserCard> ();

					foreach (var item in MyBusidexResponse.MyBusidex.Busidex) {
						if (item.Card != null) {

							//var card = 
							//if (image != null) {
								cards.Add (item);
							//}
						}
					}

					this.TableView.Source = new TableSource(cards);
					this.TableView.AllowsSelection = true;

				}
			}

		}
	}
}
