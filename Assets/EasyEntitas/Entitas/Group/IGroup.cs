using System.Collections;
using System.Collections.Generic;

namespace Entitas {

    public delegate void GroupChanged(IGroup group, Entity entity, int index, IComponentBase component);
    public delegate void GroupUpdated(IGroup group, Entity entity, int index, IComponentBase previousComponent, IComponentBase newComponent);
    public delegate void GroupBeforeUpdated(IGroup group, Entity entity, int index, IComponentBase previousComponent);

    public interface IGroup : IEnumerable<Entity> {

        int count { get; }

        void RemoveAllEventHandlers();

		event GroupChanged OnEntityAdded;
		event GroupChanged OnEntityRemoved;
		event GroupUpdated OnEntityUpdated;
		event GroupBeforeUpdated OnEntityBeforeUpdated;

		IMatcher matcher { get; }

		void HandleEntitySilently(Entity entity);
		void HandleEntity(Entity entity, int index, IComponentBase component);

		GroupChanged HandleEntity(Entity entity);

		void UpdateEntity(Entity entity, int index, IComponentBase previousComponent, IComponentBase newComponent);
		void BeforeUpdateEntity(Entity entity, int index, IComponentBase component);

		bool ContainsEntity(Entity entity);

		Entity[] GetEntities();

		List<Entity> GetEntities(List<Entity> buffer);

		Entity GetSingleEntity();
	}
}
