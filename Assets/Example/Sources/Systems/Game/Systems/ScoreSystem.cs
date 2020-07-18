using System.Collections.Generic;
using Entitas;

public sealed class ScoreSystem : ReactiveSystem, IInitializeSystem {

    public ScoreSystem() : base(Contexts.Get<Game>()) {
    }

    public void Initialize() {
	    Contexts.Get<GameState>().Add<ScoreComponent>(x => {
		    x.value = 0;
	    });
    }

	protected override ICollector GetTrigger(IContext context) {
        return context.CreateCollector(Matcher<Game>.AllOf<GameBoardElementComponent>().Removed());
	}

	protected override bool Filter(Entity entity) {
        return !entity.Has<GameBoardComponent>();
	}

	protected override void Execute(List<Entity> entities) {
        var c = Contexts.Get<GameState>().Replace<ScoreComponent>(x => {
	        x.value += entities.Count;
        });
	}
}
