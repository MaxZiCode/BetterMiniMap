using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap
{
	public class SelectWindow : Window
	{
		Dictionary<string, int> AllobjectsLablesOnTheMap;
		Dictionary<string, List<IntVec3>> AllLocations;
		Vector2 scrollPosition;

		public SelectWindow() : base()
		{
			this.doCloseX = true;
			this.preventDrawTutor = true;
			this.draggable = true;

			this.AllobjectsLablesOnTheMap = new Dictionary<string, int>();
			this.AllLocations = new Dictionary<string, List<IntVec3>>();
			this.scrollPosition = new Vector2();
		}

		public override void PreOpen()
		{
			base.PreOpen();
			FindAllThings();
		}

		public override void DoWindowContents(Rect inRect)
		{
			if (AllobjectsLablesOnTheMap.Count == 0)
				return;
			Text.Font = GameFont.Small;
			float countColumnWidth = 80f;
			Rect mainRect = new Rect(inRect);
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (AllobjectsLablesOnTheMap.Count + 1) * Text.LineHeight);
			Widgets.BeginScrollView(mainRect, ref scrollPosition, rect1, true);
			Listing_Standard l_s = new Listing_Standard();

			l_s.Begin(rect1);
			ListOfThingsMaker(l_s, countColumnWidth, "Label", "Cells count", false);
			foreach (KeyValuePair<string, int> current in AllobjectsLablesOnTheMap)
			{
				ListOfThingsMaker(l_s, countColumnWidth, current.Key, current.Value.ToString());
			}
			l_s.End();
			Widgets.EndScrollView();
		}

		public override Vector2 InitialSize { get => new Vector2(300f, 320f); }

		public static void DrawWindow() => Find.WindowStack.Add(new SelectWindow());

		void FindAllThings()
		{
			Map theMap = Find.VisibleMap;
			IEnumerable<IntVec3> cellsLocations = theMap.AllCells;
			IEnumerable<Pawn> AllPawnsOnTheMap = theMap.mapPawns.AllPawns;
			List<string> objectsLables = new List<string>();

			this.AllobjectsLablesOnTheMap.Clear();

			foreach (Pawn currentPawn in AllPawnsOnTheMap)
			{
				if (theMap.fogGrid.IsFogged(currentPawn.Position))
					continue;
				objectsLables.Add(currentPawn.def.label);
				if (this.AllLocations.ContainsKey(currentPawn.def.label))
					this.AllLocations[currentPawn.def.label].Add(currentPawn.Position);
				else
					this.AllLocations.Add(currentPawn.def.label, new List<IntVec3>(new IntVec3[] { currentPawn.Position }));
			}
			foreach (IntVec3 Location in cellsLocations)
			{
				if (theMap.fogGrid.IsFogged(Location))
					continue;
				List<Thing> allThingsOnLocation = Location.GetThingList(theMap);
				TerrainDef currentTerrain = Location.GetTerrain(theMap);
				objectsLables.Add(currentTerrain.label);
				if (this.AllLocations.ContainsKey(currentTerrain.label))
					this.AllLocations[currentTerrain.label].Add(Location);
				else
					AllLocations.Add(currentTerrain.label, new List<IntVec3>(new IntVec3[] { Location }));
				if (allThingsOnLocation.Count != 0)
				{
					foreach (Thing currentThing in allThingsOnLocation)
					{
						objectsLables.Add(currentThing.def.label);
						if (this.AllLocations.ContainsKey(currentThing.def.label))
							this.AllLocations[currentThing.def.label].Add(Location);
						else
							AllLocations.Add(currentThing.def.label, new List<IntVec3>(new IntVec3[] { Location }));
					}
				}
			}
			objectsLables.Sort();
			foreach (string currentLabel in objectsLables)
			{
				if (AllobjectsLablesOnTheMap.ContainsKey(currentLabel))
					AllobjectsLablesOnTheMap[currentLabel]++;
				else
					AllobjectsLablesOnTheMap.Add(currentLabel, 1);
			}
		}

		void ListOfThingsMaker(Listing_Standard l_s, float countColumnWidth, string label, string countOfCells, bool createFindButton = true)
		{
			float FindButtonWidth = Text.CalcSize("Find").x + 4f;
			Rect rectLabel = l_s.GetRect(Text.LineHeight);
			Widgets.DrawHighlightIfMouseover(rectLabel);
			rectLabel.width -= countColumnWidth + FindButtonWidth;
			Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
			TooltipHandler.TipRegion(rectLabel, label.ToString()); 

			Rect rectCount = new Rect(rectLabel) { x = rectLabel.width, width = countColumnWidth };
			Widgets.Label(rectCount.LeftPartPixels(rectCount.width), countOfCells);
			TooltipHandler.TipRegion(rectCount, countOfCells);

			if (createFindButton)
			{
				Rect findButtonRect = new Rect(rectCount.x + rectCount.width, rectLabel.y, FindButtonWidth, rectLabel.height);
				if (Widgets.ButtonText(findButtonRect, "Find"))
					Overlays.FoundObjects_Overlay.Positions = AllLocations[label]; 
			}
		}
	}
}
