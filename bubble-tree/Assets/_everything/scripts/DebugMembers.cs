using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Seed))]
public class DebugMembers
	: Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("HELLO", EditorStyles.boldLabel);

		base.OnInspectorGUI();
	}
}
