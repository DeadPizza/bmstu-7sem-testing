using BasedGram.TechnicalUI.MenuBase;

using BasedGram.TechnicalUI.GuestMenu.Auth;
using BasedGram.TechnicalUI.CommonMenu;

namespace BasedGram.TechnicalUI.MenuBuilders;

public class GuestMenuBuilder : MenuBuilder
{
    public override Menu Build()
    {
        return new Menu("Гость",
        [
            new Label("Аккаунт", 
            [
                new LoginCommand(),
                new RegisterCommand()
            ]),
            new Label("Интерфейс",
            [
                new EndSessionCommand()
            ])
        ]);
    }
}