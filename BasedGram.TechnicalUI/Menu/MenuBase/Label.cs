namespace BasedGram.TechnicalUI.MenuBase;

using Serilog;
public class Label(string name, List<Command> entries)
{
    private readonly string m_Name = name;
    private readonly List<Command> m_Entries = entries;
    private readonly ILogger m_logger = Log.ForContext<Label>();
    public async Task Execute(Context ctx)
    {
        if(m_Entries.Count == 1)
        {
            await m_Entries[0].Execute(ctx);
            return;
        }

        Console.WriteLine($"\nРаздел: {m_Name}");
        for (int i = 0; i < m_Entries.Count; ++i)
        {
            Console.WriteLine($"[{i + 1}] {m_Entries[i].GetDescription()}");
        }

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
    public void PrintDescription(int indent)
    {
        Console.WriteLine(m_Name);

        for (int i = 0; i < m_Entries.Count - 1; ++i)
        {
            Console.WriteLine(new string(' ', indent) + $"├ {m_Entries[i].GetDescription()}");
        }
        if (m_Entries.Count > 0)
        {
            Console.WriteLine(new string(' ', indent) + $"└ {m_Entries[m_Entries.Count - 1].GetDescription()}");
        }
    }
}