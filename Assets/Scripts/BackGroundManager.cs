using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    [SerializeField]private GameObject[] backGrounds;
    [SerializeField] private float backGroundSpeed;

    private float[] backGroundWidths;

    void Start()
    {
        backGroundWidths = new float[backGrounds.Length];

        for (int i = 0; i < backGrounds.Length; i++)
        {
            SpriteRenderer sr = backGrounds[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                backGroundWidths[i] = sr.sprite.bounds.size.x * backGrounds[i].transform.localScale.x;
            }
            else
            {
                backGroundWidths[i] = backGrounds[i].transform.localScale.x;
            }
        }
        
        float currentXPosition = 0f;
        for (int i = 0; i < backGrounds.Length; i++)
        {
            backGrounds[i].transform.position = 
                new Vector3(currentXPosition, backGrounds[i].transform.position.y, backGrounds[i].transform.position.z);
            currentXPosition += backGroundWidths[i];
        }
    }

    void Update()
    {
        for (int i = 0; i < backGrounds.Length; i++)
        {
            backGrounds[i].transform.Translate(Vector2.left * backGroundSpeed * Time.deltaTime);

            if (backGrounds[i].transform.position.x <= -backGroundWidths[i])
            {
                Vector3 rightPosition = GetRightBackGroundPosition();
                backGrounds[i].transform.position = 
                    new Vector3(rightPosition.x + backGroundWidths[i], backGrounds[i].transform.position.y, backGrounds[i].transform.position.z);
            }
        }
    }
    
    private Vector3 GetRightBackGroundPosition()
    {
        Vector3 rightPosition = backGrounds[0].transform.position;

        for (int i = 1; i < backGrounds.Length; i++)
        {
            if (backGrounds[i].transform.position.x > rightPosition.x)
            {
                rightPosition = backGrounds[i].transform.position;
            }
        }
        return rightPosition;
    }
}
