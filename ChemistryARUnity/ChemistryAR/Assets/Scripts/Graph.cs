using System.Collections.Generic;

public class Graph
{
    private int n;
    private List<int>[] nearest;
    private int[] color;

    public void prepare(int n, List<int>[] nearest)
    {
        this.n = n;
        this.nearest = nearest;
    }
    
    public List<List<int>> GetConnectionComponents()
    {
        color = new int[n];
        int currentColor = 1;
        for (int i=0;i<n;i++)
            if (color[i] == 0)
            {
                DfsPaint(i, currentColor);
                currentColor++;
            }
        
        
        var result = new List<List<int>>();
        for (int i=1;i<currentColor;i++)
            result.Add(new List<int>());

        for (int i = 0; i < n; i++)
            result[color[i] - 1].Add(i);

        return result;
    }

    void DfsPaint(int nom, int newColor)
    {
        color[nom] = newColor;

        foreach (var node in nearest[nom])
            if (color[node]==0)
                DfsPaint(node, newColor);
    }
}