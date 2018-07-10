using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
		private static Color32[] clearPixelArray;

		private static int size = -1; // NOTE: size is instead of mapID. Because mapID doesn't work correctly

		public static Color32[] GetClearPixelArray
		{
			get
			{
				Map map = Find.VisibleMap;
				if (Utilities.clearPixelArray == null || size != map.Size.x * map.Size.z) 
				{
					size = map.Size.x * map.Size.z;
					Utilities.clearPixelArray = new Color32[size];
					for (int i = 0; i < Utilities.clearPixelArray.Count<Color32>(); i++)
						Utilities.clearPixelArray[i] = Color.clear;
				}
				return Utilities.clearPixelArray;
			}
		}

	}
}
