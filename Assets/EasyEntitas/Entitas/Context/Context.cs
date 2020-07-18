﻿using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas {

	/// A context manages the lifecycle of entities and groups.
	/// You can create and destroy entities and get groups of entities.
	/// The prefered way to create a context is to use the generated methods
	/// from the code generator, e.g. var context = new Game();
	public partial class Context : IContext
	{
		public byte ContextIndex;

		public Dictionary<Type,ReactiveSystem> ListenerSystems = new Dictionary<Type, ReactiveSystem>();
		public Dictionary<Type,ReactiveSystem> RemoveListenerSystems = new Dictionary<Type, ReactiveSystem>();

		public Dictionary<Type,ReactiveSystem> ContextListenerSystems = new Dictionary<Type, ReactiveSystem>();
		public Dictionary<Type,ReactiveSystem> ContextRemoveListenerSystems = new Dictionary<Type, ReactiveSystem>();
		/// Occurs when an entity gets created.
		public event ContextEntityChanged OnEntityCreated;

		/// Occurs when an entity will be destroyed.
		public event ContextEntityChanged OnEntityWillBeDestroyed;

		/// Occurs when an entity got destroyed.
		public event ContextEntityChanged OnEntityDestroyed;

		/// Occurs when a group gets created for the first time.
		public event ContextGroupChanged OnGroupCreated;

		/// The total amount of components an entity can possibly have.
		/// This value is generated by the code generator,
		/// e.g ComponentLookup.TotalComponents.
		public int totalComponents { get { return _totalComponents; } }

		/// Returns all componentPools. componentPools is used to reuse
		/// removed components.
		/// Removed components will be pushed to the componentPool.
		/// Use entity.CreateComponent(index, type) to get a new or reusable
		/// component from the componentPool.
		public Stack<IComponentBase>[] componentPools { get { return _componentPools; } }

		/// The contextInfo contains information about the context.
		/// It's used to provide better error messages.
		public ContextInfo contextInfo { get { return _contextInfo; } }

		/// Returns the number of entities in the context.
		public int count { get { return _entities.Count; } }

		/// Returns the number of entities in the internal ObjectPool
		/// for entities which can be reused.
		public int reusableEntitiesCount { get { return _reusableEntities.Count; } }

		/// Returns the number of entities that are currently retained by
		/// other objects (e.g. Group, Collector, ReactiveSystem).
		public int retainedEntitiesCount { get { return _retainedEntities.Count; } }

		readonly int _totalComponents;

		readonly Stack<IComponentBase>[] _componentPools;
		readonly ContextInfo _contextInfo;
		readonly Func<IEntity, IAERC> _aercFactory;

		readonly HashSet<Entity> _entities = new HashSet<Entity>(EntityEqualityComparer.comparer);
		readonly Stack<Entity> _reusableEntities = new Stack<Entity>();
		readonly HashSet<Entity> _retainedEntities = new HashSet<Entity>(EntityEqualityComparer.comparer);

		readonly Dictionary<string, IEntityIndex> _entityIndices;
		readonly Dictionary<IMatcher, IGroup> _groups = new Dictionary<IMatcher, IGroup>();
		readonly List<IGroup>[] _groupsForIndex;
		readonly IGroup[] _groupForSingle;
		readonly ObjectPool<List<GroupChanged>> _groupChangedListPool;
		readonly Dictionary<int, Entity> _entitiesLookup = new Dictionary<int, Entity>();

		int _creationIndex;
		Entity[] _entitiesCache;

		// Cache delegates to avoid gc allocations
		EntityComponentChanged _cachedEntityChanged;
		EntityComponentReplaced _cachedComponentReplaced;
		EntityComponentBeforeUpdated _cachedComponentBeforeUpdated;
		EntityEvent _cachedEntityReleased;
		EntityEvent _cachedDestroyEntity;

		/// The prefered way to create a context is to use the generated methods
		/// from the code generator, e.g. var context = new Game();
		public Context(int totalComponents) : this(totalComponents, 0, null, null)
		{
		}

		// TODO Obsolete since 0.41.0, April 2017
		[Obsolete("Migration Support for 0.41.0. Please use new Context(totalComponents, startCreationIndex, contextInfo, aercFactory)")]
		public Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo)
			: this(totalComponents,
				   startCreationIndex,
				   contextInfo,
				   (entity) => new SafeAERC(entity))
		{ }

		/// The prefered way to create a context is to use the generated methods
		/// from the code generator, e.g. var context = new Game();
		public Context(int totalComponents, int startCreationIndex, ContextInfo contextInfo, Func<IEntity, IAERC> aercFactory) 
		{
			if (contextInfo != null)
			{
				_contextInfo = contextInfo;
				if (contextInfo.componentNames.Length != totalComponents)
				{
					throw new ContextInfoException(this, contextInfo);
				}
			}
			else
			{
				_contextInfo = createDefaultContextInfo();
			}

			totalComponents = totalComponents * 3;

			_totalComponents = totalComponents;
			_creationIndex = startCreationIndex;

			_aercFactory = aercFactory == null
				? (entity) => new SafeAERC(entity)
				: aercFactory;

			_groupsForIndex = new List<IGroup>[totalComponents];
			_groupForSingle = new IGroup[totalComponents];
			_componentPools = new Stack<IComponentBase>[totalComponents];
			_entityIndices = new Dictionary<string, IEntityIndex>();

			_groupChangedListPool = new ObjectPool<List<GroupChanged>>(
										() => new List<GroupChanged>(),
										list => list.Clear()

									);

			// Cache delegates to avoid gc allocations
			_cachedEntityChanged = updateGroupsComponentAddedOrRemoved;
			_cachedComponentReplaced = updateGroupsComponentReplaced;
			_cachedComponentBeforeUpdated = updateGroupsComponentBeforeReplaced;
			_cachedEntityReleased = onEntityReleased;
			_cachedDestroyEntity = onDestroyEntity;

			// Add listener for updating lookup
			OnEntityCreated += (c, entity) => _entitiesLookup.Add(entity.creationIndex, (Entity)entity);
			OnEntityDestroyed += (c, entity) => _entitiesLookup.Remove(entity.creationIndex);
		}

		ContextInfo createDefaultContextInfo()
		{
			var componentNames = new string[_totalComponents];
			const string prefix = "Index ";
			for (int i = 0; i < componentNames.Length; i++)
			{
				componentNames[i] = prefix + i;
			}

			return new ContextInfo("Unnamed Context", componentNames, null);
		}

		/// Creates a new entity or gets a reusable entity from the
		/// internal ObjectPool for entities.
		public Entity CreateEntity(int? Index = null)
		{
			Entity entity;
			int entity_index;

			if (Index != null) {
				entity_index = Index.Value;
				if (Index >= _creationIndex) {
					_creationIndex = Index.Value + 1;
				}
			}
			else {
				entity_index = _creationIndex++;
			}

			if (_reusableEntities.Count > 0)
			{
				entity = _reusableEntities.Pop();
				entity.Reactivate(entity_index);
			}
			else
			{
				entity = (Entity)Activator.CreateInstance(typeof(Entity));
				entity.Initialize(entity_index, _totalComponents, _componentPools, _contextInfo, _aercFactory(entity));
			}

			_entities.Add(entity);
			entity.Retain(this);
			_entitiesCache = null;
			entity.OnComponentAdded += _cachedEntityChanged;
			entity.OnComponentRemoved += _cachedEntityChanged;
			entity.OnComponentReplaced += _cachedComponentReplaced;
			entity.OnComponentBeforeUpdated += _cachedComponentBeforeUpdated;
			entity.OnEntityReleased += _cachedEntityReleased;
			entity.OnDestroyEntity += _cachedDestroyEntity;

			if (OnEntityCreated != null)
			{
				OnEntityCreated(this, entity);
			}

			entity.SetContext(this);

			return entity;
		}

		public Entity CreateEntity(string name)
		{
			Entity entity = CreateEntity();
			entity.name = name;
			return entity;
		}

		/// Destroys the entity, removes all its components and pushs it back
		/// to the internal ObjectPool for entities.
		private void DestroyEntity(Entity entity)
		{
			var removed = _entities.Remove(entity);
			if (!removed)
			{
				throw new ContextDoesNotContainEntityException(
					"'" + this + "' cannot destroy " + entity + "!",
					"This cannot happen!?!"
				);
			}
			_entitiesCache = null;

			if (OnEntityWillBeDestroyed != null)
			{
				OnEntityWillBeDestroyed(this, entity);
			}

			entity.InternalDestroy();

			if (OnEntityDestroyed != null)
			{
				OnEntityDestroyed(this, entity);
			}

			if (entity.retainCount == 1)
			{
				// Can be released immediately without
				// adding to _retainedEntities
				entity.OnEntityReleased -= _cachedEntityReleased;
				_reusableEntities.Push(entity);
				entity.Release(this);
				entity.RemoveAllOnEntityReleasedHandlers();
			}
			else
			{
				_retainedEntities.Add(entity);
				entity.Release(this);
			}
		}

		/// Destroys all entities in the context.
		/// Throws an exception if there are still retained entities.
		public void DestroyAllEntities()
		{
			var entities = GetEntities();
			for (int i = 0; i < entities.Length; i++)
			{
				entities[i].Destroy();
			}

			_entities.Clear();
			_entitiesLookup.Clear();

			if (_retainedEntities.Count != 0)
			{
				throw new ContextStillHasRetainedEntitiesException(this, _retainedEntities.ToArray());
			}
		}

		/// Determines whether the context has the specified entity.
		public bool HasEntity(Entity entity)
		{
			return _entities.Contains(entity);
		}

		/// Returns all entities which are currently in the context.
		public Entity[] GetEntities()
		{
			if (_entitiesCache == null)
			{
				_entitiesCache = new Entity[_entities.Count];
				_entities.CopyTo(_entitiesCache);
			}

			return _entitiesCache;
		}

		/// Returns a group for the specified matcher.
		/// Calling context.GetGroup(matcher) with the same matcher will always
		/// return the same instance of the group.
		public IGroup GetGroup(IMatcher matcher)
		{
			IGroup group;
			if (!_groups.TryGetValue(matcher, out group))
			{
				group = new Group(matcher);
				var entities = GetEntities();
				for (int i = 0; i < entities.Length; i++)
				{
					group.HandleEntitySilently(entities[i]);
				}
				_groups.Add(matcher, group);

				for (int i = 0; i < matcher.indices.Length; i++)
				{
					var index = matcher.indices[i];
					if (_groupsForIndex[index] == null)
					{
						_groupsForIndex[index] = new List<IGroup>();
					}
					_groupsForIndex[index].Add(group);
				}

				if (OnGroupCreated != null)
				{
					OnGroupCreated(this, group);
				}
			}

			return group;
		}

		/// Adds the IEntityIndex for the specified name.
		/// There can only be one IEntityIndex per name.
		public void AddEntityIndex(IEntityIndex entityIndex)
		{
			if (_entityIndices.ContainsKey(entityIndex.name))
			{
				throw new ContextEntityIndexDoesAlreadyExistException(this, entityIndex.name);
			}

			_entityIndices.Add(entityIndex.name, entityIndex);
		}

		/// Gets the IEntityIndex for the specified name.
		public IEntityIndex GetEntityIndex(string name)
		{
			IEntityIndex entityIndex;
			if (!_entityIndices.TryGetValue(name, out entityIndex))
			{
				throw new ContextEntityIndexDoesNotExistException(this, name);
			}

			return entityIndex;
		}

		/// Resets the creationIndex back to 0.
		public void ResetCreationIndex()
		{
			_creationIndex = 0;
		}

		/// Clears the componentPool at the specified index.
		public void ClearComponentPool(int index)
		{
			var componentPool = _componentPools[index];
			if (componentPool != null)
			{
				componentPool.Clear();
			}
		}

		/// Clears all componentPools.
		public void ClearComponentPools()
		{
			for (int i = 0; i < _componentPools.Length; i++)
			{
				ClearComponentPool(i);
			}
		}

		/// Resets the context (destroys all entities and
		/// resets creationIndex back to 0).
		public void Reset()
		{
			DestroyAllEntities();
			ResetCreationIndex();

			OnEntityCreated = null;
			OnEntityWillBeDestroyed = null;
			OnEntityDestroyed = null;
			OnGroupCreated = null;
		}

		public override string ToString()
		{
			return _contextInfo.name;
		}

		void updateGroupsComponentAddedOrRemoved(IEntity entity, int index, IComponentBase component)
		{
			var groups = _groupsForIndex[index];
			if (groups != null)
			{
				var events = _groupChangedListPool.Get();

				var tEntity = (Entity)entity;

				for (int i = 0; i < groups.Count; i++)
				{
					events.Add(groups[i].HandleEntity(tEntity));
				}

				for (int i = 0; i < events.Count; i++)
				{
					var groupChangedEvent = events[i];
					if (groupChangedEvent != null)
					{
						groupChangedEvent(
							groups[i], tEntity, index, component
						);
					}
				}

				_groupChangedListPool.Push(events);
			}
		}

		void updateGroupsComponentBeforeReplaced(IEntity entity, int index, IComponentBase component) {
			var groups = _groupsForIndex[index];
			if (groups != null) {

				var tEntity = (Entity)entity;

				for (int i = 0; i < groups.Count; i++) {
					groups[i].BeforeUpdateEntity(tEntity, index, component);
				}
			}
		}

		void updateGroupsComponentReplaced(IEntity entity, int index, IComponentBase previousComponent, IComponentBase newComponent)
		{
			var groups = _groupsForIndex[index];
			if (groups != null)
			{

				var tEntity = (Entity)entity;

				for (int i = 0; i < groups.Count; i++)
				{
					groups[i].UpdateEntity(
						tEntity, index, previousComponent, newComponent
					);
				}
			}
		}

		void onEntityReleased(IEntity entity)
		{
			if (entity.isEnabled)
			{
				throw new EntityIsNotDestroyedException(
					"Cannot release " + entity + "!"
				);
			}
			var tEntity = (Entity)entity;
			entity.RemoveAllOnEntityReleasedHandlers();
			_retainedEntities.Remove(tEntity);
			_reusableEntities.Push(tEntity);
		}

		void onDestroyEntity(IEntity entity)
		{
			DestroyEntity((Entity)entity);
		}


		/// returns entity matching the specified creationIndex
		public Entity GetEntity(int creationIndex)
		{
			if (_entitiesLookup == null)
				return null;

			Entity entity = null;
			_entitiesLookup.TryGetValue(creationIndex, out entity);

			return entity;
		}

		/// return unique entity with specified component
		public Entity GetSingleEntity<T>() where T : IUniqueComponent {
			return GetSingleEntity(ComponentIndex<T>.FindIn(this.contextInfo));
		}

		public Entity GetSingleEntity(int componentIndex) {
			IGroup group = _groupForSingle[componentIndex];

			if (group == null) {
				var matcher = Matcher.AllOf(componentIndex) as Matcher;
				matcher.componentNames = contextInfo.componentNames;
				group = GetGroup(matcher);
				_groupForSingle[componentIndex] = group;
			}

			return group.GetSingleEntity();
		}

		public T Get<T>() where T : IUniqueComponent {
			int componentIndex = ComponentIndex<T>.FindIn(this.contextInfo);

			IComponentBase component = GetUniqueComponent(componentIndex);
			if (component == null)
				return default(T);

			return (T)component;
		}

		public T Replace<T>(Action<T> setValue = null) where T : IUniqueComponent,new() {
			int componentIndex = ComponentIndex<T>.FindIn(this.contextInfo);
			T component;
			Entity entity = GetSingleEntity(componentIndex);
			if (entity == null) {
				entity = CreateEntity();
			}

			component = (T)entity.GetComponent(componentIndex);
			if (component == null) {
				component = entity.CreateComponent<T>(componentIndex);
			}

			entity.ReplaceComponent(componentIndex, component, () => {
				setValue?.Invoke(component);
			});

			return component;
		}

		private IComponentBase GetUniqueComponent(int componentIndex) {
			Entity entity = GetSingleEntity(componentIndex);
			if (entity == null)
				return null;
			
			return entity.GetComponent(componentIndex);
		}

		public Entity Add<T>(Action<T> setValue = null) where T : IUniqueComponent, new()
		{
			int componentIndex = ComponentIndex<T>.FindIn(this.contextInfo);

			Entity entity = GetSingleEntity(componentIndex);
			if (entity == null) {
				entity = CreateEntity();
			}

			T component = entity.CreateComponent<T>(componentIndex);
			setValue?.Invoke(component);
			entity.AddComponent(componentIndex, component);

			return entity;
		}
	}
}
