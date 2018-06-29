using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;


namespace BetterMiniMap.Overlays
{
	public class FoundObjects_Overlay : MarkerOverlay
	{
		public FoundObjects_Overlay(bool visible = true) : base(visible) { }

		public static List<IntVec3> Positions { get; set; }

		public override int GetUpdateInterval() => 100;

		public override void Render()
		{
			if (Positions != null)
			{
				foreach (IntVec3 pos in Positions)
				{
					base.CreateMarker(pos, 4, Color.black, Color.magenta, 0.05f);
				} 
			}
		}
	}
}
