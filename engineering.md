# Editor

1. [x] Configure [Referee](LudumDare43/Assets/Scripts/Go/Referee.cs)
    1. [x] Configure [Board Layout](LudumDare43/Assets/Scripts/Go/BoardLayout.cs)
    1. [x] Configure [Game Loader](LudumDare43/Assets/Scripts/Go/GameLoader.cs)
1. [x] Configure [ClickPlacer](LudumDare43/Assets/Scripts/Go/ClickPlacer.cs)
    1. [x] Go.Game
    1. [x] Cancels Illegal Move: Do not change turns when an illegal move is made.
    1. [x] Listen to Cell.OnClick
        1. [x] Make Move.
        1. [x] OnBoardSetup(Go.Game)
1. [x] Configure [Cell](LudumDare43/Assets/Scripts/Go/Cell.cs)
    1. [x] Configure [ClickInputSystemView](LudumDare43/Assets/Plugins/UnityToykit/ClickInputSystemView.cs)
    1. [x] Configure [Animated Player Tile Set](LudumDare43/Assets/Scripts/Go/AnimatedPlayerTileSet.cs)
1. [x] Configure [Animated Turn](LudumDare43/Assets/Scripts/Go/AnimatedTurn.cs)
    1. [x] Configure [Animated Player Tile Set](LudumDare43/Assets/Scripts/Go/AnimatedPlayerTileSet.cs)
    1. [x] Animate volume of Sound Loop
1. [x] Configure [Illegal Move Indicator](LudumDare43/Assets/Scripts/Go/IllegalMoveIndicator.cs)
1. [x] Configure [pixel perfect settings](https://hackernoon.com/making-your-pixel-art-game-look-pixel-perfect-in-unity3d-3534963cad1d)
1. [x] Pass
    1. [x] Referee.Game pass.
    1. [x] Two consecutive passes ends.
    1. [ ] No legal moves ends.
1. [x] End Game
    1. [x] Disable placing pieces.
    1. [x] Player Score
        1. [x] Content: Filters to this player
        1. [x] Score Text
        1. [x] Listens to On Score Changed
        1. [x] SGF: Second player compensation (5.5)
    1. [x] Player Result Status
        1. [x] Listens to Win Game
        1. [x] Win Animation
            1. [x] Configure [Animated Turn](LudumDare43/Assets/Scripts/Go/AnimatedTurn.cs)
                1. [x] Win Indicators
    1. [ ] Count territory.
        1. [x] Fill in captures.
        1. [ ] Infer dead groups.
        1. [ ] Infer ambiguous.
    1. [ ] Activate Click Killer
        1. [ ] Tap sets dead at position.
        1. [ ] Score mode.
1. [ ] Sound On Enable
    1. [ ] Sound: Only plays once if multiple sounds affected.
1. [ ] AI
    1. [x] Monte Carlo Tree Search
        1. [x] Pass on <https://github.com/CampbellAlexander/GameAI> Dependencies missing.
        1. [x] Pass on <https://github.com/rfrerebe/MCTS> Not .NET 3.5 compatible.
        1. [x] Import <https://github.com/dmacthedestroyer/mcts>
            1. [ ] Implement interfaces.
            1. [ ] Set player 2 to computer.
            1. [ ] Computer makes move.
            1. [ ] Disable pass and cell input during computer turn.
