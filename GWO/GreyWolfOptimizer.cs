using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace GWO
{
    public class GreyWolfOptimizer : IOptimizationAlgorithm
    {
        public string Name { get; set; } = "Grey_Wolf_Optimizer";
        public double[] XBest { get; set; }
        public double FBest { get; set; }
        public int dim { get; set; } = 3;
        public int NumberOfEvaluationFitnessFunction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //Część parametryczna

        public double Func(double[] X)
        {
            double przesuniecie = 1.54;
            for (int i = 0; i < this.dim; i++)
            {
                X[0] = X[0] - przesuniecie;
            }
            double A = 10;
            double n = this.dim;
            double y = A * n;
            foreach(double x in X)
            {
                y += (x * x) - A * Math.Cos(Math.PI * 2 * x);
            }
            return y;
        }

        public double Solve()
        {
            // Parametry
            double lbound = -5.12;
            double ubound = 5.12;
            int wolfs_no = 80;
            int iter_num = 10;

            // Deklaracje
            Random random = new Random();
            double[] convergence = new double[iter_num];

            // Inicjalizacja Alfa, Beta, Delta
            double[,] best_wolfs_positions = new double[3,this.dim];
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < this.dim; j++)
                {
                    best_wolfs_positions[i, j] = random.NextDouble() * (ubound - lbound) + lbound;
                }
            }

            double[] best_wolfs_values = new double[3];
            for (int i = 0; i < 3; i++)
            {
                double[] row = new double[this.dim];
                for (int j = 0; j < this.dim; j++)
                {
                    row[j] = best_wolfs_positions[i,j];
                }
                best_wolfs_values[i] = this.Func(row);
            }

            // Inicjalizacja wilków
            double[,] wolfs_positions = new double[wolfs_no, this.dim];
            for (int i = 0; i < wolfs_no; i++)
            {
                for (int j = 0; j < this.dim; j++)
                {
                    wolfs_positions[i, j] = random.NextDouble() * (ubound - lbound) + lbound;
                }
            }

            double[] wolfs_values = new double[wolfs_no];
            for (int i = 0; i < wolfs_no; i++)
            {
                double[] row = new double[this.dim];
                for (int j = 0; j < this.dim; j++)
                {
                    row[j] = wolfs_positions[i, j];
                }
                wolfs_values[i] = this.Func(row);
            }

            // Start iteracji
            for (int iter = 0; iter < iter_num; iter++)
            {
                // Zbieżność A
                double a = 2 - iter * (2 / iter_num);


                // Dla każdego wilka szukamy czy jest lepszy od liderów
                for (int i = 0; i < wolfs_no; i++)
                {

                    // Jeżeli lepszy od alfy
                    if (wolfs_values[i] < best_wolfs_values[0])
                    {
                        best_wolfs_values[2] = best_wolfs_values[1];
                        best_wolfs_values[1] = best_wolfs_values[0];
                        best_wolfs_values[0] = wolfs_values[i];

                        for (int d = 0; d < this.dim; d++)
                        {
                            best_wolfs_positions[2, d] = best_wolfs_positions[1, d];
                            best_wolfs_positions[1, d] = best_wolfs_positions[0, d];
                            best_wolfs_positions[0, d] = wolfs_positions[i, d];
                        }
                    }

                    // Jeżeli lepszy od bety
                    if (wolfs_values[i] > best_wolfs_values[0] && wolfs_values[i] < best_wolfs_values[1])
                    {
                        best_wolfs_values[2] = best_wolfs_values[1];
                        best_wolfs_values[1] = wolfs_values[i];

                        for (int d = 0; d < this.dim; d++)
                        {
                            best_wolfs_positions[2, d] = best_wolfs_positions[1, d];
                            best_wolfs_positions[1, d] = wolfs_positions[i, d];
                        }
                    }

                    // Jeżeli lepszy od delty
                    if (wolfs_values[i] > best_wolfs_values[0] && wolfs_values[i] > best_wolfs_values[1] && wolfs_values[i] < best_wolfs_values[2])
                    {
                        best_wolfs_values[2] = wolfs_values[i];

                        for (int d = 0; d < this.dim; d++)
                        {
                            best_wolfs_positions[2, d] = wolfs_positions[i, d];
                        }
                    }
                }

                // Dla każdego wilka obliczamy nową pozycję
                for(int w=0; w<wolfs_no; w++)
                {
                    double[] wolf_row = new double[this.dim];
                    for (int d=0; d<this.dim; d++)
                    {
                        double[] D = new double[3];
                        double[] X = new double[3];
                        for(int no=0; no<3; no++)
                        {
                            double r1 = random.NextDouble();
                            double r2 = random.NextDouble();
                            double A = 2 * a * r1 - a;
                            double C = 2 * r2;

                            D[no] = Math.Abs(C * best_wolfs_positions[no, d] * wolfs_positions[w, d]);
                            X[no] = best_wolfs_positions[no, d] - A * D[no];
                        }
                        wolfs_positions[w, d] = X.Sum() / 3;
                        wolf_row[d] = wolfs_positions[w, d];
                    }
                    wolfs_values[w] = this.Func(wolf_row);
                }
                //Zapisujemy na krzywej zbieżności
                convergence[iter] = best_wolfs_values[0];
                Console.WriteLine("At iter " + iter + ": " + convergence[iter]);
            }
            return convergence[iter_num - 1];
        }
    }
}