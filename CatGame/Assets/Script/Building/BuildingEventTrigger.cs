using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Enumeration;

public class BuildingEventTrigger : MonoBehaviour
{
    public int liking = 50;
    public int _event_num, _event_num_max;
    int eatOptionID_yes = 98;
    int eatOptionID_no = 99;
    houseType type;
    List<int> options = new List<int>();
    bool istriggered = false, isKeyDown = false;
    static XmlDocument houseEvent = new XmlDocument(), option = new XmlDocument(), result = new XmlDocument(), item = new XmlDocument(), skill = new XmlDocument(), eventTrigger = new XmlDocument(), resultTrigger = new XmlDocument();
    static GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        _event_num = 1;
        _event_num_max = 2 + (int)(UnityEngine.Random.value * 2);
        gm = GameManager.Instance;
        string addr = "Assets/Database/";

        houseEvent.Load(addr + "event.xml");
        option.Load(addr + "option.xml");
        result.Load(addr + "result.xml");
        item.Load(addr + "item.xml");
        skill.Load(addr + "skill.xml");
        eventTrigger.Load(addr + "eventtrigger.xml");
        resultTrigger.Load(addr + "resulttrigger.xml");
    }

    // Update is called once per frame
    void Update()
    {
        if (istriggered && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.F)) && !isKeyDown && !gm.isEncounter)
        {
            isKeyDown = true;
            if (type == houseType.cathouse)
            {
                options.Clear();
                gm.isEncounter = true;
                gm.eventContent = "Do you want to sleep now?";

                gm.options[0] = "yes";
                options.Add(1);
                gm.options[1] = "No";
                options.Add(0);
                gm.options[2] = "";
                options.Add(0);
                gm.options[3] = "";
                options.Add(0);
            }
            //else if (_event_num <= _event_num_max || (_event_num > _event_num_max && type == houseType.garbage_dump))
            else
            {
                try
                {
                    XmlNodeList houseEvents = houseEvent.SelectNodes("descendant::row[houseID=" + type.ToString("d") + "]");
                    foreach (XmlNode he in houseEvents)
                    {
                        bool qualified = false;
                        XmlNodeList triggers = eventTrigger.SelectNodes("descendant::row[eventID=" + he.SelectSingleNode("ID").InnerText + "]");
                        if (triggers.Count != 0)
                        {
                            foreach (XmlNode condition in triggers)
                            {
                                string name = condition.SelectSingleNode("triggername").InnerText;
                                int min = Int32.Parse(condition.SelectSingleNode("valuemin").InnerText);
                                int max = Int32.Parse(condition.SelectSingleNode("valuemax").InnerText);
                                qualified = isIn(name, min, max);
                                if (!qualified) break;
                            }
                        }
                        if (qualified)
                        {
                            showEvent(he);
                            break;
                        }
                        else isKeyDown = false;
                    }
                }
                catch (Exception e)
                {
                    isKeyDown = false;
                }
            }
        }

        if (gm.optionRank != -1 && options.Count > 0)
        {
            int option_index = options[gm.optionRank];
            if (option_index == 0)
            {
                gm.isSleeped = false;
                gm.isEncounter = false;
                isKeyDown = false;
                options.Clear();
            }
            else
            {
                if (type == houseType.cathouse)
                {
                    string content = "";
                    float wakeupTime = (gm._staminaPoint < 50) ? 1080 : 480;
                    float period = wakeupTime - gm._time;
                    if (period <= 0)
                    {
                        gm.addDay();
                        period += 1440;
                    }
                    int stamina_subtract = (gm._staminaPoint < 50) ? 29 : 9;
                    gm._staminaPoint += (int)(period / 60) - stamina_subtract;
                    if (gm._satietyPoint >= 90 && gm._drinkPoint >= 90)
                    {
                        content = "You've had a fulfilling day and you're full before you go to bed, so your body has recovered health.\nhealth: +10 ";
                        gm._healthPoint += 10;
                    }
                    else if (gm._satietyPoint >= 50 && gm._drinkPoint >= 50)
                    {
                        content = "You have had a nice day, and you are not hungry when you fall asleep. You have recovered some health.\nhealth: +5 ";
                        gm._healthPoint += 5;
                    }
                    else if (gm._satietyPoint <= 0 || gm._drinkPoint <= 0)
                    {
                        content = "YOU KNOW YOU WILL DIE IF YOU DON’T EAT ANYTHING. RIGHT？\nhealth:-10 ";
                        gm._healthPoint += -10;
                    }
                    else
                    {
                        content = "You have had an ok day, and you are a little hungry or thirsty before you fall asleep. ";
                    }
                    gm._satietyPoint -= 30;
                    gm._drinkPoint -= 30;
                    gm._time = wakeupTime;
                    gm._lasthour = gm._punishhour;
                    gm.isEncounter = true;
                    gm.eventContent = content + "\nFacing the unknown next day\nsatiety - 30 drinkPoint - 30";
                    gm.isSleeped = true;
                    options.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        gm.options[i] = (i == 0) ? "OK" : "";
                        options.Add(0);
                    }
                }
                else
                {
                    XmlNode theOption = option.SelectSingleNode("descendant::row[ID=" + option_index + "]"), node;
                    XmlNodeList triggers = resultTrigger.SelectNodes("descendant::row[optionID=" + theOption.SelectSingleNode("ID").InnerText + "]");
                    bool qualified = false;
                    int min, max;

                    if (triggers.Count != 0)
                    {
                        foreach (XmlNode tri in triggers)
                        {
                            if (tri.SelectSingleNode("triggername").InnerText == "skill") qualified = isSkillActive(tri.SelectSingleNode("valuemin").InnerText);
                            else
                            {
                                min = Int32.Parse(tri.SelectSingleNode("valuemin").InnerText);
                                max = Int32.Parse(tri.SelectSingleNode("valuemax").InnerText);
                                qualified = isIn(tri.SelectSingleNode("triggername").InnerText, min, max);
                            }
                            if (qualified)
                            {
                                string resultID = tri.SelectSingleNode("resultID").InnerText;
                                node = result.SelectSingleNode("descendant::row[ID=" + resultID + "]");
                                showResult(node);
                                break;
                            }
                        }
                    }

                    if (!qualified)
                    {
                        string[] results = theOption.SelectSingleNode("resultID").InnerText.Split(',');

                        int random = (int)(UnityEngine.Random.value * 100);
                        min = 0;
                        max = 0;
                        foreach (string index in results)
                        {
                            node = result.SelectSingleNode("descendant::row[ID=" + index + "]");
                            min = max;
                            max += Int32.Parse(node.SelectSingleNode("weight").InnerText);

                            if (random >= min && random <= max)
                            {
                                showResult(node);
                                break;
                            }
                        }
                    }
                }
            }
            gm.optionRank = -1;
        }

        //if (_event_num > _event_num_max && type != houseType.garbage_dump)
        ////if (_event_num > _event_num_max)
        //{
        //    GetComponent<BoxCollider>().enabled = false;
        //}
        //else
        //{
        //    GetComponent<BoxCollider>().enabled = true;
        //}

        //if (gm.isDayadd) {
        //    Debug.Log("testttttttttttt");
        //    _event_num = 1;
        //    gm.isDayadd = false;
        //}
    }

    // OnCollisionEnter is called when collision happens
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            istriggered = true;
        }
    }
    
    // OnCollisionExit is called when player goer out of range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            istriggered = false;
        }
    }

    // isIn is to find out one element is in the range or not
    bool isIn(string name, int min, int max) {
        int testee = -1;
        switch (name)
        {
            case "skill":
                {
                    testee = isSkillActive(min.ToString()) ? min : (min + max + 1);
                }
                break;
            case "time":
                {
                    testee = (int)(gm._time / 60.0f);
                }
                break;
            case "day":
                {
                    testee = gm._day;
                }
                break;
            case "liking":
                {
                    testee = liking;
                }
                break;
            case "random":
                {
                    testee = (int)(UnityEngine.Random.value * 100);
                }
                break;
            case "satietyPoint":
                {
                    testee = gm._satietyPoint;
                }
                break;
            case "healthPoint":
                {
                    testee = gm._healthPoint;
                }
                break;
            case "drinkPoint":
                {
                    testee = gm._drinkPoint;
                }
                break;
            case "staminaPoint":
                {
                    testee = gm._staminaPoint;
                }
                break;
            case "cleanPoint":
                {
                    testee = gm._cleanPoint;
                }
                break;
            case "cutePoint":
                {
                    testee = gm._cutePoint;
                }
                break;
            case "survivePoint":
                {
                    testee = gm._survivePoint;
                }
                break;
            case "intelligencePoint":
                {
                    testee = gm._intelligencePoint;
                }
                break;
            case "luckyPoint":
                {
                    testee = gm._luckyPoint;
                }
                break;
        }
        return (testee >= min && testee <= max) ? true : false;
    }

    string doEffect(string Msg)
    {
        string result = "";
        string[] effects = Msg.Split(new string[] { "(", "),(", ")" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string effect in effects)
        {
            string[] map = effect.Split(',');

            switch (map[0])
            {
                case "item":
                    {
                        XmlNodeList items = item.SelectNodes("descendant::row[level=" + map[1] + "]");
                        int index = (int)(UnityEngine.Random.value * items.Count);
                        result = items[index].SelectSingleNode("name").InnerText + "\n" + items[index].SelectSingleNode("description").InnerText;
                        eatEvent("effect", items[index].SelectSingleNode("effect").InnerText);
                    }
                    break;
                case "liking":
                    {
                        changeLiking(Int32.Parse(map[1]));
                    }
                    break;
                case "satietyPoint":
                    {
                        gm._satietyPoint += Int32.Parse(map[1]);
                    }
                    break;
                case "drinkPoint":
                    {
                        gm._drinkPoint += Int32.Parse(map[1]);
                    }
                    break;
                case "healthPoint":
                    {
                        gm._healthPoint += Int32.Parse(map[1]);
                    }
                    break;
                case "survivePoint":
                    {
                        gm._survivePoint += Int32.Parse(map[1]);
                    }
                    break;
                case "cutePoint":
                    {
                        gm._cutePoint += Int32.Parse(map[1]);
                    }
                    break;
                case "intelligencePoint":
                    {
                        gm._intelligencePoint += Int32.Parse(map[1]);
                    }
                    break;
                case "staminaPoint":
                    {
                        gm._staminaPoint += Int32.Parse(map[1]);
                    }
                    break;
                case "cleanPoint":
                    {
                        gm._cleanPoint += Int32.Parse(map[1]);
                    }
                    break;
                case "luckyPoint":
                    {
                        gm._luckyPoint += Int32.Parse(map[1]);
                    }
                    break;
            }
        }
        return result;
    }

    // isSkillActive is to find out the skill is active or not
    bool isSkillActive(string id) {
        bool inTheRange = true;
        XmlNode theSkill = skill.SelectSingleNode("descendant::row[ID=" + id + "]");
        if (theSkill.SelectSingleNode("active").InnerText == "0") inTheRange = false;
        return inTheRange;
    }

    //showEvent will show the event and options
    void showEvent(XmlNode node) {
        options.Clear();
        gm.isEncounter = true;
        gm.eventContent = node.SelectSingleNode("description").InnerText;
        string optionMsg = node.SelectSingleNode("optionID").InnerText;
        string[] optionIDs = optionMsg.Split(',');
        for (int i = 0; i < 4; i++)
        {
            string button_str = "";
            if (i < optionIDs.Length)
            {
                XmlNode theOption = option.SelectSingleNode("descendant::row[ID=" + optionIDs[i] + "]");
                button_str = theOption.SelectSingleNode("name").InnerText;
                string skillname = theOption.SelectSingleNode("isshow").InnerText;
                if (skillname != "" && isSkillActive(skillname)) button_str = "";
                options.Add(Int32.Parse(optionIDs[i]));
            }
            else options.Add(0);
            gm.options[i] = button_str;
        }
    }

    void showResult(XmlNode node) {
        string runto = node.SelectSingleNode("runto").InnerText;
        if (runto == "")
        {
            isKeyDown = false;
            options.Clear();

            gm.isEncounter = true;
            string content = node.SelectSingleNode("description").InnerText + "\n", addition = "";
            if (node.SelectSingleNode("effect").InnerText != "") addition = doEffect(node.SelectSingleNode("effect").InnerText);
            if (node.SelectSingleNode("time").InnerText != "")
            {
                if (addition != "") {
                    content += addition + "\n" + "Do you want to eat it?\n";
                    eatEvent("time", node.SelectSingleNode("time").InnerText);
                    gm.options[0] = "yes";
                    options.Add(eatOptionID_yes);
                    gm.options[1] = "no";
                    options.Add(eatOptionID_no);
                    gm.options[2] = "";
                    options.Add(eatOptionID_yes);
                    gm.options[3] = "";
                    options.Add(eatOptionID_yes);
                }
                else
                {
                    _event_num++;
                    content += "You have consumed " + node.SelectSingleNode("time").InnerText + " minutes\n";
                    gm._time += Int32.Parse(node.SelectSingleNode("time").InnerText);
                    for (int i = 0; i < 4; i++)
                    {
                        gm.options[i] = (i == 0) ? "go on" : "";
                        options.Add((i == 0) ? 0 : -1);
                    }
                }
            }
            gm.eventContent = content;
        }
        else
        {
            node = houseEvent.SelectSingleNode("descendant::row[ID = " + runto + "]");
            showEvent(node);
        }
    }

    void eatEvent(string change, string content) {
        XmlNode option_yes = option.SelectSingleNode("descendant::row[ID=" + eatOptionID_yes.ToString() + "]");
        XmlNode yes = result.SelectSingleNode("descendant::row[ID=" + option_yes.SelectSingleNode("resultID").InnerText + "]");
        switch (change) {
            case "effect": {
                    yes.SelectSingleNode("effect").InnerText = content;
                }break;
            case "time": {
                    yes.SelectSingleNode("time").InnerText = content;
                    XmlNode option_no = option.SelectSingleNode("descendant::row[ID=" + eatOptionID_no.ToString() + "]");
                    XmlNode no = result.SelectSingleNode("descendant::row[ID=" + option_no.SelectSingleNode("resultID").InnerText + "]");
                    no.SelectSingleNode("time").InnerText = content;
                }
                break;
        }
    }

    void changeLiking(int change) {
        liking += change;
        if (liking < 0) liking = 0;
        else if (liking > 100) liking = 100;
    }

    public void SetType(houseType type)
    {
        this.type = type;
    }
}