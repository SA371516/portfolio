using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimManager : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo info;
    [SerializeField,Header("再生するアニメーションクリップを入れる")]
    AnimationClip[] animations;
    [SerializeField]
    Text text;
    [SerializeField]
    Slider roteSlider;
    [SerializeField]
    GameObject PlayObj;
    float valume;

    void Start()
    {
        anim = PlayObj.GetComponent<Animator>();
        roteSlider.maxValue = 360;
    }
    private void Update()
    {
        //モデル回転処理
        if (valume != roteSlider.value)
        {
            float rote = roteSlider.value - valume;
            valume = roteSlider.value;
            PlayObj.transform.Rotate(new Vector3(0, rote, 0));
        }
        //テキスト表示処理
        info= anim.GetCurrentAnimatorStateInfo(0);
        foreach(var v in animations)
        {
            if (info.IsName(v.name))
            {
                text.text = v.name;
            }
        }
    }

    public void OnClick(string T)
    {
        anim.SetTrigger(T);
    }
    //[EnumAction(typeof(AnimPlay.type))]
    //public void OnClick(int Typed)
    //{
    //    AnimPlay.type t = (AnimPlay.type)Typed;
    //    t = AnimPlay.TP;
        
    //}
}
