using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpbarUI : MonoBehaviour
{
    [SerializeField] private Slider hpbar;
    [SerializeField] private Player player;
    private float imsi;
    void Start()
    {
        imsi = (float)player.CurHealth / (float)player.maxHealth;
        hpbar.value = imsi;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (player.CurHealth > 0)
            {
                player.CurHealth--;
            }
            else
            {
                player.CurHealth = 0;
            }
            imsi = (float) player.CurHealth / (float)player.maxHealth;
        }
        
        if(player.CurHealth < player.maxHealth)
        HandleHp();
    }

    private void HandleHp()
    {
        hpbar.value = Mathf.Lerp(hpbar.value, imsi, Time.deltaTime * 10f);
    }
}
