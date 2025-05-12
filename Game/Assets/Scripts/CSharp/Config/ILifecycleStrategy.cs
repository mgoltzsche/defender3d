using UnityEngine;
using System.Collections;

namespace Config {
	public interface ILifecycleStrategy {
	
		GameObject Instantiate(GameObject tpl);
		GameObject Instantiate(GameObject tpl, Vector3 position, Quaternion rotation);
		void Hit(AbstractCharacterController shooter, AbstractCharacterController victim);
		bool IsMine(GameObject obj);
		void Call(MonoBehaviour behaviour, string methodName, params object[] parameters);
		void Call(MonoBehaviour behaviour, string methodName, RPCMode rpcMode, params object[] parameters);
	}
}
