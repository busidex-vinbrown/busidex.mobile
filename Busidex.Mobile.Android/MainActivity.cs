using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Busidex.Mobile.Android
{
	[Activity (Label = "Busidex.Mobile.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		const string TEST_ACCOUNT_ID = "45ef6a4e-3c9d-452c-9409-9e4d7b6e532f";
		const bool DEVELOPMENT_MODE = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var btnConnect = FindViewById<Button> (Resource.Id.btnConnect);
			var btnStart = FindViewById<Button> (Resource.Id.btnStart);

			btnConnect.Click += delegate {
				var intent = new Intent(this, typeof(LoginActivity));
				StartActivity(intent);

			};

			btnStart.Click += delegate {

			};
		}

		private void GoToMain(){
		}

		private void GoToStart(){
		}

	}
}