# CPU hotspots

3x3 Windows.

- Apply Action
    - Get Legal Moves
        - Board Get Group At
            - Recursive Add Point
                - HashSet.Contains: Point.Equals

## Solutions

1. [x] Custom [HashSet comparison.](https://www.codeproject.com/Articles/1280633/Creating-a-Faster-HashSet-for-NET)
1. [ ] +30%: Cache boards of legal moves for reuse in make move.
1. [ ] Cache boards.
1. [ ] Flyweight board data structure.
1. [ ] Pool boards.
1. [ ] Pool groups.
1. [ ] Pool games.
1. [ ] Thread.
1. [ ] Job System.
