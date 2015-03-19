using UnityEngine;
using UnityEditor;
using System.Collections;
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
				string prefix = m.ReturnType == typeof(IEnumerator) ? "* " : "";
				string s = string.Format("{0}{1}.{2}()", prefix, m.DeclaringType.ToString(), m.Name);

				if (Application.isPlaying || true)
				{
					if (GUILayout.Button(s))
					{
						Invoke(m);
					}
				}
			}
		}

		base.OnInspectorGUI();
	}

	private void Invoke(MethodInfo method)
	{
		if (method.ReturnType == typeof(IEnumerator))
		{
			var go = target as MonoBehaviour;
			go.StartCoroutine(method.Name);
		}
		else
		{
			method.Invoke(target, null);
		}
	}
}
