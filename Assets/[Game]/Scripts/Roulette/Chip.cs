using UnityEngine;

public class Chip : MonoBehaviour
{
    public int value;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;
    private BetArea currentSnapArea = null;
    public static bool anyDragging = false;

    public void Init(int val)
    {
        // value = val;
        cam = Camera.main;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (!isDragging && !anyDragging)
        {
            StartDragging();
        }
    }

    public void StartDragging()
    {
        isDragging = true;
        anyDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos() + offset;
            transform.position = new Vector3(mousePos.x, 0.2f, mousePos.z);

            BetArea closest = FindClosestBetArea();
            if (closest != null)
            {
                transform.position = closest.GetSnapPosition();
                currentSnapArea = closest;
            }
            else
            {
                currentSnapArea = null;
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }

    void StopDragging()
    {
        isDragging = false;
        anyDragging = false;
        if (currentSnapArea != null)
        {
            currentSnapArea.AddChip(this);
            transform.position = currentSnapArea.GetSnapPosition();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float dist;
        if (plane.Raycast(ray, out dist))
            return ray.GetPoint(dist);
        return Vector3.zero;
    }

    BetArea FindClosestBetArea()
    {
        BetArea[] all = FindObjectsOfType<BetArea>();
        BetArea best = null;
        float minDist = float.MaxValue;
        foreach (var ba in all)
        {
            float d = Vector3.Distance(transform.position, ba.GetSnapPosition());
            if (d < ba.snapRange && d < minDist)
            {
                minDist = d;
                best = ba;
            }
        }
        return best;
    }
}
