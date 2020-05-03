using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonPattern<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Static Fields

    protected static T instance = null;
    protected static object instanceLock = new object();
    protected static bool destroyedOnApplicationQuit = false;

    #endregion

    #region Fields

    private Transform m_transform = null;
    private GameObject m_gameObject = null;

    private bool m_isInitialized = false;
    private bool m_isForcefullyDestroyed = false;

    private const string LOGTAG = "[SingletonPattern]";

    #endregion

    #region Static Properties

    /// <summary>
    /// Gets the singleton instance which will be persistent until Application quits.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance
    {
        get
        {
            System.Type _singletonType = typeof(T);

            // We are requesting an instance after application is quit
            if (destroyedOnApplicationQuit)
            {
                Debug.Log($"{LOGTAG} {_singletonType} instance is already destroyed.");
                return null;
            }

            lock (instanceLock)
            {
                if (instance == null)
                {
                    // Get all the instances that exist in the screen
                    T[] _monoComponents = FindObjectsOfType(_singletonType) as T[];

                    if (_monoComponents.Length > 0)
                    {
                        instance = _monoComponents[0];

                        //for (int iter = 1; iter < _monoComponents.Length; iter++)
                        //    Destroy(_monoComponents[iter].gameObject);
                    }

                    // We need to create new instance
                    if (instance == null)
                    {
                        // First search in resource if prefab exists for this class
                        string _singletonName = _singletonType.Name;

                        GameObject _singletonPrefab = Resources.Load("Singleton/" + _singletonName, typeof(GameObject)) as GameObject;

                        if (_singletonPrefab != null)
                        {
                            //Debug.Log("[SingletonPattern] Creating singleton using prefab => " + _singletonName);
                            instance = (Instantiate(_singletonPrefab) as GameObject).GetComponent<T>();
                        }
                        else
                        {
                            //Debug.Log("[SingletonPattern] Creating singleton new GameObject => " + _singletonName);
                            instance = new GameObject().AddComponent<T>();
                        }

                        // Update name 
                        instance.name = _singletonName;
                    }
                }
            }

            // Check if component is initialized or not
            SingletonPattern<T> _singletonInstance = (SingletonPattern<T>)(object)instance;

            if (!_singletonInstance.m_isInitialized)
                _singletonInstance.Init();

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    #endregion

    #region Properties

    public Transform CachedTransform
    {
        get
        {
            if (m_transform == null)
                m_transform = transform;

            return m_transform;
        }
    }

    public GameObject CachedGameObject
    {
        get
        {
            if (m_gameObject == null)
                m_gameObject = gameObject;

            return m_gameObject;
        }
    }

    #endregion

    #region Methods

    private void Awake()
    {
        if (!m_isInitialized)
            Init();
    }

    protected virtual void Start()
    { }

    protected virtual void Reset()
    {
        // Reset properties
        m_gameObject = null;
        m_transform = null;
        m_isInitialized = false;
        m_isForcefullyDestroyed = false;
    }

    protected virtual void OnEnable()
    { }

    protected virtual void OnDisable()
    { }

    protected virtual void OnDestroy()
    {
        // Singleton instance means same instance will run throughout the gameplay session
        // If its destroyed that means application is quit
        if (instance == this && !m_isForcefullyDestroyed)
        {
            destroyedOnApplicationQuit = true;
            Debug.Log($"{LOGTAG} OnDestory {this.GetType().Name}");
        }
    }

    #endregion

    #region Methods

    protected virtual void Init()
    {
        // Set as initialized
        m_isInitialized = true;

        // Just in case, handling so that only one instance is alive
        if (instance == null)
        {
            instance = this as T;
        }
        // Destroying the reduntant copy of this class type
        else if (instance != this)
        {
			//Debug.LogWarning($"{LOGTAG} init destory : {CachedGameObject.name}");
   //         Destroy(CachedGameObject);

            // Should replace instance to this?? or Destory this??
            //instance = this as T;

            return;
        }

        // Set it as persistent object
        DontDestroyOnLoad(CachedGameObject);
    }

    public void ForceDestroy()
    {
        Debug.LogError($"{LOGTAG} ForceDestroy {this.GetType().Name}");
        // Mark that object was forcefully destroyed
        m_isForcefullyDestroyed = true;

        // Destory
        Destroy(CachedGameObject);
    }
    
    #endregion
}
