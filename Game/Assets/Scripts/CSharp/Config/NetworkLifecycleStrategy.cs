using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Config {
	public class NetworkLifecycleStrategy : ILifecycleStrategy {
	
		private GameFacade facade;
		
		public NetworkLifecycleStrategy() {
			facade = GameFacade.instance;
		}
		
		public GameObject Instantiate(GameObject tpl) {
			return (GameObject) Network.Instantiate(tpl, Vector3.zero, Quaternion.Euler(Vector3.up),0);
		}
		
		public GameObject Instantiate(GameObject tpl, Vector3 position, Quaternion rotation) {
			return (GameObject) Network.Instantiate(tpl, position, rotation,0);
		}
		
		public void Hit(AbstractCharacterController shooter, AbstractCharacterController victim) {
			if (!victim.dead) {
				if (Network.isServer) {
					bool isEnemy = GameFacade.instance.IsEnemy(shooter, victim);
					victim.SetDead(true);
					Call(shooter, "SetPoints", shooter.points + (isEnemy ? victim.pointsWorth : -victim.pointsWorth));
					Call(facade, "VictimHit", victim.networkView.viewID);
				} else
					facade.CallOnServer(facade, "CheckHitServerside", shooter.networkView.viewID, victim.networkView.viewID);
			}
		}
		
		public bool IsMine(GameObject obj) {
			return obj.networkView.isMine;
		}
		
		public void Call(MonoBehaviour behaviour, string methodName, params object[] parameters) {
			behaviour.networkView.RPC(methodName, RPCMode.All, parameters);
		}
		
		public void Call(MonoBehaviour behaviour, string methodName, RPCMode rpcMode, params object[] parameters) {
			behaviour.networkView.RPC(methodName, rpcMode, parameters);
		}
	}
}
