using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using State;
using State.HUD;

public class GameFacade : MonoBehaviour {
	
	static GameFacade _instance;
	static public GameFacade instance {
		get {
			return _instance;
		}
	}
	
	public bool gameStarted;
	
	public float waypointPrecision = 30f;
	int defenderRequestCount = 0;
	ILifecycleStrategy lifecycle;
	TerrainFacade _terrain;
	public TerrainFacade terrain {
		get {
			return _terrain;
		}
	}
	bool _pause = false;
	public bool pause {
		get {
			return _pause;
		}
		set {
			_pause = value;
		Debug.Log (Time.timeScale);
			Time.timeScale = _pause ? 0 : 1;
		}
	}
	
	protected LinkedList<DefenderController> _defenders;
	LinkedList<ColonistController> _colonists;
	LinkedList<LanderController> _landers;
	public IEnumerable<DefenderController> defenders {
		get {
			foreach (DefenderController defender in _defenders)
				if (!defender.dead)
					yield return defender;
		}
	}
	public DefenderController anyDefender {
		get {
			int activeDefenderCount = 0;
			
			foreach (DefenderController defender in _defenders)
				if (!defender.dead)
					activeDefenderCount++;
			
			if (activeDefenderCount != 0) {
				int defenderRequestValue = defenderRequestCount % activeDefenderCount;
				int j = 0;
				
				foreach (UnityEngine.Object obj in _defenders) {
					DefenderController defender = ((MonoBehaviour) obj).GetComponent<DefenderController>();
					
					if (!defender.dead && j++ == defenderRequestValue) {
						defenderRequestCount = j;
						return defender;
					}
				}
			}
			
			return null;
		}
	}
	public IEnumerable<ColonistController> colonists {
		get {
			foreach (ColonistController colonist in _colonists)
				if (!colonist.dead)
					yield return colonist;
		}
	}
	public IEnumerable<LanderController> landers {
		get {
			foreach (LanderController lander in _landers)
				if (!lander.dead)
					yield return lander;
		}
	}
	public IEnumerable<AbstractCharacterController> allActiveCharacters {
		get {
			foreach (UnityEngine.Object obj in FindSceneObjectsOfType(typeof(AbstractCharacterController))) {
				AbstractCharacterController character = ((MonoBehaviour) obj).GetComponent<AbstractCharacterController>();
				
				if (character.gameObject.activeSelf)
					yield return character;
			}
		}
	}
	
	Dictionary<Type, HashSet<Type>> enemyTypes = new Dictionary<Type, HashSet<Type>>();
	Dictionary<AbstractCharacterController, HashSet<AbstractCharacterController>> subscriptionMap = new Dictionary<AbstractCharacterController, HashSet<AbstractCharacterController>>();
	protected GameLogic gameLogic;
	protected DefenderController player;
	
	private InGameMenu inGameMenu;
	
	void Start() {
		_instance = this;
		_terrain = new TerrainFacade();
		lifecycle = CreateLifecycleStrategy();
		gameLogic = GetComponent<GameLogic>();
		gameLogic.gameFacade = this;
		inGameMenu = GetComponent<InGameMenu>();
		
		gameLogic.ConfigureEnemyTypes(enemyTypes);
		
		InitializeGame();
	}
	
	protected void Update(){
		
		if(gameStarted && Input.GetKeyDown(KeyCode.Escape))
			inGameMenu.ToggleVisibility();
	}
	
	protected virtual ILifecycleStrategy CreateLifecycleStrategy() {
		return new LocalLifecycleStrategy();
	}
	
	protected virtual void InitializeGame() {
		gameLogic.InstantiateAI();
		gameLogic.InstantiatePlayer();
		
		StartGame();
	}
	
	public virtual void StartGame() {
		CreateCharacterIndex();
		
		foreach (AbstractCharacterController character in allActiveCharacters)
			if (lifecycle.IsMine(character.gameObject))
				character.Spawn();
		
		if (lifecycle.IsMine(gameObject))
			gameLogic.StartGame();
		
		gameLogic.OnGameStarted();
	}
	
	protected void CreateCharacterIndex() {
		_defenders = new LinkedList<DefenderController>();
		_colonists = new LinkedList<ColonistController>();
		_landers = new LinkedList<LanderController>();
		
		foreach (AbstractCharacterController character in allActiveCharacters) {
			if (character.GetType() == typeof(DefenderController))
				_defenders.AddLast((DefenderController) character);
			else if (character.GetType() == typeof(ColonistController))
				_colonists.AddLast((ColonistController) character);
			else if (character.GetType() == typeof(LanderController))
				_landers.AddLast((LanderController) character);
		}
	}
	
	public bool IsEnemy(AbstractCharacterController character, AbstractCharacterController enemy) {
		HashSet<Type> enemies;
		return enemyTypes.TryGetValue(character.GetType(), out enemies) && enemies.Contains(enemy.GetType());
	}
	
	public bool ExistsActiveDefender() {
		return ExistsActiveCharacterOfType(typeof(DefenderController));
	}
	
	public bool ExistsActiveEnemyAI() {
		return ExistsActiveCharacterOfType(typeof(LanderController)) ||
				ExistsActiveCharacterOfType(typeof(SaucererController)) ||
				ExistsActiveCharacterOfType(typeof(MutantController));
	}
	
	bool ExistsActiveCharacterOfType(Type enemyType) {
		foreach (UnityEngine.Object o in FindSceneObjectsOfType(enemyType))
			if (((MonoBehaviour) o).gameObject.activeSelf)
				return true;
		
		return false;
	}
	
	public IState GetDefaultHUD(DefenderController defender) {
		if (GetComponent<GameLogic>().IsRadarEnabled())
			return new DelegatingState(new SpaceshipHUD(defender), new RadarHUD(defender));
		else
			return new SpaceshipHUD(defender);
	}
	
	public GameObject InstantiateObject(GameObject tpl) {
		return InstantiateObject(tpl, Vector3.zero, Quaternion.Euler(Vector3.zero));
	}
	
	public GameObject InstantiateObject(GameObject tpl, Vector3 position, Quaternion rotation) {
		GameObject instance = lifecycle.Instantiate(tpl, position, rotation);
		AbstractCharacterController ctrl = instance.GetComponent<AbstractCharacterController>();
		
		if (ctrl != null) {
			ctrl.enabled = true;
			ctrl.Init(this);
		}
		
		return instance;
	}
	
	public void Hit(AbstractCharacterController shooter, AbstractCharacterController victim) {
		lifecycle.Hit(shooter, victim);
	}
	
	[RPC] // called on server
	public void CheckHitServerside(NetworkViewID shooterId, NetworkViewID victimId) {
		Hit(NetworkView.Find(shooterId).gameObject.GetComponent<AbstractCharacterController>(), NetworkView.Find(victimId).gameObject.GetComponent<AbstractCharacterController>());
	}
	
	[RPC] // broadcast
	public void VictimHit(NetworkViewID victimId) {
		AbstractCharacterController victim = NetworkView.Find(victimId).gameObject.GetComponent<AbstractCharacterController>();
		
		victim.SetDead(true);
		
		if (lifecycle.IsMine(victim.gameObject))
			victim.DieLater();
	}
	
	public void Call(MonoBehaviour behaviour, string methodName, params object[] parameters) {
		lifecycle.Call(behaviour, methodName, parameters);
	}
	
	public virtual void CallBuffered(MonoBehaviour behaviour, string methodName, params object[] parameters) {
		behaviour.GetType().GetMethod(methodName).Invoke(behaviour, parameters);
	}
	
	public virtual void CallOnServer(MonoBehaviour behaviour, string methodName, params object[] parameters) {
		behaviour.GetType().GetMethod(methodName).Invoke(behaviour, parameters);
	}
	
	protected HashSet<AbstractCharacterController> GetSubscriptions(AbstractCharacterController character) {
		HashSet<AbstractCharacterController> subscriptions;
		
		if (!subscriptionMap.TryGetValue(character, out subscriptions))
			subscriptionMap[character] = subscriptions = new HashSet<AbstractCharacterController>();
		
		return subscriptions;
	}
	
	public void Die(AbstractCharacterController character) {
		if (subscriptionMap.ContainsKey(character)) {
			foreach (AbstractCharacterController ctrl in subscriptionMap[character])
				// notify subscribers
				ctrl.UpdateAssociation();
			
			// remove all subscriptions for character
			subscriptionMap.Remove(character);
		}
		
		// remove all subscriptions from character
		foreach (HashSet<AbstractCharacterController> subscriptions in subscriptionMap.Values)
			subscriptions.Remove(character);
	}
	
	public void Subscribe(AbstractCharacterController listener, AbstractCharacterController target) {
		GetSubscriptions(target).Add(listener);
	}
	
	public void Unsubscribe(AbstractCharacterController listener, AbstractCharacterController target) {
		GetSubscriptions(target).Remove(listener);
	}
	
	public virtual void AttachColonist(ColonistController colonist, TransportingCharacterController transporter, bool isDefender) {
		if (!isDefender || colonist.attachable) {
			if (colonist.attachedTo != null)
				DetachColonist(colonist);
			
			colonist.AttachTo(transporter, isDefender);
			transporter.OnAttach(colonist);
		}
	}
	
	public virtual void DetachColonist(ColonistController colonist) {
		Call(colonist.attachedTo, "OnDetach");
		colonist.Detach();
	}
	
	public virtual void FinishGame(bool won, string reason) {
		SetAllObjectsInactive();
		gameLogic.OnLocalGameFinished();
	}
	
	protected void SetAllObjectsInactive() {
		foreach (AbstractCharacterController o in allActiveCharacters)
			o.gameObject.SetActive(false);
		
		foreach (UnityEngine.Object o in FindSceneObjectsOfType(typeof(LaserController)))
			((MonoBehaviour) o).gameObject.SetActive(false);
	}
}