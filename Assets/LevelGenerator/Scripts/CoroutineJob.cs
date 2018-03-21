using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineJob : IEnumerator {
    List<object> list;
    int curIndex;
    public object Current {
        get {
            return list[curIndex];
        }
    }

    public bool MoveNext() {
        curIndex++;
        if (curIndex >= list.Count)
            return false;
        return true;
    }

    public void Reset() {
        curIndex = 0;
    }
}
