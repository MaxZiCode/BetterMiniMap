using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BetterMiniMap
{
	public static class Utilities
	{
		private static Color32[] clearPixelArray;

		public static readonly DesignationDef FoundDesignationDef = DefDatabase<DesignationDef>.GetNamed("Found", true);

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

		public static void RemoveDesignations()
		{
			DesignationManager designManager = Find.VisibleMap.designationManager;
			List<Designation> gameDesignations = designManager.allDesignations;
			List<Designation> totalDesignations = gameDesignations.ListFullCopy();
			for (int i = 0; i < totalDesignations.ListFullCopy().Count; i++)
			{
				Designation des = totalDesignations[i];
				if (des.def == FoundDesignationDef)
				{
					gameDesignations.Remove(des);
				}
			}
		}
	}
}
