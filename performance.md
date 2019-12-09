# CPU hotspots

3x3 Windows.

- Apply Action
    - Get Legal Moves
        - Board Get Captured Groups
            - Board Get Group At
                - Recursive Add Point
                    - HashSet.Contains: Point.Equals

## Solutions

1. [x] Pool groups.
1. [x] Pool boards.
1. [x] Pool games.
1. [x] `DISABLE_GROUP_POINTS` hypothesizes next hotspots.
1. [x] Store in super ko previous board state and move that will be made.
1. [x] Query has liberty before suicide.
1. [x] Do not calculate captures for legal moves.
1. [x] Cache legal moves. Copy when cloning game. Update after making a move.
1. [x] When making a move from AI, assume legal.
1. [x] Board: clone: group caches.
1. [x] Game: black capture advantage.
1. [ ] When no legal move, cache loss for that player. Do not calc territory or score. if komi, other player keeps playing. captures modifies local komi delta. black wins ties.
1. [ ] Minimal group data.
1. [ ] Groups: Preallocate fixed length data.
1. [ ] Groups: Merge groups.
1. [ ] Liberties mask per group.
1. [ ] Static: board size.
1. [ ] Weight exploratory actions toward center position on the 3x3 or 5x5 board.
1. [ ] Flyweight board data structure.
1. [ ] when capturing, update legal moves. when playing reexamine legal moves.
1. [ ] Super ko: Preallocate fixed length data, circular array of bitmasks.
1. [ ] When applying action, reuses game instead of cloning game.
1. [ ] Precompute bitmask of move.
1. [x] Bitmask 5x5 board into 25 bits of a 32-bit integer. Masks: Black, White.
1. [ ] With each move, modify only necessary data.
1. [ ] Localize memory access.
1. [ ] Legal moves: modifies board with minimal data copying.
1. [ ] Legal moves as mask.
1. [ ] captures as bitmask.
1. [ ] Index instead of point.
1. [ ] Bitmask instead of groups.
1. [x] `DISABLE_CALC_TERRITORY`
1. [ ] Cache boards.
1. [x] `DISABLE_CAPTURES_DICTIONARY`
1. [ ] Pool actions.
1. [ ] +30%: Cache boards of legal moves for reuse in make move.
1. [ ] With the above disabled, 5x5 runs 1000 iterations in 200 ms in editor.
1. [ ] class references array of values.
1. [ ] profile struct vs class
1. [ ] seal classes
1. [x] Custom [HashSet comparison.](https://www.codeproject.com/Articles/1280633/Creating-a-Faster-HashSet-for-NET)
1. [ ] MCTS: Collections without Linq. Get max element. Random choice.
1. [ ] Thread.
1. [ ] burst compiler.
1. [ ] Job System.
1. [ ] Pool game tree node.

# Go Searcher 3x1 CPU Hotspots

- 700 ms: Cloning 20000 Go State 5x5.
    - 500 ms: Cloning Go Game State 5x5.
        - 200 ms: Cloning arrays.
        - 50 ms: Constructing lists.
- 400 ms: Apply Action 20000 times.
    - 380 ms: Go Game State 5x5 Move
        - 100 ms: Try Forbid Suicide At Index
            - 40 ms: Would Share Any Liberty
            - 20 ms: Would Capture Any
        - 100 ms: String concat.
        - 70 ms: Convert Move Mask.
        - 40 ms: Add Board To History

1. [ ] Bias central move.
