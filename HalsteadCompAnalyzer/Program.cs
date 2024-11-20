using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: HolstedAnalyzer <path to C# project or directory>");
            return;
        }

        string directoryPath = args[0];

        // Получаем все .cs файлы в директории
        var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        // Инициализируем суммарные метрики
        double totalN1 = 0, totalN2 = 0, totalUniqueOperators = 0, totalUniqueOperands = 0;

        foreach (var file in csFiles)
        {
            var metrics = AnalyzeCode(File.ReadAllText(file));
            totalN1 += metrics.Item1; // N1
            totalN2 += metrics.Item2; // N2
            totalUniqueOperators += metrics.Item3; // U1
            totalUniqueOperands += metrics.Item4; // U2
        }

        // Выводим совокупные метрики
        Console.WriteLine($"Total Unique Operators (U1): {totalUniqueOperators}");
        Console.WriteLine($"Total Unique Operands (U2): {totalUniqueOperands}");
        Console.WriteLine($"Total Operators (N1): {totalN1}");
        Console.WriteLine($"Total Operands (N2): {totalN2}");

        if ((totalUniqueOperators + totalUniqueOperands) > 0)
        {
            double vocabulary = totalUniqueOperators + totalUniqueOperands;
            double length = totalN1 + totalN2;
            double volume = length * Math.Log2(vocabulary);
            double difficulty = (totalUniqueOperators > 0 && totalUniqueOperands > 0)
                                ? totalUniqueOperators / 2.0 * (totalN2 / totalUniqueOperands)
                                : 0;
            double effort = difficulty * volume;

            Console.WriteLine($"Vocabulary (n): {vocabulary}");
            Console.WriteLine($"Length (N): {length}");
            Console.WriteLine($"Volume (V): {volume}");
            Console.WriteLine($"Difficulty (D): {difficulty}");
            Console.WriteLine($"Effort (E): {effort}");
        }
        else
        {
            Console.WriteLine("No operators or operands found in the provided project.");
        }
    }

    static Tuple<int, int, int, int> AnalyzeCode(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();

        HashSet<string> operators = [];
        HashSet<string> operands = [];

        // Перебираем узлы синтаксического дерева
        foreach (var token in root.DescendantTokens())
        {
            // Отбираем операторы
            if (token.IsKind(SyntaxKind.PlusToken) ||
                token.IsKind(SyntaxKind.MinusToken) ||
                token.IsKind(SyntaxKind.AsteriskToken) ||
                token.IsKind(SyntaxKind.SlashToken) ||
                token.IsKind(SyntaxKind.PercentToken) ||
                token.IsKind(SyntaxKind.EqualsToken) ||
                token.IsKind(SyntaxKind.GreaterThanToken) ||
                token.IsKind(SyntaxKind.LessThanToken))
            {
                operators.Add(token.Text);
            }

            // Отбираем операнды (именованные сущности, например, переменные)
            if (token.IsKind(SyntaxKind.IdentifierToken))
            {
                operands.Add(token.Text);
            }
        }

        // Подсчитываем метрики
        int n1 = operators.Count; // Количество уникальных операторов
        int n2 = operands.Count;  // Количество уникальных операндов
        int N1 = root.DescendantTokens().Count(t => t.IsOperator()); // Общее число операторов
        int N2 = root.DescendantTokens().Count(t => t.IsKind(SyntaxKind.IdentifierToken)); // Общее число операндов

        return new Tuple<int, int, int, int>(n1, n2, N1, N2);
    }

    public static void Prikols(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday:
                Console.WriteLine("Today is Monday!");
                break;
            case DayOfWeek.Tuesday:
                Console.WriteLine("Today is Tuesday!");
                break;
            case DayOfWeek.Wednesday:
                Console.WriteLine("Today is Wednesday!");
                break;
            case DayOfWeek.Thursday:
                Console.WriteLine("Today is Thursday!");
                break;
            case DayOfWeek.Friday:
                Console.WriteLine("Today is Friday!");
                break;
            case DayOfWeek.Saturday:
                Console.WriteLine("Today is Saturday!");
                break;
            case DayOfWeek.Sunday:
                Console.WriteLine("Today is Sunday!");
                break;
        }
    }

    static bool IsOperator(this SyntaxToken token)
    {
        return token.IsKind(SyntaxKind.PlusToken) ||
               token.IsKind(SyntaxKind.MinusToken) ||
               token.IsKind(SyntaxKind.AsteriskToken) ||
               token.IsKind(SyntaxKind.SlashToken) ||
               token.IsKind(SyntaxKind.PercentToken) ||
               token.IsKind(SyntaxKind.EqualsToken) ||
               token.IsKind(SyntaxKind.GreaterThanToken) ||
               token.IsKind(SyntaxKind.LessThanToken);
    }
}
