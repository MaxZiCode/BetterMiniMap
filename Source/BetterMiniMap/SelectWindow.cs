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
		Vector2 scrollPosition;

		public SelectWindow() : base()
		{
			this.doCloseX = true;
			this.preventDrawTutor = true;
			this.draggable = true;
			
			this.scrollPosition = new Vector2();
		}

		public override void PreOpen()
		{
			base.PreOpen();
			if (FoundObjects_Overlay.AllLocations == null || FoundObjects_Overlay.ObjectsCategories == null)
			{
				FoundObjects_Overlay.AllLocations = new Dictionary<string, List<IntVec3>>();
				FoundObjects_Overlay.ObjectsCategories = new Dictionary<string, List<string>>();
				FoundObjects_Overlay.FindAllThings();
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;

			Rect updateButtonRect = new Rect(inRect.x, inRect.y, inRect.width / 2f, 25f);
			Rect clearButtonRect = new Rect(inRect.x + updateButtonRect.width, updateButtonRect.y, updateButtonRect.width - 4f, updateButtonRect.height);
			if (Widgets.ButtonText(updateButtonRect, "Update"))
				FoundObjects_Overlay.FindAllThings();
			if (Widgets.ButtonText(clearButtonRect, "Clear"))
				FoundObjects_Overlay.Positions = null;

			float countColumnWidth = 80f;
			Rect mainRect = new Rect(inRect.x, inRect.y + updateButtonRect.height,inRect.width, inRect.height - updateButtonRect.height);
			Rect rect1 = new Rect(0.0f, 0.0f, mainRect.width - 16f, (FoundObjects_Overlay.AllLocations.Count + 1) * Text.LineHeight);
			Listing_Standard l_s = new Listing_Standard();

			Widgets.BeginScrollView(mainRect, ref this.scrollPosition, rect1, true);
			l_s.Begin(rect1);
			this.ListOfThingsMaker(l_s, countColumnWidth, "Label", "Cells count", false);
			foreach (string current in FoundObjects_Overlay.ObjectsCategories["Default"])
			{
				this.ListOfThingsMaker(l_s, countColumnWidth, current, FoundObjects_Overlay.AllLocations[current].Count.ToString());
			}
			l_s.End();
			Widgets.EndScrollView();
		}

		public override Vector2 InitialSize { get => new Vector2(300f, 320f); }

		public static void DrawWindow() => Find.WindowStack.Add(new SelectWindow());


		void ListOfThingsMaker(Listing_Standard l_s, float countColumnWidth, string label, string countOfCells, bool createFindButton = true)
		{
			float findButtonWidth = Text.CalcSize("Find").x + 4f;
			Rect rectLabel = l_s.GetRect(Text.LineHeight);
			Widgets.DrawHighlightIfMouseover(rectLabel);
			rectLabel.width -= countColumnWidth + findButtonWidth;
			Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
			TooltipHandler.TipRegion(rectLabel, label.ToString()); 

			Rect rectCount = new Rect(rectLabel) { x = rectLabel.width, width = countColumnWidth };
			Widgets.Label(rectCount.LeftPartPixels(rectCount.width), countOfCells);
			TooltipHandler.TipRegion(rectCount, countOfCells);

			if (createFindButton)
			{
				Rect findButtonRect = new Rect(rectCount.x + rectCount.width, rectLabel.y, findButtonWidth, rectLabel.height);
				if (Widgets.ButtonText(findButtonRect, "Find"))
					FoundObjects_Overlay.Positions = FoundObjects_Overlay.AllLocations[label]; 
			}
		}
	}
}
