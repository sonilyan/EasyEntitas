using System.Collections.Generic;
using Entitas;

public sealed class GameBoardSystem : ReactiveSystem, IInitializeSystem {

    public EntityService entityService = EntityService.singleton;
    public RandomService randomService = RandomService.game;

    readonly IGroup _gameBoardElements;

    public GameBoardSystem() : base(Contexts.Get<Game>()) {
        _gameBoardElements = Contexts.Get<Game>().GetGroup(Matcher<Game>.AllOf<GameBoardElementComponent, PositionComponent>());
    }

    public void Initialize()
    {
	    var gameBoard = entityService.CreateGameBoard().Get<GameBoardComponent>();
	    for (int row = 0; row < gameBoard.rows; row++)
	    {
		    for (int column = 0; column < gameBoard.columns; column++)
		    {
			    if (randomService.Bool(0.1f))
			    {
				    entityService.CreateBlocker(column, row);
			    }
			    else
			    {
				    entityService.CreateRandomPiece(column, row);
			    }
		    }
	    }
    }

    protected override ICollector GetTrigger(IContext context) {
        return context.CreateCollector(Matcher<Game>.AllOf<GameBoardComponent>());
	}

	protected override bool Filter(Entity entity) {
        return entity.Has<GameBoardComponent>();
	}

	protected override void Execute(List<Entity> entities) {
		var gameBoard = Contexts.Get<Game>().Get<GameBoardComponent>();
        foreach (var e in _gameBoardElements) {
            if (e.Get<PositionComponent>().value.x >= gameBoard.columns 
                || e.Get<PositionComponent>().value.y >= gameBoard.rows) {
                e.Add<DestroyedComponent>();
            }
        }
	}
}
