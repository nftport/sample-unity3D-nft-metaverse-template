using System.Collections;
using System.Collections.Generic;
using NFTPort.Internal;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class PlaceInCircle_Editor : EditorWindow
{
    public Vector3 AddTransform;
    public Vector3 AddRotation;
    
    public Vector3 LastTransform;
    public Vector3 LastRotation;
    public GameObject beadPrefab;
    
            
    public float Addz;
    public float Addx;
    public int numPoints;
    public Vector3 centrePos;
    
    [MenuItem("NFTPort/PlayGroundSceneHelper")]
    public static void ShowWindow()
    {
        var win = GetWindow<PlaceInCircle_Editor>("SceneHelper");
    }

    void OnGUI()
    {
        AddTransform = EditorGUILayout.Vector3Field("AddTransform", AddTransform); 
        AddRotation = EditorGUILayout.Vector3Field("AddRotation", AddRotation);
        beadPrefab = EditorGUILayout.ObjectField(beadPrefab, typeof(GameObject)) as GameObject;
        if (GUILayout.Button("Place"))
        {
            var go = PrefabUtility.InstantiatePrefab(beadPrefab as GameObject) as GameObject;
            go.transform.localPosition = LastTransform;
            go.transform.localRotation = Quaternion.Euler(LastRotation.x, LastRotation.y, LastRotation.z);
            
            //LastTransform = go.transform.position;
            //LastTransform = new Vector3(go.transform.rotation.eulerAngles.x, go.transform.rotation.eulerAngles.y, go.transform.rotation.eulerAngles.z);
            LastTransform +=AddTransform;
             LastRotation+=AddRotation;
        }

        if (GUILayout.Button("Reset LastT"))
        {
            LastTransform = AddTransform;
            LastRotation =AddRotation;
        }

        Addz = EditorGUILayout.FloatField("Addz", Addz);
         Addx = EditorGUILayout.FloatField("Addx", Addx);
         numPoints = EditorGUILayout.IntField("numPoints", numPoints);
         centrePos  = EditorGUILayout.Vector3Field("centrePos", centrePos);
         
         if (GUILayout.Button("Curve eet"))
         {
             for (var pointNum = 0; pointNum < numPoints; pointNum++)
             {
                 // "i" now represents the progress around the circle from 0-1
                 // we multiply by 1.0 to ensure we get a fraction as a result.
                 var i = (pointNum * 1.0) / numPoints;
                 // get the angle for this step (in radians, not degrees)
                 var angle = i * Mathf.PI * 2f;
                 // the X &amp; Y position for this angle are calculated using Sin &amp; Cos
                 var x = Mathf.Sin((float) angle) * Addz;
                 var z = Mathf.Cos((float) angle) * Addx;
                 var pos = new Vector3(x, 0, z) + centrePos;
                 // no need to assign the instance to a variable unless you're using it afterwards:
                 var go = PrefabUtility.InstantiatePrefab(beadPrefab) as GameObject;
                 go.transform.localPosition = pos;
                 go.transform.LookAt(-centrePos);
             }
         }
    }

    /*
    void Start()
    {
        var numPoints = 20;
        var centrePos = new Vector3(0, 0, 32);

        for (var pointNum = 0; pointNum < numPoints; pointNum++)
        {
            // "i" now represents the progress around the circle from 0-1
            // we multiply by 1.0 to ensure we get a fraction as a result.
            var i = (pointNum * 1.0) / numPoints;
            // get the angle for this step (in radians, not degrees)
            var angle = i * Mathf.PI * 2f;
            // the X &amp; Y position for this angle are calculated using Sin &amp; Cos
            var x = Mathf.Sin((float)angle) * Addz;
            var y = Mathf.Cos((float)angle) * Addy;
            var pos = new Vector3(x, y, 0) + centrePos;
            // no need to assign the instance to a variable unless you're using it afterwards:
            Instantiate(beadPrefab, pos, Quaternion.identity);

        }
    }
    */

}
#endif