using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class Skill : MonoBehaviour
{
    public bool isclicked;
    public string skill_id, skill_name, description;
    public Button button;
    public int index;
    public GameManager gm;
    private int opt1 = 0;
    private int opt2 = 3;
    private void Start()
    {
        gm = GameManager.Instance;
        Button btn = button.GetComponent<Button>();
        btn.onClick.AddListener(clicked);
    }
    void clicked()
    {
        Debug.Log("You have clicked: " + index);
        isclicked = true;
    }
    public void displaySkill()
    {
            gameObject.GetComponentInChildren<Text>().text = this.skill_name;
            gm.isEncounter = true;
            gm.eventContent = "New Skill!! Skill Description: " + description;
            gm.options[opt2] = "OK";
            gameObject.SetActive(true);
    }
}
