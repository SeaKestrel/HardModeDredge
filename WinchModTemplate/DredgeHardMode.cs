using UnityEngine;
using Winch.Core;

namespace DredgeHardMode
{
	public class DredgeHardMode : MonoBehaviour
	{
		public void Awake()
		{
			WinchCore.Log.Debug($"{nameof(DredgeHardMode)} has loaded!");
		}
	}
}
