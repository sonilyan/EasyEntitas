namespace Entitas {

    public static class GroupExtension {

        /// Creates a Collector for this group.
        public static ICollector CreateCollector(this IGroup group, GroupEvent groupEvent = GroupEvent.AddedOrUpdated) {
            return new Collector(group, groupEvent);
        }

		public static ICollector OnAdded(this IGroup group) {
			return new Collector(group, GroupEvent.Added);
		}

		public static ICollector OnRemoved(this IGroup group) {
			return new Collector(group, GroupEvent.Removed);
		}

		public static ICollector OnAddedOrRemoved(this IGroup group) {
			return new Collector(group, GroupEvent.AddedOrRemoved);
		}

		public static ICollector OnUpdated(this IGroup group) {
			return new Collector(group, GroupEvent.Updated);
		}

		public static ICollector OnAddedOrUpdated(this IGroup group) {
			return new Collector(group, GroupEvent.AddedOrUpdated);
		}
	}
}
