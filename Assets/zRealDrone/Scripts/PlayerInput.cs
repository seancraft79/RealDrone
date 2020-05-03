using UnityEngine;
using System.Collections;


public class PlayerInput : MonoBehaviour
{

    public VariableJoystick joystickLeft, joystickRight;

    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement, m_HorizontalMovement, m_VerticalMovement;
    protected Vector2 m_Camera;
    protected bool m_Jump;
    protected bool m_Attack;
    protected bool m_Pause;
    protected bool m_ExternalInputBlocked;

    public Vector2 MoveInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 HorizontalMoveInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_HorizontalMovement;
        }
    }

    public Vector2 VerticalMoveInput
    {
        get
        {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_VerticalMovement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput
    {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Attack
    {
        get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause
    {
        get { return m_Pause; }
    }

    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;

    const float k_AttackInputDuration = 0.03f;

    void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }


    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Keyboard value
        float horizontalUpDownKey = 0;
        float horizontalLeftRightKey = 0;
        float verticalUpDownKey = 0;
        float verticalLeftRightKey = 0;

        //if (Input.GetKey(KeyCode.UpArrow)) horizontalUpDownKey = 1f;
        //else if (Input.GetKey(KeyCode.DownArrow)) horizontalUpDownKey = -1f;

        //if (Input.GetKey(KeyCode.RightArrow)) horizontalLeftRightKey = 1f;
        //else if (Input.GetKey(KeyCode.LeftArrow)) horizontalLeftRightKey = -1f;

        //if (Input.GetKey(KeyCode.W)) verticalUpDownKey = 1f;
        //else if (Input.GetKey(KeyCode.S)) verticalUpDownKey = -1f;

        //if (Input.GetKey(KeyCode.A)) verticalLeftRightKey = 1f;
        //else if (Input.GetKey(KeyCode.D)) verticalLeftRightKey = -1f;

        //m_HorizontalMovement.Set(horizontalLeftRightKey, horizontalUpDownKey);
        //m_VerticalMovement.Set(verticalLeftRightKey, verticalUpDownKey);



        // Joystic value
        horizontalLeftRightKey = joystickRight.Horizontal;
        if (horizontalLeftRightKey >= 0.5) horizontalLeftRightKey = 1;
        else if (horizontalLeftRightKey <= -0.5) horizontalLeftRightKey = -1;
        else horizontalLeftRightKey = 0;

        horizontalUpDownKey = joystickRight.Vertical;
        if (horizontalUpDownKey >= 0.5) horizontalUpDownKey = 1;
        else if (horizontalUpDownKey <= -0.5) horizontalUpDownKey = -1;
        else horizontalUpDownKey = 0;

        verticalLeftRightKey = joystickLeft.Horizontal * -1;
        if (verticalLeftRightKey >= 0.5) verticalLeftRightKey = 1;
        else if (verticalLeftRightKey <= -0.5) verticalLeftRightKey = -1;
        else verticalLeftRightKey = 0;

        verticalUpDownKey = joystickLeft.Vertical;
        if (verticalUpDownKey >= 0.5) verticalUpDownKey = 1;
        else if (verticalUpDownKey <= -0.5) verticalUpDownKey = -1;
        else verticalUpDownKey = 0;

        m_HorizontalMovement.Set(horizontalLeftRightKey, horizontalUpDownKey);
        m_VerticalMovement.Set(verticalLeftRightKey, verticalUpDownKey);


        //Debug.Log($"Left : {m_VerticalMovement.x},{m_VerticalMovement.y}, Right : {m_HorizontalMovement.x},{m_HorizontalMovement.y}");

        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Jump = Input.GetButton("Jump");

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        m_Pause = Input.GetButtonDown ("Pause");
    }

    IEnumerator AttackWait()
    {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }

    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }
}
