using Entitas;
using Entitas.Unity;
using UnityEngine;

public class View : MonoBehaviour, IView, 
	Entity.IListener<PositionComponent>, 
	Entity.IListener<DestroyedComponent> {
	private Entity e;
	public virtual void Link(IEntity entity, IContext context) {
        gameObject.Link(entity, context);
        e = (Entity)entity;
        e.AddListener<PositionComponent>(this);
        e.AddListener<DestroyedComponent>(this);

        var pos = e.Get<PositionComponent>().value;
        transform.localPosition = new Vector3(pos.x, pos.y);
    }

    protected void destroy() {
		e.RemoveListener<PositionComponent>(this);
		e.RemoveListener<DestroyedComponent>(this);

        gameObject.Unlink();
        Destroy(gameObject);
    }

	public virtual void OnAdd(Entity entity, PositionComponent component) {
        transform.localPosition = new Vector3(component.value.x, component.value.y);
	}

	public virtual void OnAdd(Entity entity, DestroyedComponent component) {
        destroy();
	}
}
