using BasedGram.Common.Core;
using BasedGram.Services.P2PService.Callbacks;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BasedGram.Services.P2PService;

internal enum NetEvent
{
    SYNC_REQUEST,
    DIALOG_LIST,
    USER_LIST,
    MESSAGE,
    IMAGE
}