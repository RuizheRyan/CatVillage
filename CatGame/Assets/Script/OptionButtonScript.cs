using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonScript : MonoBehaviour
{
    public int rank;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            gameManager.ClickOption(rank);
        });
    }
}
