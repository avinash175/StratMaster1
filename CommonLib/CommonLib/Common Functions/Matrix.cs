using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class Matrix
    {
        // Perform matrix multiplication
        public static double[,] MatrixMul(double[,] A, double[,] B)
        {
            if (A.GetUpperBound(1) != B.GetUpperBound(0))
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] C = new double[A.GetUpperBound(0) + 1, B.GetUpperBound(1) + 1];
            for (int i = 0; i <= C.GetUpperBound(0); i++)
                for (int j = 0; j <= C.GetUpperBound(1); j++)
                    for (int k = 0; k <= A.GetUpperBound(1); k++)
                        C[i, j] = C[i, j] + A[i, k] * B[k, j];

            return C;
        }

        // Performs matrix addition
        public static double[,] MatrixAdd(double[,] A, double[,] B, bool add)
        {
            if (A.GetUpperBound(0) != B.GetUpperBound(0) || A.GetUpperBound(1) != B.GetUpperBound(1))
            {
                throw new InvalidOperationException("Error: matrix dimentions don't match");
            }
            double[,] C = new double[A.GetUpperBound(0) + 1, A.GetUpperBound(1) + 1];
            if (add == true)
            {
                for (int i = 0; i <= C.GetUpperBound(0); i++)
                    for (int j = 0; j <= C.GetUpperBound(1); j++)
                        C[i, j] = A[i, j] + B[i, j];
            }
            else
            {
                for (int i = 0; i <= C.GetUpperBound(0); i++)
                    for (int j = 0; j <= C.GetUpperBound(1); j++)
                        C[i, j] = A[i, j] - B[i, j];
            }
            return C;
        }

        // returns transpose of a matrix
        public static double[,] Transpose(double[,] Mat)
        {
            double[,] result = new double[Mat.GetUpperBound(1) + 1, Mat.GetUpperBound(0) + 1];
            for (int i = 0; i <= Mat.GetUpperBound(0); i++)
                for (int j = 0; j <= Mat.GetUpperBound(1); j++)
                    result[j, i] = Mat[i, j];

            return result;
        }

        // Sort each column using Bubble Sort
        public static double[,] BubbleSort(double[,] Unsorted, bool assending)
        {
            double[,] Sorted = new double[Unsorted.GetUpperBound(0) + 1, Unsorted.GetUpperBound(1) + 1];
            UF.Copy2DArrayL2R(Unsorted, ref Sorted);
            double temp;

            if (assending == true)
            {
                for (int k = 0; k <= Sorted.GetUpperBound(1); k++)
                {
                    for (int i = 0; i < Sorted.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < Sorted.GetUpperBound(0) - i; j++)
                        {
                            if (Sorted[j + 1, k] < Sorted[j, k])
                            {
                                temp = Sorted[j, k];
                                Sorted[j, k] = Sorted[j + 1, k];
                                Sorted[j + 1, k] = temp;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int k = 0; k <= Sorted.GetUpperBound(1); k++)
                {
                    for (int i = 0; i < Sorted.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < Sorted.GetUpperBound(0) - i; j++)
                        {
                            if (Sorted[j + 1, k] > Sorted[j, k])
                            {
                                temp = Sorted[j, k];
                                Sorted[j, k] = Sorted[j + 1, k];
                                Sorted[j + 1, k] = temp;
                            }
                        }
                    }
                }
            }

            return Sorted;
        }


    }
 
    public class inv
    {
        /*************************************************************************
        Inversion of a matrix given by its LU decomposition.

        Input parameters:
            A       -   LU decomposition of the matrix (output of RMatrixLU subroutine).
            Pivots  -   table of permutations which were made during the LU decomposition
                        (the output of RMatrixLU subroutine).
            N       -   size of matrix A.

        Output parameters:
            A       -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool rmatrixluinverse(ref double[,] a,
            ref int[] pivots,
            int n)
        {
            bool result = new bool();
            double[] work = new double[0];
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            double v = 0;
            int i_ = 0;

            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            work = new double[n-1+1];
            
            //
            // Form inv(U)
            //
            if( !trinverse.rmatrixtrinverse(ref a, n, true, false) )
            {
                result = false;
                return result;
            }
            
            //
            // Solve the equation inv(A)*L = inv(U) for inv(A).
            //
            for(j=n-1; j>=0; j--)
            {
                
                //
                // Copy current column of L to WORK and replace with zeros.
                //
                for(i=j+1; i<=n-1; i++)
                {
                    work[i] = a[i,j];
                    a[i,j] = 0;
                }
                
                //
                // Compute current column of inv(A).
                //
                if( j<n-1 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = 0.0;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            v += a[i,i_]*work[i_];
                        }
                        a[i,j] = a[i,j]-v;
                    }
                }
            }
            
            //
            // Apply column interchanges.
            //
            for(j=n-2; j>=0; j--)
            {
                jp = pivots[j];
                if( jp!=j )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        work[i_] = a[i_,j];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,j] = a[i_,jp];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,jp] = work[i_];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Inversion of a general matrix.

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Output parameters:
            A   -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixinverse(ref double[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            lu.rmatrixlu(ref a, n, n, ref pivots);
            result = rmatrixluinverse(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        Obsolete 1-based subroutine.

        See RMatrixLUInverse for 0-based replacement.
        *************************************************************************/
        public static bool inverselu(ref double[,] a,
            ref int[] pivots,
            int n)
        {
            bool result = new bool();
            double[] work = new double[0];
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            int jp1 = 0;
            double v = 0;
            int i_ = 0;

            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            work = new double[n+1];
            
            //
            // Form inv(U)
            //
            if( !trinverse.invtriangular(ref a, n, true, false) )
            {
                result = false;
                return result;
            }
            
            //
            // Solve the equation inv(A)*L = inv(U) for inv(A).
            //
            for(j=n; j>=1; j--)
            {
                
                //
                // Copy current column of L to WORK and replace with zeros.
                //
                for(i=j+1; i<=n; i++)
                {
                    work[i] = a[i,j];
                    a[i,j] = 0;
                }
                
                //
                // Compute current column of inv(A).
                //
                if( j<n )
                {
                    jp1 = j+1;
                    for(i=1; i<=n; i++)
                    {
                        v = 0.0;
                        for(i_=jp1; i_<=n;i_++)
                        {
                            v += a[i,i_]*work[i_];
                        }
                        a[i,j] = a[i,j]-v;
                    }
                }
            }
            
            //
            // Apply column interchanges.
            //
            for(j=n-1; j>=1; j--)
            {
                jp = pivots[j];
                if( jp!=j )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        work[i_] = a[i_,j];
                    }
                    for(i_=1; i_<=n;i_++)
                    {
                        a[i_,j] = a[i_,jp];
                    }
                    for(i_=1; i_<=n;i_++)
                    {
                        a[i_,jp] = work[i_];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Obsolete 1-based subroutine.

        See RMatrixInverse for 0-based replacement.
        *************************************************************************/
        public static bool inverse(ref double[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            lu.ludecomposition(ref a, n, n, ref pivots);
            result = inverselu(ref a, ref pivots, n);
            return result;
        }
    }

    public class trinverse
    {
        /*************************************************************************
        Triangular matrix inversion

        The subroutine inverts the following types of matrices:
            * upper triangular
            * upper triangular with unit diagonal
            * lower triangular
            * lower triangular with unit diagonal

        In case of an upper (lower) triangular matrix,  the  inverse  matrix  will
        also be upper (lower) triangular, and after the end of the algorithm,  the
        inverse matrix replaces the source matrix. The elements  below (above) the
        main diagonal are not changed by the algorithm.

        If  the matrix  has a unit diagonal, the inverse matrix also  has  a  unit
        diagonal, and the diagonal elements are not passed to the algorithm.

        Input parameters:
            A       -   matrix.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   True, if the matrix is upper triangular.
            IsunitTriangular
                    -   True, if the matrix has a unit diagonal.

        Output parameters:
            A       -   inverse matrix (if the problem is not degenerate).

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool rmatrixtrinverse(ref double[,] a,
            int n,
            bool isupper,
            bool isunittriangular)
        {
            bool result = new bool();
            bool nounit = new bool();
            int i = 0;
            int j = 0;
            double v = 0;
            double ajj = 0;
            double[] t = new double[0];
            int i_ = 0;

            result = true;
            t = new double[n - 1 + 1];

            //
            // Test the input parameters.
            //
            nounit = !isunittriangular;
            if (isupper)
            {

                //
                // Compute inverse of upper triangular matrix.
                //
                for (j = 0; j <= n - 1; j++)
                {
                    if (nounit)
                    {
                        if (a[j, j] == 0)
                        {
                            result = false;
                            return result;
                        }
                        a[j, j] = 1 / a[j, j];
                        ajj = -a[j, j];
                    }
                    else
                    {
                        ajj = -1;
                    }

                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    if (j > 0)
                    {
                        for (i_ = 0; i_ <= j - 1; i_++)
                        {
                            t[i_] = a[i_, j];
                        }
                        for (i = 0; i <= j - 1; i++)
                        {
                            if (i < j - 1)
                            {
                                v = 0.0;
                                for (i_ = i + 1; i_ <= j - 1; i_++)
                                {
                                    v += a[i, i_] * t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if (nounit)
                            {
                                a[i, j] = v + a[i, i] * t[i];
                            }
                            else
                            {
                                a[i, j] = v + t[i];
                            }
                        }
                        for (i_ = 0; i_ <= j - 1; i_++)
                        {
                            a[i_, j] = ajj * a[i_, j];
                        }
                    }
                }
            }
            else
            {

                //
                // Compute inverse of lower triangular matrix.
                //
                for (j = n - 1; j >= 0; j--)
                {
                    if (nounit)
                    {
                        if (a[j, j] == 0)
                        {
                            result = false;
                            return result;
                        }
                        a[j, j] = 1 / a[j, j];
                        ajj = -a[j, j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    if (j < n - 1)
                    {

                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        for (i_ = j + 1; i_ <= n - 1; i_++)
                        {
                            t[i_] = a[i_, j];
                        }
                        for (i = j + 1; i <= n - 1; i++)
                        {
                            if (i > j + 1)
                            {
                                v = 0.0;
                                for (i_ = j + 1; i_ <= i - 1; i_++)
                                {
                                    v += a[i, i_] * t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if (nounit)
                            {
                                a[i, j] = v + a[i, i] * t[i];
                            }
                            else
                            {
                                a[i, j] = v + t[i];
                            }
                        }
                        for (i_ = j + 1; i_ <= n - 1; i_++)
                        {
                            a[i_, j] = ajj * a[i_, j];
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Obsolete 1-based subroutine.
        See RMatrixTRInverse for 0-based replacement.
        *************************************************************************/
        public static bool invtriangular(ref double[,] a,
            int n,
            bool isupper,
            bool isunittriangular)
        {
            bool result = new bool();
            bool nounit = new bool();
            int i = 0;
            int j = 0;
            int nmj = 0;
            int jm1 = 0;
            int jp1 = 0;
            double v = 0;
            double ajj = 0;
            double[] t = new double[0];
            int i_ = 0;

            result = true;
            t = new double[n + 1];

            //
            // Test the input parameters.
            //
            nounit = !isunittriangular;
            if (isupper)
            {

                //
                // Compute inverse of upper triangular matrix.
                //
                for (j = 1; j <= n; j++)
                {
                    if (nounit)
                    {
                        if (a[j, j] == 0)
                        {
                            result = false;
                            return result;
                        }
                        a[j, j] = 1 / a[j, j];
                        ajj = -a[j, j];
                    }
                    else
                    {
                        ajj = -1;
                    }

                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    if (j > 1)
                    {
                        jm1 = j - 1;
                        for (i_ = 1; i_ <= jm1; i_++)
                        {
                            t[i_] = a[i_, j];
                        }
                        for (i = 1; i <= j - 1; i++)
                        {
                            if (i < j - 1)
                            {
                                v = 0.0;
                                for (i_ = i + 1; i_ <= jm1; i_++)
                                {
                                    v += a[i, i_] * t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if (nounit)
                            {
                                a[i, j] = v + a[i, i] * t[i];
                            }
                            else
                            {
                                a[i, j] = v + t[i];
                            }
                        }
                        for (i_ = 1; i_ <= jm1; i_++)
                        {
                            a[i_, j] = ajj * a[i_, j];
                        }
                    }
                }
            }
            else
            {

                //
                // Compute inverse of lower triangular matrix.
                //
                for (j = n; j >= 1; j--)
                {
                    if (nounit)
                    {
                        if (a[j, j] == 0)
                        {
                            result = false;
                            return result;
                        }
                        a[j, j] = 1 / a[j, j];
                        ajj = -a[j, j];
                    }
                    else
                    {
                        ajj = -1;
                    }
                    if (j < n)
                    {

                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        nmj = n - j;
                        jp1 = j + 1;
                        for (i_ = jp1; i_ <= n; i_++)
                        {
                            t[i_] = a[i_, j];
                        }
                        for (i = j + 1; i <= n; i++)
                        {
                            if (i > j + 1)
                            {
                                v = 0.0;
                                for (i_ = jp1; i_ <= i - 1; i_++)
                                {
                                    v += a[i, i_] * t[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            if (nounit)
                            {
                                a[i, j] = v + a[i, i] * t[i];
                            }
                            else
                            {
                                a[i, j] = v + t[i];
                            }
                        }
                        for (i_ = jp1; i_ <= n; i_++)
                        {
                            a[i_, j] = ajj * a[i_, j];
                        }
                    }
                }
            }
            return result;
        }
    }

    public class lu
    {
        public const int lunb = 8;


        /*************************************************************************
        LU decomposition of a general matrix of size MxN

        The subroutine calculates the LU decomposition of a rectangular general
        matrix with partial pivoting (with row permutations).

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices L and U in compact form (see below).
                    Array whose indexes range within [0..M-1, 0..N-1].
            Pivots - permutation matrix in compact form (see below).
                    Array whose index ranges within [0..Min(M-1,N-1)].

        Matrix A is represented as A = P * L * U, where P is a permutation matrix,
        matrix L - lower triangular (or lower trapezoid, if M>N) matrix,
        U - upper triangular (or upper trapezoid, if M<N) matrix.

        Let M be equal to 4 and N be equal to 3:

                           (  1          )    ( U11 U12 U13  )
        A = P1 * P2 * P3 * ( L21  1      )  * (     U22 U23  )
                           ( L31 L32  1  )    (         U33  )
                           ( L41 L42 L43 )

        Matrix L has size MxMin(M,N), matrix U has size Min(M,N)xN, matrix P(i) is
        a permutation of the identity matrix of size MxM with numbers I and Pivots[I].

        The algorithm returns array Pivots and the following matrix which replaces
        matrix A and contains matrices L and U in compact form (the example applies
        to M=4, N=3).

         ( U11 U12 U13 )
         ( L21 U22 U23 )
         ( L31 L32 U33 )
         ( L41 L42 L43 )

        As we can see, the unit diagonal isn't stored.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        public static void rmatrixlu(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            double[,] b = new double[0, 0];
            double[] t = new double[0];
            int[] bp = new int[0];
            int minmn = 0;
            int i = 0;
            int ip = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int cb = 0;
            int nb = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(lunb >= 1, "RMatrixLU internal error");
            nb = lunb;

            //
            // Decide what to use - blocked or unblocked code
            //
            if (n <= 1 | Math.Min(m, n) <= nb | nb == 1)
            {

                //
                // Unblocked code
                //
                rmatrixlu2(ref a, m, n, ref pivots);
            }
            else
            {

                //
                // Blocked code.
                // First, prepare temporary matrix and indices
                //
                b = new double[m - 1 + 1, nb - 1 + 1];
                t = new double[n - 1 + 1];
                pivots = new int[Math.Min(m, n) - 1 + 1];
                minmn = Math.Min(m, n);
                j1 = 0;
                j2 = Math.Min(minmn, nb) - 1;

                //
                // Main cycle
                //
                while (j1 < minmn)
                {
                    cb = j2 - j1 + 1;

                    //
                    // LU factorization of diagonal and subdiagonal blocks:
                    // 1. Copy columns J1..J2 of A to B
                    // 2. LU(B)
                    // 3. Copy result back to A
                    // 4. Copy pivots, apply pivots
                    //
                    for (i = j1; i <= m - 1; i++)
                    {
                        i1_ = (j1) - (0);
                        for (i_ = 0; i_ <= cb - 1; i_++)
                        {
                            b[i - j1, i_] = a[i, i_ + i1_];
                        }
                    }
                    rmatrixlu2(ref b, m - j1, cb, ref bp);
                    for (i = j1; i <= m - 1; i++)
                    {
                        i1_ = (0) - (j1);
                        for (i_ = j1; i_ <= j2; i_++)
                        {
                            a[i, i_] = b[i - j1, i_ + i1_];
                        }
                    }
                    for (i = 0; i <= cb - 1; i++)
                    {
                        ip = bp[i];
                        pivots[j1 + i] = j1 + ip;
                        if (bp[i] != i)
                        {
                            if (j1 != 0)
                            {

                                //
                                // Interchange columns 0:J1-1
                                //
                                for (i_ = 0; i_ <= j1 - 1; i_++)
                                {
                                    t[i_] = a[j1 + i, i_];
                                }
                                for (i_ = 0; i_ <= j1 - 1; i_++)
                                {
                                    a[j1 + i, i_] = a[j1 + ip, i_];
                                }
                                for (i_ = 0; i_ <= j1 - 1; i_++)
                                {
                                    a[j1 + ip, i_] = t[i_];
                                }
                            }
                            if (j2 < n - 1)
                            {

                                //
                                // Interchange the rest of the matrix, if needed
                                //
                                for (i_ = j2 + 1; i_ <= n - 1; i_++)
                                {
                                    t[i_] = a[j1 + i, i_];
                                }
                                for (i_ = j2 + 1; i_ <= n - 1; i_++)
                                {
                                    a[j1 + i, i_] = a[j1 + ip, i_];
                                }
                                for (i_ = j2 + 1; i_ <= n - 1; i_++)
                                {
                                    a[j1 + ip, i_] = t[i_];
                                }
                            }
                        }
                    }

                    //
                    // Compute block row of U
                    //
                    if (j2 < n - 1)
                    {
                        for (i = j1 + 1; i <= j2; i++)
                        {
                            for (j = j1; j <= i - 1; j++)
                            {
                                v = a[i, j];
                                for (i_ = j2 + 1; i_ <= n - 1; i_++)
                                {
                                    a[i, i_] = a[i, i_] - v * a[j, i_];
                                }
                            }
                        }
                    }

                    //
                    // Update trailing submatrix
                    //
                    if (j2 < n - 1)
                    {
                        for (i = j2 + 1; i <= m - 1; i++)
                        {
                            for (j = j1; j <= j2; j++)
                            {
                                v = a[i, j];
                                for (i_ = j2 + 1; i_ <= n - 1; i_++)
                                {
                                    a[i, i_] = a[i, i_] - v * a[j, i_];
                                }
                            }
                        }
                    }

                    //
                    // Next step
                    //
                    j1 = j2 + 1;
                    j2 = Math.Min(minmn, j1 + nb) - 1;
                }
            }
        }


        /*************************************************************************
        Obsolete 1-based subroutine. Left for backward compatibility.
        See RMatrixLU for 0-based replacement.
        *************************************************************************/
        public static void ludecomposition(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double[] t1 = new double[0];
            double s = 0;
            int i_ = 0;

            pivots = new int[Math.Min(m, n) + 1];
            t1 = new double[Math.Max(m, n) + 1];
            System.Diagnostics.Debug.Assert(m >= 0 & n >= 0, "Error in LUDecomposition: incorrect function arguments");

            //
            // Quick return if possible
            //
            if (m == 0 | n == 0)
            {
                return;
            }
            for (j = 1; j <= Math.Min(m, n); j++)
            {

                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for (i = j + 1; i <= m; i++)
                {
                    if (Math.Abs(a[i, j]) > Math.Abs(a[jp, j]))
                    {
                        jp = i;
                    }
                }
                pivots[j] = jp;
                if (a[jp, j] != 0)
                {

                    //
                    //Apply the interchange to rows
                    //
                    if (jp != j)
                    {
                        for (i_ = 1; i_ <= n; i_++)
                        {
                            t1[i_] = a[j, i_];
                        }
                        for (i_ = 1; i_ <= n; i_++)
                        {
                            a[j, i_] = a[jp, i_];
                        }
                        for (i_ = 1; i_ <= n; i_++)
                        {
                            a[jp, i_] = t1[i_];
                        }
                    }

                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if (j < m)
                    {

                        //
                        // CALL DSCAL( M-J, ONE / A( J, J ), A( J+1, J ), 1 )
                        //
                        jp = j + 1;
                        s = 1 / a[j, j];
                        for (i_ = jp; i_ <= m; i_++)
                        {
                            a[i_, j] = s * a[i_, j];
                        }
                    }
                }
                if (j < Math.Min(m, n))
                {

                    //
                    //Update trailing submatrix.
                    //CALL DGER( M-J, N-J, -ONE, A( J+1, J ), 1, A( J, J+1 ), LDA,A( J+1, J+1 ), LDA )
                    //
                    jp = j + 1;
                    for (i = j + 1; i <= m; i++)
                    {
                        s = a[i, j];
                        for (i_ = jp; i_ <= n; i_++)
                        {
                            a[i, i_] = a[i, i_] - s * a[j, i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Obsolete 1-based subroutine. Left for backward compatibility.
        *************************************************************************/
        public static void ludecompositionunpacked(double[,] a,
            int m,
            int n,
            ref double[,] l,
            ref double[,] u,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int minmn = 0;

            a = (double[,])a.Clone();

            if (m == 0 | n == 0)
            {
                return;
            }
            minmn = Math.Min(m, n);
            l = new double[m + 1, minmn + 1];
            u = new double[minmn + 1, n + 1];
            ludecomposition(ref a, m, n, ref pivots);
            for (i = 1; i <= m; i++)
            {
                for (j = 1; j <= minmn; j++)
                {
                    if (j > i)
                    {
                        l[i, j] = 0;
                    }
                    if (j == i)
                    {
                        l[i, j] = 1;
                    }
                    if (j < i)
                    {
                        l[i, j] = a[i, j];
                    }
                }
            }
            for (i = 1; i <= minmn; i++)
            {
                for (j = 1; j <= n; j++)
                {
                    if (j < i)
                    {
                        u[i, j] = 0;
                    }
                    if (j >= i)
                    {
                        u[i, j] = a[i, j];
                    }
                }
            }
        }


        /*************************************************************************
        Level 2 BLAS version of RMatrixLU

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        private static void rmatrixlu2(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double[] t1 = new double[0];
            double s = 0;
            int i_ = 0;

            pivots = new int[Math.Min(m - 1, n - 1) + 1];
            t1 = new double[Math.Max(m - 1, n - 1) + 1];
            System.Diagnostics.Debug.Assert(m >= 0 & n >= 0, "Error in LUDecomposition: incorrect function arguments");

            //
            // Quick return if possible
            //
            if (m == 0 | n == 0)
            {
                return;
            }
            for (j = 0; j <= Math.Min(m - 1, n - 1); j++)
            {

                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for (i = j + 1; i <= m - 1; i++)
                {
                    if (Math.Abs(a[i, j]) > Math.Abs(a[jp, j]))
                    {
                        jp = i;
                    }
                }
                pivots[j] = jp;
                if (a[jp, j] != 0)
                {

                    //
                    //Apply the interchange to rows
                    //
                    if (jp != j)
                    {
                        for (i_ = 0; i_ <= n - 1; i_++)
                        {
                            t1[i_] = a[j, i_];
                        }
                        for (i_ = 0; i_ <= n - 1; i_++)
                        {
                            a[j, i_] = a[jp, i_];
                        }
                        for (i_ = 0; i_ <= n - 1; i_++)
                        {
                            a[jp, i_] = t1[i_];
                        }
                    }

                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if (j < m)
                    {
                        jp = j + 1;
                        s = 1 / a[j, j];
                        for (i_ = jp; i_ <= m - 1; i_++)
                        {
                            a[i_, j] = s * a[i_, j];
                        }
                    }
                }
                if (j < Math.Min(m, n) - 1)
                {

                    //
                    //Update trailing submatrix.
                    //
                    jp = j + 1;
                    for (i = j + 1; i <= m - 1; i++)
                    {
                        s = a[i, j];
                        for (i_ = jp; i_ <= n - 1; i_++)
                        {
                            a[i, i_] = a[i, i_] - s * a[j, i_];
                        }
                    }
                }
            }
        }
    }
}
