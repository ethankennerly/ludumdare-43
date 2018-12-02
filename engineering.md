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
    1. [ ] Sound Loop
1. [x] Configure [Illegal Move Indicator](LudumDare43/Assets/Scripts/Go/IllegalMoveIndicator.cs)
1. [ ] Pass
    1. [ ] Two consecutive passes ends.
    1. [ ] No legal moves ends.
1. [ ] End Game
    1. [ ] Player Score
        1. [ ] Content: Filters to this player
        1. [ ] Score Text
        1. [ ] Listens to On Score Changed
        1. [ ] SGF: Second player compensation (5.5)
    1. [ ] Count territory.
        1. [ ] Infer dead groups.
        1. [ ] Fill in captures.
        1. [ ] Infer ambiguous.
    1. [ ] Player Result Status
        1. [ ] Listens to Win Game
        1. [ ] Win Animation
    1. [ ] Activate Click Killer
        1. [ ] Tap sets dead at position.
        1. [ ] Score mode.
1. [ ] AI
    1. [ ] Monte Carlo Tree Search
        1. [ ] <https://github.com/CampbellAlexander/GameAI>
        1. [ ] <https://github.com/rfrerebe/MCTS>
        1. [ ] <https://github.com/dmacthedestroyer/mcts>
1. [ ] Sound On Enable
    1. [ ] Sound: Only plays once if multiple sounds affected.
