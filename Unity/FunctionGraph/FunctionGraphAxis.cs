using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FunctionGraphAxis : MonoBehaviour
{
    public int length;

    public GameObject mark;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
    }

    public void SetLength(float len)
    {
        transform.localScale = new Vector3(1, len, 1);
        mark.transform.localPosition = new Vector3(0, len * 0.5f, 0);
    }
}
