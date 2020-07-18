
using System.Collections.Generic;

namespace Entitas
{
	/// Execute on each entity which matches
	public abstract class ExecuteSystem : IExecuteSystem
	{
		protected IMatcher _matcher;
		protected Context _context;
		private List<Entity> buffer;

		public ExecuteSystem(Context context)
		{
			_context = context;
			_matcher = GetMatcher(context);
			buffer = new List<Entity>();
		}

		public virtual void Execute()
		{
			var entities = _context.GetEntities(_matcher, buffer);
			foreach (var e in entities)
			{
				Execute(e);
			}
		}

		protected abstract void Execute(Entity entity);
		protected abstract IMatcher GetMatcher(Context context);
	}
}