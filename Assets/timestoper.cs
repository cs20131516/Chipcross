using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timestoper : MonoBehaviour
{

    public float timeLeft = 7f;
    bool timebool = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timebool)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timebool = false;
            }
        }

    }

    public void last()
    {
        timeLeft = 7f;
        timebool = true;
    }

}
