using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectCountroll : MonoBehaviour
{
    [SerializeField]
    GameObject Player01_Obj,Player02_Obj;
    [SerializeField]
    Image _player1Text, _player2Text;
    [SerializeField]
    SpriteRenderer Description_1P, Description_2P;
    [SerializeField]
    Slider ReturnSlider;
    [SerializeField]
    GameObject FlameObj;
    [SerializeField,Header("ボスコマンド設定")]
    ControllerManager.Button[] Commands;
    [SerializeField]
    Sprite _boss_Sprite;
    [SerializeField]
    Sprite _void;
    [SerializeField]
    GameObject _Ready;
    [SerializeField]
    Sprite[] _ChataText, ChareDescriptions;
    [SerializeField]
    GameObject _ReadyBackGraund;
    [SerializeField]
    Text text;
    [SerializeField]
    GameObject[] Moves = new GameObject[2];

    Vector3[] Gole = new Vector3[2];//画面にいる
    Vector3[] Gole2 = new Vector3[2];//画面外
    List<CharaSelectObj> CharaObj;
    int length;

    ControllerManager.Controller _1Pcontroller = ControllerManager.Instance.Player1;
    ControllerManager.Controller _2Pcontroller = ControllerManager.Instance.Player2;
    SceneLoader loader = SceneLoader.Instance;
    SoundManager _soundManager=SoundManager.Instance;

    //キャラ立ち絵
    SpriteRenderer Player01, Player02;
    //キャラクターID
    int _Player1, _Player2;
    //キャラクター選択
    bool Player1_OK,Player2_OK;
    //戻る時間
    float Player1_Time, Player2_Time;
    //戻る画面移動時間
    float ReturnTime;
    float ReturnTimeValumes;
    //説明表示
    bool _1PDes,_2PDes;
    float time;
    public float interval;
    float _changeTransparency = 1;
    [SerializeField]
    float _MoveTime_1P,_MoveTime_2P;

    //Boss変数
    bool[] _boss=new bool[2] {false,false};

    void BossSelect(int playerid)
    {
        if( playerid==0) {
            _boss[0] = true;
            Setting.p1c = (Setting.Chara)4;
        }
        else if(playerid == 1)
        {
            _boss[1] = true;
            Setting.p2c = (Setting.Chara)4;
        }
    }

    float[] _xSize =
    {
        0.7f,
        0.4f,
        0.3f,
        0.35f
    };
    float[] _ySize =
    {
        0.7f,
        0.4f,
        0.3f,
        0.35f
    };

    void Start()
    {
        CharaObj = new List<CharaSelectObj>();
        foreach (Transform v in FlameObj.transform)
        {
            var CObj = v.GetComponent<CharaSelectObj>();
            CharaObj.Add(CObj);
        }
        length = FlameObj.transform.childCount;

        foreach (var c in CharaObj)
        {
            c.Init();
        }

        #region ここでテープの初期化
        _MoveTime_1P = _MoveTime_2P = 0f;
        for (int i = 0; i < 2; ++i)
        {
            Gole[i] = Moves[i].transform.position;
            if(i==0) Gole2[i]=Moves[i].transform.position+ new Vector3(-500f, 0, 0);
            else if(i==1) Gole2[i]= Moves[i].transform.position + new Vector3(500f, 0, 0);
        }
        #endregion

        #region　ここで1P、2Pのオブジェクトの初期化
        Player01 = Player01_Obj.GetComponent<SpriteRenderer>();
        Player02 = Player02_Obj.GetComponent<SpriteRenderer>();

        _Player1 = _Player2 = 0;
        Player1_OK = Player2_OK = false;
        _1PDes = _2PDes= false;

        CharaObj[_Player1].charaSelect(1, true);
        CharaObj[_Player2].charaSelect(2, true);
        _player1Text.sprite = _ChataText[_Player1];
        _player2Text.sprite = _ChataText[_Player2];

        Player01.sprite = CharaObj[_Player1].GetCharaSprite;
        Player02.sprite = CharaObj[_Player2].GetCharaSprite;

        Player01_Obj.transform.localScale = new Vector3(_xSize[_Player1], _ySize[_Player1], 1);
        Player02_Obj.transform.localScale = new Vector3(_xSize[_Player2], _ySize[_Player2], 1);

        Description_1P.sprite = ChareDescriptions[_Player1];
        Description_2P.sprite = ChareDescriptions[_Player2];

        Description_1P.enabled = _1PDes;
        Description_2P.enabled = _2PDes;
        #endregion

        _Ready.SetActive(false);

        //戻り時間
        ReturnTime = 1.5f;
        ReturnSlider.maxValue = ReturnTime;
        string commandStr = string.Empty;
        foreach(var v in Commands)
        {
            commandStr += v.ToString();
        }
        //CommandManager.instance.registCommand(commandStr, (int i)=> { Debug.Log("Boss!"); _boss[i] = true; });
        CommandManager.instance.registCommand(commandStr, BossSelect);
        _soundManager.PlayBGM(BGMID.CharacterSelect);

        _ReadyBackGraund.SetActive(false);
        Color color = text.color;
        color.a = 0;
        text.color = color;

    }

    // Update is called once per frame
    void Update()
    {
        if (loader.isLoading) return;
        SelectMove();
        //=========ここが新しいところ==============
        _MoveTime_1P= ReadyBerMove(0, Player1_OK,_MoveTime_1P);
        _MoveTime_2P= ReadyBerMove(1, Player2_OK, _MoveTime_2P);
        //=========================================
        //Charaが二人とも選択されたとき
        if (Player1_OK&& Player2_OK)
        {
            //決定ボタンを入力したら
            if (ControllerManager.Instance.GetButtonDown_Menu(ControllerManager.Button.Start))
            {
                if (!_boss[0])
                {
                    Setting.p1c = (Setting.Chara)_Player1;
                }
                if (!_boss[1])
                {
                    Setting.p2c = (Setting.Chara)_Player2;
                }
                _soundManager.PlaySE(SEID.CharacterSelect_GameStart);
                SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainGame);
                Debug.Log("battleSceneへ");
            }
            _ReadyBackGraund.SetActive(true);
            _Ready.SetActive(true);
            TextColorChange();
            return;
        }
        _ReadyBackGraund.SetActive(false);
        _Ready.SetActive(false);
    }

    void SelectMove()
    {
        //1P処理
        if (!_boss[0]&& _1Pcontroller.GetAxis(ControllerManager.Axis.DpadY) != 0)
        {
            _Player1 = InputProcess(Player01_Obj, _player1Text, _1Pcontroller, Player01, _Player1, Player1_OK, Description_1P, 1);
        }
        else if(_boss[0])
        {
            Player01.sprite = _boss_Sprite;
            _player1Text.sprite = _void;
            Description_1P.sprite = _void;
        }
        //2P処理
        if (!_boss[1] && _2Pcontroller.GetAxis(ControllerManager.Axis.DpadY) != 0)
        {
            _Player2 = InputProcess(Player02_Obj, _player2Text, _2Pcontroller, Player02, _Player2, Player2_OK, Description_2P, 2);
        }
        else if (_boss[1])
        {
            Player02.sprite = _boss_Sprite;
            _player2Text.sprite = _void;
            Description_2P.sprite = _void;
        }

        #region=============ここからボタン操作======================
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.A))
        {
            Player1_OK = true;
            _MoveTime_1P = 0f;
            if (Player2_OK)
            {
                _soundManager.PlaySE(SEID.General_Siren);
            }
            else
            {
                _soundManager.PlaySE(SEID.General_Controller_Decision);
            }
        }
        if (_2Pcontroller.GetButtonDown(ControllerManager.Button.A))
        {
            Player2_OK = true;
            _MoveTime_2P = 0f;
            if (Player1_OK)
            {
                _soundManager.PlaySE(SEID.General_Siren);
            }
            else
            {
                _soundManager.PlaySE(SEID.General_Controller_Decision);
            }
        }
        //×ボタンの処理
        //長押しで画面移動処理
        if (_1Pcontroller.GetButton(ControllerManager.Button.B))
        {
            //キャラ選択時は選択を外す
            if (_1Pcontroller.GetButtonDown(ControllerManager.Button.B))
            {
                Player1_OK = false;
                _MoveTime_1P = 0;
                _soundManager.PlaySE(SEID.General_Controller_Back);
            }
            float difference = Time.time - Player1_Time;
            SetSilder(difference);
            if (difference > ReturnTime)
            {
                _soundManager.PlaySE(SEID.General_Controller_Back);
                SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
            }
        }
        if (_2Pcontroller.GetButton(ControllerManager.Button.B))
        {
            //キャラ選択時は選択を外す
            if (_2Pcontroller.GetButtonDown(ControllerManager.Button.B))
            {
                Player2_OK = false;
                _MoveTime_2P = 0f;
                _soundManager.PlaySE(SEID.General_Controller_Back);
            }
            float difference = Time.time - Player2_Time;
            SetSilder(difference);
            if (difference > ReturnTime)
            {
                SceneLoader.Instance.LoadScene(SceneLoader.Scenes.MainMenu);
            }
        }
        //説明画面
        if (_1Pcontroller.GetButtonDown(ControllerManager.Button.X))
        {
            if (_1PDes) _1PDes = false;
            else _1PDes = true;
        }
        if (_2Pcontroller.GetButtonDown(ControllerManager.Button.X))
        {
            if (_2PDes) _2PDes = false;
            else _2PDes = true;
        }
        //両方入力されていない
        if (!_1Pcontroller.GetButton(ControllerManager.Button.B) && !_2Pcontroller.GetButton(ControllerManager.Button.B))
        {
            Player1_Time = Time.time;
            Player2_Time = Time.time;
            //片方が入力しているとそのまま継続
            ReturnSlider.value = 0f;
        }
        #endregion
        Description_1P.enabled = _1PDes;
        Description_2P.enabled = _2PDes;
    }
    //上下移動のところ
    private int InputProcess(GameObject Player_Obj, Image _playerText, ControllerManager.Controller _controller, SpriteRenderer Player, int _Player, bool Player_OK,SpriteRenderer _Description, int _playerid)
    {
        if (!Player_OK)
        {
            if (_controller.GetAxisUp(ControllerManager.Axis.DpadY) < 0)//上入力
            {
                CharaObj[_Player].charaSelect(_playerid, false);
                _Player++;
                _Player = _Player % length;
                CharaObj[_Player].charaSelect(_playerid, true);

                Player.sprite = CharaObj[_Player].GetCharaSprite;
                _playerText.sprite = _ChataText[_Player];
                _Description.sprite = ChareDescriptions[_Player];

                Player_Obj.transform.localScale = new Vector3(_xSize[_Player], _ySize[_Player], 1);
                _soundManager.PlaySE(SEID.General_Controller_Select);
            }
            else if (_controller.GetAxisUp(ControllerManager.Axis.DpadY) > 0)//下入力
            {
                CharaObj[_Player].charaSelect(_playerid, false);
                _Player--;
                _Player = _Player % length;
                if (_Player < 0) _Player = 3;
                CharaObj[_Player].charaSelect(_playerid, true);

                Player.sprite = CharaObj[_Player].GetCharaSprite;
                _playerText.sprite = _ChataText[_Player];
                _Description.sprite = ChareDescriptions[_Player];

                Player_Obj.transform.localScale = new Vector3(_xSize[_Player], _ySize[_Player], 1);
                _soundManager.PlaySE(SEID.General_Controller_Select);
            }
        }

        return _Player;
    }

    void SetSilder(float t)
    {
        if (t < ReturnSlider.value) return;
        ReturnSlider.value = t;
    }
    void TextColorChange()//ここでテキストのフェードイン、アウトをしている
    {
        //一定期間で表示・非表示
        time += Time.deltaTime * _changeTransparency;
        if (time >= interval) { _changeTransparency = -1; time = interval; }
        if (time <= 0) { _changeTransparency = 1; time = 0; }
        Color color = text.color;
        color.a = time / interval;
        text.color = color;
    }
    //新しい関数
    float ReadyBerMove(int id,bool _Chack,float _MoveTime)
    {
        if (_MoveTime <= 1)
        {
            _MoveTime += 0.1f;
        }
        Transform rect = Moves[id].GetComponent<Transform>();
        if (!_Chack) Moves[id].GetComponent<Transform>().position= Vector3.Lerp(rect.position, Gole2[id], _MoveTime);
        else if (_Chack) Moves[id].GetComponent<Transform>().position = Vector3.Lerp(rect.position, Gole[id], _MoveTime);
        return _MoveTime;
    }
}
