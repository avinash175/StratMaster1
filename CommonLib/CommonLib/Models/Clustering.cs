using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommonLib
{
    class Clustering
    {
    }

    public class Kmeans
    {
        public static int[] Classify(double[,] obj, int numClust, DistanceType DT)
        {
            int numObj = obj.GetUpperBound(0) + 1;
            int numParas = obj.GetUpperBound(1) + 1;
            int[] clusters = new int[numObj];
            int MaxIter = 1000;
            double epsilon = 10e-5;
            
            double[,] Centroids = new double[numClust, numParas];
            double[,] PrevCentroids = new double[numClust, numParas];

            //initially choose centroids randomly
            Random rnd = new Random(1);

            for (int i = 0; i < numClust; i++)
            {
                int idx = rnd.Next(numObj);
                UF.Set_ith_row(ref Centroids, UF.Get_ith_row(obj, idx), i);
            }

            double error = 1.0;

            for (int i = 0; i < MaxIter && error < epsilon; i++)
            {
                // Find clusters for all the stocks
                for (int j = 0; j < numObj; j++)
                {
                    double[] dist = new double[numClust];
                    for (int k = 0; k < numClust; k++)
                    {
                        double[] X = UF.Get_ith_row(obj,j);
                        double[] Y = UF.Get_ith_row(Centroids,k);
                        if (DT == DistanceType.Eucledian)
                        {
                            dist[k] = UF.EucledianDist(X, Y);
                        }
                        else if (DT == DistanceType.Correlation)
                        {
                            dist[k] = 1.0 - UF.Correlation(X, Y);
                        }                        
                    }
                    clusters[j] = UF.MinArrayIdx(dist);
                }
                // Find new centroids of the clusters
                UF.Copy2DArrayL2R(Centroids, ref PrevCentroids);
            }
            return clusters;
        }

        public static int[] Classify(double[][] obj, int numClust, DistanceType DT)
        {
            int numObj = obj.Length;
            int numParas = obj[0].Length;
            int[] clusters = new int[numObj];
            int MaxIter = 1000;
            double epsilon = 10e-5;

            double[,] Centroids = new double[numClust, numParas];
            double[,] PrevCentroids = new double[numClust, numParas];

            //initially choose centroids randomly
            Random rnd = new Random(1);

            for (int i = 0; i < numClust; i++)
            {
                int idx = rnd.Next(numObj);
                UF.Set_ith_row(ref Centroids, obj[idx], i);
            }

            double error = 1.0;

            //0 to numClust-1 clusters
            for (int i = 0; i < MaxIter && error > epsilon; i++)
            {
                // Find clusters for all the obj
                for (int j = 0; j < numObj; j++)
                {
                    double[] dist = new double[numClust];
                    for (int k = 0; k < numClust; k++)
                    {
                        double[] X = obj[j];
                        double[] Y = UF.Get_ith_row(Centroids, k);
                        if (DT == DistanceType.Eucledian)
                        {
                            dist[k] = UF.EucledianDist(X, Y);
                        }
                        else if (DT == DistanceType.Correlation)
                        {
                            dist[k] = 1.0 - UF.Correlation(X, Y);
                        }
                    }
                    clusters[j] = UF.MinArrayIdx(dist);
                }
                // Find new centroids of the clusters
                UF.Copy2DArrayL2R(Centroids, ref PrevCentroids);

                for (int k = 0; k < numClust; k++)
                {
                    int[] idxs = clusters.Select((x, j) => new { Idx = j, Val = x }).
                        Where(x => x.Val == k).Select(x => x.Idx).ToArray();

                    double[][] cenVals = obj.Where((x, j) => idxs.Contains(j)).ToArray();
                    double[] sum;
                    if (idxs.Length > 0)
                    {
                        sum = cenVals[0];
                        for (int j = 1; j < idxs.Length; j++)
                        {
                            sum = UF.ArrayAdd(cenVals[j], sum);
                        }
                        sum = UF.MulArrayByConst(sum, 1.0 / idxs.Length);
                        UF.Set_ith_row(ref Centroids, sum, k);
                    }              
                }
                double errorSum = 0;
                for (int k = 0; k < numClust; k++)
                {
                    for (int j = 0; j < numParas; j++)
                    {
                        errorSum+=Math.Pow(Centroids[k,j]-PrevCentroids[k,j],2.0);
                    }
                }
                error = Math.Sqrt(errorSum / (numClust * numParas));
            }
            return clusters;
        }
    }
}
