using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;



public class SkillManager : MonoBehaviour
{
    public GameObject[] skills;
    public bool[] isFull;
    static XmlDocument skillXml;
    XmlNodeList skillList;
    XmlNode currenNode;
    public GameManager gm;
    public Dictionary<string, string> all_Skill = new Dictionary<string, string>(), cute_Skill = new Dictionary<string, string>(), 
        survive_Skill = new Dictionary<string, string>(), intelligence_Skill = new Dictionary<string, string>();
    private string docPath, isLearntValue, isActiveValue;


    private void Start()
    {
        docPath = "Assets/Database/skill.xml";
        isLearntValue = isActiveValue = "0";
        gm = GameManager.Instance;
        //read data from xml
        skillXml = new XmlDocument();
        skillXml.Load(docPath);
        resetSkillXml();

        //all skill table with is learnt <skill ID, is learned>
        skillList = skillXml.SelectNodes("descendant::row");
        string skillId = "",  skillValue = "", isLearnt = "";
        foreach(XmlNode skill in skillList)
        {
            skillId = skill.SelectSingleNode("ID").InnerText; //key
            isLearnt = skill.SelectSingleNode("islearned").InnerText; //value
            all_Skill.Add(skillId, isLearnt);
        }
        loadCuteSkill(skillId, skillValue);
        loadSurSkill(skillId, skillValue);
        loadIntlSkill(skillId, skillValue);
        //no skills displayed in the beginning
        foreach (GameObject skill in skills)
        {
            skill.SetActive(false);
        }
    }
    private void Update()
    {
        //check whether the value trigger a new skill
        //if trigger a new skill, check avaiable skill slot, and add
        string skillId, islearnt;
        int index = checkAvaiableIndex();

        if (gm.optionRank == 3)
        {
            gm.optionRank = -1;
            gm.eventContent = "";
            gm.isEncounter = false;
        }

        if (gm.isEncounter == true) return;
        //check cute
        skillId = checkCutePoint(gm._cutePoint);
        
        if(skillId != "") //there's a new skill can be leanrt
        {
            addSkilltoList(skillId, index);
        }
        //check sur
        skillId = checkSurPoint(gm._survivePoint);
        if (skillId != "")
            addSkilltoList(skillId, index);
        //check intel
        skillId = checkIntlPoint(gm._intelligencePoint);
        if (skillId != "")
            addSkilltoList(skillId, index);


    }

    private void resetSkillXml()
    {
        //reset 
        skillList = skillXml.SelectNodes("descendant::row[islearned='1']");
        foreach (XmlNode node in skillList)
        {
            node.SelectSingleNode("islearned").InnerText = "0";
            skillXml.Save(docPath);
        }

        skillList = skillXml.SelectNodes("descendant::row[active='1']");
        foreach (XmlNode node in skillList)
        {
            node.SelectSingleNode("active").InnerText = "0";
            skillXml.Save(docPath);
        }
    }

    private void loadCuteSkill(string skillId, string skillValue)
    {

        skillList = skillXml.SelectNodes("descendant::row[triggername='cutePoint']");
        //cute skill table <skill ID, req value>
        foreach (XmlNode skill in skillList)
        {
            skillId = skill.SelectSingleNode("ID").InnerText;
            skillValue = skill.SelectSingleNode("value").InnerText;
            cute_Skill.Add(skillId, skillValue);
        }
    }

    private void loadSurSkill(string skillId, string skillValue)
    {
        //survive_Skill table <skill ID, req value>
        skillList = skillXml.SelectNodes("descendant::row[triggername='survivePoint']");
        foreach (XmlNode skill in skillList)
        {
            skillId = skill.SelectSingleNode("ID").InnerText;
            skillValue = skill.SelectSingleNode("value").InnerText;
            survive_Skill.Add(skillId, skillValue);
        }
    }

    private void loadIntlSkill(string skillId, string skillValue)
    {

        //intelligence_Skill table <skill ID, req value>
        skillList = skillXml.SelectNodes("descendant::row[triggername='intelligencePoint']");
        foreach (XmlNode skill in skillList)
        {
            skillId = skill.SelectSingleNode("ID").InnerText;
            skillValue = skill.SelectSingleNode("value").InnerText;
            intelligence_Skill.Add(skillId, skillValue);
        }
    }

    //input: cat local input value
    //output: the new skill id or empty string(no new skill)
    //use cat value to exam whether there's a new skill can be learnt
    //if the skill haven't been learnt, return the skill ID
    public string checkCutePoint(int cat_value)
    {
        foreach (KeyValuePair<string, string> skill in cute_Skill)
        {
            all_Skill.TryGetValue(skill.Key, out isLearntValue);
            if (isLearntValue.Equals("0") && cat_value >= Int32.Parse(skill.Value))
            {
                return skill.Key;
            }
        }
        return ""; //no new skill
    }

    public string checkSurPoint(int cat_value)
    {
        foreach (KeyValuePair<string, string> skill in survive_Skill)
        {
            all_Skill.TryGetValue(skill.Key, out isLearntValue);
            if (isLearntValue.Equals("0") && cat_value >= Int32.Parse(skill.Value))
            {
                return skill.Key;
            }
        }
        return ""; //no new skill
    }

    public string checkIntlPoint(int cat_value)
    {
        foreach (KeyValuePair<string, string> skill in intelligence_Skill)
        {
            all_Skill.TryGetValue(skill.Key, out isLearntValue);
            if (isLearntValue.Equals("0") && cat_value >= Int32.Parse(skill.Value))
            {
                return skill.Key;
            }
        }
        return ""; //no new skill
    }

    //return -1, when the skill list is full
    public int checkAvaiableIndex()
    {
        for(int i = 0; i < isFull.Length; i++)
        {
            if (!isFull[i])
                return i; 
        }
        return -1;
    }

    public void addSkill(int index, string skillID, string name, string description)
    {
        Skill skill = skills[index].GetComponent<Skill>();
        skill.skill_id = skillID;
        skill.skill_name = name;
        skill.description = description; 
        skill.displaySkill();
        isFull[index] = true;
    }

    //input: new skill ID, and the index of the skill bar
    //use the skill id to search node, get name, description, update skill name on the screen
    //modify islearnt and active in xml
    public void addSkilltoList(string skillID, int index)
    {
        string name, description;
        currenNode = skillXml.SelectSingleNode("descendant::row[ID=" + skillID + "]");
        name = currenNode.SelectSingleNode("name").InnerText;
        description = currenNode.SelectSingleNode("description").InnerText;
        if (index == -1) //no avaiable spot
        {
            //remove one skill first,  ???alert???
            //if (gm.isEncounter == true) return; 
            gm.isEncounter = true;
            gm.eventContent = "You can learn a new skill! Click one skill to forget and learn a new skill! ";
            //get input from user
            int clickedIndex = checkClick();
            if (clickedIndex == -1) return;
            else
            {
                addSkill(clickedIndex, skillID, name, description);
            }
        }
        else
        {
            addSkill(index, skillID, name, description);
        }
            //modify xml and all skill
        isLearntValue = isActiveValue = "1";
        all_Skill[skillID] = isLearntValue;
        currenNode.SelectSingleNode("islearned").InnerText = isLearntValue;
        currenNode.SelectSingleNode("active").InnerText = isActiveValue;
        skillXml.Save(docPath);
    }

    //if return -1 -- no button is clicked
    //modify active in xml
    public int checkClick()
    {
        Skill skill;
        foreach(GameObject item in skills) 
        {
            skill = item.GetComponent<Skill>();
            if (skill.isclicked)
            {
                isActiveValue = "0";
                skill.isclicked = false;//reset it
                Debug.Log("The skill will be forget You have clicked: " + skill.index + "ID: " + skill.skill_id);
                currenNode = skillXml.SelectSingleNode("descendant::row[ID=" + skill.skill_id + "]");
                currenNode.SelectSingleNode("active").InnerText = isActiveValue;
                skillXml.Save(docPath);
                return skill.index;
            }
        }
        return -1;
    }
}
