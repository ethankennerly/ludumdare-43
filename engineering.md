# Editor

1. [ ] Go Referee
    1. [ ] Game State (Go.Game)
    1. [ ] Listen to GoCellView.OnClick
        1. [ ] Make Move.

                m_GameState = m_GameState.MakeMove(3, 3, out legal);
        1. [ ] OnSetup(Go.Game)
        1. [ ] OnCaptured(Group)
        1. [ ] OnOccupied(PointContent)
    1. [ ] Players
1. [ ] Board Loader
    1. [ ] SGF File
        1. [ ] Board Size
    1. [ ] Go Referee
        1. [ ] Set Game State
1. [ ] Board Layout
    1. [ ] Go Referee
    1. [ ] Tile Center Position.
    1. [ ] Tile Offset: to support shearing or isometric.
1. [ ] Playing
    1. [ ] Sound Loop
1. [ ] ClickInputSystemView
1. [ ] Go Cell View
    1. [ ] Address (Vector2Int)
    1. [ ] Click (Collider 2D)
    1. [ ] Static Event OnClick(int, int)
        1. [ ] Static (FineGameDesign.Utils.ClickInputSystem) OnCollisionEnter2D
            1. [ ] If not GoCellView: stop here.
            1. [ ] OnClick(column, row)
1. [ ] SoundOnEnable
    1. [ ] Sound: Only plays once if multiple tiles affected.
1. [ ] Go Player
    1. [ ] Tile Prefab
        1. [ ] Pools
    1. [ ] Status
1. [ ] Go Player Tile
    1. [ ] Occupied
        1. [ ] Sound: Only plays once if multiple tiles affected.
        1. [ ] Sprite
    1. [ ] Captured
        1. [ ] Sound
        1. [ ] Sprite
    1. [ ] Claimed
        1. [ ] Sound
        1. [ ] Sprite
1. [ ] Go Player Status
    1. [ ] Sound Loop
    1. [ ] Sprite
    1. [ ] Score Text
    1. [ ] Illegal Move Animation
    1. [ ] Win Animation
