using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃は違うのでクラスを分ける
public class KaguraAnimation :BasePlayerAnimation
{
    MeshRenderer _enemyRenderer;
    //[SerializeField]
    Animator _kusariAnim;
    Animator _toguroAnim;
    AnimatorStateInfo _KusariInfo;
    protected override void Start()
    {
        base.Start();
        _kusariAnim = gameObject.transform.GetChild(2).GetComponent<Animator>();
        anim.SetTrigger("Start");
    }

    protected override void Update()
    {
        base.Update();
        if (_toguroAnim!=null)//巻きつくアニメーションが終了したら
        {
            _KusariInfo = _toguroAnim.GetCurrentAnimatorStateInfo(0);
            if (!_KusariInfo.IsName("Toguro"))
            {
                _enemyRenderer.enabled = false;//非表示
                KusariAnim(_KusariAnimList.Finish);
                _toguroAnim = null;
                _enemyRenderer = null;
            }
        }

    }
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
        anim.SetTrigger("PullAttack");
        KusariAnim(_KusariAnimList.Start);
        PlayAnim = "PullAttack";
    }
    protected override void Attack4()
    {
        anim.SetTrigger("Wait");
        PlayAnim = "Wait";
    }
    //鎖のアニメーションを再生する関数（敵のオブジェクト,鎖攻撃が当たった）
    public override void KusariAnim(_KusariAnimList _triggerName,GameObject _enemyObj=null, bool _attack = false)
    {
        if (_attack)//相手につけている鎖のアニメーションを再生させる
        {
            _toguroAnim = _enemyObj.transform.GetChild(1).GetComponent<Animator>();
            _enemyRenderer = _enemyObj.GetComponent<BasePlayerAnimation>()._renderer;//親要素として取得することが出来るのか？
            _enemyRenderer.enabled = true;//表示
            _toguroAnim.SetTrigger("Toguro");//"Toguro"
        }
        else//鎖の最初&終了
        {
            switch (_triggerName)
            {
                case _KusariAnimList.Start:
                    _kusariAnim.SetTrigger("Start");
                    break;
                case _KusariAnimList.Finish:
                    _kusariAnim.SetTrigger("Finish");
                    break;
            }
        }
    }

    public override void AttackWaitEnd(int waitAttackId)
    {
        switch (waitAttackId)
        {
            case 1:
                anim.SetTrigger("Attack1");
                PlayAnim = "Attack1";
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
