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
					new FloatMenuOption("BMME_BuildingCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = "BMME_BuildingCategoryLabel".Translate(); }),
					new FloatMenuOption("BMME_TerrainCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = "BMME_TerrainCategoryLabel".Translate(); }),
					new FloatMenuOption("BMME_PlantCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = "BMME_PlantCategoryLabel".Translate(); }),
					new FloatMenuOption("BMME_PawndsCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = "BMME_PawndsCategoryLabel".Translate(); }),
					new FloatMenuOption("BMME_OtherCategoryLabel".Translate(), delegate{ SWD.SelectedCategory = "BMME_OtherCategoryLabel".Translate(); })
				}));
			}

			Rect mainRect = new Rect(inRect){ y = updateButtonRect.yMax };
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (SWD.ObjectsCategories[SWD.SelectedCategory].Count + 1) * Text.LineHeight);
			
			Widgets.BeginScrollView(mainRect, ref ScrollPosition, rect1, true);
			GUI.BeginGroup(rect1);
			float curY = 0; ;
			curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, "BMME_NameLabel".Translate(), "BMME_CellsCountLabel".Translate(), false);
			SWD.ObjectsCategories[SWD.SelectedCategory].Sort();
			foreach (string currentValue in SWD.ObjectsCategories[SWD.SelectedCategory])
			{
				curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, currentValue, SWD.AllLocations[currentValue].Count.ToString(), true);
			}
			GUI.EndGroup();
			Widgets.EndScrollView();
		}

		public override Vector2 InitialSize { get => new Vector2(300f, 354f); }

		public static void DrawWindow() => Find.WindowStack.Add(new SelectWindow());

		float GroupOfThingsMaker(float x, float y, float width, string label, string countOfCells, bool createFindButton)
		{
			Rect rectLabel = new Rect(x, y, width, Text.LineHeight);
			
			if (label == SWD.ThingToRender)
				Widgets.DrawHighlightSelected(rectLabel);
			else if(createFindButton)
			{
				if (Widgets.ButtonInvisible(rectLabel))
				{
					SWD.ThingToRender = label;
					FoundObjects_Overlay.HasUpdated = false;
				}
				Widgets.DrawHighlightIfMouseover(rectLabel);
			}

			Rect rectCount = rectLabel;
			rectCount.width = 100f;
			rectLabel.xMax -= rectCount.width;
			rectCount.x = rectLabel.xMax;

			Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
			TooltipHandler.TipRegion(rectLabel, label.ToString());
			Widgets.Label(rectCount.LeftPartPixels(rectCount.width), countOfCells);
			TooltipHandler.TipRegion(rectCount, countOfCells);
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
			SelectedCategory = "BMME_BuildingCategoryLabel".Translate();
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
				FillData<TerrainDef>(location, location.GetTerrain(CurrentMap).label, "BMME_TerrainCategoryLabel".Translate());
				List <Thing> allThingsOnLocation = location.GetThingList(CurrentMap);
				if (allThingsOnLocation.Count > 0)
				{
					foreach (Thing currentThing in allThingsOnLocation)
					{
						if (FillData<Plant>(location, currentThing.def.label, "BMME_PlantCategoryLabel".Translate(), currentThing))
							continue;
						if (FillData<Pawn>(location, currentThing.def.label, "BMME_PawndsCategoryLabel".Translate(), currentThing))
							continue;

						string label;
						if (currentThing.Stuff != null)
							label = $"{currentThing.def.label} ({currentThing.Stuff.LabelAsStuff})";
						else
							label = currentThing.def.label;

						if (FillData<Building>(location, label, "BMME_BuildingCategoryLabel".Translate(), currentThing))
							continue;
						FillData<Thing>(location, currentThing.def.label, "BMME_OtherCategoryLabel".Translate(), currentThing);
					}
				}
			}
			FoundObjects_Overlay.HasUpdated = false;
		}


		private bool FillData<T>(IntVec3 location, string label, string categoryName, Thing currentThing = null)
		{
			if (currentThing is T || currentThing == null)
			{
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
