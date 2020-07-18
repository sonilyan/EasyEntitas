using Entitas;
using UnityEngine;
using Entitas.VisualDebugging.Unity;
using System;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    public Services services = Services.singleton;

	private Systems _feature;

	void Awake() {
        var contexts = Contexts.sharedInstance;
#if UNITY_EDITOR
		ContextObserverHelper.ObserveAll(contexts);
#endif
		services.Initialize(contexts, this);

		_feature = new GameSystems();
		_feature.Initialize();
    }

    void Update() {
		_feature.Execute();
		_feature.Cleanup();
	}


    void OnDestroy() {
		_feature.TearDown();
    }

	private void OnEnable() {
	}
}


