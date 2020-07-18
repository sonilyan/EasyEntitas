using System.Collections.Generic;

namespace Entitas {

	/// Implement this interface if you want to create a component which
	/// you can add to an entity.
	public interface IComponentBase {
	}

	public interface IComponent : IComponentBase {
	}

	public interface IUniqueComponent : IComponentBase {
	}

	public interface ILookupComponent<V> : IComponent {
		V GetLookupValue();
	}

	public abstract class ILookupComponent<C, V> : ILookupComponent<V> where C : ILookupComponent<C,V> {
		public static void InitEntityIndex(Context context) {
			var tmp = Matcher.AllOf(ComponentIndex<C>.FindIn(context)) as Matcher;
			tmp.componentNames = context.contextInfo.componentNames;
			context.AddEntityIndex(new EntityIndex<V>(
				typeof(C).Name,
				context.GetGroup(tmp),
				(e, c) => ((C) c).GetLookupValue()));
		}

		public static HashSet<Entity> GetEntitiesWithValue(Context context, V value) {
			return ((EntityIndex<V>)context.GetEntityIndex(typeof(C).Name)).GetEntities(value);
		}

		public abstract V GetLookupValue();
	}
}
