using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using BetterMiniMap.Overlays;
using RimWorld;

namespace BetterMiniMap
{
	public class SelectWindow : Window
	{
		Vector2 ScrollPosition;
		SelectWindowData SWD;

		public SelectWindow() : base()
		{
			this.doCloseX = true;
			this.preventDrawTutor = true;
			this.draggable = true;
			this.preventCameraMotion = false;

			this.ScrollPosition = new Vector2();
			this.SWD = FoundObjects_Overlay.SWD;
		}

		public override void PreOpen()
		{
			base.PreOpen();
			if (SWD.AllLocations.Count == 0 || SWD.CurrentMap != Find.VisibleMap)
				SWD.FindAllThings();
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Rect titleRect = new Rect(inRect){ height = Text.LineHeight + 7f };
			Rect updateButtonRect = new Rect(titleRect.x, titleRect.yMax, titleRect.width / 3f, 25f);
			Rect clearButtonRect = updateButtonRect;
			clearButtonRect.x = updateButtonRect.xMax;
			Rect categoryButtonRect = clearButtonRect;
			categoryButtonRect.x = clearButtonRect.xMax;
			
			Widgets.Label(titleRect, "BMME_ObjectsSeekerLabel".Translate());
			Text.Font = GameFont.Small;

			if (Widgets.ButtonText(updateButtonRect, "BMME_UpdateButtonLabel".Translate()))
			{
				SWD.FindAllThings();
			}
			if (Widgets.ButtonText(clearButtonRect, "BMME_ClearButtonLabel".Translate()))
			{
				SWD.ThingToRender = string.Empty;
				FoundObjects_Overlay.HasUpdated = false;
			}
			if (Widgets.ButtonText(categoryButtonRect, SWD.SelectedCategory))
			{
				Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
				{
					new FloatMenuOption("BMME_BuildingCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = typeof(Building).Name; }),
					new FloatMenuOption("BMME_PlantCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = typeof(Plant).Name; }),
					new FloatMenuOption("BMME_PawndsCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = typeof(Pawn).Name; }),
					new FloatMenuOption("BMME_OtherCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = typeof(Thing).Name; })
				}));
			}

			Rect mainRect = new Rect(inRect){ yMin = updateButtonRect.yMax };
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (SWD.ObjectsCategories[SWD.SelectedCategory].Count + 1) * Text.LineHeight);
			
			Widgets.BeginScrollView(mainRect, ref ScrollPosition, rect1, true);
			GUI.BeginGroup(rect1);
			float curY = 0; ;
			curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, "BMME_NameLabel".Translate(), "BMME_CellsCountLabel".Translate(), false);
			SWD.ObjectsCategories[SWD.SelectedCategory].Sort();
			foreach (string currentValue in SWD.ObjectsCategories[SWD.SelectedCategory])
			{
				curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, currentValue, SWD.AllLocations[currentValue].Count.ToString());
			}
			GUI.EndGroup();
			Widgets.EndScrollView();
		}

		public override Vector2 InitialSize { get => new Vector2(300f, 354f); }

		public static void DrawWindow() => Find.WindowStack.Add(new SelectWindow());

		float GroupOfThingsMaker(float x, float y, float width, string label, string countOfCells, bool createFindButton = true)
		{
			float findButtonWidth = Text.CalcSize("BMME_FindButtonLabel".Translate()).x + 8f;

			Rect rectLabel = new Rect(x, y, width - findButtonWidth, Text.LineHeight);
			Widgets.DrawHighlightIfMouseover(rectLabel);
			Rect rectCount = rectLabel;
			rectCount.width = 80f;
			rectLabel.xMax -= rectCount.width;
			rectCount.x = rectLabel.xMax;

			Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
			TooltipHandler.TipRegion(rectLabel, label.ToString());
			Widgets.Label(rectCount.LeftPartPixels(rectCount.width), countOfCells);
			TooltipHandler.TipRegion(rectCount, countOfCells);

			if (createFindButton)
			{
				Rect findButtonRect = rectCount;
				findButtonRect.x = rectCount.xMax;
				findButtonRect.width = findButtonWidth;
				if (Widgets.ButtonText(findButtonRect, "BMME_FindButtonLabel".Translate()))
				{
					SWD.ThingToRender = label;
					FoundObjects_Overlay.HasUpdated = false;
				}
			}
			return rectLabel.height;
		}
	}



	public class SelectWindowData
	{
		public SelectWindowData()
		{
			AllLocations = new Dictionary<string, List<IntVec3>>{ { String.Empty, new List<IntVec3>() } };
			ObjectsCategories = new Dictionary<string, List<string>>();
			ThingToRender = string.Empty;
			SelectedCategory = typeof(Building).Name;
			CurrentMap = Find.VisibleMap;
		}

		public List<IntVec3> Positions
		{
			get
			{
				if (CurrentMap == Find.VisibleMap && AllLocations.ContainsKey(ThingToRender))
					return AllLocations[ThingToRender];
				else
					return null;
			}
		}
		public Dictionary<string, List<IntVec3>> AllLocations { get; set; }
		public Dictionary<string, List<string>> ObjectsCategories { get; set; }
		public string ThingToRender { get; set; }
		public string SelectedCategory { get; set; }
		public Map CurrentMap { get; set; }

		public void FindAllThings()
		{
			CurrentMap = Find.VisibleMap;
			IEnumerable<IntVec3> cellsLocations = CurrentMap.AllCells;
			
			AllLocations.Clear();
			ObjectsCategories.Clear();
			AllLocations.Add(String.Empty, new List<IntVec3>());

			foreach (IntVec3 location in cellsLocations)
			{
				if (CurrentMap.fogGrid.IsFogged(location))
					continue;
				FillData<TerrainDef>(location, location.GetTerrain(CurrentMap).label);
				List<Thing> allThingsOnLocation = location.GetThingList(CurrentMap);
				if (allThingsOnLocation.Count > 0)
				{
					foreach (Thing currentThing in allThingsOnLocation)
					{
						if (FillData<Plant>(location, currentThing.def.label, currentThing))
							continue;
						if (FillData<Pawn>(location, currentThing.def.label, currentThing))
							continue;

						string label;
						if (currentThing.Stuff != null)
							label = $"{currentThing.def.label} ({currentThing.Stuff.LabelAsStuff})";
						else
							label = currentThing.def.label;

						if (FillData<Building>(location, label, currentThing))
							continue;
						FillData<Thing>(location, currentThing.def.label, currentThing);
					}
				}
			}
			FoundObjects_Overlay.HasUpdated = false;
		}


		private bool FillData<T>(IntVec3 location, string label, Thing currentThing = null)
		{
			if (currentThing is T || currentThing == null)
			{
				string categoryName = typeof(T).Name;
				if (ObjectsCategories.ContainsKey(categoryName))
				{
					if (!ObjectsCategories[categoryName].Contains(label))
						ObjectsCategories[categoryName].Add(label);
				}
				else
					ObjectsCategories.Add(categoryName, new List<string>(new string[] { label }));

				if (AllLocations.ContainsKey(label))
					AllLocations[label].Add(location);
				else
					AllLocations.Add(label, new List<IntVec3>(new IntVec3[] { location }));
				return true;
			}
			else
				return false;
		}
	}
}
