using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class Graph
    {
        //public static 
        public Nodes Nodes { get; set; }
        public Edges Edges { get; set; }
        public Paths CliquePaths { get; set; }
        public Paths MaximalPaths { get; set; }

        public Graph()
        {
            Nodes = new Nodes();
            Edges = new Edges();
            CliquePaths = new Paths();
            MaximalPaths = new Paths();
        }

        public void FillCyclesForOrderedNodes()
        {
            List<bool> isMaximal = new List<bool>();
            for (int i = 0; i < Edges.E.Count; i++)
            {
                string line = Edges.E[i];
                string[] nodes = line.Split(',');
                CliquePaths.P.Add(nodes);
                isMaximal.Add(true);
            }
            
            for (int i = 0; i < CliquePaths.P.Count; i++)
            {                
                for (int j = i; j < CliquePaths.P.Count; j++)
                {
                    if (MatchInitialNodes(CliquePaths.P[i], CliquePaths.P[j]))
                    {
                        int len = CliquePaths.P[i].Length;
                        if(Edges.E.Contains(CliquePaths.P[i][len-1]+","+CliquePaths.P[j][len-1]))
                        {
                            string[] temp = UF.AppendArray<string>(CliquePaths.P[i], 
                                new string[1] { CliquePaths.P[j][len - 1] });
                            isMaximal[i] = false;
                            isMaximal[j] = false;
                            CliquePaths.P.Add(temp);
                            isMaximal.Add(true);
                        }
                    }
                    else
                    {
                        break;
                    }
                }               
            }

            for (int i = 0; i < CliquePaths.P.Count; i++)
            {
                if (isMaximal[i])
                {
                    MaximalPaths.P.Add(CliquePaths.P[i]);
                }
            }
        }

        private bool MatchInitialNodes(string[] path1, string[] path2)
        {
            if (path1.Length == path2.Length)
            {
                bool match = true;
                for (int i = 0; i < path1.Length - 1; i++)
                {
                    if (path1[i] != path2[i])
                    {
                        match = false;
                        break;
                    }
                }
                return match;
            }
            else
            {
                return false;
            }
        }
    }

    public class Nodes
    {
        public List<string> N { get; set; }

        public Nodes()
        {
            N = new List<string>();
        }
    }
       
    public class Edges
    {
        public List<string> E { get; set; }

        public Edges()
        {
            E = new List<string>();
        }
    }

    public class Paths
    {
        public List<string[]> P { get; set; }

        public Paths()
        {
            P = new List<string[]>();
        }
    }
}
