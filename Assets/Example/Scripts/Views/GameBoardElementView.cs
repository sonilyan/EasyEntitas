using DG.Tweening;
using UnityEngine;
using Entitas;

public class GameBoardElementView : View {

    public SpriteRenderer sprite;
    public float destroyDuration;

	public override void OnAdd(Entity entity, PositionComponent component) {
        var isTopRow = component.value.y == Contexts.Get<Game>().Get<GameBoardComponent>().rows - 1;
        if (isTopRow) {
            transform.localPosition = new Vector3(component.value.x, component.value.y + 1);
        }

        transform.DOMove(new Vector3(component.value.x, component.value.y, 0f), 0.3f);
	}

	public override void OnAdd(Entity entity, DestroyedComponent component) {
		var color = sprite.color;
        color.a = 0f;
        sprite.material.DOColor(color, destroyDuration);
        gameObject.transform
            .DOScale(Vector3.one * 1.5f, destroyDuration)
            .OnComplete(destroy);
	}
}
