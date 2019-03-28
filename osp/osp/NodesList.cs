using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using JourneyExceptions;

// Для дебаггинга.
using System.Windows.Forms;


/*
 * В модуле описываются типы и структуры данных, которые 
 * используются во многих других модулях программы.
 */

namespace osp
{
    /* Структура, в которой хранятся координаты. */
    public struct Coordinates
    {
        public double X, Y, Z;

        public void Coords(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /* Класс Node используется для представление узлов (городов) задачи. */ 
    public class Node
    {
        public int Id; // Номер узла в дипазоне [ 1 .. DIMENSION ].
        public int Rank; // Приоритет узла во время восхождения, в противном случае порядковый номер узла в туре.
        public Node Pred = null; // Ссылка на предыдущий узел.
        public Node Succ = null; // Ссылка на следующий узел.
        public Coordinates Coords; // Координаты узла.
        public byte Tag = 0; // Переменная-тег для отмечания узлов. 
        public double[] Costs = null; // Стоимость маршрутов к различным узлам. 
    }

    /* Класс NodesList — абстрактный тип данных, представлющий собой список узлов (тур) и операций над ними. */ 
    public class NodesList
    {
        // Поля класса.
        private List<Node> nodesList = new List<Node>(); // Непосредственно сам список узлов.
        private Node firstNode = null; // Ссылка на первый узел. 
        private Node lastNode = null; // Ссылка на последний узел.
        private double bestCost = double.MaxValue; // Лучшая стоимость тура, устанавливается вручную при вычислениях.   
        private Distance distance = Distances.Distance_1; // Используемая функция при вычислениях расстояний.
        private double[][] costMatrix = null; // Матрица расстояний. 
        private int dimension; // Измерение — общее количество узлов.
        private bool isCostsAssiciates = false; // Сообщает, связаны ли строки матрицы расстояний с узлами в списке. 
       
        //===============================================================================
        // Свойства класса.
        public List<Node> List
        {
            get { return nodesList; }
            set { nodesList = value; }
        }
        public Node FirstNode
        {
            get { return firstNode; }
            set { 
                    firstNode = value;
                    lastNode = firstNode.Pred;
                }
        }
        public Node LastNode
        {
            get { return lastNode; }
            set { 
                    lastNode = value;
                    firstNode = lastNode.Succ;
                }
        }
        public double Cost
        {
            get 
            { 
                return FindTourCost(); 
            }
        }
        public double BestCost
        {
            get { return bestCost; }
            set { bestCost = value; }
        }
        public Distance Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        public double[][] CostMatrix
        {
            get { return costMatrix; }
            set 
            { 
                costMatrix = value;
                AssociateCosts();
            }
        }
        public int Dimension
        {
            get { return this.nodesList.Count(); }
        }
        public bool IsCostsAssiciates
        {
            get { return isCostsAssiciates; }
        }

        //===============================================================================
        // Методы класса.

        /* Создаёт объект NodesList с количеством элементов равным Dimension. Индексы устанавливаются по порядку, если не указано иное в параметре zeroIndex. */
        public void CreateNodes(int dimension, Boolean zeroIndex = false)
        {
            bestCost = double.MaxValue;

            int i; 
            
            if (dimension <= 0)
                throw new InvalidTSPLibValueException("DIMENSION меньше или равен нулю.");

            //if (ProblemType == ATSP)
            //    Dimension *= 2;
            //else if (ProblemType == HPP) {
            //    Dimension++;
            //    if (Dimension > MaxMatrixDimension)
            //        eprintf("Dimension too large in HPP problem");
    //}

            for (i = 0; i < dimension; i++)
            {
                Node node = new Node();


                // TO DO — разобраться с индексами и обработкой ошибки.
                if (zeroIndex)
                    node.Id = 0;
                else
                    node.Id = i + 1;

             //   node.Coords.X = i;
             //   node.Coords.Y = i;
             //   node.Coords.Z = i;
                nodesList.Add(node);
 
                if ((i != 0) && ( i + 1 <= dimension))
                {
                    Link(nodesList[i - 1], nodesList[i]);
            //        nodesList[i].Pred = nodesList[i - 1];
            //        nodesList[i - 1].Succ = node;
                }
            }

            // Первый и последний узлы ссылаются друг на друга.
            nodesList[0].Pred = nodesList[dimension - 1];
            nodesList[dimension - 1].Succ = nodesList[0];

            // Записываем в объект ссылки на первый и последний узлы.
            firstNode = nodesList[0];
            lastNode = nodesList[dimension - 1];
        }

        /* Связывает два узла ссылками друг на друга. */
        static public void Link(Node a, Node b)
        {
            a.Succ = b;
            b.Pred = a;
        }

        /* Связывает два узла так, чтобы b следовал после a. */
        static public void Follow(Node b, Node a)
        {
            if (a.Succ != b) 
            {
                Link(b.Pred, b.Succ);
                Link(b, a.Succ);
                Link(a, b);
            }       
        }

        /* Меняет на обратный порядок следования узлов в туре начиная с узла b, который становится первым. */
        static public void Reverse(Node a, Node b)
        {
//            MessageBox.Show("a.Id = " + a.Id.ToString() + " b.Id = " + b.Id.ToString());
            // Выходим, если переданы одинаковые узлы. 
            if ((a == b))
            {
                return; 
            }

            if (b.Succ == a)
            {
           //     MessageBox.Show("b.Succ == a");
                Follow(b, a);
                return;
            }

            // Особый случай для двух элементов.
            if (a.Succ == b)
            {
          //      MessageBox.Show("a.Succ == b");
                Follow(a, b);
                return;
            }

            Node n = b; // Начинаем с конечного элемента, который должен стать первым.
            Node next = b.Succ; // Очерёдный элемент, который требуется поставить на первое место.
            Node prevN = a.Pred; // Предыдущий элемент, перед которым должен стать очерёдный.

            while (n != a)
            {
             //   MessageBox.Show("Разбираем узел " + n.Id.ToString());
                // Ставим n перед prevN. 
                Follow(n, prevN);
                // Теперь n должеть стать элементом перед новым n.
                prevN = n;
                // Очерёдный элемент, это первый элмент с конца. 
                n = next.Pred;
            }
        }

        /* Устанавливает в 0 свойство "тег" всех узлов. */
        public void ClearTags()
        {
            int i;
            for (i = 0; i < nodesList.Count; i++)
            {
                nodesList[i].Tag = 0;
            }
        }

        /* Процедура связывает затраты из матрицы затрат с каждым узлом (первому узлу — стоимость поездок в первый город, и т.д.). */
        public void AssociateCosts()
        {
            int i;
            for (i = 0; i < nodesList.Count; i++)
            {
                nodesList[i].Costs = CostMatrix[i];
            }
            isCostsAssiciates = true; 
        }

        /* Возвращает строку со всеми узлами в ней. */
        public string GetAllNodesToString()
        {
            Node n = firstNode;
            StringBuilder str = new StringBuilder();
            do
            {
                str.Append("ID = " + n.Id + "\n" + "X = " + n.Coords.X + " Y = " + n.Coords.Y + " Z = " + n.Coords.Z + "\n");
                n = n.Succ;
            } while (n != FirstNode);
            return str.ToString();
        }

        /* Считает текущую стоимость тура, используя для этого координаты (если есть) или матриу расстояний. */
        public double FindTourCost()
        {
            Node n = firstNode;
            double result = 0;
            if (distance == Distances.Distance_EXPLICIT)
            {
                if (costMatrix == null)
                {
                    throw new Exception("Матрица расстояний не создана!");
                }
                // Считаем по матрице расстояний.
                do 
                {
                    
                    result = result + n.Costs[n.Succ.Id - 1];
                    n = n.Succ;
                } while (n != FirstNode);                
            }
            else
            {
                // Считаем по координатам.
                do 
                {
                    result = result + distance(n, n.Succ);
                    n = n.Succ;
                } while (n != FirstNode);
            }
            return result;
        }

        /* Создаёт матрицу расстояний, используя введённые координаты. */
        public void CreateCostMatrix()
        {
            double[][] costMatrix = new double[nodesList.Count][];
            // Сначала просто выделяем память под матрицу.
            for (int i = 0; i < nodesList.Count; i++)
            {
                costMatrix[i] = new double[nodesList.Count];
                costMatrix[i][i] = 0; 
            }

            // Теперь считаем расстояния и заполняем матрицу. 
            for (int i = 0; i < nodesList.Count - 1; i++)
            {
                for(int j = i + 1; j < nodesList.Count; j++)
                {
                    costMatrix[i][j] = Distance(List[i],List[j]);
                    costMatrix[j][i] = costMatrix[i][j]; // Зеркально дублируем данные.
                }
            }

            this.costMatrix = costMatrix;
        } 

        /* Возвращает матрицу расстояний в строковом виде. */
        public string CostMatrixToString()
        {
            if (costMatrix == null)
                return "";
            StringBuilder strCostMatrix = new StringBuilder();

            // Сначала просто выделяем память под матрицу.
            for (int i = 0; i < nodesList.Count; i++)
            {
                for (int j = 0; j < nodesList.Count; j++)
                {
                    strCostMatrix.Append(Math.Ceiling(costMatrix[i][j]).ToString());
                    strCostMatrix.Append(" ");
                }
                strCostMatrix.Append("\r\n");
            }
            return strCostMatrix.ToString();

        }

        public void CreateTourFromArray(int[] tour)
        {
            CreateNodes(tour.Count());
            for (int i = 0; i < tour.Count(); i++)
            {
                nodesList[i].Id = tour[i];
            }
        }

        public void SaveTourToArray(ref int[] tour)
        {
            tour = new int[tour.Count()];
            for (int i = 0; i < tour.Count(); i++)
            {
                tour[i] = nodesList[i].Id;
            }            
        }

    }
}