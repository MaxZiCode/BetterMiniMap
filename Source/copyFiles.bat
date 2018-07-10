@echo off
<<<<<<< HEAD
SET "ProjectName=BetterMiniMap"
SET "SolutionDir=F:\GitHub Repos\Rimworld\%ProjectName%\Source"
=======
SET "ProjectName=BetterMiniMapExtended"
SET "SolutionDir=F:\GitHub Repos\Rimworld\BetterMiniMap\Source"
>>>>>>> master
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Textures\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\Textures\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "F:\Steam\SteamApps\common\RimWorld\Mods\%ProjectName%\Languages\"