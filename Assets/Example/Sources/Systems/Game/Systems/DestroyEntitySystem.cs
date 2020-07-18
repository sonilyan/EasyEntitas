using System.Collections.Generic;
using Entitas;

public sealed class DestroyEntitySystem : ICleanupSystem {
    readonly IGroup _group;
	private List<Entity> entities;

    public DestroyEntitySystem() {
        _group = Contexts.Get<Game>().GetGroup(Matcher<Game>.AllOf<DestroyedComponent>());
		entities = new List<Entity>();
	}

    public void Cleanup() {
        foreach (var e in _group.GetEntities(entities)) {
	        e.Destroy();
        }
    }
}
