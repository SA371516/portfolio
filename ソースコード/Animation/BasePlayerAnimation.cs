using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class BasePlayerAnimation:MonoBehaviour
{
    public enum _KusariAnimList
    {
        Toguro,
        Start,
        Finish
    }
    [SerializeField]
    protected GameObject PlayerObj;
    protected Player player;
    AnimatorStateInfo info_;
    RythmManager rythm;

    //自分のとぐろ
    public MeshRenderer _renderer;
    public bool AnimCheck;
    public float interval;

    protected int PosID;
    protected Animator anim;
    protected string PlayAnim;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        player = PlayerObj.GetComponent<Player>();
        _renderer = transform.GetChild(1).GetComponent<MeshRenderer>();
        _renderer.enabled = false;
        rythm = GameObject.Find("Manager").GetComponent<RythmManager>();
        interval = rythm.getbps;
    }

    protected virtual void Update()
    {
        interval = rythm.getbps / 2;
        anim.speed = rythm.getbps * 2;
    }
    //（プレイヤーの場所,目的地,コマンド）
    public virtual void Move(GameObject Player,Vector3 Goal, Player.MoveComand comand)
    {
        AnimFunc(comand);
        StartCoroutine(enumerator(Player, Goal));
    }
    //（コマンド）
    public void Attack(Player.MoveComand comand)
    {
        AnimFunc(comand);
    }
    //この関数をダメージを受けたときに呼ぶ
    public virtual void Damage()
    {
        MoveDamage();
    }

    //タイミングを同じにするため(動かすもの,到達点)
    IEnumerator enumerator(GameObject obj, Vector3 Goal)
    {
        float time = 0;

        Vector3 Oragin = obj.transform.position;
        Vector3 pos = Vector3.Lerp(Oragin, Goal,time/interval);
        obj.transform.position = pos;
        //移動
        while (time<interval)
        {
            time += Time.deltaTime;
            obj.transform.position  = Vector3.Lerp(Oragin, Goal, time / interval);
            yield return new WaitForFixedUpdate();
        }
    }
    //アニメーション再生
    void AnimFunc(Player.MoveComand comand)
    {
        info_ = anim.GetCurrentAnimatorStateInfo(0);
        //いつでも来てしまうため
        if (!info_.IsName("idol")) return;
        AnimCheck = true;
        switch (comand)
        {
            case Player.MoveComand.None:
                break;
            case Player.MoveComand.Left:
                if (player.PlayerID == 1)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Right:
                if (player.PlayerID == 2)
                {
                    MoveBack();
                }
                else
                {
                    MoveFront();
                }
                break;
            case Player.MoveComand.Up:
                if (player.PlayerID == 1)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Down:
                if (player.PlayerID == 2)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
                break;
            case Player.MoveComand.Attack_1:
                Attack1();
                break;
            case Player.MoveComand.Attack_2:
                Attack2();
                break;
            case Player.MoveComand.Attack_3:
                Attack3();
                break;
            case Player.MoveComand.Attack_4:
                Attack4();
                break;
        }
    }

    protected virtual void Attack1()
    {
    }
    protected virtual void Attack2()
    {
    }
    protected virtual void Attack3()
    {
    }
    protected virtual void Attack4()
    {
    }

    public virtual void KusariAnim(_KusariAnimList _triggerName, GameObject _enemyObj = null, bool _attack = false)
    {

    }

    //2P視点
    //移動は同じなので継承しない
    private void MoveFront()
    {
        anim.SetTrigger("FrontT");
        PlayAnim = "Front";
    }

    private void MoveBack()
    {
        anim.SetTrigger("BackT");
        PlayAnim = "Back";
    }

    private void MoveRight()
    {
        anim.SetTrigger("RightT");
        PlayAnim = "Right";
    }

    private void MoveLeft()
    {
        anim.SetTrigger("LeftT");
        PlayAnim = "left";
    }
    private void MoveDamage()
    {
        anim.SetTrigger("Damage");
        PlayAnim = "Damage";
    }

    //（再生する技のID）
    public virtual void AttackWaitEnd(int waitAttackId)
    {

    }
}
