using System.Collections.Generic;
using System.Linq;
using Entitas;
using UnityEngine;

public sealed class FallSystem : ReactiveSystem {

    public GameBoardService gameBoardService = GameBoardService.singleton;

    public FallSystem() : base(Contexts.Get<Game>()) {
    }

    void moveDown(Entity e, IntVector2 position) {
        var nextRowPos = gameBoardService.GetNextEmptyRow(position);
        if (nextRowPos != position.y) {
	        e.Replace<PositionComponent>(x=> {
		        x.value = new IntVector2(position.x, nextRowPos);
	        });
        }
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
            for (int row = 1; row < gameBoard.rows; row++) {
                var position = new IntVector2(column, row);

				var movables = PositionComponent.GetEntitiesWithValue(Contexts.Get<Game>(), position)
					.Where(e => e.Has<MovableComponent>()).ToArray();

                foreach (var e in movables) {
                    moveDown(e, position);
                }
            }
        }
	}
}
