using UnityEngine;
using UnityEngine.UI;
using Entitas;

public class ScoreLabelController : MonoBehaviour, Context.IListener<ScoreComponent> {

    public Text label;

    void Start() {
	    Contexts.Get<GameState>().AddListener<ScoreComponent>(this);
    }

	public void OnAdd(Entity entity, ScoreComponent component) {
        label.text = "Score " + component.value;
	}
}
