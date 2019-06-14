using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector3 lastPosition;

    const float leftEdge = 0;
    const float rightEdge = 70;

    bool m_isMeTouched = false;
    
    private void OnMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Camera.main.transform.forward);
        m_isMeTouched = hit.transform.Equals(transform);
        
        Debug.Log("OnMouseDown : " + hit.transform.name);
        lastPosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        if (!m_isMeTouched)
            return;
        
        Debug.Log("OnMouseDrag");
        Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition)  - Camera.main.ScreenToWorldPoint(lastPosition);
        distance = new Vector3(distance.x, 0, 0);
        lastPosition = Input.mousePosition;
    
        Camera.main.transform.Translate(distance, Space.World);

        if (Camera.main.transform.position.x < leftEdge && distance.x < 0)
            Camera.main.transform.position = new Vector3(leftEdge, Camera.main.transform.position.y, Camera.main.transform.position.z);
 
        if (Camera.main.transform.position.x > rightEdge && distance.x > 0)
            Camera.main.transform.position = new Vector3(rightEdge, Camera.main.transform.position.y, Camera.main.transform.position.z);

    }

    private void OnMouseUp()
    {
        m_isMeTouched = false;
    }
}
