using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // ���� �������� Ȱ���ϴ°ſ��µ�
    // �̰� ���� �Ŵ������� ������ ������ �ؼ� entity �����͸� �� static �ּ�ȭ��
    public List<PlayerInfo> players;

    public PlayerInfo currentPlayer;
    public PlayerInfo currentOpponent;
}
