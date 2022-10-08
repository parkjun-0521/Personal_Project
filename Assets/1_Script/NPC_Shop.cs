using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Shop : MonoBehaviour
{
    public RectTransform uiGroup;

    public int upgradePrice;
    int priceCount = 0;

    Player enterPlayer;
 
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        // 상점에서 나갈때 UI를 -1000 아래 방향으로 내린다 
        uiGroup.anchoredPosition = Vector3.down * -1000;
    }

    public void Buy()
    {
        if (priceCount <= 9) {
            int price = upgradePrice;
            Debug.Log("구입");
            enterPlayer.point -= price;
            enterPlayer.power += 10;
            upgradePrice *= 2;
            priceCount++;
        }
        else
            return;
    }
}
