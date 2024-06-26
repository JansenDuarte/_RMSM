using UnityEngine;
using UnityEditor;
using System.Collections;
using ExtensionMethods;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
	BezierCurve curve;
	SerializedProperty resolutionProp;
	SerializedProperty closeProp;
	SerializedProperty pointsProp;
	SerializedProperty colorProp;

	//Modifications made by Jansen Duarte
	public string[] trackOptions = { "Circuit", "Rally", "Hill Climb", "Oval", "Drag" };
	public int trackType = 0;

	private static bool showPoints = true;

	void OnEnable()
	{
		curve = (BezierCurve)target;

		resolutionProp = serializedObject.FindProperty("resolution");
		closeProp = serializedObject.FindProperty("_close");
		pointsProp = serializedObject.FindProperty("points");
		colorProp = serializedObject.FindProperty("drawColor");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(resolutionProp);
		EditorGUILayout.PropertyField(closeProp);
		EditorGUILayout.PropertyField(colorProp);



		showPoints = EditorGUILayout.Foldout(showPoints, "Points");

		if (showPoints)
		{
			int pointCount = pointsProp.arraySize;

			for (int i = 0; i < pointCount; i++)
			{
				DrawPointInspector(curve[i], i);
			}

			if (GUILayout.Button("Add Point"))
			{
#pragma warning disable CS0618 // Type or member is obsolete
				Undo.RegisterSceneUndo("Add Point");
#pragma warning restore CS0618 // Type or member is obsolete

				GameObject pointObject = new GameObject("Point " + pointsProp.arraySize);
				pointObject.transform.parent = curve.transform;
				pointObject.transform.localPosition = Vector3.zero;
				BezierPoint newPoint = pointObject.AddComponent<BezierPoint>();

				newPoint.curve = curve;
				newPoint.handle1 = Vector3.right * 0.1f;
				newPoint.handle2 = -Vector3.right * 0.1f;

				pointsProp.InsertArrayElementAtIndex(pointsProp.arraySize);
				pointsProp.GetArrayElementAtIndex(pointsProp.arraySize - 1).objectReferenceValue = newPoint;
			}

		}

		//Modifications made by Jansen Duarte
		EditorGUILayout.Space(10f);

		EditorGUILayout.BeginFoldoutHeaderGroup(true, "DataBase");

		if (GUILayout.Button("Randomize Track"))
		{
			BezierPoint[] tempArray = curve.GetAnchorPoints();
			for (int i = tempArray.Length - 1; i > 0; i--)
			{
				curve.RemovePoint(tempArray[i]);
			}
			curve.Randomize_Track();
		}

		trackType = EditorGUILayout.Popup("Track Type", trackType, trackOptions);

		if (GUILayout.Button("Export Curve"))
			TrackSaver.Save_Track(ExM.Convert_TrackToString(curve), (TrackType)trackType, curve.length);

		EditorGUILayout.EndFoldoutHeaderGroup();



		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(target);
		}
	}

	void OnSceneGUI()
	{
		for (int i = 0; i < curve.pointCount; i++)
		{
			DrawPointSceneGUI(curve[i]);
		}
	}

	void DrawPointInspector(BezierPoint point, int index)
	{
		SerializedObject serObj = new SerializedObject(point);

		SerializedProperty handleStyleProp = serObj.FindProperty("handleStyle");
		SerializedProperty handle1Prop = serObj.FindProperty("_handle1");
		SerializedProperty handle2Prop = serObj.FindProperty("_handle2");

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("X", GUILayout.Width(20)))
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Undo.RegisterSceneUndo("Remove Point");
#pragma warning restore CS0618 // Type or member is obsolete

			pointsProp.MoveArrayElement(curve.GetPointIndex(point), curve.pointCount - 1);
			pointsProp.arraySize--;
			DestroyImmediate(point.gameObject);
			return;
		}

		EditorGUILayout.ObjectField(point.gameObject, typeof(GameObject), true);

		if (index != 0 && GUILayout.Button(@"/\", GUILayout.Width(25)))
		{
			UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue;
			pointsProp.GetArrayElementAtIndex(index - 1).objectReferenceValue = point;
			pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}

		if (index != pointsProp.arraySize - 1 && GUILayout.Button(@"\/", GUILayout.Width(25)))
		{
			UnityEngine.Object other = pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue;
			pointsProp.GetArrayElementAtIndex(index + 1).objectReferenceValue = point;
			pointsProp.GetArrayElementAtIndex(index).objectReferenceValue = other;
		}

		EditorGUILayout.EndHorizontal();

		EditorGUI.indentLevel++;
		EditorGUI.indentLevel++;

		int newType = (int)((object)EditorGUILayout.EnumPopup("Handle Type", (HandleStyle)handleStyleProp.enumValueIndex));

		if (newType != handleStyleProp.enumValueIndex)
		{
			handleStyleProp.enumValueIndex = newType;
			if (newType == 0)
			{
				if (handle1Prop.vector3Value != Vector3.zero) handle2Prop.vector3Value = -handle1Prop.vector3Value;
				else if (handle2Prop.vector3Value != Vector3.zero) handle1Prop.vector3Value = -handle2Prop.vector3Value;
				else
				{
					handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
					handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
				}
			}

			else if (newType == 1)
			{
				if (handle1Prop.vector3Value == Vector3.zero && handle2Prop.vector3Value == Vector3.zero)
				{
					handle1Prop.vector3Value = new Vector3(0.1f, 0, 0);
					handle2Prop.vector3Value = new Vector3(-0.1f, 0, 0);
				}
			}

			else if (newType == 2)
			{
				handle1Prop.vector3Value = Vector3.zero;
				handle2Prop.vector3Value = Vector3.zero;
			}
		}

		Vector3 newPointPos = EditorGUILayout.Vector3Field("Position : ", point.transform.localPosition);
		if (newPointPos != point.transform.localPosition)
		{
			Undo.RegisterCompleteObjectUndo(point.transform, "Move Bezier Point");
			point.transform.localPosition = newPointPos;
		}

		if (handleStyleProp.enumValueIndex == 0)
		{
			Vector3 newPosition;

			newPosition = EditorGUILayout.Vector3Field("Handle 1", handle1Prop.vector3Value);
			if (newPosition != handle1Prop.vector3Value)
			{
				handle1Prop.vector3Value = newPosition;
				handle2Prop.vector3Value = -newPosition;
			}

			newPosition = EditorGUILayout.Vector3Field("Handle 2", handle2Prop.vector3Value);
			if (newPosition != handle2Prop.vector3Value)
			{
				handle1Prop.vector3Value = -newPosition;
				handle2Prop.vector3Value = newPosition;
			}
		}

		else if (handleStyleProp.enumValueIndex == 1)
		{
			EditorGUILayout.PropertyField(handle1Prop);
			EditorGUILayout.PropertyField(handle2Prop);
		}

		EditorGUI.indentLevel--;
		EditorGUI.indentLevel--;

		if (GUI.changed)
		{
			serObj.ApplyModifiedProperties();
			EditorUtility.SetDirty(serObj.targetObject);
		}
	}

	static void DrawPointSceneGUI(BezierPoint point)
	{
		Handles.Label(point.position + new Vector3(0, HandleUtility.GetHandleSize(point.position) * 0.4f, 0), point.gameObject.name);

		Handles.color = Color.green;
		Vector3 newPosition = Handles.FreeMoveHandle(point.position, point.transform.rotation, HandleUtility.GetHandleSize(point.position) * 0.1f, Vector3.zero, Handles.RectangleHandleCap);

		if (newPosition != point.position)
		{
			Undo.RegisterCompleteObjectUndo(point.transform, "Move Point");
			point.transform.position = newPosition;
		}

		if (point.handleStyle != HandleStyle.None)
		{
			Handles.color = Color.cyan;
			Vector3 newGlobal1 = Handles.FreeMoveHandle(point.globalHandle1, point.transform.rotation, HandleUtility.GetHandleSize(point.globalHandle1) * 0.075f, Vector3.zero, Handles.CircleHandleCap);
			if (point.globalHandle1 != newGlobal1)
			{
				Undo.RegisterCompleteObjectUndo(point, "Move Handle");
				point.globalHandle1 = newGlobal1;
				if (point.handleStyle == HandleStyle.Connected) point.globalHandle2 = -(newGlobal1 - point.position) + point.position;
			}

			Vector3 newGlobal2 = Handles.FreeMoveHandle(point.globalHandle2, point.transform.rotation, HandleUtility.GetHandleSize(point.globalHandle2) * 0.075f, Vector3.zero, Handles.CircleHandleCap);
			if (point.globalHandle2 != newGlobal2)
			{
				Undo.RegisterCompleteObjectUndo(point, "Move Handle");
				point.globalHandle2 = newGlobal2;
				if (point.handleStyle == HandleStyle.Connected) point.globalHandle1 = -(newGlobal2 - point.position) + point.position;
			}

			Handles.color = Color.yellow;
			Handles.DrawLine(point.position, point.globalHandle1);
			Handles.DrawLine(point.position, point.globalHandle2);
		}
	}

	public static void DrawOtherPoints(BezierCurve curve, BezierPoint caller)
	{
		foreach (BezierPoint p in curve.GetAnchorPoints())
		{
			if (p != caller) DrawPointSceneGUI(p);
		}
	}

	[MenuItem("GameObject/Create Other/Bezier Curve")]
	public static void CreateCurve(MenuCommand command)
	{
		GameObject curveObject = new GameObject("BezierCurve");
		Undo.RegisterCompleteObjectUndo(curveObject, "Undo Create Curve");
		BezierCurve curve = curveObject.AddComponent<BezierCurve>();

		BezierPoint p1 = curve.AddPointAt(Vector3.forward * 0.5f);
		p1.handleStyle = HandleStyle.Connected;
		p1.handle1 = new Vector3(-0.28f, 0, 0);

		BezierPoint p2 = curve.AddPointAt(Vector3.right * 0.5f);
		p2.handleStyle = HandleStyle.Connected;
		p2.handle1 = new Vector3(0, 0, 0.28f);

		BezierPoint p3 = curve.AddPointAt(-Vector3.forward * 0.5f);
		p3.handleStyle = HandleStyle.Connected;
		p3.handle1 = new Vector3(0.28f, 0, 0);

		BezierPoint p4 = curve.AddPointAt(-Vector3.right * 0.5f);
		p4.handleStyle = HandleStyle.Connected;
		p4.handle1 = new Vector3(0, 0, -0.28f);

		curve.close = true;
	}
}
