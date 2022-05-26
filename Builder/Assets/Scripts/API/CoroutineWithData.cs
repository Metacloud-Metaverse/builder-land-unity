using System.Collections;
using UnityEngine;

public class CoroutineWithData<T>
{
    private IEnumerator _target;
    public T result;
    public Coroutine Coroutine { get; private set; }

    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        _target = target;
        Coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
         {
            result = (T)_target.Current;
            yield return result;
        }
    }

}
