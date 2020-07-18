using System.Collections.Generic;
using System.Linq;
using Entitas;

public sealed class InputSystem : ReactiveSystem, ICleanupSystem {
    readonly IGroup _input;
	private List<Entity> entities;

    public InputSystem() : base(Contexts.Get<Input>()) {
        _input = Contexts.Get<Input>().GetGroup(Matcher<Input>.AllOf<InputComponent>());
		entities = new List<Entity>();
	}

    public void Cleanup() {
        foreach (var e in _input.GetEntities(entities)) {
            e.Destroy();
        }
    }

	protected override ICollector GetTrigger(IContext context) {
        return context.CreateCollector(Matcher<Input>.AllOf<InputComponent>());
	}

	protected override bool Filter(Entity entity) {
        return entity.Has<InputComponent>();
	}

	protected override void Execute(List<Entity> entities) {
		foreach (var entity in entities) {
			var input = entity.Get<InputComponent>();

			foreach (var e in PositionComponent.GetEntitiesWithValue(Contexts.Get<Game>(), new IntVector2(input.x, input.y))
				.Where(e => e.Has<InteractiveComponent>())) {
				e.Add<DestroyedComponent>();
			}
		}
	}
}
