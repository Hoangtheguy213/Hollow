using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image heartShard;
    [SerializeField] Image manaShard;
    [SerializeField] GameObject upCast, sideCast, downCast;
    [SerializeField] GameObject dash, varJump, wallJump;
    private void OnEnable()
    {
        // heart shard
        heartShard.fillAmount = Player.Instance.heartShards * 0.25f;

        // mana shard
        manaShard.fillAmount = Player.Instance.orbShard * 0.34f;

        // ability
        if (Player.Instance.unlockDash)
        {
            dash.SetActive(true);
        }
        else
        {
            dash.SetActive(false);
        }
        if (Player.Instance.unlockVarJump)
        {
            varJump.SetActive(true);
        }
        else
        {
            varJump.SetActive(false);
        }
        if (Player.Instance.unlockWallJump)
        {
            wallJump.SetActive(true);
        }
        else
        {
            wallJump.SetActive(false);
        }
        // spell
        if (Player.Instance.unlockSideSpell)
        {
            sideCast.SetActive(true);
        }
        else
        {
            sideCast.SetActive(false);
        }
        if (Player.Instance.unlockUpSpell)
        {
            upCast.SetActive(true);
        }
        else
        {
            upCast.SetActive(false);
        }
        if (Player.Instance.unlockDownSpell)
        {
            downCast.SetActive(true);
        }
        else
        {
            downCast.SetActive(false);
        }
    }
}
