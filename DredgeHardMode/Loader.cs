using HarmonyLib;
using UnityEngine;

namespace DredgeHardMode
{
	public class Loader
	{
		/// <summary>
		/// This method is run by Winch to initialize your mod
		/// </summary>
		public static void Initialize()
		{
			var gameObject = new GameObject(nameof(DredgeHardMode));
			gameObject.AddComponent<DredgeHardMode>();
			GameObject.DontDestroyOnLoad(gameObject);
			new Harmony("com.dredge.hardmode").PatchAll();
        }
	}
}