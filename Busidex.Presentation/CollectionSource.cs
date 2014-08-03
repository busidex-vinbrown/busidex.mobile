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
using System.IO;
using System.Drawing;

namespace Busidex.Presentation.IOS
{
	public class CollectionSource : UICollectionViewSource
	{

		List<UserCard> Cards;
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		public CollectionSource (List<UserCard> items)
		{
			Cards = items ?? new List<UserCard>();

		}

		public override int GetItemsCount (UICollectionView tableview, int section)
		{
			return Cards.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (SearchController.cellID, indexPath) as SearchViewCell;

			var card = Cards [indexPath.Row];

			UIImageView	CardImage = null;

			CardImage = cell.ContentView.Subviews.FirstOrDefault () as UIImageView ?? new UIImageView ();
			bool needsCardImage = CardImage.Tag <= 0;
			CardImage.Tag = 1;

			var fileName = System.IO.Path.Combine (documentsPath, card.Card.FrontFileId + "." + card.Card.FrontType);
			if (File.Exists (fileName)) {

				//CardImage.Image = UIImage.FromFile (fileName); 
				cell.Image = UIImage.FromFile (fileName);

			} else {
				var imagePath = Busidex.Mobile.Utils.CARD_PATH + card.Card.FrontFileId + "." + card.Card.FrontType;
				var fName = card.Card.FrontFileId + "." + card.Card.FrontType;

				Busidex.Mobile.Utils.DownloadImage (imagePath, documentsPath, fName).ContinueWith (t => {
				
					fileName = t.Result;
					string jpgFilename = System.IO.Path.Combine (documentsPath, fileName);

					using (var f = File.Open (t.Result, FileMode.Open)) {
						//var newFile = File.Create(jpgFilename);
						//newFile.Close();

						using (var data = NSData.FromFile (jpgFilename)) {
							CardImage.Image = UIImage.LoadFromData (data); 
						}
					}
				});
			}

//			if (needsCardImage) {
//				CardImage.Frame = new RectangleF (10, 10f, 120f, 80f);
//				cell.ContentView.AddSubview (CardImage);
//			}
				
			return cell;
		}
	}
}

