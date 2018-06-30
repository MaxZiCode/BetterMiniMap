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
		private static List<IntVec3> s_positions;
		static bool HasUpdated = false;

		public FoundObjects_Overlay(bool visible = true) : base(visible) { }

		public static List<IntVec3> Positions { get => s_positions; set { HasUpdated = false; s_positions = value; } }
		public static Dictionary<string, List<IntVec3>> AllLocations { get; set; }
		public static Dictionary<string, List<string>> ObjectsCategories { get; set; }

		public override int GetUpdateInterval() => 10;

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

		public override bool GetShouldUpdateOverlay()
		{
			if(!HasUpdated)
			{
				HasUpdated = true;
				return true;
			}
			else
				return false;
		}

		public static void FindAllThings()
		{
			Map theMap = Find.VisibleMap;
			IEnumerable<IntVec3> cellsLocations = theMap.AllCells;
			IEnumerable<Pawn> AllPawnsOnTheMap = theMap.mapPawns.AllPawns;
			List<string> objectsLables = new List<string>();

			AllLocations.Clear();
			ObjectsCategories.Clear();

			foreach (Pawn currentPawn in AllPawnsOnTheMap)
			{
				if (theMap.fogGrid.IsFogged(currentPawn.Position))
					continue;
				objectsLables.Add(currentPawn.def.label);
				if (AllLocations.ContainsKey(currentPawn.def.label))
					AllLocations[currentPawn.def.label].Add(currentPawn.Position);
				else
					AllLocations.Add(currentPawn.def.label, new List<IntVec3>(new IntVec3[] { currentPawn.Position }));
			}
			foreach (IntVec3 Location in cellsLocations)
			{
				if (theMap.fogGrid.IsFogged(Location))
					continue;
				List<Thing> allThingsOnLocation = Location.GetThingList(theMap);
				TerrainDef currentTerrain = Location.GetTerrain(theMap);
				objectsLables.Add(currentTerrain.label);
				if (AllLocations.ContainsKey(currentTerrain.label))
					AllLocations[currentTerrain.label].Add(Location);
				else
					AllLocations.Add(currentTerrain.label, new List<IntVec3>(new IntVec3[] { Location }));
				if (allThingsOnLocation.Count != 0)
				{
					foreach (Thing currentThing in allThingsOnLocation)
					{
						objectsLables.Add(currentThing.def.label);
						if (AllLocations.ContainsKey(currentThing.def.label))
							AllLocations[currentThing.def.label].Add(Location);
						else
							AllLocations.Add(currentThing.def.label, new List<IntVec3>(new IntVec3[] { Location }));
					}
				}
			}
			objectsLables.Sort();
			ObjectsCategories.Add("Default", new List<string>());
			foreach (string currentLabel in objectsLables)
			{
				if (!ObjectsCategories["Default"].Contains(currentLabel))
				{
					ObjectsCategories["Default"].Add(currentLabel);
				}
			}
		}
	}
}
