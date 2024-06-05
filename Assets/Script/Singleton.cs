using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if(instance == null)
                {
                    GameObject singletonObj = new GameObject();
                    instance = singletonObj.AddComponent<T>();
                }

            }
            return instance;
        }
    }
}
