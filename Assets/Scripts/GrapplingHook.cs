using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform hook;
    private Vector2 mousedir;

    private bool isHook;
    private bool isLineMax;
    void Start()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.05f;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hook.position);
        line.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCenter = transform.position + new Vector3(0f, 0.8f, 0f);
        line.SetPosition(0, playerCenter);
        line.SetPosition(1, hook.position);

        if (Input.GetMouseButtonDown(0) && !isHook)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousedir = (mousePos - transform.position).normalized;
            isHook = true;
            hook.gameObject.SetActive(true);
            isLineMax = false;
            
            hook.Translate(mousedir * Time.deltaTime * 10f);
        }

        if (isHook && !isLineMax)
        {
            hook.Translate(mousedir.normalized * Time.deltaTime * 10f);

            if (Vector2.Distance(transform.position, hook.position) > 5f)
            {
                isLineMax = true;
            }
        }
        else if (isHook && isLineMax)
        {
            hook.position = Vector2.MoveTowards(hook.position, transform.position, Time.deltaTime * 10f);
            if (Vector2.Distance(transform.position, hook.position) < 0.1f)
            {
                isHook = false;
                hook.gameObject.SetActive(false);
                isLineMax = false;
            }
        }
    }
}
