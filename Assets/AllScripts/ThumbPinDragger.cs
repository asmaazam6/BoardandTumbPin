using UnityEngine;
using UnityEngine.EventSystems; // 1. Add this namespace!

// 2. Implement the Pointer interfaces
public class ThumbPinDragger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoord;

    [Header("Placement Settings")]
    [SerializeField] private LayerMask boardLayer;
    [SerializeField] private float insertionDepth = 0.02f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDragging)
        {
            DragPin();
        }
    }

    // 3. Replace OnMouseDown with this
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[Pin Clicked via Pointer] Successfully clicked on: {gameObject.name}", this);
        isDragging = true;
        zCoord = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    // 4. Replace OnMouseUp with this
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("[Pin Released via Pointer] Mouse released.", this);
        isDragging = false;
        TryInsertPin();
    }

    private void DragPin()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    
    private void TryInsertPin()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // This will now hit because the objects are on the correct layer
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, boardLayer))
        {
            // Double-check names or tags safely
            if (hit.collider.CompareTag("Paper") || hit.collider.name.Contains("DrawingBoard"))
            {
                Debug.Log("<color=green>[Success]</color> Pin conditions met! Inserting pin.", this);

                // 1. Align the rotation to match the surface normal perfectly
                transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                // 2. Position the pin directly at the surface hit point
                Vector3 targetPosition = hit.point;

                // 3. Sink it straight DOWN along the surface normal (negative goes into the board)
                // Adjust insertionDepth in the Inspector to control the depth!
                targetPosition -= hit.normal * insertionDepth;

                transform.position = targetPosition;
            }
        }
        else
        {
            Debug.LogWarning("[Insertion Failed] Raycast did not hit any object on the specified Board Layer.", this);
        }
    }
}