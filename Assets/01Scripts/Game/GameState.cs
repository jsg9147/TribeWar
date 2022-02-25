using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // 게임 서버에서 활용하는거였는데
    // 이거 게임 매니저에서 데이터 관리를 해서 entity 데이터를 좀 static 최소화로
    public List<PlayerInfo> players;

    public PlayerInfo currentPlayer;
    public PlayerInfo currentOpponent;
}
