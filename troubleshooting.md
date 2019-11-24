# Troubleshooting AI moves

1. Open Unity.
1. Test Runner.
1. Run unit tests.
1. Enable logs.
1. Define one of the symbols of interest, from high-level to low level:

        LOG_GO_SEARCHER
        LOG_GO_GAME
        LOG_GO_SHARP_MCTS

1. Searching for all conditionals finds them. Example with vim grep:

        :lvimgrep /Conditional(.*LOG/ **/*.cs
        Plugins/GoSharp/Game.cs|452 col 10| [Conditional("LOG_GO_GAME")]
        Scripts/Go/GoSearcher.cs|52 col 10| [Conditional("LOG_GO_SEARCHER")]
        Scripts/Go/GoSearcher.cs|58 col 10| [Conditional("LOG_GO_SEARCHER")]
        Scripts/Go/GoSharpMCTS.cs|127 col 10| [Conditional("LOG_GO_SHARP_MCTS")]
1. Rerun unit tests.
1. Example of a successful log in unit test MakeMoveOn3x3:

        MakeMoveOn3x3 (4.047s)
        ---
        Black, (1,1)206/404, (1,2)89/206, (2,1)85/198, (0,0)71/174, (1,0)69/170, (0,1)58/150, (2,0)51/137, (0,2)46/128, (2,2)45/126
        . . . 
        . . . 
        . . . 

        MakeMoveOn3x3: 4015ms
