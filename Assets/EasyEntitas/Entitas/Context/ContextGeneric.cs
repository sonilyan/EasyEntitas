
namespace Entitas
{
	public static class Context<C> where C : ContextAttribute
	{
		public static Context Instance { get { return Contexts.Get<C>(); } }

		public static IGroup AllOf<T1>() where T1 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1>()); }
		public static IGroup AllOf<T1, T2>() where T1 : IComponentBase where T2 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1, T2>()); }
		public static IGroup AllOf<T1, T2, T3>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1, T2, T3>()); }
		public static IGroup AllOf<T1, T2, T3, T4>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1, T2, T3, T4>()); }
		public static IGroup AllOf<T1, T2, T3, T4, T5>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase where T5 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1, T2, T3, T4, T5>()); }
		public static IGroup AllOf<T1, T2, T3, T4, T5, T6>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase where T5 : IComponentBase where T6 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AllOf<T1, T2, T3, T4, T5, T6>()); }

		public static IGroup AnyOf<T1>() where T1 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1>()); }
		public static IGroup AnyOf<T1, T2>() where T1 : IComponentBase where T2 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1, T2>()); }
		public static IGroup AnyOf<T1, T2, T3>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1, T2, T3>()); }
		public static IGroup AnyOf<T1, T2, T3, T4>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1, T2, T3, T4>()); }
		public static IGroup AnyOf<T1, T2, T3, T4, T5>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase where T5 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1, T2, T3, T4, T5>()); }
		public static IGroup AnyOf<T1, T2, T3, T4, T5, T6>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase where T4 : IComponentBase where T5 : IComponentBase where T6 : IComponentBase
		{ return Instance.GetGroup(Matcher<C>.AnyOf<T1, T2, T3, T4, T5, T6>()); }
	}
}
