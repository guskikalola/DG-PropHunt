using System.Reflection;
using System.Collections.Generic;

// The title of your mod, as displayed in menus
[assembly: AssemblyTitle("PropHunt")]

// The author of the mod
[assembly: AssemblyCompany("guskikalola")]

// The description of the mod
[assembly: AssemblyDescription("Prop Hunt!")]

// The mod's version
[assembly: AssemblyVersion("1.2.0.1")]


namespace DuckGame.PropHunt
{
    public class PropHunt : Mod
    {

		public static List<PHTaunt> taunts = new List<PHTaunt>();

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
			LoadTaunts();
		}

		protected override void OnStart() {
			base.OnStart();
			core = new PHCore();
		}

		private void LoadTaunts()
        {
			Sprite lotsofdamage = new Sprite(GetPath("sprites/taunts/lotsofdamage"));
			Sprite snorememe = new Sprite(GetPath("sprites/taunts/snorememe"));

			AddTaunt(GetPath("sounds/taunts/lotofdamage.ogg"),lotsofdamage);
			AddTaunt(GetPath("sounds/taunts/tiktok_snore.ogg"),snorememe);

        }

		public void AddTaunt(string path, Sprite icon)
        {
			int index = taunts.Count;
			taunts.Add(new PHTaunt(path, index,icon));
		}

		public void AddTaunt(string path)
		{
			int index = taunts.Count;
			taunts.Add(new PHTaunt(path, index));
		}
	}
}
