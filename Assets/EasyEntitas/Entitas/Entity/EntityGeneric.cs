
using System;

namespace Entitas
{
	/// using ComponentIndex<T> to Component->Index mapping
	public partial class Entity {
		/// add a new Component, return the old one if exists
		public void Add<T>(Action<T> setValue = null, bool useExisted = true) where T : IComponent, new() {
			T component;

			int index = ComponentIndex<T>.FindIn(_contextInfo);

			if (useExisted && HasComponent(index)) {
				component = (T)GetComponent(index);
				ReplaceComponent(index, component, () => {
					setValue?.Invoke(component);
				});
			}
			else {
				component = CreateComponent<T>(index);
				setValue?.Invoke(component);
				AddComponent(index, component);
			}
		}

		/// replace Component with a NEW one
		public void Replace<T>(Action<T> setValue = null) where T : IComponent, new() {
			int index = ComponentIndex<T>.FindIn(_contextInfo);

			T component = (T)GetComponent(index);
			if (component == null) {
				component = CreateComponent<T>(index);
			}

			ReplaceComponent(index, component, () => {
				setValue?.Invoke(component);
			});
		}

		public void Remove<T>(bool ignoreNotFound = true) where T : IComponent {
			int index = ComponentIndex<T>.FindIn(_contextInfo);
			if (ignoreNotFound && !HasComponent(index))
				return;

			RemoveComponent(index);
		}

		public bool Has<T>() where T : IComponentBase {
			int index = ComponentIndex<T>.FindIn(_contextInfo);
			return HasComponent(index);
		}

		public int Index<T>() where T : IComponent {
			return ComponentIndex<T>.FindIn(_contextInfo);
		}

		public T Get<T>() where T : IComponentBase {
			int index = ComponentIndex<T>.FindIn(_contextInfo);
			return (T)GetComponent(index);
		}

		public void Remove(int index) {
			if (!HasComponent(index))
				return;

			RemoveComponent(index);
		}
	}
}
