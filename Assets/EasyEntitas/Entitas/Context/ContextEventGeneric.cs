
using System;
using System.Collections.Generic;
using Entitas.Utils;
using UnityEngine;

namespace Entitas {
	public partial class Context : IContext {
		public interface IListener<T> where T : IComponentBase {
			void OnAdd(Entity entity, T component);
		}

		public interface IRemovedListener<T> where T : IComponentBase {
			void OnRemove(Entity entity, Type type);
		}

		internal class AddListenerComponent<T> : IUniqueComponent where T : IComponentBase, new() {
			public List<IListener<T>> values = new List<IListener<T>>();
			private static string _name;
			public override string ToString() {
				if (_name == null) {
					_name = $"ContextListener<{typeof(T).FullName.RemoveSuffix("Component")}>";
				}
				return _name;
			}
		}

		internal class RemovedListenerComponent<T> : IUniqueComponent where T : IComponentBase, new() {
			public List<IRemovedListener<T>> values = new List<IRemovedListener<T>>();
			private static string _name;
			public override string ToString() {
				if (_name == null) {
					_name = $"ContextRemoved<{typeof(T).FullName.RemoveSuffix("Component")}>";
				}
				return _name;
			}
		}

		void AddListenerSystem<T>(Context context) where T : IComponentBase, new() {
			if (!context.ContextListenerSystems.ContainsKey(typeof(T))) {
				var system = new ListenerSystem<T>(context);
				context.ContextListenerSystems.Add(typeof(T), system);
				ListenerSystem.AddListenerSystem(system);
			}
		}

		void AddRemoveListenerSystem<T>(Context context) where T : IComponentBase, new() {
			if (!context.ContextRemoveListenerSystems.ContainsKey(typeof(T))) {
				var system = new RemoveListenerSystem<T>(context);
				context.ContextRemoveListenerSystems.Add(typeof(T), system);
				ListenerSystem.AddListenerSystem(system);
			}
		}

		public void AddListener<T>(IListener<T> listener) where T : IComponentBase, new() {
			AddListenerSystem<T>(this);

			int index = ComponentIndex<AddListenerComponent<T>>.FindIn(_contextInfo);

			Entity entity = GetSingleEntity(index);
			if (entity != null) {
				Replace<AddListenerComponent<T>>(component => {
					if (!component.values.Contains(listener))
						component.values.Add(listener);
				});
			}
			else {
				entity = CreateEntity();
				var component = entity.CreateComponent<AddListenerComponent<T>>(index);
				component.values.Clear();
				component.values.Add(listener);
				entity.AddComponent(index, component);
			}
		}

		public bool RemovedListener<T>(IListener<T> listener) where T : IComponentBase, new() {
			AddListenerComponent<T> component;

			int index = ComponentIndex<AddListenerComponent<T>>.FindIn(_contextInfo);

			Entity entity = GetSingleEntity(index);
			if (entity == null) {
				return false;
			}

			component = Replace<AddListenerComponent<T>>();

			if (component.values.Contains(listener))
				component.values.Remove(listener);

			if (component.values.Count == 0) {
				entity.RemoveComponent(index);
			}

			return true;
		}

		public void AddRemovedListener<T>(IRemovedListener<T> listener) where T : IComponentBase, new() {
			AddRemoveListenerSystem<T>(this);

			int index = ComponentIndex<RemovedListenerComponent<T>>.FindIn(_contextInfo);

			Entity entity = GetSingleEntity(index);
			if (entity != null) {
				Replace<RemovedListenerComponent<T>>(component => {
					if (!component.values.Contains(listener))
						component.values.Add(listener);
				});
			}
			else {
				entity = CreateEntity();
				var component = entity.CreateComponent<RemovedListenerComponent<T>>(index);
				component.values.Clear();
				component.values.Add(listener);
				entity.AddComponent(index, component);
			}
		}

		public bool RemoveRemovedListener<T>(IRemovedListener<T> listener) where T : IComponentBase, new() {
			RemovedListenerComponent<T> component;

			int index = ComponentIndex<RemovedListenerComponent<T>>.FindIn(_contextInfo);

			Entity entity = GetSingleEntity(index);
			if (entity == null) {
				return false;
			}

			component = Replace<RemovedListenerComponent<T>>();

			if (component.values.Contains(listener))
				component.values.Remove(listener);

			if (component.values.Count == 0) {
				entity.RemoveComponent(index);
			}

			return true;
		}

		public class ListenerSystem<T> : ReactiveSystem where T : IComponentBase, new() {
			private readonly Entitas.IGroup _listeners;
			public ListenerSystem(Context context) : base(context) {
				var tmp = Matcher.AllOf(ComponentIndex<AddListenerComponent<T>>.FindIn(context)) as Matcher;
				tmp.componentNames = context.contextInfo.componentNames;
				_listeners = context.GetGroup(tmp);
			}

			protected override void Execute(List<Entity> entities) {
				foreach (var e in entities) {
					var value = e.Get<T>();
					foreach (var listenerEntity in _listeners) {
						var component = listenerEntity.Get<AddListenerComponent<T>>();
						foreach (var listener in component.values) {
							listener.OnAdd(e, value);
						}
					}
				}
			}

			protected override bool Filter(Entity entity) {
				return entity.Has<T>();
			}

			protected override ICollector GetTrigger(IContext context) {
				var Context = context as Context;
				var tmp = Matcher.AllOf(ComponentIndex<T>.FindIn(Context)) as Matcher;
				tmp.componentNames = context.contextInfo.componentNames;
				return context.CreateCollector(tmp.AddedOrUpdated());
			}
		}

		public class RemoveListenerSystem<T> : ReactiveSystem where T : IComponentBase, new() {
			private readonly Entitas.IGroup _listeners;
			public RemoveListenerSystem(Context context) : base(context) {
				var tmp = Matcher.AllOf(ComponentIndex<RemovedListenerComponent<T>>.FindIn(context)) as Matcher;
				tmp.componentNames = context.contextInfo.componentNames;
				_listeners = context.GetGroup(tmp);
			}

			protected override void Execute(List<Entity> entities) {
				foreach (var e in entities) {
					foreach (var listenerEntity in _listeners) {
						var component = listenerEntity.Get<RemovedListenerComponent<T>>();
						foreach (var listener in component.values) {
							listener.OnRemove(e, typeof(T));
						}
					}
				}
			}

			protected override bool Filter(Entity entity) {
				return !entity.Has<T>();
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