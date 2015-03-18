using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class ParticleFilter
    {
        public int numOfParticles; // Number of particles
        public int numOfDataPoints;
        public double dt;
        public double[] y;

        public ParticleFilter(int numOfDataPoints_, int numOfParticles_, double dt_)
        {
            numOfDataPoints = numOfDataPoints_;
            numOfParticles = numOfParticles_;
            dt = dt_;
        }

        public ParticleFilterOutput ParticleFilterMain()
        {
            //********** Initial setting of state **********
            double[] vp1;
            vp1 = SpecialFunction.randN(numOfParticles, 0.25, 0.02);

            // local variable declarations
            //int i,j,k; // indexes
            int ite; // negative vol correction index
            //bool negVol;
            double temp;
            double noisev, noisevw;
            double def1, def2, def3;
            double sdt = Math.Sqrt(dt);
            double ddt, sddt,yy, def21,def22;
            double NS_eff;
            Random rand1 = new Random();
            double[] vp2 = new double[numOfParticles];
            double[] vp21 = new double[5*50]; //for -ve vol correction
            double[] W = new double[numOfParticles];
            double[] Wn = new double[numOfParticles];
            double[] C = new double[numOfParticles + 1];
            double[] U = new double[numOfParticles];
            double[] NumC = new double[numOfParticles + 1];
            int[] Bins = new int[numOfParticles];
            double[] TempParticleArray = new double[numOfParticles];
            double[] vE = new double[numOfDataPoints-1];
            double[] kappaE = new double[numOfDataPoints-1];
            //double[] kappaV = new double[numOfDataPoints];
            double[] thetaE = new double[numOfDataPoints-1];
            //double[] thetaV = new double[numOfDataPoints];
            double[] xiE = new double[numOfDataPoints-1];
            //double[] xiV = new double[numOfDataPoints];
            double[] muE = new double[numOfDataPoints-1];
            //double[] muV = new double[numOfDataPoints];
            double[] rhoE = new double[numOfDataPoints-1];
            //double[] rhoV = new double[numOfDataPoints];
            ParticleFilterOutput Output = new ParticleFilterOutput();
                            
        //   ********** Initial setting of parameters **********
        // 
        //   par[0,:] = Kappa;  1 < Kappa < 10
        //   par[1,:] = Theta;  0.05 < Theta < 0.5
        //   par[2,:] = Xi;     0.1 < Xi < 0.9
        //   par[3,:] = Mu;     0.05 < Mu < 0.3
        //   par[4,:] = Rho;    -0.999 < Rho < =0
        
            double[,] par = new double[5, numOfParticles];

            UF.Set_ith_row(ref par, SpecialFunction.randU(numOfParticles, 1, 9), 0); // kappa
            UF.Set_ith_row(ref par, SpecialFunction.randU(numOfParticles, 0.05, 0.45), 1); // theta
            UF.Set_ith_row(ref par, SpecialFunction.randU(numOfParticles, 0.1, 0.8), 2); // Xi
            UF.Set_ith_row(ref par, SpecialFunction.randU(numOfParticles, 0.05, 0.25), 3); // mu
            UF.Set_ith_row(ref par, SpecialFunction.randU(numOfParticles, 0.0, -1), 4); // Rho

            int NS_thres = (int) Math.Ceiling(2.0 * numOfParticles / 3.0);

            
            for (int ii = 0; ii < numOfParticles; ii++)
            {
                W[ii] = 1.0/(numOfParticles);
            }

            double[] IND = new double[numOfDataPoints];
            
            for (int i = 0; i < numOfDataPoints-1; i++)
            {
                // small noise are added
                
                UF.Set_ith_row(ref par, UF.ArrayAdd(UF.Get_ith_row(par, 0), SpecialFunction.randU(numOfParticles, -6 * dt, 2 * 6 * dt)),0);
                UF.Set_ith_row(ref par, UF.ArrayAdd(UF.Get_ith_row(par, 1), SpecialFunction.randU(numOfParticles, -4 * dt, 2 * 4 * dt)), 1);
                UF.Set_ith_row(ref par, UF.ArrayAdd(UF.Get_ith_row(par, 2), SpecialFunction.randU(numOfParticles, -3 * dt, 2 * 3 * dt)), 2);
                UF.Set_ith_row(ref par, UF.ArrayAdd(UF.Get_ith_row(par, 3), SpecialFunction.randU(numOfParticles, -4 * dt, 2 * 4 * dt)), 3);
                UF.Set_ith_row(ref par, UF.ArrayAdd(UF.Get_ith_row(par, 4), SpecialFunction.randU(numOfParticles, -0.2 * dt, 2 * 0.2 * dt)), 4);

                for (int j = 0; j < numOfParticles; j++)
                {
                    //negVol = false;
                    noisev = SpecialFunction.inverseCummNormal(rand1.NextDouble());
                    noisevw = noisev;

                    def1 = vp1[j] + par[0, j] * (par[1, j] - vp1[j]) * dt -
                        par[2, j] * par[4, j] * (par[3, j] - 0.5 * vp1[j]) * dt;
                    def2 = vp1[j] * (1 - par[4, j] * par[4, j]);
                    def3 = 1 + 0.5 * par[2, j] * par[4, j] * dt;

                    // ******** System update ***********

                    vp2[j] = def1 + par[2, j] * Math.Sqrt(def2) * noisev * sdt +
                        par[2, j] * par[4, j] * (y[i + 1] - y[i]);

                    // adjust negative vol

                    if (vp2[j] < 0)
                    {
                        //negVol = true;
                        vp2[j] = -vp2[j];
                    }
                    else if (vp2[j] == 0)
                    {
                        //negVol = true;
                        vp2[j] = vp1[j];
                    }


                    //ite = 0;

                    //while (vp2[j] <= 0 || Double.IsNaN(vp2[j]))
                    //{
                    //    ite = ite + 1;
                    //    vp21[0] = vp1[j];
                    //    ddt = dt / (ite * 50);
                    //    sddt = Math.Sqrt(ddt);

                    //    for (int jj = 0; jj < ite * 50; jj++)
                    //    {
                    //        yy = (y[i + 1] - y[i]) / (ite * 50);
                    //        noisev = SpecialFunction.inverseCummNormal(rand1.NextDouble());
                    //        def21 = vp21[jj] + par[0, j] * (par[1, j] - vp21[jj]) * ddt - par[2, j] * par[4, j] * (par[3, j] - 0.5 * vp21[jj]) * ddt;
                    //        def22 = vp21[jj] * (1 - par[4, j] * par[4, j]);
                    //        vp21[jj + 1] = def21 + par[2, j] * Math.Sqrt(def22) * noisev * sddt +
                    //            par[2, j] * par[4, j] * (yy);
                    //    }
                    //    vp2[j] = vp21[ite * 50];

                    //    noisevw = (vp2[j] - vp1[j] - par[0, j]) * (par[1, j] - vp2[j]) * dt +
                    //        par[2, j] * par[4, j] * (par[3, j] - 0.5 * vp1[j]) * dt -
                    //        par[2, j] * par[4, j] * (y[i + 1] - y[i]) / (par[2, j] * Math.Sqrt(vp1[j] * (1 - par[4, j] * par[4, j])) * sdt);
                        
                    //    if (ite >= 5)
                    //        break;

                    //}

                    double mL, vL, L;
                    double mI, vI, I;
                    double mT, vT, T;

                    // *************** L.H. function ****************

                    mL = y[i] + (par[3, j] - 0.5 * vp2[j]) * dt;
                    vL = vp1[j] * dt;
                    L = SpecialFunction.normalPDF(y[i + 1], mL, vL);

                    // ************** Importance function ***********

                    mI = def1 + par[2, j] * par[4, j] * (y[i + 1] - y[i]);
                    vI = par[2, j] * par[2, j] * def2 * dt;
                    I = SpecialFunction.normalPDF(vp2[j], mI, vI);

                    // ************** Transition density ************

                    mT = (def1 + par[2, j] * par[3, j] * par[4, j] * dt) / def3;
                    vT = par[2, j] * par[2, j] * vp1[j] * dt / (def3 * def3);
                    T = SpecialFunction.normalPDF(vp2[j], mT, vT);

                    temp = W[j];
                    W[j] = W[j] * L * T / I;
                    if (W[j] == 0 || Double.IsNaN(W[j]))
                    {
                        W[j] = temp;
                    }
                    
                    //W[j] = W[j] * L;
                    //W[j] = W[j] + Math.Log(L);

                    if (Double.IsNaN(W[j])||W[j]==0)
                    {
                        TempParticleArray[0] = 0;
                    }

                } // end of for num of particles

                Wn = UF.MulArrayByConst(W, 1 / UF.SumArray(W));
                
                vE[i] = UF.DotProduct(vp2, Wn);
                
                if(Double.IsNaN(vE[i]))
                    TempParticleArray[0] = 0;

                kappaE[i] = UF.DotProduct(UF.Get_ith_row(par, 0), Wn);
                //kappaV[i] = UF.DotProduct(UF.ArrayProduct(
                //    UF.Get_ith_row(par, 0), UF.Get_ith_row(par, 0)), Wn) - 
                //    kappaE[i] * kappaE[i];

                thetaE[i] = UF.DotProduct(UF.Get_ith_row(par, 1), Wn);
                //thetaV[i] = UF.DotProduct(UF.ArrayProduct(
                //    UF.Get_ith_row(par, 1), UF.Get_ith_row(par, 1)), Wn) -
                //    thetaE[i] * thetaE[i];

                xiE[i] = UF.DotProduct(UF.Get_ith_row(par, 2), Wn);
                //xiV[i] = UF.DotProduct(UF.ArrayProduct(
                //    UF.Get_ith_row(par, 2), UF.Get_ith_row(par, 2)), Wn) -
                //    xiE[i] * xiE[i];

                muE[i] = UF.DotProduct(UF.Get_ith_row(par, 3), Wn);
                //muV[i] = UF.DotProduct(UF.ArrayProduct(
                //    UF.Get_ith_row(par, 3), UF.Get_ith_row(par, 3)), Wn) -
                //    muE[i] * muE[i];

                rhoE[i] = UF.DotProduct(UF.Get_ith_row(par, 4), Wn);
                //rhoV[i] = BasicOperations.DotProduct(BasicOperations.ArrayProduct(
                //    BasicOperations.Get_ith_row(par, 4), BasicOperations.Get_ith_row(par, 4)), Wn) -
                //    rhoE[i] * rhoE[i];

                NS_eff = 1 / UF.SumArray(UF.ArrayProduct(Wn, Wn));

                if (NS_eff >= NS_thres)
                {
                    IND[i] = 0;
                    UF.Copy1DArrayL2R(vp2, ref vp1);
                }
                else
                {
                    IND[i] = 1;
                    C = UF.CummSum(UF.Append(Wn, 0, false));
                    // try out with different combinations
                    for (int k = 0; k < numOfParticles; k++)
                        U[k] = k;
                    U = UF.AddConst2Array(U, rand1.NextDouble());
                    U = UF.MulArrayByConst(U,1.0/numOfParticles);

                    NumC = UF.Histc(U, C, ref Bins);

                    TempParticleArray = UF.GetIndexVals(UF.Get_ith_row(par, 0), Bins);
                    UF.Set_ith_row(ref par, TempParticleArray, 0);

                    TempParticleArray = UF.GetIndexVals(UF.Get_ith_row(par, 1), Bins);
                    UF.Set_ith_row(ref par, TempParticleArray, 1);

                    TempParticleArray = UF.GetIndexVals(UF.Get_ith_row(par, 2), Bins);
                    UF.Set_ith_row(ref par, TempParticleArray, 2);

                    TempParticleArray = UF.GetIndexVals(UF.Get_ith_row(par, 3), Bins);
                    UF.Set_ith_row(ref par, TempParticleArray, 3);

                    TempParticleArray = UF.GetIndexVals(UF.Get_ith_row(par, 4), Bins);
                    UF.Set_ith_row(ref par, TempParticleArray, 4);

                    vp1 = UF.GetIndexVals(vp2, Bins);

                    for (int ii = 0; ii < numOfParticles; ii++)
                    {
                        W[ii] = 1.0 / numOfParticles;
                    }

                }

            }// end for Num data points

            Output.vol = vE;
            Output.kappa = kappaE;
            Output.theta = thetaE;
            Output.xi = xiE;
            Output.mu = muE;
            Output.rho = rhoE;

            return Output;

        }// end particle fun

    }

    public struct ParticleFilterOutput
    {
        public double[] vol;
        public double[] kappa;
        public double[] theta;
        public double[] xi;
        public double[] mu;
        public double[] rho;
    }
}
