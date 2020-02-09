using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumeration;

public class BuildingManager : MonoBehaviour
{
    public GameObject cathouse;
    public GameObject chickenfarm;
    public GameObject grocery;
    public GameObject restaurant;
    public GameObject bar;
    public GameObject grandmahouse;
    public GameObject otakuhouse;
    public GameObject familyhouse;
    public GameObject garbagedump;
    public GameObject workerhouse;
    public GameObject bohouse;
    public GameObject rohouse;
    public GameObject gohouse;

    // Start is called before the first frame update
    void Start()
    {
        AddEventTrigger(cathouse, houseType.cathouse);
        AddEventTrigger(chickenfarm, houseType.chicken_farm);
        AddEventTrigger(grocery, houseType.grocery);
        AddEventTrigger(restaurant, houseType.restaurant);
        AddEventTrigger(bar, houseType.bar);
        AddEventTrigger(grandmahouse.gameObject, houseType.grandma);
        AddEventTrigger(otakuhouse, houseType.otaku);
        AddEventTrigger(familyhouse, houseType.family);
        AddEventTrigger(garbagedump, houseType.garbage_dump);
        AddEventTrigger(workerhouse, houseType.worker);
        AddEventTrigger(bohouse, houseType.bar_owner);
        AddEventTrigger(rohouse, houseType.restaurant_owner);
        AddEventTrigger(gohouse, houseType.grocery_owner);
    }

    void AddEventTrigger(GameObject obj, houseType type) 
    {
        GameObject door = obj.transform.Find("door/trigger").gameObject;
        if (!door.GetComponent<BuildingEventTrigger>()) door.AddComponent<BuildingEventTrigger>();
        door.GetComponent<BuildingEventTrigger>().SetType(type);
    }
}
