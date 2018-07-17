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
		static readonly SelectWindowData _sWD = new SelectWindowData();
		readonly DesignationDef posDef = DefDatabase<DesignationDef>.GetNamed("Found", true);
		List<Designation> _previousDes = new List<Designation>();
		Map _map;

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

					Designation des = new Designation(pos, posDef);
					_previousDes.Add(des);
					_map = Find.VisibleMap;
					_map.designationManager.AddDesignation(des);
				}
			}
		}

		public override bool GetShouldUpdateOverlay()
		{
			if (!HasUpdated)
			{
				if (_previousDes.Count != 0 && _map != null)
					foreach (var curDes in _previousDes)
						_map.designationManager.RemoveDesignation(curDes);
				_previousDes.Clear();

				HasUpdated = true;
				return true;
			}
			else
				return false;
		}
	}
}
