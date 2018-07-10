@echo off
SET "ProjectName=BetterMiniMapExtended"
SET "SolutionDir=F:\GitHub Repos\Rimworld\BetterMiniMap\Source"
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Textures\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\Textures\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\Languages\"