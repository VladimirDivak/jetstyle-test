using System.Collections;
using UnityEngine;

public class PlayerControllerMovement : MonoBehaviour
{
    [SerializeField]
    private float MovingSpeed;
    [SerializeField]
    private float MouseRotationSpeed;
    [SerializeField]
    private Hand Hand;

    private Transform _playerTransform;
    private Rigidbody _rigidBody;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;

    private float _mouseXRot;
    private float _mouseYRot;

    private bool _isMoving;
    private bool _isSpellMode;

    private Coroutine C_Moving;
    private Coroutine C_Rotating;

    void Awake()
    {
        var _mainScript = FindObjectOfType<JetStyleTest>();

        if(_mainScript.controllerInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _mainScript.controllerInstance = this;
    }

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _playerTransform = transform;

        _mainCamera = Camera.main;
        _mainCameraTransform = _mainCamera.transform;

        C_Moving = StartCoroutine(MovingRoutine());
        C_Rotating = StartCoroutine(RotatingRotinge());
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Hand.SetSpellMode(true);
        }

        if(Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Hand.SetSpellMode(false);
        }
    }

    IEnumerator MovingRoutine()
    {
        while(true)
        {
            if(!Hand.isSpellMode)
            {
                var cameraForward = new Vector3(_mainCameraTransform.forward.x,
                0,
                _mainCameraTransform.forward.z).normalized;

                var playerHorizontalDir = Input.GetAxis("Horizontal");
                var playerVerticalDir = Input.GetAxis("Vertical");
                var velocity = playerVerticalDir + playerHorizontalDir;

                var moveSpeed = MovingSpeed * Time.deltaTime;

                var direction = cameraForward * playerVerticalDir
                + _mainCameraTransform.transform.right * playerHorizontalDir;

                Vector3 movePosition = Vector3.Lerp(_playerTransform.position,
                    _playerTransform.position + direction,
                    moveSpeed);

                _rigidBody.MovePosition(movePosition);
                
                if(velocity != 0)
                {
                    if(!_isMoving)
                    {
                        _isMoving = true;
                        Hand.SetMovingAnimation(true);
                    }
                }
                else
                {
                    if(_isMoving)
                    {
                        _isMoving = false;
                        Hand.SetMovingAnimation(false);
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator RotatingRotinge()
    {
        while(true)
        {
            if(!Hand.isSpellMode)
            {
                var mouseSpeed = MouseRotationSpeed * Time.deltaTime;

                var mouseX = Input.GetAxis("Mouse X");
                var mouseY = Input.GetAxis("Mouse Y");

                Vector3 mouseRotation;

                _mouseYRot += mouseX;
                _mouseXRot -= mouseY;
                _mouseXRot = Mathf.Clamp(_mouseXRot, -90, 90);

                mouseRotation = new Vector3(_mouseXRot, _mouseYRot, 0);
                _mainCameraTransform.localRotation = Quaternion.Euler(mouseRotation);
            }

            yield return null;
        }
    }
}