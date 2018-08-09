using UnityEngine;
using Verse;

namespace BetterMiniMap.Overlays
{
	public class Fog_Overlay : Overlay
	{
        public Fog_Overlay() : base(!BetterMiniMapMod.settings.disableFog) { }

        public void Update() => base.Update(false);

        public override void Render()
		{
            bool[] fogGrid = Find.VisibleMap.fogGrid.fogGrid;
            // NOTE: consider SetPixels32?
            for (int i = 0; i < Find.VisibleMap.cellIndices.NumGridCells; i++)
			{
				IntVec3 intVec = Find.VisibleMap.cellIndices.IndexToCell(i);
				base.Texture.SetPixel(intVec.x, intVec.z, fogGrid[i] ? BetterMiniMapMod.settings.overlayColors.fog : Color.clear);
			}
        }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.fog;

		public override bool GetShouldUpdateOverlay()
		{
			bool shouldUpdate = base.GetShouldUpdateOverlay();
			if (Visible == BetterMiniMapMod.settings.disableFog)
				Visible = !Visible;
			return shouldUpdate;
		}
	}
}
