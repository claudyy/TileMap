using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LevelTilePainter))]
public class LevelTilePainter_Editor : Editor {

    List<GUIContent> brushContent;
    bool paintMode;
    int selectedPaint;
    public override void OnInspectorGUI() {

        LevelTilePainter painter = (LevelTilePainter)target;
        
        if (paintMode) {
            InPaintMode(painter);
        } else {
            NotInPaintMode(painter);
        }
        

    }
    void OnSceneGUI() {
        LevelTilePainter painter = (LevelTilePainter)target;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            int x = (int)(mousePosition.x - painter.level.transform.position.x);
            int y = (int)(mousePosition.y - painter.level.transform.position.y);
            painter.level.OverrideTile(x, y, painter.brushes[selectedPaint]);
            painter.level.UpdateViews(x,y);
            Debug.Log(x +
                " / " +
                y +
                "Left-Mouse Down");
        }
        if (paintMode) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        } else {
            HandleUtility.Repaint();
        }
    }
    void InPaintMode(LevelTilePainter painter) {
        if (GUILayout.Button("exit paint mode"))
            paintMode = false;
        if (brushContent == null)
            UpdateBrushes(painter);
        for (int i = 0; i < brushContent.Count; i++)
            if(GUILayout.Button(brushContent[i], GUILayout.Width(40), GUILayout.Height(40))) {
                selectedPaint = i;
            }
    }
    void NotInPaintMode(LevelTilePainter painter) {
        if (GUILayout.Button("paint mode"))
            paintMode = true;
        DrawDefaultInspector();

        if (GUILayout.Button("UpdateBrushes"))
            UpdateBrushes(painter);
    }
    void UpdateBrushes(LevelTilePainter painter) {
        brushContent = new List<GUIContent>();
        for (int i = 0; i < painter.brushes.Count; i++) {
            //brushContent.Add(new GUIContent(painter.brushes[i].tile.sprite.texture));
        }
    }
}
