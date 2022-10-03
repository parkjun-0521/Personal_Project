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
        // �������� ������ UI�� -1000 �Ʒ� �������� ������ 
        uiGroup.anchoredPosition = Vector3.down * -1000;
    }

    public void Buy()
    {
        if (priceCount <= 9) {
            int price = upgradePrice;
            Debug.Log("����");
            enterPlayer.point -= price;
            enterPlayer.power += 10;
            upgradePrice *= 2;
            priceCount++;
        }
        else
            return;
    }
}
