using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entitas
{
	/// A singleton Contexts, with name-Context lookup, and a defaultContext
	public class Contexts : IContexts
	{
		public static int startCreationIndex = 1;
		public static bool useSafeAERC = true;

		private static Contexts _sharedInstance;

		public static Contexts sharedInstance
		{
			get
			{
				if (_sharedInstance == null)
					_sharedInstance = new Contexts();
				return _sharedInstance;
			}
		}

		private Dictionary<string, Context> _contextLookup;
		private Context[] _contextList;
		private Context _defaultContext;
		
		public IContext[] allContexts { get { return _contextList; } }
		public Context defaultContext { get { return _defaultContext; } }

		public static Context Game { get { return sharedInstance.defaultContext; } }
		public static Context Get<S>() where S:ContextAttribute { return sharedInstance._contextLookup[ContextAttribute.GetName<S>()]; }
		public static Context Get(string contextName) { return sharedInstance._contextLookup[contextName]; }
		public static Context Get(byte contextIndex) { return sharedInstance._contextList[contextIndex]; }


		public Contexts()
		{
			InitAllContexts();
		}

		public void Reset()
		{
			int count = _contextList.Length;
			for (int i = 0; i < count; i++)
			{
				_contextList[i].Reset();
			}
		}

		/// Build contexts' list and lookup according to collected Component-Types
		private void InitAllContexts()
		{
			var comps = CollectAllComponents();

			var contextList = new List<Context>();
			_contextLookup = new Dictionary<string, Context>();

			foreach (var cc in comps)
			{
				var name = cc.Key;
				var list = cc.Value.ToArray();
				var c = new Context(list.Length, startCreationIndex, new ContextInfo(name, list), GetAERC());

				InitLookupComponent(c, list);

				_contextLookup[name] = c;
				c.ContextIndex = (byte)contextList.Count;
				contextList.Add(c);
			}

			_defaultContext = _contextLookup[Entitas.Game.NAME];
			_contextList = contextList.ToArray();
		}

		private void InitLookupComponent(Context context, Type[] tlist) {
			foreach (var t in tlist) {
				if (t.BaseType.Name.StartsWith("ILookupComponent")) {
					var basetype = t.BaseType;

					MethodInfo[] a = basetype.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

					foreach (MethodInfo item in a) {
						if (item.Name == "InitEntityIndex") {
							item.Invoke(this, new object[] {context});
						}
					}
				}
			}
		}

		/// Collect all Compoent-Types in current domain
		private static Dictionary<string, List<Type>> CollectAllComponents()
		{
			var compType = typeof(IComponentBase);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => p.IsClass && p.IsPublic && !p.IsAbstract && !p.IsGenericType &&
				            compType.IsAssignableFrom(p));

			Dictionary<string, List<Type>> comps = new Dictionary<string, List<Type>>();
			comps[Entitas.Game.NAME] = new List<Type>();

			var attrType = typeof(ContextAttribute);

			foreach (var t in types)
			{
				var attribs = t.GetCustomAttributes(attrType, false);

				if (attribs == null || attribs.Length <= 0)
				{
					CollectComponent(comps, Entitas.Game.NAME, t);
				}
				else
				{
					foreach (var attr in attribs)
					{
						CollectComponent(comps, ((ContextAttribute)attr).name, t);
					}
				}
			}

			return comps;
		}

		private static void CollectComponent(Dictionary<string, List<Type>> comps, string name, Type t)
		{
			List<Type> list;
			if (!comps.TryGetValue(name, out list))
			{
				list = new List<Type>();
				comps[name] = list;
			}

			list.Add(t);
		}

		private static Func<IEntity, IAERC> GetAERC()
		{
			if (useSafeAERC)
				return (entity) => new SafeAERC(entity);
			else
				return (entity) => new UnsafeAERC();
		}

	}
}
