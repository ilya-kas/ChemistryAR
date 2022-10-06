using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MarkerPrefab
{
    public MarkerType prefab;
    public TrackableId id;

    public MarkerPrefab(MarkerType prefab, TrackableId id)
    {
        this.prefab = prefab;
        this.id = id;
    }
}

public class SceneManager: MonoBehaviour
{
    public List<MarkerPrefab> placed = new List<MarkerPrefab>();
    const Double CONNECTION_DIST = 0.07*1.2;
    
    public void Init()
    {
        placed.Clear();
    }
    
    /**
     * public for list management
     */

    public void Add(MarkerType variant, ARTrackedImage trackedImage)
    {
        GameObject newbieObject = Instantiate(variant.targetPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
        newbieObject.SetActive(true);
        MarkerType newbie = new MarkerType(variant.type, newbieObject); 
        placed.Add(new MarkerPrefab(newbie, trackedImage.trackableId));
    }

    public void Delete(MarkerPrefab target)
    {
        Destroy(target.prefab.targetPrefab);
        placed.Remove(target);
    }
    
    public void TeleportModel(MarkerPrefab target, Transform newPosition)
    {
        var targetPosition = target.prefab.targetPrefab.transform;
        
        Vector3 position = newPosition.position;
        position.y += 0.05f;
        targetPosition.position = position;

        targetPosition.rotation = newPosition.rotation;
    }
    
    /**
     * calculations
     */

    public void UpdateChemicals()
    {
        String[] connections = CalculateNear();
        Log("connected groups: "+connections.Length);
        String[] molecules = AskForMolecules(connections);
        Log("molecules: "+molecules.Length);
        JoinMoleculesModels(molecules);
    }
    
    // Calculates connected tiles and returns string names for potential molecules.
    // Result is like ["H-1,O-2,S-3,",...] where 1 is position in prefab array
    String[] CalculateNear()
    {
        int n = placed.Count;
        List<int>[] nearest = new List<int>[n];

        for (int i=0;i<n;i++)
        {
            nearest[i] = new List<int>();
        }
        
        for (int i=0;i<n;i++)
            for (int j=0;j<n;j++)
                if (i != j &&
                    Distance(placed[i].prefab.targetPrefab.transform.position,
                        placed[j].prefab.targetPrefab.transform.position) <= CONNECTION_DIST)
                {
                    nearest[i].Add(j);
                }

        Graph graph = new Graph();
        graph.prepare(n, nearest);
        List<List<int>> components = graph.GetConnectionComponents();

        String[] result = new String[components.Count];
        for (int i=0;i<components.Count;i++)
            foreach (var node in components[i])
                result[i] += placed[node].prefab.type + "-" + node + ",";

        return result;
    }

    Double Distance(Vector3 a, Vector3 b)
    {
        Double dist1 = Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
        return Math.Sqrt((a.y - b.y) * (a.y - b.y) + dist1 * dist1);
    }

    // asks android app for string containing real molecules
    // returns ["H-1,O-2,H-3","H-4,Cl-5"]
    String[] AskForMolecules(String[] connections)
    {
        String request = "";
        foreach (var component in connections)
            request += component.Substring(0, component.Length - 1) + ";";
        request = request.Remove(request.Length - 1);
        
        AndroidJavaClass jc = new AndroidJavaClass("com.rubon.chemistryar.OverrideUnityActivity");
        AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
        String response = overrideActivity.Call<String>("calculateMolecules", request);

        if (response.Length == 0)
            return Array.Empty<string>();
        String[] molecules = response.Split(';');
        return molecules;
    }

    void JoinMoleculesModels(String[] molecules)
    {
        foreach (var molecule in molecules)
        {
            String[] atomsWithNums = molecule.Split(',');
            MarkerPrefab[] atoms = new MarkerPrefab[atomsWithNums.Length];
            for (int i = 0; i < atomsWithNums.Length; i++)
                atoms[i] = placed[GetAtomId(atomsWithNums[i])];
            
            JoinMolecule(atoms);
        }
    }

    int GetAtomId(String x)
    {
        Log("atom: "+x);
        var num = Regex.Replace(x, "[^0-9]", "");
        Log("atom id: "+num);
        return Int32.Parse(num);
    }

    void JoinMolecule(MarkerPrefab[] atoms)
    {
        Log("atoms in molecule: "+atoms.Length);
        var sumX = 0f;
        var sumY = 0f;
        var sumZ = 0f;
        var n = atoms.Length;
        foreach (var atom in atoms)
        {
            var position = atom.prefab.targetPrefab.transform.position;
            sumX += position.x;
            sumY += position.y;
            sumZ += position.z;
        }

        Vector3 center = new Vector3(sumX/n, sumY/n, sumZ/n);
        for (int i = 0; i < n; i++)
        {
            atoms[i].prefab.targetPrefab.transform.position = center;
            center = new Vector3(center.x, center.y + 0.05f, center.z);
        }
    }

    private void Log(String msg)
    {
        Debug.Log("My debug: "+msg);
    }
}
