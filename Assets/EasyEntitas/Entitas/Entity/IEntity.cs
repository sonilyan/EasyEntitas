using System;
using System.Collections.Generic;

namespace Entitas {

    public delegate void EntityComponentChanged(
        IEntity entity, int index, IComponentBase component
    );

    public delegate void EntityComponentReplaced(
        IEntity entity, int index, IComponentBase previousComponent, IComponentBase newComponent
    );

	public delegate void EntityComponentBeforeUpdated(
		IEntity entity, int index, IComponentBase component
	);

	public delegate void EntityEvent(IEntity entity);

    public interface IEntity : IAERC {

        event EntityComponentChanged OnComponentAdded;
        event EntityComponentChanged OnComponentRemoved;
        event EntityComponentReplaced OnComponentReplaced;
        event EntityEvent OnEntityReleased;
        event EntityEvent OnDestroyEntity;

        int totalComponents { get; }
        int creationIndex { get; }
        bool isEnabled { get; }

        Stack<IComponentBase>[] componentPools { get; }
        ContextInfo contextInfo { get; }
        IAERC aerc { get; }

        void Initialize(int creationIndex,
                        int totalComponents,
                        Stack<IComponentBase>[] componentPools,
                        ContextInfo contextInfo = null,
                        IAERC aerc = null);

		void Reactivate(int creationIndex);

        void AddComponent(int index, IComponentBase component);
        void RemoveComponent(int index);
	    void ReplaceComponent(int index, IComponentBase component, Action invokeReplace);

        IComponentBase GetComponent(int index);
        IComponentBase[] GetComponents();
        int[] GetComponentIndices();

        bool HasComponent(int index);
        bool HasComponents(int[] indices);
        bool HasAnyComponent(int[] indices);

        void RemoveAllComponents();

        Stack<IComponentBase> GetComponentPool(int index);
        IComponentBase CreateComponent(int index, Type type);
        T CreateComponent<T>(int index) where T : new();

        void Destroy();
        void InternalDestroy();
        void RemoveAllOnEntityReleasedHandlers();
    }
}
