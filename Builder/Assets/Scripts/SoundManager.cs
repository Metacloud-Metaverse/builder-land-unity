using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour 
{
	public int channelCount = 4;
	private AudioSource[] _channels;
	public AudioClip[] clips;
    private List<AudioSource> _referencedChannels = new List<AudioSource>();

	public static SoundManager Instance { get; private set; }
	
	
	void Awake()
	{
		if(Instance == null)	
			Instance = this;
		
		else
			throw new System.Exception("There are more than one SoundManager Instance");
	}
	

	void Start () 
	{
		_channels = new AudioSource[channelCount];
		
		for (int i = 0; i < channelCount; i++)
		{
			_channels[i] = Camera.main.gameObject.AddComponent<AudioSource>();
		}
	}
	
	
	public void PlaySound(int ID, bool loop = false, float volume = 1)
	{
		int index = FindChannel ();
		
		if(index == -1)
		{
			Debug.LogError("All channels are bussy.");
			return;
		}
		
		_channels [index].clip = clips [ID];
		_channels [index].volume = volume;
		_channels [index].loop = loop;
		_channels [index].Play ();
	}


    public int PlayReferencedSound(int id, bool loop, float volume)
    {
		var index = FindReferencedChannel();
		AudioSource channel;

		if(index == -1)
        {
			channel = Camera.main.gameObject.AddComponent<AudioSource>();
			_referencedChannels.Add(channel);
			index = _referencedChannels.Count - 1;
		}
		else
        {
			channel = _referencedChannels[index];
        }

		channel.clip = clips[id];
		channel.volume = volume;
		channel.loop = loop;
		channel.Play();
		
		return index;
    }


	private int FindReferencedChannel()
    {
		for (int i = 0; i < _referencedChannels.Count; i++)
		{
			if (!_referencedChannels[i].isPlaying) return i;
		}
		return -1;
	}


	public void StopReferencedSound(int index)
    {
        _referencedChannels[index].Stop();
        _referencedChannels.RemoveAt(index);
    }


    public void StopSound()
	{
		for(int i = 0; i < channelCount; i++)
		{
			_channels [i].Stop ();
		}
	}


	private int FindChannel()
	{		
		for (int i = 0; i < _channels.Length; i++)
		{
			if(!_channels[i].isPlaying) return i;
		}
		
		return -1;
	}
}
