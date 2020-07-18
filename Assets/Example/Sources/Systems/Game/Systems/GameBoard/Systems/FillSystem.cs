using System.Collections.Generic;
using Entitas;

public sealed class FillSystem : ReactiveSystem {

    public EntityService entityService = EntityService.singleton;
    public GameBoardService gameBoardService = GameBoardService.singleton;

    public FillSystem() : base(Contexts.Get<Game>()) {
    }

	protected override ICollector GetTrigger(IContext context) {
        return context.CreateCollector(Matcher<Game>.AllOf<GameBoardElementComponent>().Removed());
	}

	protected override bool Filter(Entity entity) {
        return !entity.Has<GameBoardElementComponent>();
	}

	protected override void Execute(List<Entity> entities) {
		var gameBoard = Contexts.Get<Game>().Get<GameBoardComponent>();
        for (int column = 0; column < gameBoard.columns; column++) {
            var position = new IntVector2(column, gameBoard.rows);
            var nextRowPos = gameBoardService.GetNextEmptyRow(position);
            while (nextRowPos != gameBoard.rows) {
                var tmp = entityService.CreateRandomPiece(column, nextRowPos);
				nextRowPos = gameBoardService.GetNextEmptyRow(position);
            }
        }
	}
}
