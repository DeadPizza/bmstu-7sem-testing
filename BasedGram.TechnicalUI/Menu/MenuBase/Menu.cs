namespace BasedGram.TechnicalUI.MenuBase;

using Serilog;

public class Menu(string name, List<Label> entries)
{
    private readonly string m_Name = name;
    private readonly List<Label> m_Entries = entries;
    private readonly ILogger m_logger = Log.ForContext<Menu>();

    public async Task Execute(Context ctx)
    {
        Console.Write("Введите номер: ");
        if (!int.TryParse(Console.ReadLine(), out int no))
        {
            Console.WriteLine("[!] Ошибка");
            return;
        }

        m_logger.Information($"User input: \"{no}\"");

        if (!(1 <= no && no <= m_Entries.Count))
        {
            m_logger.Error($"{no} not in [1, {m_Entries.Count}]");
            Console.WriteLine("[!] Ошибка");
            return;
        }

        await m_Entries[no - 1].Execute(ctx);
    }
    public void PrintDescription()
    {
        Console.WriteLine($"Меню для роли: {m_Name}");
        for (int i = 0; i < m_Entries.Count; ++i)
        {
            Console.Write($"[{i + 1}] ");
            m_Entries[i].PrintDescription(4);
        }
    }
}