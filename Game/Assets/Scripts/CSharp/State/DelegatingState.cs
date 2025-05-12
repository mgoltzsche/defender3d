using UnityEngine;
using System;
using System.Collections.Generic;

namespace State {

	public class DelegatingState : IState {
		
		IState[] states;
		
		public DelegatingState(params IState[] states) {
			this.states = states;
		}
		
		public void Init() {
			foreach (IState state in states)
				state.Init();
		}
		
		public void Update() {
			foreach (IState state in states)
				state.Update();
		}
		
		public override string ToString() {
			string[] stateNames = new string[states.Length];
			
			for (int i = 0; i < states.Length; i++)
				stateNames[i] = states[i].ToString();
			
			return "DelegatingState[" + String.Join(", ", stateNames) + "]";
		}
	}
}
