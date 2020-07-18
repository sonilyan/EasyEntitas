using System;

namespace Entitas {

    public abstract class AbstractEntityIndex<TKey> : IEntityIndex {

        public string name { get { return _name; } }

        protected readonly string _name;
        protected readonly IGroup _group;
        protected readonly Func<Entity, IComponentBase, TKey> _getKey;
        protected readonly Func<Entity, IComponentBase, TKey[]> _getKeys;
        protected readonly bool _isSingleKey;

        protected AbstractEntityIndex(string name, IGroup group, Func<Entity, IComponentBase, TKey> getKey) {
            _name = name;
            _group = group;
            _getKey = getKey;
            _isSingleKey = true;
        }

        protected AbstractEntityIndex(string name, IGroup group, Func<Entity, IComponentBase, TKey[]> getKeys) {
            _name = name;
            _group = group;
            _getKeys = getKeys;
            _isSingleKey = false;
        }

        public virtual void Activate() {
            _group.OnEntityAdded += onEntityAdded;
	        _group.OnEntityUpdated += onEntityUpdated;
	        _group.OnEntityBeforeUpdated += onEntityBeforeUpdated;
            _group.OnEntityRemoved += onEntityRemoved;
        }

        public virtual void Deactivate() {
            _group.OnEntityAdded -= onEntityAdded;
	        _group.OnEntityUpdated -= onEntityUpdated;
	        _group.OnEntityBeforeUpdated -= onEntityBeforeUpdated;
            _group.OnEntityRemoved -= onEntityRemoved;
            clear();
        }

        public override string ToString() {
            return name;
        }

        protected void indexEntities(IGroup group) {
            foreach (var entity in group) {
                if (_isSingleKey) {
                    addEntity(_getKey(entity, null), entity);
                } else {
                    var keys = _getKeys(entity, null);
                    for (int i = 0; i < keys.Length; i++) {
                        addEntity(keys[i], entity);
                    }
                }
            }
        }

        protected void onEntityAdded(IGroup group, Entity entity, int index, IComponentBase component) {
            if (_isSingleKey) {
                addEntity(_getKey(entity, component), entity);
            } else {
                var keys = _getKeys(entity, component);
                for (int i = 0; i < keys.Length; i++) {
                    addEntity(keys[i], entity);
                }
            }
        }

        protected void onEntityRemoved(IGroup group, Entity entity, int index, IComponentBase component) {
            if (_isSingleKey) {
                removeEntity(_getKey(entity, component), entity);
            } else {
                var keys = _getKeys(entity, component);
                for (int i = 0; i < keys.Length; i++) {
                    removeEntity(keys[i], entity);
                }
            }
        }

		protected void onEntityBeforeUpdated(IGroup group, Entity entity, int index, IComponentBase component) {
			onEntityRemoved(group, entity, index, component);
		}

		protected void onEntityUpdated(IGroup group, Entity entity, int index, IComponentBase previousComponent,
		    IComponentBase newComponent) {
			if (previousComponent != newComponent) {
				onEntityRemoved(group, entity, index, previousComponent);
			}

			onEntityAdded(group, entity, index, newComponent);
	    }

		protected abstract void addEntity(TKey key, Entity entity);

        protected abstract void removeEntity(TKey key, Entity entity);

        protected abstract void clear();

        ~AbstractEntityIndex() {
            Deactivate();
        }
    }
}
