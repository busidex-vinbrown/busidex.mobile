using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Busidex.Mobile
{
	public static class Utils
	{


		public static long DecodeUserId(string id){

			long userId = 0;

			try{
				byte[] raw = Convert.FromBase64String(id); 
				string s = System.Text.Encoding.UTF8.GetString(raw);
				long.TryParse(s, out userId);

			}catch(Exception ex){

			}

			return userId;
		}



		public const string CARD_PATH = "https://az381524.vo.msecnd.net/cards/";
	}
}

