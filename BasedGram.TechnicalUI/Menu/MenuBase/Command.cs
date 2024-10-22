namespace BasedGram.TechnicalUI.MenuBase;

abstract public class Command
{
    abstract public Task Execute(Context ctx);
    abstract public String GetDescription();
}