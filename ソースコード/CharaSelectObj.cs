using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaSelectObj : MonoBehaviour
{
    //status
    public Sprite[] Flames = new Sprite[3];
    SpriteRenderer _sprite;
    [SerializeField]
    Sprite CharaSprite;
    public Sprite GetCharaSprite
    {
        get { return CharaSprite; }
    }
    private bool P1select;
    private bool P2select;

    private void Awake()
    {
        _sprite= gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    public void Init()
    {
        P1select = false;
        P2select = false;
        _sprite.sprite = Flames[3];
    }
    //(操作している人のID,キャラが選択されているか)
    public void charaSelect(int ID,bool OK)
    {
        //何Pが操作したかを判断
        switch (ID)
        {
            case 1:
                P1select = OK;
                break;
            case 2:
                P2select = OK;
                break;
        }
        //枠の画像を変更する
        if (P1select && P2select)
        {
            _sprite.sprite = Flames[0];
        }
        else if (P1select&&!P2select)
        {
            _sprite.sprite = Flames[1];
        }
        else if (P2select&&!P1select)
        {
            _sprite.sprite = Flames[2];
        }
        else if (!P1select && !P2select)
        {
            _sprite.sprite = Flames[3];

        }
    }

}
