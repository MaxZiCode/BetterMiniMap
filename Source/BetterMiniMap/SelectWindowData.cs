using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using BetterMiniMap.Overlays;
using RimWorld;

namespace BetterMiniMap
{
	public class SelectWindowData : IComparer<string>
	{
		readonly List<FloatMenuOption> _floatMenuCategoriesOpt;

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
					new FloatMenuOption("BMME_СorpsesCategoryLabel".Translate(), delegate{ SelectedCategory = CategoryOfObjects.Corpses; }),
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
