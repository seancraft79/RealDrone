using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public const int
        DRONE_STATE_DISARMED = 0,
        DRONE_STATE_ARMED    = 1,
        DRONE_STATE_FLYING   = 2,
        DRONE_STATE_FALLING  = 3;
    
    /// <summary>
    /// 드론 날개 회전값
    /// </summary>
    private float ROTER_SPIN = 30f;

    /// <summary>
    /// 비행가능 최저 배터리량
    /// </summary>
    public const int BATTERY_LIMIT = 30;

    public const int BATTERY_WARNING = 60;

    /// <summary>
    /// 비행 고도 제하
    /// </summary>
    public const int HEIGHT_LIMIT = 8;
    public const int HEIGHT_WARNING = 7;

    /// <summary>
    /// Horizontal move 속도 제한
    /// </summary>
    const float MAX_SPEED = 16f;

    /// <summary>
    /// Yaw 회전 속도 제한
    /// </summary>
    const float MAX_SPEED_YAW_ROTATION = 1f;

    /// <summary>
    /// 추락 속도 제한
    /// </summary>
    const float FALL_SPPED = -16f;

    /// <summary>
    /// Horizontal 움직일때 뱅킹 각도 제
    /// </summary>
    const float MAX_ROTATE_Z = 16f;            // Whether or not Ellen can swing her staff.

    /// <summary>
    /// Horizontal move 가속도 증가
    /// </summary>
    const float ACCELERATION = 20f;
    /// <summary>
    /// Horizontal move 가속도 감소
    /// </summary>
    const float DECELERATION = -20f;

    /// <summary>
    /// Yaw 회전 가속도 증
    /// </summary>
    const float ACCELERATION_YAW = 10f;
    /// <summary>
    /// Yaw 회전 가속도 감소
    /// </summary>
    const float DECELERATION_YAW = -10f;

    /// <summary>
    /// 실제 드론 오브젝트
    /// </summary>
    public Transform droneBody;

    /// <summary>
    /// 드론 프리펩 배열
    /// </summary>
    public GameObject[] dronePrefabs;
    
    /// <summary>
    /// 드론 스폰 위치
    
    /// </summary>
    Vector3 droneIdentityPos = Vector3.zero;

    /// <summary>
    /// 드론 날개
    /// </summary>
    public List<Transform> roters;

    private bool isRoterRotate = false;

    public bool isGame = false;

    public CameraFollow cameraFollow;
    public DroneLevel1Controller gameController;
    
    [SerializeField]
    protected float m_HorizontalXSpeed, m_HorizontalYSpeed, m_VerticalXSpeed, m_VerticalYSpeed;
    protected float m_VerticalSpeed;
    protected float m_DesiredHorizontalXSpeed, m_DesiredHorizontalYSpeed, m_DesiredVerticalXSpeed, m_DesiredVerticalYSpeed;

    CharacterController m_CharCtrl;

    protected PlayerInput m_Input;

    private float droneY;

    public int DroneState { get; set; } = DRONE_STATE_FLYING;

    /// <summary>
	/// Height of flying drone
	/// </summary>
	public float DroneHeight { get; set; }

    public int DroneYaw { get; set; }

    public int DroneBattery { get; set; }

    private float timeSec = 0;
    private bool shouldUpdateTime = true;

    /// <summary>
	/// Distance of drone from home
	/// </summary>
	public float DroneDistanceFromHome { get; set; }

    /// <summary>
    /// 제한 고도에 닿은 상태인
    /// </summary>
    public bool IsHeightLimitExeed { get; set; } = false;

    private AudioSource audioSource, droneFlyingAudio, droneCrashAudio;

    // Audio clips
    public AudioClip acSpawn, acCrash, acFlying, acAquired, acSuccess;

    // Fail particle effect
    public ParticleSystem failParticle;
    
    Action<ControllerColliderHit> droneHitObjListener;

    public void AddHitObjListener(Action<ControllerColliderHit> listener)
    {
        droneHitObjListener += listener;
    }

    protected bool IsMoveInput
    {
        get { return !Mathf.Approximately(m_Input.MoveInput.sqrMagnitude, 0f); }
    }

    private void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        m_CharCtrl = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Debug.Log("DroneController ... Start dronType: " + GameManager.Instance.DroneType);
        SetDroneByType();
        
        m_CharCtrl.detectCollisions = true;
        if (droneIdentityPos.Equals(Vector3.zero))
            droneIdentityPos = transform.position;
        droneY = transform.rotation.y;

        droneFlyingAudio = gameObject.AddComponent<AudioSource>();
        droneFlyingAudio.clip = acFlying;
        droneFlyingAudio.volume = .5f;
        droneCrashAudio = gameObject.AddComponent<AudioSource>();
        droneCrashAudio.clip = acCrash;

        // Sound
        StartCoroutine(PlayResetSound());
        
        Debug.Log($"DroneController isGame: {isGame}, isRotorRotate: {isRoterRotate}");
    }

    void SetDroneByType()
    {
        var prefab = GetDronePrefab(GameManager.Instance.DroneType);
        if (prefab != null)
        {
            Debug.Log($"SetDroneByType Instantiate {prefab.name}");
            var body = Instantiate(prefab, transform);
            body.name = prefab.name;
            
            body.transform.localPosition = droneBody.localPosition;
            body.transform.localRotation = droneBody.localRotation;
            body.transform.localScale = droneBody.localScale;
            
            Destroy(droneBody.gameObject);

            var children = body.GetComponentsInChildren<Transform>();
            roters = new List<Transform>();
            foreach (var child in children)
            {
                if (child.name.Contains("rotor"))
                {
                    Debug.Log($"Add rotor {child.name}");
                    roters.Add(child);
                }
            }
            if (roters.Count > 0) isRoterRotate = true;
            

            droneBody = body.transform;
        } else Debug.LogError("SetDroneByType prefab is null");
    }

    GameObject GetDronePrefab(string type)
    {
        foreach (var prefab in dronePrefabs)
        {
            if (prefab.name.Equals(type))
                return prefab;
        }

        return dronePrefabs[0];
    }
    
    public void Reset()
    {
        ResetPosition();
        ResetRotation();
        DroneState = DRONE_STATE_FLYING;
        canDetectHit = true;
        timeSec = 0;
        shouldUpdateTime = true;
    }

    IEnumerator PlayResetSound()
    {
        audioSource.PlayOneShot(acSpawn);
        yield return new WaitForSeconds(3f);
        droneFlyingAudio.loop = true;
        droneFlyingAudio.Play();
    }

    void PlayCrashSound()
    {
        droneFlyingAudio.Stop();
        droneCrashAudio.PlayOneShot(acCrash);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraFollow.CamUpdate();
        
        // Roters
        if (isRoterRotate)
        {
            // Debug.Log("Rotor rotate Count: " + roters.Count);
            for (int i = 0; i < roters.Count; i++)
            {
                roters[i].Rotate(0, ROTER_SPIN, 0);
            }
        }
        
        if (!isGame) return;
        
        if(DroneState == DRONE_STATE_FLYING)
        {
            CalculateHorizontal_X_Movement();
            CalculateHorizontal_Y_Movement();
            CalculateVertical_X_Movement();
            CalculateVertical_Y_Movement();
			CalculateAttitude();

            if (rotationZ > MAX_ROTATE_Z) rotationZ = MAX_ROTATE_Z;
            else if (rotationZ < MAX_ROTATE_Z * -1) rotationZ = MAX_ROTATE_Z * -1;
            droneBody.localRotation = Quaternion.Euler(rotationX, 0, rotationZ);
            transform.rotation = Quaternion.Euler(transform.rotation.x, rotationY, transform.rotation.z);

            if (shouldUpdateTime)
            {
                timeSec += Time.deltaTime;
            }
        }
        else if(DroneState == DRONE_STATE_FALLING)
        {
            CalculateDroneFall();
        }
    }
    
    public void ResetPosition()
    {
        transform.position = droneIdentityPos;
    }

    public void ResetRotation()
    {
        rotationY = 0;
        transform.rotation = Quaternion.identity;
    }

    void CalculateHorizontal_X_Movement()
    {
        Vector2 moveInput = m_Input.HorizontalMoveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();


        m_DesiredHorizontalXSpeed = moveInput.magnitude * MAX_SPEED;

        float absFowardSpeed = Mathf.Abs(m_HorizontalXSpeed);

        float acceleration = 0;

        if (moveInput.x > 0)
        {
            // To Right
            acceleration = ACCELERATION;
        }
        else if (moveInput.x < 0)
        {
            // To Left
            acceleration = DECELERATION;
        }
        else if (Mathf.Approximately(moveInput.x, 0))
        {
            if (absFowardSpeed > 0)
            {
                acceleration = DECELERATION * -1;
                m_DesiredHorizontalXSpeed = 0;
            }
        }

        if (absFowardSpeed <= MAX_SPEED)
        {
            m_HorizontalXSpeed = Mathf.MoveTowards(m_HorizontalXSpeed, m_DesiredHorizontalXSpeed, acceleration * Time.deltaTime);

            if (m_HorizontalXSpeed > MAX_SPEED) m_HorizontalXSpeed = MAX_SPEED;
            else if (m_HorizontalXSpeed < MAX_SPEED * -1) m_HorizontalXSpeed = MAX_SPEED * -1;
        }

        //Debug.Log($"HorizontalX X : {moveInput.x}, Y : {moveInput.y}, desiredSpeed : {m_DesiredHorizontalXSpeed}, speed : {m_HorizontalXSpeed}, accel : {acceleration}");

        Vector3 movement = m_HorizontalXSpeed * transform.right;

        movement *= Time.deltaTime;

        m_CharCtrl.Move(movement);


        // Rotate Z
        rotationZ = m_HorizontalXSpeed * -2f;
    }

    float rotationX, rotationY, rotationZ = 0;

    void CalculateHorizontal_Y_Movement()
    {
        Vector2 moveInput = m_Input.HorizontalMoveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();


        m_DesiredHorizontalYSpeed = moveInput.magnitude * MAX_SPEED;

        float absFowardSpeed = Mathf.Abs(m_HorizontalYSpeed);

        float acceleration = 0;

        if (moveInput.y > 0)
        {
            acceleration = ACCELERATION;
        }
        else if (moveInput.y < 0)
        {
            acceleration = DECELERATION;
        }
        else if (Mathf.Approximately(moveInput.y, 0))
        {
            if (absFowardSpeed > 0)
            {
                acceleration = DECELERATION * -1;
                m_DesiredHorizontalYSpeed = 0;
            }
        }

        if (absFowardSpeed <= MAX_SPEED)
        {
            m_HorizontalYSpeed = Mathf.MoveTowards(m_HorizontalYSpeed, m_DesiredHorizontalYSpeed, acceleration * Time.deltaTime);

            if (m_HorizontalYSpeed > MAX_SPEED) m_HorizontalYSpeed = MAX_SPEED;
            else if (m_HorizontalYSpeed < MAX_SPEED * -1) m_HorizontalYSpeed = MAX_SPEED * -1;
        }

        //Debug.Log($"HorizontalY X : {moveInput.x}, Y : {moveInput.y}, desiredSpeed : {m_DesiredHorizontalYSpeed}, speed : {m_HorizontalYSpeed}, accel : {acceleration}");

        Vector3 movement = m_HorizontalYSpeed * transform.forward;

        movement *= Time.deltaTime;

        m_CharCtrl.Move(movement);

        // Rotate X
        rotationX = m_HorizontalYSpeed * 2f;
    }

    // Yaw 회전 - rotationY
    bool isYawRotating = false;
    void CalculateVertical_X_Movement()
    {
        Vector2 moveInput = m_Input.VerticalMoveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        m_DesiredVerticalXSpeed = moveInput.magnitude * MAX_SPEED_YAW_ROTATION;

        float absFowardSpeed = Mathf.Abs(m_VerticalXSpeed);

        float acceleration = 0;

        if (moveInput.x > 0)
        {
            acceleration = ACCELERATION_YAW;
            isYawRotating = true;
        }
        else if (moveInput.x < 0)
        {
            acceleration = DECELERATION_YAW;
            isYawRotating = true;
        }
        else if (Mathf.Approximately(moveInput.x, 0))
        {
            if (absFowardSpeed > 0)
            {
                acceleration = DECELERATION_YAW * -1;
                m_DesiredVerticalXSpeed = 0;
            }
            if (isYawRotating)
            {
                float offsetX = droneY - transform.rotation.y;
                //Debug.Log($"YAW before : {droneY}, after : {transform.rotation.eulerAngles.y}, offsetX : {offsetX}");
                //cameraFollow.SetOffsetX(offsetX);
                droneY = transform.rotation.y;
                
                isYawRotating = false;
            }
        }

        if (absFowardSpeed <= MAX_SPEED_YAW_ROTATION)
        {
            m_VerticalXSpeed = Mathf.MoveTowards(m_VerticalXSpeed, m_DesiredVerticalXSpeed, acceleration * Time.deltaTime);

            if (m_VerticalXSpeed > MAX_SPEED_YAW_ROTATION) m_VerticalXSpeed = MAX_SPEED_YAW_ROTATION;
            else if (m_VerticalXSpeed < MAX_SPEED_YAW_ROTATION * -1) m_VerticalXSpeed = MAX_SPEED_YAW_ROTATION * -1;
        }
        
        //Debug.Log("VerticalX : " + m_VerticalXSpeed);
        rotationY -= m_VerticalXSpeed;
    }
    
    void CalculateVertical_Y_Movement()
    {
        Vector2 moveInput = m_Input.VerticalMoveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        // 고도 제한 넘었을때
        if (moveInput.y > 0 && DroneHeight > HEIGHT_LIMIT)
        {
            return;
        }

        if(DroneHeight > HEIGHT_WARNING) IsHeightLimitExeed = true;
        else IsHeightLimitExeed = false;
        
        m_DesiredVerticalYSpeed = moveInput.magnitude * MAX_SPEED;

        float absFowardSpeed = Mathf.Abs(m_VerticalYSpeed);

        float acceleration = 0;

        if (moveInput.y > 0)
        {
            acceleration = ACCELERATION;
        }
        else if (moveInput.y < 0)
        {
            acceleration = DECELERATION;
        }
        else if (Mathf.Approximately(moveInput.y, 0))
        {
            if (absFowardSpeed > 0)
            {
                acceleration = DECELERATION * -1;
                m_DesiredVerticalYSpeed = 0;
            }
        }

        if (absFowardSpeed <= MAX_SPEED)
        {
            m_VerticalYSpeed = Mathf.MoveTowards(m_VerticalYSpeed, m_DesiredVerticalYSpeed, acceleration * Time.deltaTime);

            if (m_VerticalYSpeed > MAX_SPEED) m_VerticalYSpeed = MAX_SPEED;
            else if (m_VerticalYSpeed < MAX_SPEED * -1) m_VerticalYSpeed = MAX_SPEED * -1;
        }

        //Debug.Log($"VerticalY  desiredSpeed : {m_DesiredVerticalYSpeed}, speed : {m_VerticalYSpeed}, accel : {acceleration}");

        Vector3 movement = m_VerticalYSpeed * transform.up;

        movement *= Time.deltaTime;

        m_CharCtrl.Move(movement);
    }

    void CalculateDroneFall()
    {
        //m_VerticalYSpeed = Mathf.MoveTowards(m_VerticalYSpeed, -40, 20f * Time.deltaTime);
        ////if (m_VerticalSpeed < -16) m_VerticalSpeed = -16;

        //Vector3 movement = m_VerticalYSpeed * transform.up;
        Vector3 movement = transform.up * -40 * Time.deltaTime;

        //movement *= Time.deltaTime;
        
        //Debug.Log($"Fall  speed : {m_VerticalYSpeed}");

        m_CharCtrl.Move(movement);

        if(m_CharCtrl.transform.position.y < 0.6)
        {
            DroneState = DRONE_STATE_DISARMED;
        }
    }

    void CalculateAttitude()
	{
        if(DroneState == DRONE_STATE_FLYING)
        {
            // Height
            DroneHeight = (float)Math.Round(transform.position.y / 8.7f);

            // Distance
            Vector2 homePos = new Vector2(droneIdentityPos.x, droneIdentityPos.z);
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            DroneDistanceFromHome = (float)Math.Round(Vector2.Distance(homePos, pos) / 6);

            // Yaw
            DroneYaw = (int)transform.rotation.eulerAngles.y;

            // Battery
            DroneBattery = 100 - (int)(timeSec / 3);
            if (DroneBattery < BATTERY_LIMIT) onDroneFail();
        }
        else
        {
            DroneHeight = 0;
            DroneDistanceFromHome = 0;
            DroneYaw = 0;
            DroneBattery = 0;
        }

    }

    /// <summary>
    /// Drone fall due to accident etc.
    /// </summary>
    public void onDroneFail()
    {
        Debug.Log("OnDroneFail");
        
        PlayCrashSound();
        DroneState = DRONE_STATE_FALLING;
        shouldUpdateTime = false;
        m_CharCtrl.enabled = false;
        Vector3 pos = transform.position;

        transform.position = new Vector3(pos.x , -100, pos.z);

        var clonedBody = Instantiate(droneBody);
        clonedBody.position = pos;

        // Fail partice effect
        var fParticle = clonedBody.transform.Find("FailParticle")?.GetComponent<ParticleSystem>();
        if (fParticle != null)
        {
            fParticle.Play();
        } else Debug.LogError("clone particle is null");

        //clonedBody.Rotate(Vector3.forward, 10);
        clonedBody.gameObject.AddComponent<BoxCollider>();

        cameraFollow.fallingTarget = clonedBody.transform;
        cameraFollow.ShouldFollow = false;

        float randForceX = UnityEngine.Random.Range(50, 500);
        float randForceY = UnityEngine.Random.Range(50, 500);
        float randForceZ = UnityEngine.Random.Range(50, 500);
        clonedBody.gameObject.AddComponent<Rigidbody>().AddTorque(randForceX, randForceY, randForceZ);
        clonedBody.gameObject.AddComponent<DroneCollision>().SetListener(() => {
            StartCoroutine(DroneFallProcess(clonedBody.gameObject));
        });

        m_CharCtrl.enabled = true;
    }

    IEnumerator DroneFailProcess()
    {
        Debug.Log("DroneFailProcess start");
        // failParticle.Play();
        // yield return new WaitForSeconds(.2f);
        yield return null;
        Debug.Log("DroneFailProcess end");
    }

    public void OnDroneSuccess()
    {
        Debug.Log("OnDroneSuccess");
        audioSource.PlayOneShot(acSuccess);
    }

    
    IEnumerator DroneFallProcess(GameObject fallDrone)
    {
        yield return new WaitForSeconds(3f);
        //droneBody.gameObject.SetActive(true);
        // Sound
        
        gameController.ResetGame();
        
        StartCoroutine(PlayResetSound());
        
        Destroy(fallDrone);
        Reset();
        cameraFollow.ShouldFollow = true;
    }

    bool canDetectHit = true;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!canDetectHit) return;

        Debug.Log($"{gameObject.name} OnControllerColliderHit : {hit.gameObject.name}, parent: {hit.transform.parent.name}, Tag: {hit.collider.tag}");

        if (hit.collider.CompareTag("WayPointItem"))
        {
            // Debug.Log("Drone hit waypoint");
            // hit.gameObject.SetActive(false);
        }
        else
        {
            onDroneFail();
            canDetectHit = false;
        }

        if(droneHitObjListener != null) droneHitObjListener(hit);
    }

    public void OnDroneWayPoint()
    {
        audioSource.PlayOneShot(acAquired);
    }

    public void OnSoundToggle(bool isSoundMute)
    {
        audioSource.mute = isSoundMute;
        droneFlyingAudio.mute = isSoundMute;
        droneCrashAudio.mute = isSoundMute;
        Debug.Log("OnSoundToggle : " + isSoundMute);
    }
}
