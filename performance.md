# CPU hotspots

3x3 Windows.

- Apply Action
    - Get Legal Moves
        - Board Get Captured Groups
            - Board Get Group At
                - Recursive Add Point
                    - HashSet.Contains: Point.Equals

## Solutions

1. [ ] Pool groups.
1. [ ] Pool boards.
1. [ ] Pool games.
1. [x] `DISABLE_GROUP_POINTS` hypothesizes next hotspots.
1. [ ] Flyweight board data structure.
1. [ ] With each move, modify only necessary data.
1. [ ] Localize memory access.
1. [ ] Preallocate fixed length data.
1. [ ] Bitmask 5x5 board into 25 bits of a 32-bit integer. Masks: Black, White.
1. [ ] legal moves.
1. [ ] Query has liberty before suicide.
1. [ ] captures.
1. [ ] Legal moves: modifies board with minimal data copying.
1. [ ] Superko by circular array of bitmasks.
1. [ ] Index instead of point.
1. [ ] Bitmask instead of groups.
1. [x] `DISABLE_CALC_TERRITORY`
1. [ ] When no legal move, cache loss for that player. Do not calc territory.
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
