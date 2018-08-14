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
			if (SWD.LocationsDict.Count == 0 || SWD.MapInProcess != Find.VisibleMap)
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
				SWD.ThingToSeek = string.Empty;
			}

			float curY = updateButtonRect.yMax;
			string partOfCategoryLabel = "BMME_CategoryLabel".Translate() + ": ";
			Widgets.ListSeparator(ref curY, inRect.width, partOfCategoryLabel + " " + SWD.SelectedCategoryName);
			Rect categoryButtonRect = new Rect(inRect.x, updateButtonRect.yMax, inRect.width, curY - updateButtonRect.yMax);
			if (Widgets.ButtonInvisible(categoryButtonRect))
				Find.WindowStack.Add(SWD.CategoryMenu);
			Widgets.DrawHighlightIfMouseover(categoryButtonRect);

			if (!SWD.CategoriesDict.ContainsKey(SWD.SelectedCategory))
			{
				Widgets.Label(new Rect(titleRect) { y = curY },   "BMME_NotFoundString".Translate(SWD.SelectedCategoryName));
				return;
			}
			Rect mainRect = new Rect(inRect){ yMin = curY };
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (SWD.CategoriesDict[SWD.SelectedCategory].Count + 1) * Text.LineHeight);
			curY = rect1.y;

			Widgets.BeginScrollView(mainRect, ref ScrollPosition, rect1, true);
			GUI.BeginGroup(rect1);
			curY += this.GroupOfThingsMaker(rect1.x, curY, rect1.width, "BMME_NameLabel".Translate(), SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses? "BMME_TimeUntilRotted".Translate() : "BMME_CellsCountLabel".Translate(), false);

			SWD.CategoriesDict[SWD.SelectedCategory].Sort();
			if (SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses)
				SWD.CategoriesDict[SWD.SelectedCategory].Sort(SWD);
			foreach (string currentName in SWD.CategoriesDict[SWD.SelectedCategory])
			{
				string param = SWD.SelectedCategory == SelectWindowData.CategoryOfObjects.Corpses ? (SWD.CorpsesTimeRemainDict[currentName] > 0 ? SWD.CorpsesTimeRemainDict[currentName].ToStringTicksToDays() : "-") : SWD.LocationsDict[currentName]?.Count.ToString();
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

			if (label == SWD.ThingToSeek)
				Widgets.DrawHighlightSelected(rectLabel);
			else if(createFindButton)
			{
				if (Widgets.ButtonInvisible(rectLabel))
				{
					SWD.ThingToSeek = label;
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
}
