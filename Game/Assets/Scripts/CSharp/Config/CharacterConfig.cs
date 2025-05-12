using UnityEngine;
using System.Collections;

namespace Config {
	public class CharacterConfig {
		
		private Transform _template;
		public Transform template {
			get {
				return _template;
			}
		}
		private int _totalAmount;
		public int totalAmount {
			get {
				return _totalAmount;
			}
		}
		private int _maxAmount = 5;
		public int maxAmount {
			get {
				return _maxAmount;
			}
		}
		public int leftAmount;
		
		public CharacterConfig(Transform template, int totalAmount, int maxAmount) {
			this._template = template;
			this._totalAmount = totalAmount;
			this._maxAmount = maxAmount;
			this.leftAmount = totalAmount;
		}
		
		public CharacterConfig(Transform template, int totalAmount) {
			this._template = template;
			this._totalAmount = totalAmount;
			this._maxAmount = totalAmount;
			this.leftAmount = totalAmount;
		}
	}
}