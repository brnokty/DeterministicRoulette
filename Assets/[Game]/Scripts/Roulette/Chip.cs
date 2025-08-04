using Game.Core;
using UnityEngine;

public class Chip : MonoBehaviour
{
    public int value;
    private bool isDragging = false;
    private Camera cam;
    private BetArea currentSnapArea = null;
    public static bool anyDragging = false;

    public void Init(int val)
    {
        value = val;
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
                    GameManager.Instance.SetBalance(GameManager.Instance.Balance + value); // Para iade!
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
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseHitPosition();
            BetArea closest = null;
            float minDist = float.MaxValue;

            foreach (var ba in FindObjectsOfType<BetArea>(true))
            {
                float dist = Vector3.Distance(mousePos, ba.transform.position);
                if (dist < 0.03f && dist < minDist)
                {
                    closest = ba;
                    minDist = dist;
                    if (closest != currentSnapArea)
                        SoundManager.Instance.PlaySound(SoundManager.SoundType.ChipMovement);
                }
            }

            if (closest != null)
            {
                int stackCount = closest.GetChipCount();
                Vector3 snapPos = closest.transform.position + Vector3.up * (0.004f + 0.008f * stackCount);
                transform.position = snapPos;
                currentSnapArea = closest;
            }
            else
            {
                transform.position = new Vector3(mousePos.x, mousePos.y, mousePos.z);
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
            int stackCount = currentSnapArea.GetChipCount() - 1;
            Vector3 snapPos = currentSnapArea.transform.position + Vector3.up * (0.004f + 0.008f * stackCount);
            transform.position = snapPos;

            GameManager.Instance.SetBalance(GameManager.Instance.Balance - value); // Para azalt!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask hitLayers = Physics.DefaultRaycastLayers;

    private Vector3 GetMouseHitPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
            return hit.point;
        return Vector3.zero;
    }
}