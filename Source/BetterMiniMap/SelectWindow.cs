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
			Rect updateButtonRect = new Rect(titleRect.x, titleRect.yMax, titleRect.width / 2f - 2f, 20f);
			Rect clearButtonRect = updateButtonRect;
			clearButtonRect.x = updateButtonRect.xMax + 4f;
			
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

			float curY = updateButtonRect.yMax;
			string partOfCategoryLabel = "Category:";
			Widgets.ListSeparator(ref curY, inRect.width, partOfCategoryLabel + " " + SWD.SelectedCategoryString);
			Rect categoryButtonRect = new Rect(inRect.x, updateButtonRect.yMax, inRect.width, curY - updateButtonRect.yMax);
			if (Widgets.ButtonInvisible(categoryButtonRect))
				Find.WindowStack.Add(SWD.CategoryMenu);
			Widgets.DrawHighlightIfMouseover(categoryButtonRect);

			if (!SWD.ObjectsCategories.ContainsKey(SWD.SelectedCategory))
			{
				Widgets.Label(new Rect(titleRect) { y = curY },  SWD.SelectedCategoryString + " not found");
				return;
			}
			Rect mainRect = new Rect(inRect){ yMin = curY };
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (SWD.ObjectsCategories[SWD.SelectedCategory].Count + 1) * Text.LineHeight);
			curY = rect1.y;

			Widgets.BeginScrollView(mainRect, ref ScrollPosition, rect1, true);
			GUI.BeginGroup(rect1);
			curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, "BMME_NameLabel".Translate(), SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses? "TimeToRottle": "BMME_CellsCountLabel".Translate(), false);

			SWD.ObjectsCategories[SWD.SelectedCategory].Sort();
			if (SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses)
				SWD.ObjectsCategories[SWD.SelectedCategory].Sort(SWD);
			foreach (string currentName in SWD.ObjectsCategories[SWD.SelectedCategory])
			{
				string param = SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses ? SWD.CorpsesTimeRemain[currentName] > 0 ? SWD.CorpsesTimeRemain[currentName].ToStringTicksToDays() : "-" : SWD.AllLocations[currentName].Count.ToString();
				curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, currentName, param);
			}
			GUI.EndGroup();
			Widgets.EndScrollView();
		}

		public override Vector2 InitialSize { get => new Vector2(250f, 365f); }

		public static void DrawWindow() => Find.WindowStack.Add(new SelectWindow());

		float GroupOfThingsMaker(float x, float y, float width, string label, string param, bool createFindButton = true)
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
			
			Rect rectParam = new Rect(width - Text.CalcSize(param).x, y, Text.CalcSize(param).x, Text.LineHeight);
			Widgets.Label(rectLabel.RightPartPixels(rectParam.width), param);

			rectLabel.width -= rectParam.width;
			Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
			TooltipHandler.TipRegion(rectLabel, label.ToString());
			return rectLabel.height;
		}
	}



	public class SelectWindowData : IComparer<string>
	{
		List<FloatMenuOption> _floatMenuCategoriesOpt;

		public SelectWindowData()
		{
			AllLocations = new Dictionary<string, List<IntVec3>>{ { String.Empty, new List<IntVec3>() } };
			ObjectsCategories = new Dictionary<CategoryOfObjects, List<string>>();
			CorpsesTimeRemain = new Dictionary<string, int>();
			ThingToRender = string.Empty;
			SelectedCategory = CategoryOfObjects.Buildings;
			CurrentMap = Find.VisibleMap;
			
			_floatMenuCategoriesOpt = new List<FloatMenuOption>()
				{
					new FloatMenuOption("BMME_BuildingCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Buildings; }),
					new FloatMenuOption("BMME_TerrainCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Terrains; }),
					new FloatMenuOption("BMME_PlantCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Plants; }),
					new FloatMenuOption("BMME_PawnsCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Pawns; }),
					new FloatMenuOption("Corpses", delegate{ SelectedCategory = CategoryOfObjects.Corpses; }),
					new FloatMenuOption("BMME_OtherCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Other; })
				};
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
		public Dictionary<CategoryOfObjects, List<string>> ObjectsCategories { get; set; }
		public Dictionary<string, int> CorpsesTimeRemain { get; set; }
		public Map CurrentMap { get; set; }
		public FloatMenu CategoryMenu { get => new FloatMenu(_floatMenuCategoriesOpt); }
		public string ThingToRender { get; set; }
		public string SelectedCategoryString { get => _floatMenuCategoriesOpt[(int)SelectedCategory].Label; }
		public CategoryOfObjects SelectedCategory { get; set; }

		public void FindAllThings()
		{
			CurrentMap = Find.VisibleMap;
			IEnumerable<IntVec3> cellsLocations = CurrentMap.AllCells;
			
			ObjectsCategories.Clear();
			CorpsesTimeRemain.Clear();
			AllLocations.Clear();
			AllLocations.Add(String.Empty, new List<IntVec3>());

			foreach (IntVec3 location in cellsLocations)
			{
				if (CurrentMap.fogGrid.IsFogged(location))
					continue;
				FillData<TerrainDef>(location, location.GetTerrain(CurrentMap).label, CategoryOfObjects.Terrains);
				List <Thing> allThingsOnLocation = location.GetThingList(CurrentMap);
				if (allThingsOnLocation.Count > 0)
				{
					foreach (Thing currentThing in allThingsOnLocation)
					{
						string label = currentThing.def.label;
						if (FillData<Plant>(location, label, CategoryOfObjects.Plants, currentThing))
							continue;
						if (FillData<Pawn>(location, label, CategoryOfObjects.Pawns, currentThing))
							continue;

						if (FillData<Corpse>(location, label, CategoryOfObjects.Corpses, currentThing))
						{
							CompRottable comp = ((Corpse)currentThing).GetComp<CompRottable>();
							int currentTicksRemain = Mathf.RoundToInt(comp.PropsRot.TicksToRotStart - comp.RotProgress);
							currentTicksRemain = currentTicksRemain > 0 ? currentTicksRemain : 0;
							if (CorpsesTimeRemain.ContainsKey(label))
							{
								if (CorpsesTimeRemain[label] > currentTicksRemain && currentTicksRemain > 0)
									CorpsesTimeRemain[label] = currentTicksRemain;
							}
							else
								CorpsesTimeRemain.Add(label, currentTicksRemain);
							continue;
						}
						
						if (currentThing.Stuff != null)
							label = $"{label} ({currentThing.Stuff.LabelAsStuff})";

						if (FillData<Building>(location, label, CategoryOfObjects.Buildings, currentThing))
							continue;
						FillData<Thing>(location, currentThing.def.label, CategoryOfObjects.Other, currentThing);
					}
				}
			}
			FoundObjects_Overlay.HasUpdated = false;
		}

		private bool FillData<T>(IntVec3 location, string label, CategoryOfObjects category, Thing currentThing = null)
		{
			if (currentThing is T || currentThing == null)
			{
				if (ObjectsCategories.ContainsKey(category))
				{
					if (!ObjectsCategories[category].Contains(label))
						ObjectsCategories[category].Add(label);
				}
				else
					ObjectsCategories.Add(category, new List<string>(new string[] { label }));

				if (AllLocations.ContainsKey(label))
					AllLocations[label].Add(location);
				else
					AllLocations.Add(label, new List<IntVec3>(new IntVec3[] { location }));
				return true;
			}
			else
				return false;
		}


		int IComparer<string>.Compare(string x, string y)
		{
			if (CorpsesTimeRemain[x] > 0 && CorpsesTimeRemain[x] < CorpsesTimeRemain[y] || CorpsesTimeRemain[y] == 0 && CorpsesTimeRemain[x] > 0)
				return -1;
			if (CorpsesTimeRemain[x] > CorpsesTimeRemain[y] || CorpsesTimeRemain[x] == 0 && CorpsesTimeRemain[y] > 0)
				return 1;
			else
				return 0;
		}

		public enum CategoryOfObjects
		{
			Buildings,
			Terrains,
			Plants,
			Pawns,
			Corpses,
			Other,
		}
	}
}
