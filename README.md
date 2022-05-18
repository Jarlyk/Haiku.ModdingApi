[//]: # ( Haiku Core Modding API )

# Introduction
This is the core modding API for the game Haiku, the Robot.  It's designed with the BepInEx modloader framework and provides a standard set of functions for use by other mods.  In particular, it includes a merged MonoMod Hook interface, that allows for easy runtime injection modding using On and IL hooks.  

### Getting Started with Modding
For a preliminary introduction to MonoMod modding using BepInEx, see this guide: https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/index.html
For Haiku in particular, you'll want to reference this modding API to get access to the hooks, as well as the game code to be able to use objects defined in the code.  Depending on the nature of your mod, you may also need to reference one or more Unity assemblies from the game. Within the NameSpace Modding you'll find CanvasUtil for easier UI creation and a ConfigManagerUtil to assist the creation of Configs for your Mods. See https://github.com/Jarlyk/Haiku.DebugMod for examples.
More modding information to be documented later, hopefully.

### Building the API
This Git repo includes the BepInEx and Monomod dependencies, but does not include the Unity or Haiku Assemblies.  Prior to building, you'll need to copy the necessary files from the Managed folder in your Haiku installation to the lib/Game folder.
If the solution builds successfully, you should have a bin folder with two files in it.  Haiku.CoreModdingApi.dll is the modding API with embedded MMHook tooling and should go in the BepInEx plugins folder in the deployment.  The Assembly-CSharp.dll file in Bin is the stripped and publicized game dll for use by mod makers; this should _not_ be deployed to the game's Managed folder.

### Contact
You can reach me via Github.  As with most mods, this is a hobby project, so please understand that response times to questions and time to update for new game releases may vary.

### License
All mods contained herein are released under the standard MIT license, which is a permissive license that allows for free use.  The text of this is included in the release archive in License.txt under each mod.
