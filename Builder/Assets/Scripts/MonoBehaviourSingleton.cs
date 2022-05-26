using UnityEngine;


public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance;

	/// <summary>
	/// If you override this then make sure you call base.Awake()
	/// </summary>
	public virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;
			transform.parent = null;
			DontDestroyOnLoad(gameObject);
			
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// If you override this then make sure you call base.OnDestroy() somewhere in your OnDestroy code.
	/// </summary>
	protected void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
