using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public class TilemapMouseDebug_Display : MonoBehaviour {
    public LevelTilemap target;
    StringBuilder str = new StringBuilder();
    private void OnDrawGizmos() {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
            return;
        var pos = UnityEditor.HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        pos.x = Mathf.Floor(pos.x);
        pos.y = Mathf.Floor(pos.y);
        var tile = target.GetITile((int)pos.x, (int)pos.y,false);

        if (UnityEditor.EditorWindow.mouseOverWindow!=null && UnityEditor.EditorWindow.mouseOverWindow.ToString() == " (UnityEditor.GameView)") {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            pos = mousePos;
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            tile = target.GetITile((int)pos.x, (int)pos.y,false);
        }

        var mouseDebugList = GetComponents<TilemapMouseDebug_Base>();
        if (tile != null) {
            str = new StringBuilder();
            str.AppendLine(tile.DebugInfo());
            if(tile.DebugDataInfo() != "")
                str.AppendLine(tile.DebugDataInfo());
            for (int i = 0; i < mouseDebugList.Length; i++) {
                if(mouseDebugList[i].displayText)
                    str.AppendLine(mouseDebugList[i].TileText(tile));
            }
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(pos + Vector3.right, str.ToString());
        }
#endif
    }

}
