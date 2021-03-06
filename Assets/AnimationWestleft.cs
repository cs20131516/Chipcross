﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWestleft : MonoBehaviour
{
    Animator anim;
    [HideInInspector]
    public int piece = 1;
    public TutorialAnimationFixed Tutofix;
    bool centercheck = false;
    [HideInInspector]
    public bool teleport = false;
    [HideInInspector]
    public bool tile8preference = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        teleport = false;
    }

    // Update is called once per frame
    void Update()
    {
     if(PlayerPrefs.GetInt("tutorial") == 0)
        {
            anim.enabled = false;
        }
     else if(PlayerPrefs.GetInt("tutorial") == 1 && PlayerPrefs.GetInt("Piecedata") == 1)
        {
            anim.enabled = true;
            switch (piece)
            {
                case 1: 
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 3);
                    break;
                case 2:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 5);
                    centercheck = false;
                    break;
                case 3:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 2);
                    break;
                case 4:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 7);
                    break;
            }
        }
     else if(PlayerPrefs.GetInt("tutorial") == 1 && PlayerPrefs.GetInt("Piecedata") == 2)
        {
            anim.enabled = true;
            switch (piece)
            {
                case 1:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 5);
                    break;
                case 2:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 7);
                    break;
                case 3:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 5);
                    break;
                case 4:
                    Tutofix.Noprefabyet = true;
                    anim.SetInteger("Tilenumber", 7);
                    break;
            }
        }
        else// teleport
        {
            if (teleport==false)
            {
                Tutofix.Noprefabyet = true;
                anim.SetInteger("Tilenumber", 8);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Eastright")
        {
            piece++;
            Tutofix.Noprefabyet = true;
            if (piece == 5)
                piece = 1;
            if (teleport)
                teleport = false;
        }
        else if (collision.gameObject.name == "North")
        {
            piece++;
            Tutofix.Noprefabyet = true;
            if (piece == 5)
                piece = 1;
        }
        else if (collision.gameObject.name == "South")
        {
            piece++;
            Tutofix.Noprefabyet = true;
            if (piece == 5)
                piece = 1;
        }
        else if (collision.gameObject.name == "Center" && centercheck == false && piece == 3)
        {
            centercheck = true;
        }
        else if (collision.gameObject.name == "Westright" && centercheck && piece == 3)
        {
            piece++;
            Tutofix.Noprefabyet = true;
            if (piece == 5)
                piece = 1;
        }
        if (collision.gameObject.name == "Westright" && teleport == false && tile8preference)
        {
            //순간이동
            teleport = true;
        }
        else if (collision.gameObject.name == "Eastleft" && teleport && tile8preference)
        {
            teleport = false;
        }
    }
}
