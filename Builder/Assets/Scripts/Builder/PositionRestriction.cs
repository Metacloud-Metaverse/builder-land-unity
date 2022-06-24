using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRestriction : MonoBehaviour
{
    public bool isProhibited { get; private set; }
    private Renderer[] _renderers;
    private List<Wall> _wallsCollisioned = new List<Wall>();
    private Vector3 _chunkSize;
    private Vector2 _chunks;
    private float _maxHeight;
    private Animation _animation;
    private Vector3 _hitPosition;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _animation = GetComponent<Animation>();
    }

    private void Start()
    {
        _chunkSize = SceneManagement.Instance.chunkSize;
        _chunks = SceneManagement.Instance.chunks;
        _maxHeight = SceneManagement.Instance.maxHeight;
    }

    public void SetErrorMaterial()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetColor("_BaseColor", Color.red);
        }
    }

    public void SetMainMaterial()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetColor("_BaseColor", Color.white);
        }

    }

    public void AddCollisionedWall(Wall wall)
    {
        _wallsCollisioned.Add(wall);
        isProhibited = true;
        SetErrorMaterial();
    }

    public void RemoveCollisionedWall(Wall wall)
    {
        _wallsCollisioned.Remove(wall);
        if (_wallsCollisioned.Count == 0)
        {
            isProhibited = false;
            SetMainMaterial();
        }
    }

    private void Update()
    {
        CheckOutOfLimits();
    }

    private void CheckOutOfLimits()
    {
        var minX = _chunkSize.x / 2 * -1;
        var minZ = _chunkSize.z / 2 * -1;
        var maxX = _chunkSize.x * _chunks.x - (_chunkSize.x / 2);
        var maxZ = _chunkSize.z * _chunks.y - (_chunkSize.z / 2);
        var minY = 0;
        var maxY = _maxHeight;

        if (transform.position.x < minX)
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);

        if (transform.position.z < minZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);

        if (transform.position.y < minY)
            transform.position = new Vector3(transform.position.x, minY, transform.position.z);

        if (transform.position.x > maxX)
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);

        if (transform.position.z > maxZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);

        if (transform.position.y > maxY)
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);

    }
}
