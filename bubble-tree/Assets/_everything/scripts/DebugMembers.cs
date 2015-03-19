using UnityEngine;
using UnityEditor;
using System.Reflection;

public class DebugMembers<T>
	: Editor
{
	public override void OnInspectorGUI()
	{
		var type = typeof(T);
		var members = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

		EditorGUILayout.LabelField("Methods", EditorStyles.boldLabel);

		foreach (var m in members)
		{
			if (m.DeclaringType == typeof(T))
			{
				string s = string.Format("{0}.{1}()", m.DeclaringType.ToString(), m.Name);

				if (Application.isPlaying || true )
				{
					if (GUILayout.Button(s))
					{
						var o = target;
						m.Invoke(target, null);
					}
				}
			}
		}

		base.OnInspectorGUI();
	}
}
