using UnityEngine;

namespace State {

	public class Falling : IState {
		
		AbstractCharacterController ctx;
		Vector3 initPosition;
		TerrainFacade terrain;
		
		public Falling(AbstractCharacterController ctx) {
			this.ctx = ctx;
		}
		
		public void Init() {
			terrain = GameFacade.instance.terrain;
			initPosition = ctx.transform.position;
		}
		
		public void Update() {
			if (ctx.transform.position.y > terrain.yMax)
				// stop character if falls out of terrain
				ctx.rigidbody.velocity = Vector3.down * 20f;
			
			if (terrain.GetMinY(ctx.transform.position) + 15 > ctx.transform.position.y) {
				if (Vector3.Distance(initPosition, ctx.transform.position) > 200)
					// fallen to far: die
					ctx.DieLater();
				else
					// do whatever you do normally
					ctx.ResetState();
			}
		}
		
		/*public void OnCollisionEnter(Collision collision) {
			if (collision.gameObject == Terrain.activeTerrain.gameObject) {
				if (ctx.rigidbody.position.y < 50)
					ctx.ResetState();
				
				// die if collision caused too much damage
				int contactCount = 0;
				Vector3 avgNormal = Vector3.zero;
		
				foreach (ContactPoint contact in collision.contacts) {
					contactCount++;
					avgNormal += contact.normal;
					Debug.DrawRay(contact.point, contact.normal, Color.green, 2, false);
				}
		
				avgNormal /= collision.contacts.Length;
				
		 		float damage = Vector3.Dot(avgNormal, collision.relativeVelocity);
				
				if (damage > 50f)
					ctx.DieLater();
			}
		}*/
	}
}