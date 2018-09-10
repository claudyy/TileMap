using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace ClaudeFehlen.Tileset.Pathfinding {
    public class PathRequestManager : MonoBehaviour {

        Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        PathRequest currentPathRequest;

        static PathRequestManager instance;
        Pathfinding pathfinding;

        bool isProcessingPath;

        void Awake() {
            instance = this;
            pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, null, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }
        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, INeighbourFilter _filter) {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, _filter, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }
        void TryProcessNext() {
            if (!isProcessingPath && pathRequestQueue.Count > 0) {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd,currentPathRequest.filter);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success) {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        struct PathRequest {
            public Vector3 pathStart;
            public Vector3 pathEnd;
            public Action<Vector3[], bool> callback;
            public INeighbourFilter filter;
            public PathRequest(Vector3 _start, Vector3 _end, INeighbourFilter _filter, Action<Vector3[], bool> _callback) {
                filter = _filter;
                pathStart = _start;
                pathEnd = _end;
                callback = _callback;
            }

        }
    }
}