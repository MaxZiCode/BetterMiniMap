using UnityEngine;
using Verse;
using SettingsHelper;
using ColorPicker.Dialog;

namespace BetterMiniMap
{
    class BetterMiniMapSettings : ModSettings
	{
        // TODO: look at clustering these parameters better in a struct/class => fix GetIndicatorProperities()
        #region NestedClasses
        public class UpdatePeriods : IExposable
        {
            public int viewpoint = 5;
            public int colonists = 15;
            public int noncolonists = 15;
            public int robots = 15;
            public int ships = 60;
            public int powerGrid = 60;
            public int wildlife = 80;
            public int buildings = 250;
            public int mining = 250;
            public int areas = 250;
            public int terrain = 2500;
            public int fog = 2500;

            public void ExposeData()
            {
                Scribe_Values.Look(ref this.viewpoint, "viewpoint", 5);
                Scribe_Values.Look(ref this.colonists, "colonists", 15);
                Scribe_Values.Look(ref this.noncolonists, "noncolonists", 15);
                Scribe_Values.Look(ref this.robots, "robots", 15);
                Scribe_Values.Look(ref this.ships, "ships", 60);
                Scribe_Values.Look(ref this.powerGrid, "powerGrid", 60);
                Scribe_Values.Look(ref this.wildlife, "wildlife", 80);
                Scribe_Values.Look(ref this.buildings, "buildings", 250);
                Scribe_Values.Look(ref this.mining, "mining", 250);
                Scribe_Values.Look(ref this.areas, "areas", 250);
                Scribe_Values.Look(ref this.terrain, "terrain", 2500);
                Scribe_Values.Look(ref this.fog, "fog", 2500);
            }
        }

        public class IndicatorSizes : IExposable
		{
            public float colonists = 4f;
            public float tamedAnimals = 2f;
            public float enemyPawns = 4f;
            public float traderPawns = 3f;
            public float visitorPawns = 3f;
            public float robots = 3f;
            public float ships = 3f;
            public float wildlife = 2f;
            public float wildlifeTaming = 3f;
            public float wildlifeHunting = 3f;
            public float wildlifeHostiles = 4f;
			public float wildlifePredators = 3f;

            public void ExposeData()
            {
                Scribe_Values.Look(ref this.colonists, "colonists", 4f);
                Scribe_Values.Look(ref this.tamedAnimals, "tamedAnimals", 2f);
                Scribe_Values.Look(ref this.enemyPawns, "enemyPawns", 4f);
                Scribe_Values.Look(ref this.traderPawns, "traderPawns", 3f);
                Scribe_Values.Look(ref this.visitorPawns, "visitorPawns", 3f);
                Scribe_Values.Look(ref this.robots, "robots", 3f);
                Scribe_Values.Look(ref this.ships, "ships", 3f);
                Scribe_Values.Look(ref this.wildlife, "wildlife", 2f);
                Scribe_Values.Look(ref this.wildlifeTaming, "wildlifeTaming", 3f);
                Scribe_Values.Look(ref this.wildlifeHunting, "wildlifeHunting", 3f);
                Scribe_Values.Look(ref this.wildlifeHostiles, "wildlifeHostiles", 4f);
				Scribe_Values.Look(ref this.wildlifePredators, "wildlifePredators", 3f);
			}
        }

        public class OverlayColors : IExposable
		{
            private static readonly Color miningColorDefault = new Color(0.75f, 0.4f, 0.125f, 1f);
            //private static readonly Color fadedBlack = new Color(0, 0, 0, 0.25f);
            private static readonly Color darkGray = new Color(0.65f, 0.65f, 0.65f, 1f);

            public Color viewpoint = Color.black;
            public Color fog = darkGray;
            public Color buildings = Color.white;

            public Color colonists = new Color(0.020f, 0.996f, 0.043f, 1.000f);
            public Color tamedAnimals = new Color(0.310f, 1.000f, 0.965f, 1.000f);
            public Color enemyPawns = Color.red;
            public Color traderPawns = Color.blue;
            public Color visitorPawns = new Color(0.992f, 0.757f, 1.000f, 1.000f);
            public Color robots = new Color(0.537f, 0.522f, 0.996f, 1.000f);
            public Color ships = new Color(0.682f, 0.000f, 0.855f, 1.000f);
            public Color wildlife = Color.yellow;
            public Color wildlifeTaming = new Color(0.020f, 1.000f, 0.431f, 1.000f);
            public Color wildlifeHunting = new Color(0.996f, 0.435f, 0.643f, 1.000f);
            public Color wildlifeHostiles = new Color(0.671f, 0.000f, 0.157f, 1.000f);
			public Color wildlifePredators = Color.red;

            // used for edge colors
            public Color viewpointFaded;
            public Color colonistsFaded;
            public Color tamedAnimalsFaded;
            public Color enemyPawnsFaded;
            public Color traderPawnsFaded;
            public Color visitorPawnsFaded;
            public Color robotsFaded;
            public Color shipsFaded;
            public Color wildlifeFaded;
            public Color wildlifeTamingFaded;
            public Color wildlifeHuntingFaded;
            public Color wildlifeHostilesFaded;
			public Color wildlifePredatorsFaded;


			public Color poweredOn = new Color(0.937f, 1.000f, 0.616f, 1.000f);
            public Color poweredByBatteries = new Color(0.478f, 1.000f, 0.482f, 1.000f);
            public Color notPowered = new Color(1.000f, 0.408f, 0.392f, 1.000f);
            public Color powererOff = Color.gray;

            public Color mining = miningColorDefault;

            public OverlayColors() => this.SetFadedColors();

            public void ExposeData()
			{
                Scribe_Values.Look(ref viewpoint, "viewpoint", Color.black);
                Scribe_Values.Look(ref fog, "fog", darkGray);
                Scribe_Values.Look(ref buildings, "buildings", Color.white);

                Scribe_Values.Look(ref colonists, "colonists", new Color(0.020f, 0.996f, 0.043f, 1.000f));
                Scribe_Values.Look(ref tamedAnimals, "tamedAnimals", new Color(0.310f, 1.000f, 0.965f, 1.000f));
                Scribe_Values.Look(ref enemyPawns, "enemyPawns", Color.red);
                Scribe_Values.Look(ref traderPawns, "traderPawns", Color.blue);
                Scribe_Values.Look(ref visitorPawns, "visitorPawns", new Color(0.992f, 0.757f, 1.000f, 1.000f));
                Scribe_Values.Look(ref robots, "robots", new Color(0.537f, 0.522f, 0.996f, 1.000f));
                Scribe_Values.Look(ref ships, "ships", new Color(0.682f, 0.000f, 0.855f, 1.000f));
                Scribe_Values.Look(ref wildlife, "wildlife", Color.yellow);
                Scribe_Values.Look(ref wildlifeTaming, "wildlifeTaming", new Color(0.020f, 1.000f, 0.431f, 1.000f));
                Scribe_Values.Look(ref wildlifeHunting, "wildlifeHunting", new Color(0.996f, 0.435f, 0.643f, 1.000f));
                Scribe_Values.Look(ref wildlifeHostiles, "wildlifeHostiles", new Color(0.671f, 0.000f, 0.157f, 1.000f));
				Scribe_Values.Look(ref wildlifePredators, "wildlifePredators", Color.red);

				Scribe_Values.Look(ref poweredOn, "poweredOn", new Color(0.937f, 1.000f, 0.616f, 1.000f));
                Scribe_Values.Look(ref poweredByBatteries, "poweredByBatteries", new Color(0.478f, 1.000f, 0.482f, 1.000f));
                Scribe_Values.Look(ref notPowered, "notPowered", new Color(1.000f, 0.408f, 0.392f, 1.000f));
                Scribe_Values.Look(ref powererOff, "powererOff", Color.gray);

                Scribe_Values.Look(ref mining, "mining", miningColorDefault);

                this.SetFadedColors();
            }

            private void SetFadedColors()
            {
                this.viewpointFaded = BetterMiniMapSettings.FadedColor(this.viewpoint, 0.25f);

                this.colonistsFaded = BetterMiniMapSettings.FadedColor(this.colonists);
                this.tamedAnimalsFaded = BetterMiniMapSettings.FadedColor(this.tamedAnimals);
                this.enemyPawnsFaded = BetterMiniMapSettings.FadedColor(this.enemyPawns);
                this.traderPawnsFaded = BetterMiniMapSettings.FadedColor(this.traderPawns);
                this.visitorPawnsFaded = BetterMiniMapSettings.FadedColor(this.visitorPawns);
                this.robotsFaded = BetterMiniMapSettings.FadedColor(this.robots);
                this.shipsFaded = BetterMiniMapSettings.FadedColor(this.ships);
                this.wildlifeFaded = BetterMiniMapSettings.FadedColor(this.wildlife);
                this.wildlifeTamingFaded = BetterMiniMapSettings.FadedColor(this.wildlifeTaming);
                this.wildlifeHuntingFaded = BetterMiniMapSettings.FadedColor(this.wildlifeHunting);
                this.wildlifeHostilesFaded = BetterMiniMapSettings.FadedColor(this.wildlifeHostiles);
				this.wildlifePredatorsFaded = BetterMiniMapSettings.FadedColor(this.wildlifePredators);
			}
        }
        #endregion NestedClasses

        // Initializes settings for users without config file
        public BetterMiniMapSettings()
        {
            this.updatePeriods = new UpdatePeriods();
            this.indicatorSizes = new IndicatorSizes();
            this.overlayColors = new OverlayColors();
        }

        public string rootDir; // NOTE: no need to expose
        public UpdatePeriods updatePeriods;
        public IndicatorSizes indicatorSizes;
        public OverlayColors overlayColors;
		public bool disableFog = false;

		// HIDDEN SETTINGS
		public bool mipMaps = false;
        public int anisoLevel = 1;

        public override void ExposeData() 
        {
            base.ExposeData();
            Scribe_Deep.Look(ref this.updatePeriods, "updatePeriods");
            Scribe_Deep.Look(ref this.indicatorSizes, "indicatorSizes");
            Scribe_Deep.Look(ref this.overlayColors, "overlayColors");
			Scribe_Values.Look<bool>(ref this.disableFog, "disableFog", false);

			// HIDDEN SETTINGS
			Scribe_Values.Look<bool>(ref this.mipMaps, "mipMaps", false);
            Scribe_Values.Look<int>(ref this.anisoLevel, "anisoLevel", 1);

            // Handles upgrading settings
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (this.updatePeriods == null)
                    this.updatePeriods = new UpdatePeriods();
                if (this.indicatorSizes == null)
                    this.indicatorSizes = new IndicatorSizes();
                if (this.overlayColors == null)
                    this.overlayColors = new OverlayColors();
            }
        }

        public static Color FadedColor(Color inColor, float alpha = 0.5f)
        {
            Color faded = inColor;
            faded.a = alpha;
            return faded;
        }
    }

    class BetterMiniMapMod : Mod
    {
        public static BetterMiniMapSettings settings;

        public BetterMiniMapMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<BetterMiniMapSettings>();
            settings.rootDir = content.RootDir;
            ListingStandardHelper.Gap = 10f;
        }

        public override string SettingsCategory() => "BMM_SettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect rect)
		{
            rect.height += 50f;
            Listing_Standard listing_Standard = new Listing_Standard() { ColumnWidth = (rect.width / 3f) - 15f };
            listing_Standard.Begin(rect);

            // UpdatePeriods
            listing_Standard.AddLabelLine("BMM_TimeUpdateLabel".Translate(), 2f * Text.LineHeight);

            listing_Standard.AddLabeledNumericalTextField<int>("BMM_AreasOverlayLabel".Translate(), ref settings.updatePeriods.areas, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_BuildingsOverlayLabel".Translate(), ref settings.updatePeriods.buildings, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_ColonistsOverlayLabel".Translate(), ref settings.updatePeriods.colonists, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_MiningOverlayLabel".Translate(), ref settings.updatePeriods.mining, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_NoncolonistOverlayLabel".Translate(), ref settings.updatePeriods.noncolonists, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_PowerGridOverlayLabel".Translate(), ref settings.updatePeriods.powerGrid, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_RobotsOverlayLabel".Translate(), ref settings.updatePeriods.robots, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_ShipsOverlayLabel".Translate(), ref settings.updatePeriods.ships, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_TerrainOverlayLabel".Translate(), ref settings.updatePeriods.terrain, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_WildlifeOverlayLabel".Translate(), ref settings.updatePeriods.wildlife, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_FogOverlayLabel".Translate(), ref settings.updatePeriods.fog, 0.75f);
            listing_Standard.AddLabeledNumericalTextField<int>("BMM_ViewpointOverlayLabel".Translate(), ref settings.updatePeriods.viewpoint, 0.75f);

			listing_Standard.AddLabeledCheckbox("BMME_DisableFogLabel".Translate(), ref settings.disableFog);

			listing_Standard.NewColumn();

            // IndicatorSizes
            listing_Standard.AddLabelLine("BMM_IndicatorSizeLabel".Translate());

            listing_Standard.AddLabeledNumericalTextField<float>("BMM_ColonistIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.colonists, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_AnimalIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.tamedAnimals, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_RobotsIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.robots, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_EnemyIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.enemyPawns, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_TraderIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.traderPawns, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_VisitorIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.visitorPawns, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_ShipsIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.ships, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_WildlifeIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlife, 0.9f, 1f, 9f);
			listing_Standard.AddLabeledNumericalTextField<float>("BMME_WildlifePredatorsSizeLabel".Translate(), ref settings.indicatorSizes.wildlifePredators, 0.9f, 1f, 9f);
			listing_Standard.AddLabeledNumericalTextField<float>("BMM_TamingIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeTaming, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_HostileAnimalIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeHostiles, 0.9f, 1f, 9f);
            listing_Standard.AddLabeledNumericalTextField<float>("BMM_HuntingIndicatorSizeLabel".Translate(), ref settings.indicatorSizes.wildlifeHunting, 0.9f, 1f, 9f);

			listing_Standard.NewColumn();

            listing_Standard.AddLabelLine("BMM_ColorPickersLabel".Translate());

            // ColorPicker
            listing_Standard.AddColorPickerButton("BMM_ViewpointOverlayLabel".Translate(), settings.overlayColors.viewpoint, (SelectionColorWidget scw) => {
                settings.overlayColors.viewpoint = scw.SelectedColor;
                settings.overlayColors.viewpointFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor, 0.25f);
            });

            //listing_Standard.AddColorPickerButton("BMM_FogOverlayLabel".Translate(), settings.overlayColors.fog, (SelectionColorWidget scw) => { settings.overlayColors.fog = scw.SelectedColor; });
            listing_Standard.AddColorPickerButton("BMM_BuildingsOverlayLabel".Translate(), settings.overlayColors.buildings, (SelectionColorWidget scw) => { settings.overlayColors.buildings = scw.SelectedColor; });

            listing_Standard.AddColorPickerButton("BMM_ColonistIndicatorSizeLabel".Translate(), settings.overlayColors.colonists, (SelectionColorWidget scw) => {
                settings.overlayColors.colonists = scw.SelectedColor;
                settings.overlayColors.colonistsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_AnimalIndicatorSizeLabel".Translate(), settings.overlayColors.tamedAnimals, (SelectionColorWidget scw) => {
                settings.overlayColors.tamedAnimals = scw.SelectedColor;
                settings.overlayColors.tamedAnimalsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_EnemyIndicatorSizeLabel".Translate(), settings.overlayColors.enemyPawns, (SelectionColorWidget scw) => {
                settings.overlayColors.enemyPawns = scw.SelectedColor;
                settings.overlayColors.enemyPawnsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_TraderIndicatorSizeLabel".Translate(), settings.overlayColors.traderPawns, (SelectionColorWidget scw) => {
                settings.overlayColors.traderPawns = scw.SelectedColor;
                settings.overlayColors.traderPawnsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_VisitorIndicatorSizeLabel".Translate(), settings.overlayColors.visitorPawns, (SelectionColorWidget scw) => {
                settings.overlayColors.visitorPawns = scw.SelectedColor;
                settings.overlayColors.visitorPawnsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_RobotsIndicatorSizeLabel".Translate(), settings.overlayColors.robots, (SelectionColorWidget scw) => {
                settings.overlayColors.robots = scw.SelectedColor;
                settings.overlayColors.robotsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_ShipsIndicatorSizeLabel".Translate(), settings.overlayColors.ships, (SelectionColorWidget scw) => {
                settings.overlayColors.ships = scw.SelectedColor;
                settings.overlayColors.shipsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_WildlifeIndicatorSizeLabel".Translate(), settings.overlayColors.wildlife, (SelectionColorWidget scw) => {
                settings.overlayColors.wildlife = scw.SelectedColor;
                settings.overlayColors.wildlifeFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
			listing_Standard.AddColorPickerButton("BMME_WildlifePredatorsSizeLabel".Translate(), settings.overlayColors.wildlifePredators, (SelectionColorWidget scw) => {
				settings.overlayColors.wildlifePredators = scw.SelectedColor;
				settings.overlayColors.wildlifePredatorsFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
			});
			listing_Standard.AddColorPickerButton("BMM_TamingIndicatorSizeLabel".Translate(), settings.overlayColors.wildlifeTaming, (SelectionColorWidget scw) => {
                settings.overlayColors.wildlifeTaming = scw.SelectedColor;
                settings.overlayColors.wildlifeTamingFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_HuntingIndicatorSizeLabel".Translate(), settings.overlayColors.wildlifeHunting, (SelectionColorWidget scw) => {
                settings.overlayColors.wildlifeHunting = scw.SelectedColor;
                settings.overlayColors.wildlifeHuntingFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });
            listing_Standard.AddColorPickerButton("BMM_HostileAnimalIndicatorSizeLabel".Translate(), settings.overlayColors.wildlifeHostiles, (SelectionColorWidget scw) => {
                settings.overlayColors.wildlifeHostiles = scw.SelectedColor;
                settings.overlayColors.wildlifeHostilesFaded = BetterMiniMapSettings.FadedColor(scw.SelectedColor);
            });

			listing_Standard.AddColorPickerButton("BMM_PoweredOnLabel".Translate(), settings.overlayColors.poweredOn, (SelectionColorWidget scw) => { settings.overlayColors.poweredOn = scw.SelectedColor; });
            listing_Standard.AddColorPickerButton("BMM_PoweredByBatteriesLabel".Translate(), settings.overlayColors.poweredByBatteries, (SelectionColorWidget scw) => { settings.overlayColors.poweredByBatteries = scw.SelectedColor; });
            listing_Standard.AddColorPickerButton("BMM_NotPoweredLabel".Translate(), settings.overlayColors.notPowered, (SelectionColorWidget scw) => { settings.overlayColors.notPowered = scw.SelectedColor; });
            listing_Standard.AddColorPickerButton("BMM_PoweredOffLabel".Translate(), settings.overlayColors.powererOff, (SelectionColorWidget scw) => { settings.overlayColors.powererOff = scw.SelectedColor; });

            listing_Standard.AddColorPickerButton("BMM_MiningOverlayLabel".Translate(), settings.overlayColors.mining, (SelectionColorWidget scw) => { settings.overlayColors.mining = scw.SelectedColor; });

            listing_Standard.End();

            settings.Write();
        }
    }

    internal static class ColorPickerHelper
    {
        public static void AddColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, SelectionChange selectionChange, string buttonText = "Change")
        {
            listing_Standard.Gap(ListingStandardHelper.Gap);
            Rect lineRect = listing_Standard.GetRect();

            float textSize = Text.CalcSize(buttonText).x + 10f;
            float rightSize = textSize + 5f + lineRect.height; 
            Rect rightPart = lineRect.RightPartPixels(textSize + 5f + lineRect.height);

            // draw button leaving room for color rect in rightHalf rect (plus some padding)
            if (Widgets.ButtonText(rightPart.LeftPartPixels(textSize), buttonText))
                Find.WindowStack.Add(new ColorSelectDialog(buttonText, color, selectionChange));
            GUI.color = color;
            // draw square with color in rightHalf rect
            GUI.DrawTexture(rightPart.RightPartPixels(rightPart.height), BaseContent.WhiteTex);
            GUI.color = Color.white;

            Rect leftPart = lineRect.LeftPartPixels(lineRect.width - rightSize);
            Widgets.Label(leftPart, label);
        }
    }
}
