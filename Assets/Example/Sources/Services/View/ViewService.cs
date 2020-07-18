using UnityEngine;
using Entitas;
using System;

public class ViewService : Context.IListener<AssetComponent>,
	Context.IRemovedListener<AssetComponent> {

    public static ViewService singleton = new ViewService();

    Transform _parent;

    public void Initialize(Transform parent) {
        _parent = parent;
	    Contexts.Get<Game>().AddListener<AssetComponent>(this);
	    Contexts.Get<Game>().AddRemovedListener<AssetComponent>(this);
    }

	public void OnAdd(Entity entity, AssetComponent component) {
		var prefab = Resources.Load<GameObject>(component.value);
		var view = UnityEngine.Object.Instantiate(prefab, _parent).GetComponent<IView>();
		view.Link(entity, Contexts.Get<Game>());
	}

	public void OnRemove(Entity entity, Type type) {
	}
}
