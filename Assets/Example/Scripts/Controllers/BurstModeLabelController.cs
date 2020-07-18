using UnityEngine;
using UnityEngine.UI;
using Entitas;
using System;

public class BurstModeLabelController : MonoBehaviour,
	Context.IListener<BurstModeComponent> { 

	public Text label;

	string _text;

	void Awake() {
		_text = label.text;
	}

	void Start() {
		Context input = Contexts.Get<Input>();

		input.Add<BurstModeComponent>();
		input.AddListener<BurstModeComponent>(this);

		if (input.Get<BurstModeComponent>().value != false) {
			label.text = _text + ": on";
		}
		else {
			label.text = _text + ": off";
		}
	}

	public void OnAdd(Entity entity, BurstModeComponent component) {
		if (component.value == true) {
			label.text = _text + ": on";
		}
		else {
			label.text = _text + ": off";
		}
	}
}
