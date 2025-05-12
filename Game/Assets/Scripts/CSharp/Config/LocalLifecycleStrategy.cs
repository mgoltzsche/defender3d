using UnityEngine;
using System.Collections;

namespace Config {
	public class LocalLifecycleStrategy : ILifecycleStrategy {
	
		public GameObject Instantiate(GameObject tpl) {
			return (GameObject) GameObject.Instantiate(tpl);
		}
		
		public GameObject Instantiate(GameObject tpl, Vector3 position, Quaternion rotation) {
			return (GameObject) GameObject.Instantiate(tpl, position, rotation);
		}

		public void Hit(AbstractCharacterController shooter, AbstractCharacterController victim) {
			if (!victim.dead) {
				bool isEnemy = GameFacade.instance.IsEnemy(shooter, victim);
				shooter.SetPoints(shooter.points + (isEnemy ? victim.pointsWorth : -victim.pointsWorth));
				victim.DieLater();
			}
		}
		
		public bool IsMine(GameObject obj) {
			return true;
		}
		
		public void Call(MonoBehaviour behaviour, string methodName, params object[] parameters) {
			behaviour.GetType().GetMethod(methodName).Invoke(behaviour, parameters);
		}
		
		public void Call(MonoBehaviour behaviour, string methodName, RPCMode rpcMode, params object[] parameters) {
			
		}
	}
}