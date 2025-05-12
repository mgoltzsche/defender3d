using UnityEngine;

namespace State {
	
	public interface IState {
	
		void Init();
		void Update();
	}
}