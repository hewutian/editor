using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public static class ExposeProperties
{
	public static void Expose(PropertyField[] properties)
	{
		var emptyOptions = new GUILayoutOption[0];
        var re = SceneView.lastActiveSceneView.position;
        GUILayout.BeginArea(new Rect(0, re.height - 100, re.width-100, 100), EditorStyles.textArea);
        GUILayout.BeginVertical();
        foreach (PropertyField field in properties)
		{
            GUILayout.BeginHorizontal();
            if (field.Type == SerializedPropertyType.Integer)
			{
				var oldValue = (int)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = int.Parse(GUILayout.TextField(oldValue.ToString()));
				if (oldValue != newValue)
					field.SetValue(newValue);
			}
			else if (field.Type == SerializedPropertyType.Float)
			{
				var oldValue = (float)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = float.Parse(GUILayout.TextField(oldValue.ToString()));
                if (oldValue != newValue)
					field.SetValue(newValue);
			}
			else if (field.Type == SerializedPropertyType.Boolean)
			{
				var oldValue = (bool)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue = bool.Parse(GUILayout.TextField(oldValue.ToString()));
                if (oldValue != newValue)
					field.SetValue(newValue);
			}
			else if (field.Type == SerializedPropertyType.String)
			{
				var oldValue = (string)field.GetValue();
                GUILayout.Label(field.Name);
                var newValue =GUILayout.TextField(oldValue.ToString());
                if (oldValue != newValue)
					field.SetValue(newValue);
			}
            //else if (field.Type == SerializedPropertyType.Vector2)
            //{
            //}
            else if (field.Type == SerializedPropertyType.Vector3)
            {
                var oldValue = (Vector3)field.GetValue();

                GUILayout.Label(field.Name);
                GUILayout.Label("x:");
                var newValuex =float.Parse( GUILayout.TextField(oldValue.x.ToString()));
                GUILayout.Label("y:");
                var newValuey = float.Parse(GUILayout.TextField(oldValue.y.ToString()));
                GUILayout.Label("z:");
                var newValuez = float.Parse(GUILayout.TextField(oldValue.z.ToString()));
                //var newValue = EditorGUILayout.TextField(field.Name, oldValue, emptyOptions);
               // if (oldValue.x != newValuex)
                    field.SetValue(new Vector3(newValuex,newValuey,newValuez));
            }
            //else if (field.Type == SerializedPropertyType.Enum)
            //{
            //	var oldValue = (Enum)field.GetValue();
            //	var newValue = EditorGUILayout.EnumPopup(field.Name, oldValue, emptyOptions);
            //	if (oldValue != newValue)
            //		field.SetValue(newValue);
            //}
            //else if (field.Type == SerializedPropertyType.ObjectReference)
            //{
            //	UnityEngine.Object oldValue = (UnityEngine.Object)field.GetValue();
            //	UnityEngine.Object newValue = EditorGUILayout.ObjectField(field.Name, oldValue, field.Info.PropertyType, emptyOptions);
            //	if (oldValue != newValue)
            //		field.SetValue(newValue);
            //}
            GUILayout.EndHorizontal();
		}
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
 
	public static PropertyField[] GetProperties(object obj)
	{
		var fields = new List<PropertyField>();
        // PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.GetProperty|BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);//BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] infos = obj.GetType().GetFields(BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
        foreach (FieldInfo info in infos)
		{
		//	if (!(info.CanRead && info.CanWrite))
		//		continue;
 			var type = SerializedPropertyType.Integer;
			if (PropertyField.GetPropertyType(info, out type))
			{
				var field = new PropertyField(obj, info, type);
				fields.Add(field);
			}
		}
 		return fields.ToArray();
	}
}
 
public class PropertyField
{
	object obj;
    FieldInfo info;
	SerializedPropertyType type;
 
	//MethodInfo getter;
	//MethodInfo setter;
 
	public FieldInfo Info
	{
		get { return info; }
	}
 
	public SerializedPropertyType Type
	{
		get { return type; }
	}
 
	public string Name
	{
		get { return ObjectNames.NicifyVariableName(info.Name); }
	}
 
	public PropertyField(object obj, FieldInfo info, SerializedPropertyType type)
	{
		this.obj = obj;
		this.info = info;
		this.type = type;
 
		//getter = this.info.GetGetMethod();
		//setter = this.info.GetSetMethod();
	}
 
//	public object GetValue() { return getter.Invoke(obj, null); }
    public object GetValue()
    {
        return info.GetValue(obj);
    }
//	public void SetValue(object value) { setter.Invoke(obj, new[] {value}); }
    public void SetValue(object value)
    {
        info.SetValue(obj, value);
    }
 
	public static bool GetPropertyType(FieldInfo info, out SerializedPropertyType propertyType)
	{
		System.Type type = info.FieldType;
		propertyType = SerializedPropertyType.Generic;
		if (type == typeof(int))
			propertyType = SerializedPropertyType.Integer;
		else if (type == typeof(float))
			propertyType = SerializedPropertyType.Float;
		else if (type == typeof(bool))
			propertyType = SerializedPropertyType.Boolean;
		else if (type == typeof(string))
			propertyType = SerializedPropertyType.String;
		else if (type == typeof(Vector2))
			propertyType = SerializedPropertyType.Vector2;
		else if (type == typeof(Vector3))
			propertyType = SerializedPropertyType.Vector3;
		else if (type.IsEnum)
			propertyType = SerializedPropertyType.Enum;
		else if (typeof(MonoBehaviour).IsAssignableFrom(type))
			propertyType = SerializedPropertyType.ObjectReference;
		return propertyType != SerializedPropertyType.Generic;
	}
}