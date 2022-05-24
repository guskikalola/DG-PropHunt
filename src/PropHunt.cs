using System.Reflection;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("PropHunt")]

// The author of the mod
[assembly: AssemblyCompany("guskikalola")]

// The description of the mod
[assembly: AssemblyDescription("Prop Hunt!")]

// The mod's version
[assembly: AssemblyVersion("1.1.0.0")]


namespace DuckGame.PropHunt
{
    public class PropHunt : Mod
    {
		public static PHCore core;
		// The mod's priority; this property controls the load order of the mod.
		public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
		}

		protected override void OnStart() {
			base.OnStart();
			core = new PHCore();
		}
	}
}
