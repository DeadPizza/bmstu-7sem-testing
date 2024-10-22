using BasedGram.TechnicalUI.MenuBase;

using BasedGram.TechnicalUI.CommonMenu.Dialogs;
using BasedGram.TechnicalUI.CommonMenu.Users;
using BasedGram.TechnicalUI.CommonMenu.Auth;
using BasedGram.TechnicalUI.CommonMenu;

namespace BasedGram.TechnicalUI.MenuBuilders;

public class UserMenuBuilder : MenuBuilder
{
    public override Menu Build()
    {
        return new Menu("Пользователь",
        [
            new Label("Аккаунт", 
            [
                new LogoutCommand(),
                new ProfileInfoCommand()
            ]),
            new Label("Диалоги",
            [
                new ListDialogsCommand(),
                new ListMessagesCommand(),
                new WriteMessageCommand(),
                new ListNewMessagesCommand(),
                new BlockDialogCommand(),
                new SetReactionCommand()
            ]),
            new Label("Пользователи",
            [
                new FindUserCommand(),
                new NewDialogCommand(),
            ]),
            new Label("Интерфейс",
            [
                new EndSessionCommand()
            ])
        ]);
    }
}