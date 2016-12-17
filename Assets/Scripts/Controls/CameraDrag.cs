using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDrag : MonoBehaviour {
    private Vector3 lastPoint;

    public float zoomRate = 0.9f;
    public float minZoom = 2;
    public float maxZoom = 512;
    public float maxPan = 512;

    void Update() 
    {
        if (Input.mouseScrollDelta.y > 0 && GetComponent<Camera>().orthographicSize > minZoom) 
        {
            GetComponent<Camera>().orthographicSize *= zoomRate;
            Vector3 newPoint = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            newPoint = Vector3.Lerp(newPoint, transform.position, zoomRate);
            transform.position = newPoint;
        }
        else if (Input.mouseScrollDelta.y < 0 && GetComponent<Camera>().orthographicSize < maxZoom) 
        {
            GetComponent<Camera>().orthographicSize /= zoomRate;
            Vector3 newPoint = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            newPoint = transform.position - newPoint;
            newPoint += transform.position;
            newPoint = Vector3.Lerp(newPoint, transform.position, zoomRate);
            transform.position = newPoint;
        }


        if (Input.GetMouseButtonDown(2)) {
            lastPoint = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 dir = lastPoint - GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        transform.Translate(dir);

        Vector2 maxPanScreen = new Vector2(maxPan * GetComponent<Camera>().aspect, maxPan);

        transform.position = transform.position.WithX(Mathf.Clamp(transform.position.x, -maxPanScreen.x, maxPanScreen.x));
        transform.position = transform.position.WithY(Mathf.Clamp(transform.position.y, -maxPanScreen.y, maxPanScreen.y));

        lastPoint = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
    }


}