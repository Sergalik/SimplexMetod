using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplex
{
    public class Simplex
    {
        // Симплекс таблица
        double[,] table; 
        int str, stlb;

        // Список базисных переменных
        List<int> basis;

        // simtab - симплекс таблица без базисных переменных
        public Simplex(double[,] simtab)
        {
            str = simtab.GetLength(0);
            stlb = simtab.GetLength(1);
            table = new double[str, stlb + str - 1];
            basis = new List<int>();

            // Добавление фиктивных переменных
            for (int i = 0; i < str; i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    if (j < stlb)
                        table[i, j] = simtab[i, j];
                    else
                        table[i, j] = 0;
                }

                // Присваиваем коэффициент 1 перед базисной переменной в строке, для корректного выстраивания фиктивных переменных, проверка
                if ((stlb + i) < table.GetLength(1))
                {
                    table[i, stlb + i] = 1;
                    basis.Add(stlb + i);
                }
            }
            stlb = table.GetLength(1);
        }

        // Нахождение разрешающего столбец
        private int FindCol()
        {
            int mainCol = 1;
            for (int j = 2; j < stlb; j++)
                if (table[str - 1, j] < table[str - 1, mainCol])
                    mainCol = j;
            Debug.WriteLine("Разрешающий столбец: " + mainCol);
            return mainCol;
        }

        // Нахождение разрещающей строки
        private int FindRow(int mainCol)
        {
            int mainRow = 0;
            for (int i = 0; i < str - 1; i++)
                if (table[i, mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
            for (int i = mainRow + 1; i < str - 1; i++)
                if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow, mainCol])))
                    mainRow = i;
            Debug.WriteLine("Разрешающая строка: " + mainRow);
            return mainRow;
        }




        // result - в этот массив будут записаны полученные значения X
        public double[,] Solution(double[] result)
        {
            // Результирующие столбец и строка
            int Col, Row;
            while (!Finale())
            {
                Col = FindCol();
                Row = FindRow(Col);
                basis[Row] = Col;
                double[,] new_table = new double[str, stlb];
                for (int j = 0; j < stlb; j++)
                    new_table[Row, j] = table[Row, j] / table[Row, Col];
                for (int i = 0; i < str; i++)
                {
                    if (i == Row)
                        continue;
                    for (int j = 0; j < stlb; j++)
                        new_table[i, j] = table[i, j] - table[i, Col] * new_table[Row, j];
                }
                table = new_table;
            }


            // Запись в result найденные значения X
            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }
            return table;
        }

        // Завершение программы, если строка оценок меньше 0
        private bool Finale() 
        {
            bool end = true;
            for (int j = 1; j < stlb; j++)
            {
                if (table[str - 1, j] < 0)
                {
                    end = false;
                    break;
                }
            }
            return end;
        }
       
    }
    public class VvodPer
    {
        public double[,] mas;
        public double[,] tableresult;
        /// <summary>
        /// Метод ввода и вывода данных
        /// </summary>
        public void simplexBol()
        {
            double[] massiv = { };
            string strok = "";
            int raz1 = 0, d = 0;

            // Взятие данных из файла csv и ввод их в массив
            try
            {
                using (StreamReader sr = new StreamReader(@"Ввод.csv"))
                {
                    sr.ReadLine();
                    strok = sr.ReadToEnd();
                    string[] st = strok.Split('\n');
                    raz1 = st.Length;
                    massiv = Array.ConvertAll(st[0].Split(';'), double.Parse);
                    d = massiv.Length;
                    mas = new double[raz1, d];
                    for (int i = 0; i < raz1; i++)
                    {
                        massiv = Array.ConvertAll(st[i].Split(';'), double.Parse);
                        for (int j = 0; j < d; j++)
                        {
                            mas[i, j] = massiv[j];

                        }
                    }

                    // Меняем первый и последний столбцы местами для того что бы удобно вводить в csv файл ограничения
                    for (int i = 0; i < raz1; i++)
                    {
                        for (int j = 0; j < d; j += d - 1)
                        {
                            double tmp = mas[i, j];
                            mas[i, j] = mas[i, d - 1];
                            mas[i, d - 1] = tmp;
                        }

                    }
                    // делаем строку оценок отрицательной для корректного вывода
                    for (int i = 0; i < raz1; i++)
                    {
                        for (int j = 0; j < d; j++)
                        {
                            if (i == raz1 - 1)
                            {
                                mas[i, j] = mas[i, j] * (-1);
                            }
                        }
                    }
                    Console.WriteLine("Исходные данные матрицы: ");
                    for (int i = 0; i < raz1; i++)
                    {
                        for (int j = 0; j < d; j++)
                        {
                            Console.Write($"{mas[i, j],5}");
                        }
                        Console.WriteLine();
                    }
                }

                //ОИнициализируем массив размерностью в два раза больше, чем введенный массив для фиктивных переменных
                double[] result = new double[raz1 * 2];

                //Конструктор класса
                Simplex Simpl = new Simplex(mas);

                //Основной метод программы
                tableresult = Simpl.Solution(result);
                for (int i = 0; i < tableresult.GetLength(0); i++)
                {
                    for (int j = 0; j < tableresult.GetLength(1); j++)
                    {
                        if (i == raz1 - 1)
                        {
                            tableresult[i, j] = tableresult[i, j] * (-1);
                        }
                    }
                }

                Console.WriteLine("\nРешённая матрица:");
                for (int i = 0; i < tableresult.GetLength(0); i++)
                {
                    for (int j = 0; j < tableresult.GetLength(1); j++)
                        Console.Write($"{Math.Round(tableresult[i, j]),5}" + ";");
                    Console.WriteLine("");
                }

                int ind1 = 1;
                Console.WriteLine("\nКонечные результаты: ");
                for (int j = d - 2; j >= 0; j--)
                {
                    Console.WriteLine("X[{0}] = {1}", ind1, result[j]);
                    ind1++;
                }

                Console.WriteLine("F = " + (tableresult[tableresult.GetLength(0) - 1, 0] * -1));
                Console.WriteLine("F' = " + (tableresult[tableresult.GetLength(0) - 1, 0]));
                using (StreamWriter sw = new StreamWriter(@"Вывод.csv"))
                {
                    sw.WriteLine("reshenie:");
                    for (int i = 0; i < tableresult.GetLength(0); i++)
                    {
                        for (int j = 0; j < tableresult.GetLength(1); j++)
                            sw.Write($"{Math.Round(tableresult[i, j]),5}" + ";");
                        sw.WriteLine();
                    }
                    ind1 = 1;
                    for (int j = d - 2; j >= 0; j--)
                    {
                        sw.WriteLine("X[{0}] = {1}", ind1, result[j]);
                        ind1++;
                    }
                    sw.WriteLine("F = " + (tableresult[tableresult.GetLength(0) - 1, 0] * -1));
                    sw.WriteLine("F' = " + (tableresult[tableresult.GetLength(0) - 1, 0]));
                }
            }
            catch
            {
                Console.WriteLine("При вводе данных допущена ошибка, проверьте правильность ввода");
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(File.CreateText("Промежуточные.txt")));
            Debug.AutoFlush = true;
            VvodPer vz = new VvodPer();
            vz.simplexBol();
            Console.ReadKey();
        }
    }
}
