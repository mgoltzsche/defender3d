using UnityEngine;
using System.Collections;

namespace Config {
	public interface IGameLogic {
		
		void OnCharacterDestroyed(GameObject character);
	}
}
