using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HumanRestSpace))]
public class RestSpaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add New RestSpot"))
            AddRestSpot();
            
    }

    private void AddRestSpot()
    {
        HumanRestSpace restSpace = (HumanRestSpace) target;

        var newRestSpot = new GameObject();
        newRestSpot.transform.parent = restSpace.transform;
        newRestSpot.transform.position = restSpace.transform.position;
        
        int number = restSpace.transform.childCount;
        var newRestSpotName = $"RestSpot{number}";
        newRestSpot.name = newRestSpotName;
        
        
        newRestSpot.AddComponent<HumanRestSpot>();
        
        var boxCollider = newRestSpot.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1, 2, 1);
        boxCollider.center = new Vector3(0, 1, 0);
        
        var rigidbody = newRestSpot.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        Selection.activeGameObject = newRestSpot;
    }
}