
namespace Entitas {
	public static class Matcher<C> where C : ContextAttribute {
		public static IAllOfMatcher AllOf<T1>() where T1 : IComponentBase {
			return __Matcher<C, T1>.All();
		}

		public static IAllOfMatcher AllOf<T1, T2>() where T1 : IComponentBase where T2 : IComponentBase {
			return __Matcher<C, T1, T2>.All();
		}

		public static IAllOfMatcher AllOf<T1, T2, T3>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase {
			return __Matcher<C, T1, T2, T3>.All();
		}

		public static IAllOfMatcher AllOf<T1, T2, T3, T4>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4>.All();
		}

		public static IAllOfMatcher AllOf<T1, T2, T3, T4, T5>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4, T5>.All();
		}

		public static IAllOfMatcher AllOf<T1, T2, T3, T4, T5, T6>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase
			where T6 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4, T5, T6>.All();
		}

		public static IAnyOfMatcher AnyOf<T1>() where T1 : IComponentBase {
			return __Matcher<C, T1>.Any();
		}

		public static IAnyOfMatcher AnyOf<T1, T2>() where T1 : IComponentBase where T2 : IComponentBase {
			return __Matcher<C, T1, T2>.Any();
		}

		public static IAnyOfMatcher AnyOf<T1, T2, T3>() where T1 : IComponentBase where T2 : IComponentBase where T3 : IComponentBase {
			return __Matcher<C, T1, T2, T3>.Any();
		}

		public static IAnyOfMatcher AnyOf<T1, T2, T3, T4>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4>.Any();
		}

		public static IAnyOfMatcher AnyOf<T1, T2, T3, T4, T5>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4, T5>.Any();
		}

		public static IAnyOfMatcher AnyOf<T1, T2, T3, T4, T5, T6>() where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase
			where T6 : IComponentBase {
			return __Matcher<C, T1, T2, T3, T4, T5, T6>.Any();
		}



		class MatcherGeneric<__C> where __C : ContextAttribute {
			protected static int idx<T>() where T : IComponentBase {
				return ComponentIndex<__C, T>.value;
			}
		}

		class __Matcher<__C, T1> : MatcherGeneric<__C> where __C : ContextAttribute where T1 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}

		class __Matcher<__C, T1, T2> : MatcherGeneric<__C>
			where __C : ContextAttribute where T1 : IComponentBase where T2 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>(), idx<T2>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>(), idx<T2>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}

		class __Matcher<__C, T1, T2, T3> : MatcherGeneric<__C> where __C : ContextAttribute
			where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>(), idx<T2>(), idx<T3>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>(), idx<T2>(), idx<T3>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}

		class __Matcher<__C, T1, T2, T3, T4> : MatcherGeneric<__C> where __C : ContextAttribute
			where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}

		class __Matcher<__C, T1, T2, T3, T4, T5> : MatcherGeneric<__C> where __C : ContextAttribute
			where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>(), idx<T5>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>(), idx<T5>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}

		class __Matcher<__C, T1, T2, T3, T4, T5, T6> : MatcherGeneric<__C> where __C : ContextAttribute
			where T1 : IComponentBase
			where T2 : IComponentBase
			where T3 : IComponentBase
			where T4 : IComponentBase
			where T5 : IComponentBase
			where T6 : IComponentBase {
			private static IAllOfMatcher _all;
			private static IAnyOfMatcher _any;

			public static IAllOfMatcher All() {
				if (_all == null) {
					var tmp = Matcher.AllOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>(), idx<T5>(), idx<T6>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_all = tmp;
				}

				return _all;
			}

			public static IAnyOfMatcher Any() {
				if (_any == null) {
					var tmp = Matcher.AnyOf(idx<T1>(), idx<T2>(), idx<T3>(), idx<T4>(), idx<T5>(), idx<T6>()) as Matcher;
					tmp.componentNames = Contexts.Get<__C>().contextInfo.componentNames;
					_any = tmp;
				}

				return _any;
			}
		}
	}
}