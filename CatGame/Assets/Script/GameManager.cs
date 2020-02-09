using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    //cat property
    public int _satietyPoint = 80;
    public int _healthPoint = 100;
    public int _drinkPoint = 80;
    public int _staminaPoint = 100;
    public int _walkSpeed = 1;
    public int _cleanPoint = 50;
    public int _cutePoint = 0;
    public int _survivePoint = 0;
    public int _intelligencePoint = 0;
    public int _luckyPoint = 1;
    public float _timeScale = 4;
    [HideInInspector]
    public int _maxSatietyPoint = 100;
    [HideInInspector]
    public int _maxDrinkPoint = 100;
    [HideInInspector]
    public int _maxHealthPoint = 100;
    [HideInInspector]
    public int _maxStaminaPoint = 100;
    [HideInInspector]
    public int _maxWalkSpeed;
    [HideInInspector]
    public int _maxCleanPoint = 100;
    [HideInInspector]
    public int _maxLuckyPoint = 10;

    //environment
    //[HideInInspector]
    public float _time; //unit is minute
    public int _day;
    public bool isDayadd = false;
    private Light _mainLight;
    [SerializeField]
    private Color nightCol, sunSetCol, dayCol;

    //force to sleep
    [HideInInspector]
    public int _lasthour;
    [HideInInspector]
    public int _punishhour;
    [HideInInspector]
    public bool isPunish = false;
    [HideInInspector]
    public bool isSleeped = false;

    //UI
    public GameObject gamePlay;
    public GameObject eventBox;
    public GameObject gameover;
    
    //Event
    public bool isEncounter;
    public string eventContent;
    public string[] options;
    public int optionRank = -1; //get which option been clicked
    [HideInInspector]
    public List<Transform> actionButton;


    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                var obj = new GameObject();
                obj.AddComponent<GameManager>();
                _instance = obj.GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _time = 480.0f;
        _punishhour = 22;
        _lasthour = _punishhour;
        dayCol = new Color(1f, 0.9927168f, 0.9292453f);
        sunSetCol = new Color(1f, 0.4720881f, 0.3726415f);
        nightCol = new Color(0.5840601f, 0.6810673f, 0.990566f);
        foreach (Transform transform in eventBox.transform)
        {
            if (transform.CompareTag("Button")) { actionButton.Add(transform); }
        }
        _mainLight = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        //time update
        Time.timeScale = isEncounter ? 0 : 1;
        _time += Time.deltaTime * _timeScale;
        var hour = (int)(_time / 60);
        var minute = (int)(_time % 60);
        gamePlay.GetComponentsInChildren<Text>()[3].text = "Time: " + (hour < 10 ? "0" + hour.ToString() : hour.ToString()) + ":" + (minute < 10 ? "0" + minute.ToString() : minute.ToString());
        if (_time / 60 >= 24)
        {
            _time = 0;
            addDay();
        }

        //stamina update
        if ((_time / 60 >= 2 && _time / 60 < 8) || _staminaPoint <= 0)
        {
            //force to sleep
            isPunish = true;
            isSleeped = true;
        }
        else if (hour >= 22 && _lasthour != hour)
        {
            int timespan = (hour > _lasthour) ? hour - _lasthour : hour - _lasthour + 24;
            _staminaPoint -= 10 * timespan;
            _lasthour = hour;
        }
        if (_staminaPoint >= _maxStaminaPoint) _staminaPoint = _maxStaminaPoint;

        //light change with time
        _mainLight.intensity = Mathf.Lerp(0.1f, 0.6f, Mathf.Sin(_time / 1440 * Mathf.PI) / 0.707f - 0.707f); 
        _mainLight.shadowStrength = Mathf.Lerp(0.1f, 1f, Mathf.Sin(_time / 1440 * Mathf.PI) / 0.707f - 0.707f);
        if (_time < 360 || 1080 < _time)
        {
            RenderSettings.ambientSkyColor = Color.Lerp(nightCol, sunSetCol, Mathf.Pow(0.5f - Mathf.Cos(_time / 360 * Mathf.PI) / 2, 200));
        }
        else
        {
            RenderSettings.ambientSkyColor = Color.Lerp(dayCol, sunSetCol, Mathf.Pow(0.5f - Mathf.Cos(_time / 360 * Mathf.PI) / 2, 200));
        }
        RenderSettings.ambientSkyColor *= Mathf.Lerp(0.2f, 2.25f, Mathf.Sin(_time / 1440 * Mathf.PI) - 0.6f);

        //day update
        gamePlay.GetComponentsInChildren<Text>()[4].text = "Day" + _day;

        //health update
        _healthPoint = Mathf.Min(_maxHealthPoint, _healthPoint);
        if(_healthPoint <= 0)
        {
            isEncounter = true;
            eventContent = "You Die!";
        }
        gamePlay.GetComponentsInChildren<Text>()[0].text = "Health: " + _healthPoint;

        //satiety update
        _satietyPoint = Mathf.Min(_maxSatietyPoint, _satietyPoint);
        gamePlay.GetComponentsInChildren<Text>()[1].text = "Satiety: " + _satietyPoint;

        //drink point update
        _drinkPoint = Mathf.Min(_maxDrinkPoint, _drinkPoint);
        gamePlay.GetComponentsInChildren<Text>()[2].text = "Drink Point: " + _drinkPoint;       

        //drink point update
        _cleanPoint = Mathf.Min(_maxCleanPoint, _cleanPoint);
        _cleanPoint = Mathf.Max(0, _cleanPoint);
        gamePlay.GetComponentsInChildren<Text>()[5].text = "Clean Point: " + _cleanPoint;

        //event update
        eventBox.SetActive(isEncounter);
        eventBox.GetComponentsInChildren<Text>()[0].text = eventContent;
        for (int i = 0; i < 4; i++)
        {
            if (options[i] == ""){ actionButton[i].gameObject.SetActive(false); }
            else 
            {
                actionButton[i].gameObject.SetActive(true);
                actionButton[i].GetComponentInChildren<Text>().text = options[i];
            }
        }

        //cutePoint update
        if(_cutePoint < 0) { _cutePoint = 0; }

        //force sleep event option
        if (isSleeped)
        {
            _mainLight.intensity = 0;
            RenderSettings.ambientSkyColor = Color.black;
        }
        if (isPunish)
        {
            if (optionRank == -1)
            {
                isEncounter = true;
                eventContent = "You are so tired and sleep in the wild.\n Something hurt you.\nAt the same time, you consume some food and water in your sleep.\nFacing the unknown next day\nsatiety - 30, drinkPoint - 30";
                options[0] = "OK";
            }
            else
            {
                isEncounter = false;
                optionRank = -1;
                isSleeped = false;
                isPunish = false;

                float wakeUpTime = 720, period = wakeUpTime - _time;
                if (period < 600)
                {
                    addDay();
                    period += 1440;
                }
                _time = wakeUpTime;
                _lasthour = _punishhour;
                _staminaPoint += (int)(period / 60) * 2 - 19;
                _satietyPoint -= 30;
                _drinkPoint -= 30;
            }
        }
    }

    public void ClickOption(int rank)
    {
        optionRank = rank;
        for(int i =0; i<options.Length;i++)
        {
            options[i] = "";
        }
    }

    public void addDay() {
        isDayadd = true;
        _day++;
    }
}

