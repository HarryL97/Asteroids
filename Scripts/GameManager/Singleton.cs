using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<U>: MonoBehaviour where U : MonoBehaviour
{
    private static U instance;

    public static U Instance
    {

       get
        {
           if (instance == null)
            {
                instance = FindObjectOfType<U>();
            }
                else if (instance != FindObjectOfType<U>())
            {
                Destroy(FindObjectOfType<U>()); 
            }    

            // KOWNISSUE throws up warning messages on I destroy other Objects
            DontDestroyOnLoad(FindObjectOfType<U>().gameObject);


            return instance;
        }
    }


}
