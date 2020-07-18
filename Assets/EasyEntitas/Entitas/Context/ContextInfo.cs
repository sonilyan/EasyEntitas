using System;
using UnityEngine;

namespace Entitas {

    public class ContextInfo {

        public readonly string name;
        public readonly string[] componentNames;
        public readonly Type[] componentTypes;

        public ContextInfo(string name, string[] componentNames, Type[] componentTypes) {
            this.name = name;
            this.componentNames = componentNames;
            this.componentTypes = componentTypes;
        }

		public ContextInfo(string name, Type[] componentTypes) {
			int count = componentTypes.Length;
			string[] componentNames = new string[count];

			for (int i = 0; i < count; i++) {
				componentNames[i] = componentTypes[i].Name.RemoveComponentSuffix();
			}

			this.name = name;
			this.componentNames = componentNames;
			this.componentTypes = componentTypes;
		}

		internal int Find<T>() where T : IComponentBase {
			var type = typeof(T);
			if (type.IsGenericType) {
				var argtype = type.GetGenericArguments()[0];
				var index = Array.IndexOf(componentTypes, argtype);

				if (index < 0) {
					return index;
				}
				else if (type.Name == typeof(Entity.AddListenerComponent<>).Name) {
					return index + componentTypes.Length;
				}
				else if (type.Name == typeof(Entity.RemovedListenerComponent<>).Name) {
					return index + componentTypes.Length * 2;
				}
				else if (type.Name == typeof(Context.AddListenerComponent<>).Name) {
					return index + componentTypes.Length;
				}
				else if (type.Name == typeof(Context.RemovedListenerComponent<>).Name) {
					return index + componentTypes.Length * 2;
				}
				else {
					return -1;
				}
			}

			return Array.IndexOf(componentTypes, type);
		}
	}
}
