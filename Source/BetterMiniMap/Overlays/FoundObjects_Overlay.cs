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
		public static bool HasUpdated = false;
		static SelectWindowData _sWD = new SelectWindowData();

		public FoundObjects_Overlay(bool visible = true) : base(visible) { }

		public static SelectWindowData SWD { get => _sWD;}

		public override int GetUpdateInterval() => 10;

		public override void Render()
		{
			if ( SWD.Positions != null && SWD.Positions.Count != 0)
			{
				foreach (IntVec3 pos in SWD.Positions)
				{
					base.CreateMarker(pos, 5, Color.black, Color.magenta, 0.3f);
				}
			}
		}

		public override bool GetShouldUpdateOverlay()
		{
			if (!HasUpdated)
			{
				HasUpdated = true;
				return true;
			}
			else
				return false;
		}
	}
}
