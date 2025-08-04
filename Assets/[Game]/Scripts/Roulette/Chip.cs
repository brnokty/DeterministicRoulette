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
            // Eğer önceden BetArea’ya eklendiyse eski yerinden kaldır
            if (transform.parent != null)
            {
                BetArea prevArea = transform.parent.GetComponent<BetArea>();
                if (prevArea != null)
                {
                    prevArea.RemoveChip(this);
                }
                transform.parent = null;
            }
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
            BetArea closest = null;
            float minDist = float.MaxValue;

            // En yakın BetArea'yı ve mesafesini bul
            foreach (var ba in FindObjectsOfType<BetArea>())
            {
                float dist = Vector3.Distance(mousePos, ba.transform.position);
                if (dist < ba.snapRange && dist < minDist)
                {
                    closest = ba;
                    minDist = dist;
                }
            }

            if (closest != null)
            {
                // Snaple ve stacklenmiş yüksekliğe yerleştir
                int stackCount = closest.GetChipCount(); // Şu an kaç chip var?
                Vector3 snapPos = closest.transform.position + Vector3.up * (0.004f + 0.008f * stackCount);
                transform.position = snapPos;
                currentSnapArea = closest;
            }
            else
            {
                // Snap yoksa mouse’un ucunda hareket ettir
                transform.position = new Vector3(mousePos.x, 0.2f, mousePos.z);
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
            // Pozisyonu tekrar stack’li olarak güncelle (diğer chiplerle üst üste)
            int stackCount = currentSnapArea.GetChipCount() - 1; // Az önce eklendiği için -1
            Vector3 snapPos = currentSnapArea.transform.position + Vector3.up * (0.004f + 0.008f * stackCount);
            transform.position = snapPos;
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
