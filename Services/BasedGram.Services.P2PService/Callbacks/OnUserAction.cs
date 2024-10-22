using BasedGram.Common.Core;
using BasedGram.Services.P2PService.Callbacks.Enums;

namespace BasedGram.Services.P2PService.Callbacks;

public delegate Task OnUserActionCallback(OnUserActionEnum action, User user);