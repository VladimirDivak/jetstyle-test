using UnityEngine;

public class JetStyleTest : MonoBehaviour
{
    [SerializeField]
    public PlayerControllerMovement controllerInstance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}