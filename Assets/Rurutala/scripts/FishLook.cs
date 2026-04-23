using System;
using UnityEngine;

/// <summary>
/// 魚を回転する
/// </summary>
public class FishLook : MonoBehaviour
{
    [Header("回転設定")]
    public float rotationSpeed = 2.0f;
    public bool invertX = false;
    public bool invertY = false;
    
    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Camera mainCamera;
    
    private Quaternion initialRotation;

    private void Awake()
    {
        initialRotation = transform.rotation;
    }

    private void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            var col = gameObject.AddComponent<SphereCollider>();
            col.radius = 1.5f;
        }
        
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
    }
    
    private void OnEnable()
    {
        // 魚の回転を初期状態にリセット
        transform.rotation = initialRotation;
    
        // ドラッグ状態もリセット
        isDragging = false;
    }
    
    private void Update()
    {
        HandleMouseInput();
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // マウスクリック時に魚がクリックされたかチェック
            if (IsMouseOverFish())
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
        if (isDragging && Input.GetMouseButton(0))
        {
            RotateFish();
        }
    }
    
    private bool IsMouseOverFish()
    {
        if (mainCamera == null) return false;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.IsChildOf(transform) || hit.transform == transform;
        }
        
        return false;
    }
    
    private void RotateFish()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 deltaPosition = currentMousePosition - lastMousePosition;

        // X軸とY軸の回転量を計算
        float rotationX = deltaPosition.y * rotationSpeed;
        float rotationY = deltaPosition.x * rotationSpeed;

        // 反転設定を適用
        if (invertX) rotationX = -rotationX;
        if (invertY) rotationY = -rotationY;

        // 魚を回転（縦・横回転の方向を統一）
        transform.Rotate(rotationX, -rotationY, 0, Space.World);

        lastMousePosition = currentMousePosition;
    }
    
    private void OnDisable()
    {
        isDragging = false;
    }
}