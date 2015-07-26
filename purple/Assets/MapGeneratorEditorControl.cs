using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditorControl : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		
		if (GUILayout.Button ("Generate"))
		{
			var m = target as MapGenerator;
			m.GenerateMap();
		}
	}
}
