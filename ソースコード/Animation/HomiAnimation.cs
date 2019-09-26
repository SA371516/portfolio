using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class HomiAnimation : BasePlayerAnimation
{
    protected override void Attack1()
    {
        anim.SetTrigger("Attack1");
        PlayAnim = "Attack1";
    }
    protected override void Attack2()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    protected override void Attack3()
    {
        anim.SetTrigger("Buff");
        PlayAnim = "Buff";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("MusicAttack");
                PlayAnim = "MusicAttack";
                break;
            case 3:
                anim.SetTrigger("Special");
                PlayAnim = "Special";
                break;
            default:
                Debug.Log("入力が違います");
                break;
        }
    }
}
