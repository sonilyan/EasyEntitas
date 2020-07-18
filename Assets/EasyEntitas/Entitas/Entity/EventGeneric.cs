
using System;
using System.Collections.Generic;
using Entitas.Utils;
using UnityEngine;

namespace Entitas {
	public partial class Entity {
		public interface IListener<T> where T : IComponent {
			void OnAdd(Entity entity, T component);
		}

		public interface IRemovedListener<T> where T : IComponent {
			void OnRemove(Entity entity, Type type);
		}

		internal class AddListenerComponent<T> : IComponent where T : IComponent, new() {
			public List<IListener<T>> values = new List<IListener<T>>();
			private static string _name;
			public override string ToString() {
				if (_name == null) {
					_name = $"Listener<{typeof(T).FullName.RemoveSuffix("Component")}>";
				}
				return _name;
			}
		}

		internal class RemovedListenerComponent<T> : IComponent where T : IComponent, new() {
			public List<IRemovedListener<T>> values = new List<IRemovedListener<T>>();
			private static string _name;
			public override string ToString() {
				if (_name == null) {
					_name = $"Removed<{typeof(T).FullName.RemoveSuffix("Component")}>";
				}
				return _name;
			}
		}

		void AddListenerSystem<T>(Context context) where T : IComponent, new() {
			if (!context.ListenerSystems.ContainsKey(typeof(T))) {
				var system = new ListenerSystem<T>(context);
				context.ListenerSystems.Add(typeof(T), system);
				ListenerSystem.AddListenerSystem(system);
			}
		}

		void AddRemoveListenerSystem<T>(Context context) where T : IComponent, new() {
			if (!context.RemoveListenerSystems.ContainsKey(typeof(T))) {
				var system = new RemoveListenerSystem<T>(context);
				context.RemoveListenerSystems.Add(typeof(T), system);
				ListenerSystem.AddListenerSystem(system);
			}
		}

		public void AddListener<T>(IListener<T> listener) where T : IComponent, new() {
			AddListenerSystem<T>(context);

			int index = ComponentIndex<AddListenerComponent<T>>.FindIn(_contextInfo);

			if (HasComponent(index)) {
				Replace<AddListenerComponent<T>>(component => {
					if (!component.values.Contains(listener))
						component.values.Add(listener);
				});
			}
			else {
				var component = CreateComponent<AddListenerComponent<T>>(index);
				component.values.Clear();
				component.values.Add(listener);
				AddComponent(index, component);
			}
		}

		public bool RemoveListener<T>(IListener<T> listener) where T : IComponent, new() {
			AddListenerComponent<T> component;

			int index = ComponentIndex<AddListenerComponent<T>>.FindIn(_contextInfo);

			if (!HasComponent(index)) {
				return false;
			}

			component = (AddListenerComponent<T>) GetComponent(index);
			replaceComponent(index, component);

			if (component.values.Contains(listener))
				component.values.Remove(listener);

			if (component.values.Count == 0) {
				RemoveComponent(index);
			}

			return true;
		}

		public void AddRemoveListener<T>(IRemovedListener<T> listener) where T : IComponent, new() {
			AddRemoveListenerSystem<T>(context);

			int index = ComponentIndex<RemovedListenerComponent<T>>.FindIn(_contextInfo);

			if (HasComponent(index)) {
				Replace<RemovedListenerComponent<T>>(component => {
					if (!component.values.Contains(listener))
						component.values.Add(listener);
				});
			}
			else {
				var component = CreateComponent<RemovedListenerComponent<T>>(index);
				component.values.Clear();
				component.values.Add(listener);
				AddComponent(index, component);
			}
		}

		public bool RemoveRemoveListener<T>(IRemovedListener<T> listener) where T : IComponent, new() {
			RemovedListenerComponent<T> component;

			int index = ComponentIndex<RemovedListenerComponent<T>>.FindIn(_contextInfo);

			if (!HasComponent(index)) {
				return false;
			}

			component = (RemovedListenerComponent<T>) GetComponent(index);
			replaceComponent(index, component);

			if (component.values.Contains(listener))
				component.values.Remove(listener);

			if (component.values.Count == 0) {
				RemoveComponent(index);
			}

			return true;
		}

		class ListenerSystem<T> : ReactiveSystem where T : IComponent, new() {
			public ListenerSystem(Context context) : base(context) {
			}

			protected override void Execute(List<Entity> entities) {
				foreach (var e in entities) {
					var value = e.Get<T>();
					var component = e.Get<AddListenerComponent<T>>();
					foreach (var listener in component.values) {
						listener.OnAdd(e, value);
					}
				}
			}

			protected override bool Filter(Entity entity) {
				return entity.Has<T>() && entity.Has<AddListenerComponent<T>>();
			}

			protected override ICollector GetTrigger(IContext context) {
				var Context = context as Context;
				var tmp = Matcher.AllOf(ComponentIndex<T>.FindIn(Context)) as Matcher;
				tmp.componentNames = context.contextInfo.componentNames;
				return context.CreateCollector(tmp.AddedOrUpdated());
			}
		}

		class RemoveListenerSystem<T> : ReactiveSystem where T : IComponent, new() {
			public RemoveListenerSystem(Context context) : base(context) {
			}

			protected override void Execute(List<Entity> entities) {
				foreach (var e in entities) {
					var component = e.Get<RemovedListenerComponent<T>>();
					foreach (var listener in component.values) {
						listener.OnRemove(e, typeof(T));
					}
				}
			}

			protected override bool Filter(Entity entity) {
				return !entity.Has<T>() && entity.Has<RemovedListenerComponent<T>>();
			}

			protected override ICollector GetTrigger(IContext context) {
				var Context = context as Context;
				var tmp = Matcher.AllOf(ComponentIndex<T>.FindIn(Context)) as Matcher;
				tmp.componentNames = context.contextInfo.componentNames;
				return context.CreateCollector(tmp.Removed());
			}
		}
	}
}