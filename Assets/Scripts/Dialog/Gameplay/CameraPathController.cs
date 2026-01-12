using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CameraPathController : MonoBehaviour
{
    public enum CameraMode
    {
        Static,
        FollowPlayer,
        FollowPath,
        LookAtTarget,
        SmoothTransition
    }

    [Header("Camera Mode")]
    [SerializeField] private CameraMode currentMode = CameraMode.Static;
    [SerializeField] private CameraMode defaultMode = CameraMode.FollowPlayer;

    [Header("Camera References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraPivot; // Добавляем это поле

    [Header("Follow Settings")]
    [SerializeField] private Vector3 followOffset = new Vector3(0, 10, -10);
    [SerializeField] private float followSmoothness = 5f;
    [SerializeField] private float rotationSmoothness = 3f;

    [Header("Path Settings")]
    [SerializeField] private List<Transform> pathPoints = new List<Transform>();
    [SerializeField] private float pathSpeed = 2f;
    [SerializeField] private bool loopPath = false;
    [SerializeField] private bool lookAtNextPoint = true;

    [Header("Transition Settings")]
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float transitionDuration = 2f;

    [Header("Cinematic Settings")]
    [SerializeField] private float cinematicFOV = 40f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float fovTransitionSpeed = 3f;

    private int currentPathIndex = 0;
    private bool isMovingForward = true;
    private bool isTransitioning = false;
    private Transform cameraPivotRef; // Ссылка на созданный pivot

    public static CameraPathController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCamera();
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void InitializeCamera()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Создаем cameraPivot если его нет
        if (cameraPivot == null)
        {
            GameObject pivotObj = new GameObject("CameraPivot");
            cameraPivotRef = pivotObj.transform;
            cameraPivotRef.position = mainCamera.transform.position;
            cameraPivotRef.rotation = mainCamera.transform.rotation;
        }
        else
        {
            cameraPivotRef = cameraPivot;
        }
    }

    void SubscribeToEvents()
    {
        GameEvents.OnCameraTransitionRequested += HandleCameraTransitionRequested;
    }

    void UnsubscribeFromEvents()
    {
        GameEvents.OnCameraTransitionRequested -= HandleCameraTransitionRequested;
    }

    void Update()
    {
        if (isTransitioning) return;

        switch (currentMode)
        {
            case CameraMode.FollowPlayer:
                FollowPlayer();
                break;

            case CameraMode.FollowPath:
                FollowPath();
                break;

            case CameraMode.LookAtTarget:
                LookAtTarget();
                break;
        }
    }

    void FollowPlayer()
    {
        if (playerTransform == null) return;

        Vector3 targetPosition = playerTransform.position + followOffset;

        cameraPivotRef.position = Vector3.Lerp(
            cameraPivotRef.position,
            targetPosition,
            Time.deltaTime * followSmoothness
        );

        Quaternion targetRotation = Quaternion.LookRotation(
            playerTransform.position - mainCamera.transform.position
        );

        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothness
        );

        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            normalFOV,
            Time.deltaTime * fovTransitionSpeed
        );
    }

    void FollowPath()
    {
        if (pathPoints.Count == 0) return;

        Transform targetPoint = pathPoints[currentPathIndex];

        cameraPivotRef.position = Vector3.MoveTowards(
            cameraPivotRef.position,
            targetPoint.position,
            pathSpeed * Time.deltaTime
        );

        if (lookAtNextPoint)
        {
            int nextIndex = isMovingForward ?
                (currentPathIndex + 1) % pathPoints.Count :
                (currentPathIndex - 1 + pathPoints.Count) % pathPoints.Count;

            if (nextIndex >= 0 && nextIndex < pathPoints.Count)
            {
                Vector3 direction = pathPoints[nextIndex].position - mainCamera.transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                mainCamera.transform.rotation = Quaternion.Slerp(
                    mainCamera.transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSmoothness
                );
            }
        }

        if (Vector3.Distance(cameraPivotRef.position, targetPoint.position) < 0.1f)
            GetNextPathPoint();

        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            cinematicFOV,
            Time.deltaTime * fovTransitionSpeed
        );
    }

    void GetNextPathPoint()
    {
        if (loopPath)
        {
            currentPathIndex = (currentPathIndex + 1) % pathPoints.Count;
        }
        else
        {
            if (isMovingForward)
            {
                if (currentPathIndex < pathPoints.Count - 1)
                    currentPathIndex++;
                else
                {
                    isMovingForward = false;
                    currentPathIndex--;
                }
            }
            else
            {
                if (currentPathIndex > 0)
                    currentPathIndex--;
                else
                {
                    isMovingForward = true;
                    currentPathIndex++;
                }
            }
        }
    }

    void LookAtTarget()
    {
        if (playerTransform == null) return;

        Quaternion targetRotation = Quaternion.LookRotation(
            playerTransform.position - mainCamera.transform.position
        );

        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothness
        );
    }

    void HandleCameraTransitionRequested(Transform target)
    {
        MoveToNextLevelArea(target, () => {
            GameEvents.InvokeCameraTransitionComplete();
        });
    }

    // Публичные методы
    public void MoveToNextLevelArea(Transform target, System.Action onComplete = null)
    {
        if (isTransitioning) return;

        StartCoroutine(SmoothTransitionToTarget(target, onComplete));
    }

    public void StartPathFollowing()
    {
        if (pathPoints.Count == 0)
        {
            Debug.LogWarning("No path points assigned!");
            return;
        }

        SetCameraMode(CameraMode.FollowPath);
        currentPathIndex = 0;
        isMovingForward = true;
    }

    public void StopPathFollowing()
    {
        SetCameraMode(defaultMode);
    }

    public void SetCameraMode(CameraMode newMode)
    {
        if (isTransitioning) return;

        currentMode = newMode;

        switch (newMode)
        {
            case CameraMode.Static:
                break;

            case CameraMode.FollowPlayer:
                mainCamera.fieldOfView = normalFOV;
                break;

            case CameraMode.FollowPath:
                mainCamera.fieldOfView = cinematicFOV;
                break;
        }
    }

    public void AddPathPoint(Transform newPoint)
    {
        if (!pathPoints.Contains(newPoint))
            pathPoints.Add(newPoint);
    }

    public void ClearPathPoints()
    {
        pathPoints.Clear();
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    // Корутины
    private IEnumerator SmoothTransitionToTarget(Transform target, System.Action onComplete)
    {
        isTransitioning = true;

        Vector3 transitionStartPos = cameraPivotRef.position;
        Quaternion transitionStartRot = mainCamera.transform.rotation;
        float transitionStartTime = Time.time;
        float startFOV = mainCamera.fieldOfView;

        while (Time.time - transitionStartTime < transitionDuration)
        {
            float t = (Time.time - transitionStartTime) / transitionDuration;
            float curvedT = transitionCurve.Evaluate(t);

            cameraPivotRef.position = Vector3.Lerp(transitionStartPos, target.position, curvedT);

            Quaternion targetRotation = Quaternion.LookRotation(target.forward);
            mainCamera.transform.rotation = Quaternion.Slerp(transitionStartRot, targetRotation, curvedT);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, cinematicFOV, curvedT);

            yield return null;
        }

        cameraPivotRef.position = target.position;
        mainCamera.transform.rotation = Quaternion.LookRotation(target.forward);
        isTransitioning = false;

        onComplete?.Invoke();
    }

    public IEnumerator ShakeCamera(float duration = 0.5f, float magnitude = 0.2f)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
}