using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitas {
	public class ListenerSystem : Systems {
		static public readonly ListenerSystem Instance = new ListenerSystem();
		static public void AddListenerSystem(ISystem system) {
			Instance.Add(system);
		}

		private ListenerSystem() {

		}
	}
}
