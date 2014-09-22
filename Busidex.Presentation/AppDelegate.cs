using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using NewRelic;

namespace Busidex.Presentation.IOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations

		public override UIWindow Window {
			get;
			set;
		}
		//EntryElement UserName, Password;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
//			Window = new UIWindow (UIScreen.MainScreen.Bounds);
//			Window.MakeKeyAndVisible ();
//			Window.RootViewController = new DialogViewController (new RootElement("Login"){
//				new Section("Credentials"){
//					(UserName = new EntryElement("Login", "User Name", "")),
//					(Password = new EntryElement("Password", "", "", true))
//				},
//				new Section(){
//					new StringElement("Login", delegate{
//						var userId = Busidex.Mobile.LoginController.DoLogin(UserName.Value, Password.Value);
//					})
//				}
//			});
			NewRelic.NewRelic.StartWithApplicationToken ("AA3daf678d9a5fa49827982c9ab491f23491afb53b");

			//RegisterOnSharedPreferenceChangeListener
		     
			return true;
		}
			

		//
		// This method is invoked when the application is about to move from active to inactive state.
		//
		// OpenGL applications should use this method to pause.
		//
		public override void OnResignActivation (UIApplication application)
		{
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}

		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

//		public override void OpenUrl(NSUrl url){
//
//			if (UIApplication.SharedApplication.CanOpenUrl (url)) {
//				UIApplication.SharedApplication.OpenUrl ( url);
//			}
//		}
	}
}

