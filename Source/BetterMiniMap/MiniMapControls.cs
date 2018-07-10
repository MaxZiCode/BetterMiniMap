﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Verse;

using BetterMiniMap.Overlays;

namespace BetterMiniMap
{
    internal class MiniMapControls : Window
    {
		public const float buttonWidth = 20f;
		private const float buttonMargin = 6f;

        private Rect configButtonRect;
        private Rect dragButtonRect;
        private Rect homeButtonRect;
        private Rect resizeButtonRect;
		private Rect FindButtonRect;

        private MiniMapWindow miniMap;

        private List<FloatMenuOption> areasOptions;

        private FloatMenu overlayMenu;
        
        public MiniMapControls(MiniMapWindow miniMap)
        {
            this.miniMap = miniMap;

            this.closeOnEscapeKey = false;
            this.preventCameraMotion = false;
            this.layer = WindowLayer.GameUI;

            float xPos = buttonMargin;
			this.FindButtonRect = new Rect(buttonMargin, buttonMargin, buttonWidth, buttonWidth);
			this.homeButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.configButtonRect = new Rect(0 , buttonMargin, buttonWidth, buttonWidth);
            this.resizeButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);
            this.dragButtonRect = new Rect(0, buttonMargin, buttonWidth, buttonWidth);

            float xDiff = buttonWidth + 2f * buttonMargin;

			this.homeButtonRect.x = xPos += xDiff;
            this.configButtonRect.x = xPos += xDiff;
            this.resizeButtonRect.x = xPos += xDiff;
            this.dragButtonRect.x = xPos += xDiff;

            //this.SetLocality();
        }

        protected override float Margin { get => 0f; }

        public void SetLocality()
        {
            float width = this.dragButtonRect.x + buttonWidth + buttonMargin;
            this.windowRect = new Rect(miniMap.windowRect.x + miniMap.windowRect.width - width, miniMap.windowRect.y + miniMap.windowRect.height, width, this.DefaultHeight);
        }

        public float DefaultHeight => buttonWidth + 2f * buttonMargin;

        public FloatMenu OverlayMenu
        {
            get
            {
                if (this.overlayMenu == null)
                {
                    this.overlayMenu = new FloatMenu(miniMap.GenerateOverlayMenuItems())
                    {
                        closeOnEscapeKey = true,
                        preventCameraMotion = false,
                    };
                }
                return this.overlayMenu;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();
            this.SetLocality();
        }

        public override void Notify_ResolutionChanged() => this.SetLocality();

        public override void DoWindowContents(Rect inRect)
        {
            if (miniMap.draggable || miniMap.resizing)
                this.SetLocality();

            Widgets.DrawHighlightIfMouseover(this.homeButtonRect);
            TooltipHandler.TipRegion(this.homeButtonRect, "BMM_HomeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.homeButtonRect, MiniMapTextures.homeA))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.GetAreaOptions());
            }

            Widgets.DrawHighlightIfMouseover(this.configButtonRect);
            TooltipHandler.TipRegion(this.configButtonRect, "BMM_ConfigButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.configButtonRect, MiniMapTextures.config))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                    Find.WindowStack.Add(this.OverlayMenu);
            }

            Widgets.DrawHighlightIfMouseover(this.resizeButtonRect);
            TooltipHandler.TipRegion(this.resizeButtonRect, "BMM_ResizeButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.resizeButtonRect, miniMap.resizing ? MiniMapTextures.resizeA : MiniMapTextures.resizeD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.resizing = !miniMap.resizing;
                    miniMap.draggable = false;
                }
            }

            Widgets.DrawHighlightIfMouseover(this.dragButtonRect);
            TooltipHandler.TipRegion(this.dragButtonRect, "BMM_DragButtonTooltip".Translate());
            if (Widgets.ButtonImage(this.dragButtonRect, miniMap.draggable ? MiniMapTextures.dragA : MiniMapTextures.dragD))
            {
                if (Event.current.button == 0 || Event.current.button == 1) // left/right click
                {
                    miniMap.draggable = !miniMap.draggable;
                    miniMap.resizing = false;
                }
            }

<<<<<<< HEAD
			Widgets.DrawHighlightIfMouseover(this.FindButtonRect); //TODO: will change
=======
			Widgets.DrawHighlightIfMouseover(this.FindButtonRect);
>>>>>>> master
			TooltipHandler.TipRegion(this.FindButtonRect, "BMME_FindButtonTooltip".Translate());
			if (Widgets.ButtonImage(this.FindButtonRect, MiniMapTextures.find))
			{
				if (Event.current.button == 0 || Event.current.button == 1) // left/right click
					SelectWindow.DrawWindow();
			}
		}

        private FloatMenu GetAreaOptions()
        {
            this.areasOptions = new List<FloatMenuOption>();

            List<Area> allAreas = Find.VisibleMap.areaManager.AllAreas;

            for (int i=0; i < allAreas.Count; i++)
                this.areasOptions.Add(new FloatMenuRadioButton(allAreas[i], this.miniMap));

            return new FloatMenu(this.areasOptions)
            {
                closeOnEscapeKey = true,
                preventCameraMotion = false,
            };
        }

    }
}
