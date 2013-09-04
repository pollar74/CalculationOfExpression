using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//* Write a program that calculates the value of given arithmetical expression. 
//The expression can contain the following elements only:
//Real numbers, e.g. 5, 18.33, 3.14159, 12.6
//Arithmetic operators: +, -, *, / (standard priorities)
//Mathematical functions: ln(x), sqrt(x), pow(x,y)
//Brackets (for changing the default priorities)
//    Examples:
//    (3+5.3) * 2.7 - ln(22) / pow(2.2, -1.7)  ~ 10.6
//    pow(2, 3.14) * (3 - (3 * sqrt(2) - 3.2) + 1.5*0.3)  ~ 21.22
//    Hint: Use the classical "shunting yard" algorithm and "reverse Polish notation".


namespace CalculateExpression
{
    class CalculateExpression
    {
        static List<string> parsedExpression = new List<string>();
        static List<string> polishNotatin = new List<string>();
        static Stack<string> operatorsStack = new Stack<string>();
        static Dictionary<string, int> operators = new Dictionary<string, int>()
        {
              {"+", 0},
              {"-", 0 },
              {"*", 1},
              {"/", 1 },
              {"(", -1},
              {")",-1},
              {"sqrt", 2},
              {"ln", 2},
              {"pow", 2},
        };

        static void Main(string[] args)
        {
            //Console.WriteLine("Please, input arithmetical expression for calculation:");
            //string inputExpression = Console.ReadLine();
            //string inputExpression = "1+(3 + 5)*2-4+sqrt(4)";
            //string inputExpression = "(3+5.3) * 2.7 - ln(22) / pow(2.2, -1.7)";

            string inputExpression = "pow(2, 3.14) * (3 - (3 * sqrt(2) - 3.2) + 1.5*0.3)";

            List<string> expressionToString = ParseInput(inputExpression);

            ParseToPolishNotation();

            double result = CalculatePolishExpression();
            Console.WriteLine("Result is {0}", result);
        }

        static List<string> ParseInput(string inputExpression)
        {

            string token = "";
            //string operatorToken = "";
            string type = "";
            for (int i = 0; i < inputExpression.Length; i++)
            {
                if (i == 0)
                {
                    if (Char.IsDigit(inputExpression[i]) || inputExpression[i] == '.')
                    {
                        token += inputExpression[i];
                        type = "digit";
                    }
                    else
                    {
                        token += inputExpression[i];
                        type = "operator";
                    }
                }
                else
                {
                    if (inputExpression[i] == ' ')
                    {
                        continue;
                    }
                    if (inputExpression[i] == ',')
                    {
                        parsedExpression.Add(token);
                        token = "";
                        continue;
                    }
                    if (inputExpression[i] == '-' && parsedExpression[parsedExpression.Count - 3] == "pow")
                    {
                        token = "-";
                        continue;
                    }
                    if (Char.IsDigit(inputExpression[i]) && type == "digit" || inputExpression[i] == '.')
                    {
                        token += inputExpression[i];
                    }
                    else if (!Char.IsDigit(inputExpression[i]) && type == "operator")
                    {
                        token += inputExpression[i];
                        if (IsOperator(token))
                        {
                            parsedExpression.Add(token);
                            token = "";
                        }
                    }
                    else if (Char.IsDigit(inputExpression[i]) && type == "operator")
                    {
                        if (token != "")
                        {
                            parsedExpression.Add(token);
                        }

                        token = "";
                        token += inputExpression[i];
                        type = "digit";
                    }
                    else if (!Char.IsDigit(inputExpression[i]) && type == "digit")
                    {
                        parsedExpression.Add(token);
                        token = "";
                        token += inputExpression[i];
                        if (IsOperator(token))
                        {
                            parsedExpression.Add(token);
                            token = "";
                        }
                        type = "operator";
                    }
                    if (i == inputExpression.Length - 1 && token != "")
                    {
                        parsedExpression.Add(token);
                    }
                }


            }
            return parsedExpression;
        }

        static void ParseToPolishNotation()
        {
            for (int i = 0; i < parsedExpression.Count; i++)
            {
                double number = 0;
                bool isNumber = double.TryParse(parsedExpression[i], out number);
                if (isNumber)
                {
                    polishNotatin.Add(number.ToString());
                }
                else if (!isNumber)
                {
                    if (operatorsStack.Count == 0 || ReturnKey(parsedExpression[i]) > ReturnKey(operatorsStack.Peek()))
                    {
                        operatorsStack.Push(parsedExpression[i]);
                    }
                    //if (parsedExpression[i]==")"&&operatorsStack.Peek()=="(")
                    //{

                    //}
                    else if (ReturnKey(parsedExpression[i]) <= ReturnKey(operatorsStack.Peek()))
                    {
                        if (parsedExpression[i] == "(")
                        {
                            operatorsStack.Push(parsedExpression[i]);
                            continue;
                        }
                        if (parsedExpression[i] == ")")
                        {
                            while (operatorsStack.Peek() != "(")
                            {
                                polishNotatin.Add(operatorsStack.Pop());
                            }
                            operatorsStack.Pop();
                            continue;
                        }
                        while (operatorsStack.Count > 0 && ReturnKey(parsedExpression[i]) <= ReturnKey(operatorsStack.Peek()))
                        {
                            polishNotatin.Add(operatorsStack.Pop());
                        }
                        operatorsStack.Push(parsedExpression[i]);
                    }
                }
            }
            while (operatorsStack.Count > 0)
            {
                polishNotatin.Add(operatorsStack.Pop());
            }

        }

        static double CalculatePolishExpression()
        {
            List<double> calculationList = new List<double>();
            //double result=0;

            while (polishNotatin.Count > 1)
            {
                double number = 0;
                int position = 0;
                bool tokenIsFunction = false;
                for (position = 0; position < polishNotatin.Count; position++)
                {
                    bool isNumber = double.TryParse(polishNotatin[position], out number);
                    tokenIsFunction = IsFunction(polishNotatin[position]);

                    if (!isNumber && calculationList.Count == 0 && !tokenIsFunction)
                    {
                        calculationList.Add(Convert.ToDouble(polishNotatin[position - 2]));
                        calculationList.Add(Convert.ToDouble(polishNotatin[position - 1]));
                        polishNotatin.Remove(polishNotatin[position - 2]);
                        polishNotatin.Remove(polishNotatin[position - 2]);

                        break;
                    }
                    else if (!isNumber && calculationList.Count == 0 && tokenIsFunction)
                    {
                        switch (polishNotatin[position])
                        {
                            case "sqrt":
                                {
                                    double result = Math.Sqrt(Convert.ToDouble(polishNotatin[position - 1]));
                                    polishNotatin.Remove(polishNotatin[position]);
                                    polishNotatin.Remove(polishNotatin[position - 1]);
                                    polishNotatin.Insert(position - 1, result.ToString());
                                }
                                break;
                            case "ln":
                                {
                                    double result = Math.Log(Convert.ToDouble(polishNotatin[position - 1]));
                                    polishNotatin.Remove(polishNotatin[position]);
                                    polishNotatin.Remove(polishNotatin[position - 1]);
                                    polishNotatin.Insert(position - 1, result.ToString());
                                }
                                break;
                            case "pow":
                                {
                                    double result = Math.Pow(Convert.ToDouble(polishNotatin[position - 2]), Convert.ToDouble(polishNotatin[position - 1]));
                                    polishNotatin.Remove(polishNotatin[position - 2]);
                                    polishNotatin.Remove(polishNotatin[position - 2]);
                                    polishNotatin.Remove(polishNotatin[position - 2]);
                                    polishNotatin.Insert(position - 2, result.ToString());
                                }
                                break;
                        }
                        break;
                    }
                }
                if (!tokenIsFunction)
                {
                    if (polishNotatin[position - 2] == "+")
                    {
                        number = calculationList.First() + calculationList.Last();
                        calculationList.Clear();

                    }
                    else if (polishNotatin[position - 2] == "-")
                    {
                        number = calculationList.First() - calculationList.Last();
                        calculationList.Clear();
                    }
                    else if (polishNotatin[position - 2] == "*")
                    {
                        number = calculationList.First() * calculationList.Last();
                        calculationList.Clear();
                    }
                    else if (polishNotatin[position - 2] == "/")
                    {
                        number = calculationList.First() / calculationList.Last();
                        calculationList.Clear();
                    }

                    polishNotatin.Insert(position - 2, number.ToString());
                    polishNotatin.Remove(polishNotatin[position - 1]);
                }
            }
            return Convert.ToDouble(polishNotatin.First());
        }

        static int ReturnKey(string value)
        {
            int key = -1;
            foreach (KeyValuePair<string, int> pair in operators)
            {
                if (pair.Key == value)
                {
                    return pair.Value;
                }
            }
            return key;
        }

        static bool IsOperator(string value)
        {
            bool isOperator = false;
            foreach (KeyValuePair<string, int> pair in operators)
            {
                if (pair.Key == value)
                {
                    return true;
                }
            }
            return isOperator;
        }

        static bool IsFunction(string value)
        {
            bool isFunction = false;
            foreach (KeyValuePair<string, int> pair in operators)
            {
                if (pair.Key == value && pair.Key.Length > 1)
                {
                    return true;
                }
            }
            return isFunction;
        }

    }
}
