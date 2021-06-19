using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Música : MonoBehaviour
{
    public static Música music;

    void Start()
    {
        music = this;
        DontDestroyOnLoad(transform.gameObject);
        var objects = FindObjectsOfType<Música>();
        if(objects.Length > 1){
            Destroy(objects[objects.Length - 1].gameObject);
        }
    }

}
