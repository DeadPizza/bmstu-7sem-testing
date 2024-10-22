using BasedGram.Common.Core;
using BasedGram.Services.P2PService.Callbacks;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using Serilog;

namespace BasedGram.Services.P2PService;

public class P2PService : IP2PService
{
    private event OnMessageReceiveCallback? m_OnMessageEvent;
    private event OnUserActionCallback? m_OnUserActionEvent;
    private event OnSyncDataCallback? m_OnSyncDataEvent;
    private event OnUserListReceiveCallback? m_OnUserListEvent;
    private event OnDialogListReceiveCallback? m_OnDialogListEvent;
    private event OnImageReceiveCallback? m_OnImageReceiveCallback;
    private List<TcpClient> m_tcp_clients;
    private Thread m_RunNodeThread;
    private bool m_WorkingState;
    private readonly ILogger m_logger = Log.ForContext<P2PService>();

    public P2PService()
    {
        // m_OnMessageEvent = delegate {};
        // m_OnUserActionEvent = new OnUserActionCallback(async (OnUserActionEnum anum, User usr) => {});
        // m_OnSyncDataEvent = new OnSyncDataCallback(async () => {});
        m_tcp_clients = [];
        m_RunNodeThread = new Thread(RunNodeInternal);
        m_WorkingState = true;

        // RunNode();
    }

    ~P2PService()
    {

    }

    public void OnMessageReceive(OnMessageReceiveCallback callback)
    {
        m_logger.Verbose("OnMessageReceiveCallback added");
        m_OnMessageEvent += callback;
    }

    public void OnSyncData(OnSyncDataCallback callback)
    {
        m_logger.Verbose("OnSyncDataCallback added");
        m_OnSyncDataEvent += callback;
    }

    public void OnUserAction(OnUserActionCallback callback)
    {
        m_logger.Verbose("OnUserActionCallback added");
        m_OnUserActionEvent += callback;
    }

    public async void RunNode()
    {
        m_logger.Information("Running node...");
        IPAddress start = IPAddress.Parse("192.168.81.1");
        var bytes = start.GetAddressBytes();
        var leastSigByte = start.GetAddressBytes().Last();
        var range = 255 - leastSigByte;

        var pingReplyTasks = Enumerable.Range(leastSigByte, range)
            .Select(async x =>
            {
                var bb = start.GetAddressBytes();
                bb[3] = (byte)x;
                var destIp = new IPAddress(bb);

                try
                {
                    TcpClient clnt = new();
                    await clnt.ConnectAsync(new IPEndPoint(destIp, 9025)).WaitAsync(TimeSpan.FromMilliseconds(2000));

                    m_tcp_clients.Add(clnt);

                    // Console.WriteLine(clnt.Connected);

                    // await m_OnSyncDataEvent.Invoke();

                    // Console.WriteLine("DONE INVOKE");

                    // var stream = clnt.GetStream();
                    // var sr = new StreamReader(stream);
                    // var sw = new StreamWriter(stream);
                    // var serialized_message_type = JsonSerializer.Serialize(NetEvent.SYNC_REQUEST);
                    // sw.WriteLine(serialized_message_type);
                    // await sw.FlushAsync().ConfigureAwait(false);

                    // Console.WriteLine($"CONNECTED STATE: {clnt.Connected}");

                    // Task.Run(() => ClientHandler(clnt));
                }
                catch (TimeoutException)
                {
                    return "";
                }
                catch (System.Exception)
                {
                    // Console.WriteLine(ex.ToString());
                    // Console.WriteLine($"failed to connect to: {destIp.ToString()}");
                    return "";
                }
                return destIp.ToString();
            })
            .ToList();

        var strings_all = await Task.WhenAll(pingReplyTasks);

        foreach (var pr in strings_all)
        {
            if (pr != "")
            {
                m_logger.Information($"Connected: {pr}");
                // Console.WriteLine($"Connected: {pr}");
            }
        }


        foreach (var clnt in m_tcp_clients)
        {
            var stream = clnt.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            var serialized_message_type = JsonSerializer.Serialize(NetEvent.SYNC_REQUEST);
            sw.WriteLine(serialized_message_type);
            await sw.FlushAsync().ConfigureAwait(false);

            // Console.WriteLine($"CONNECTED STATE: {clnt.Connected}");

            await Task.Run(() => ClientHandler(clnt));
        }

        if (m_OnSyncDataEvent is not null)
            await m_OnSyncDataEvent.Invoke();
        // Console.WriteLine("DONE INVOKE");

        m_RunNodeThread.Start();
    }

    private void RunNodeInternal()
    {
        m_logger.Verbose("RunNodeInternal()");
        var server_sock = new TcpListener(IPAddress.Any, 9025);
        server_sock.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        server_sock.Start();

        while (m_WorkingState)
        {
            var accepted_client = server_sock.AcceptTcpClient();
            m_tcp_clients.Add(accepted_client);
            Task.Run(() => ClientHandler(accepted_client));
        }
    }

    private async Task ClientHandler(TcpClient client)
    {
        m_logger.Verbose($"Running ClientHandler() for {client}");
        // Console.WriteLine($"workin: {client.Connected}");
        try
        {
            var stream = client.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);

            while (true)
            {
                // Console.WriteLine("awaiting for msg");
                var msg_type_raw = await sr.ReadLineAsync().ConfigureAwait(false);
                if (msg_type_raw is null)
                {
                    break;
                }
                var msg_type = JsonSerializer.Deserialize<NetEvent>(msg_type_raw);
                // Console.WriteLine(msg_type);
                m_logger.Information($"Received {msg_type} from {client}");
                switch (msg_type)
                {
                    case NetEvent.SYNC_REQUEST:
                        {
                            if (m_OnSyncDataEvent is not null)
                                await m_OnSyncDataEvent.Invoke();
                        }
                        break;
                    case NetEvent.DIALOG_LIST:
                        {
                            var msg_raw = await sr.ReadLineAsync().ConfigureAwait(false);
                            if (msg_raw is not null && m_OnDialogListEvent is not null)
                            {
                                var msg = JsonSerializer.Deserialize<List<Dialog>>(msg_raw);
                                if (msg is not null)
                                    await m_OnDialogListEvent.Invoke(msg);
                            }
                        }
                        break;
                    case NetEvent.USER_LIST:
                        {
                            var msg_raw = await sr.ReadLineAsync().ConfigureAwait(false);
                            if (msg_raw is not null && m_OnUserListEvent is not null)
                            {
                                var msg = JsonSerializer.Deserialize<List<User>>(msg_raw);
                                if (msg is not null)
                                    await m_OnUserListEvent.Invoke(msg);
                            }
                        }
                        break;
                    case NetEvent.MESSAGE:
                        {
                            var msg_raw = await sr.ReadLineAsync().ConfigureAwait(false);
                            if (msg_raw is not null && m_OnMessageEvent is not null)
                            {
                                var msg = JsonSerializer.Deserialize<Message>(msg_raw);
                                if (msg is not null)
                                    await m_OnMessageEvent.Invoke(msg);
                            }
                        }
                        break;
                    case NetEvent.IMAGE:
                        {
                            var msg_raw = await sr.ReadLineAsync().ConfigureAwait(false);
                            if (msg_raw is not null && m_OnMessageEvent is not null)
                            {
                                var msg  = JsonSerializer.Deserialize<DTO.Image>(msg_raw);
                                if (msg is not null)
                                    await m_OnImageReceiveCallback!.Invoke(msg);
                            }
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            // Console.WriteLine(ex.ToString());
            m_logger.Error("ClientHandler(): {}", ex.ToString());
        }
    }

    public async Task SendDialogList(List<Dialog> dialogs)
    {
        m_logger.Verbose("SendDialogList() enter");
        var serialized_message_type = JsonSerializer.Serialize(NetEvent.DIALOG_LIST);
        var serialized_data = JsonSerializer.Serialize(dialogs);

        foreach (TcpClient cl in m_tcp_clients)
        {
            var stream = cl.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            {
                await sw.WriteLineAsync(serialized_message_type).ConfigureAwait(false);
                await sw.WriteLineAsync(serialized_data).ConfigureAwait(false);
                await sw.FlushAsync().ConfigureAwait(false);
            }
            m_logger.Information($"Sent DialogList to {cl}");
        }
        m_logger.Verbose("SendDialogList() exit");
    }

    public async Task SendMessage(Message message)
    {
        m_logger.Verbose("SendMessage() enter");
        var serialized_message_type = JsonSerializer.Serialize(NetEvent.MESSAGE);
        var serialized_data = JsonSerializer.Serialize(message);

        foreach (TcpClient cl in m_tcp_clients)
        {
            var stream = cl.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            {
                await sw.WriteLineAsync(serialized_message_type).ConfigureAwait(false);
                await sw.WriteLineAsync(serialized_data).ConfigureAwait(false);
                await sw.FlushAsync().ConfigureAwait(false);
            }
            m_logger.Information($"Sent Message to {cl}");
        }
        m_logger.Verbose("SendMessage() enter");
    }

    public async Task SendUserList(List<User> users)
    {
        m_logger.Verbose("SendUserList() enter");
        var serialized_message_type = JsonSerializer.Serialize(NetEvent.USER_LIST);
        var serialized_data = JsonSerializer.Serialize(users);

        foreach (TcpClient cl in m_tcp_clients)
        {
            var stream = cl.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            {
                await sw.WriteLineAsync(serialized_message_type).ConfigureAwait(false);
                await sw.WriteLineAsync(serialized_data).ConfigureAwait(false);
                await sw.FlushAsync().ConfigureAwait(false);
            }
            m_logger.Information($"Sent UserList to {cl}");
        }
        m_logger.Verbose("SendUserList() exit");
    }

    public void OnUserListReceive(OnUserListReceiveCallback callback)
    {
        m_logger.Verbose("OnUserListReceiveCallback added");
        m_OnUserListEvent += callback;
    }

    public void OnDialogListReceive(OnDialogListReceiveCallback callback)
    {
        m_logger.Verbose("OnDialogListReceiveCallback added");
        m_OnDialogListEvent += callback;
    }

    public async Task SendImage(string name, Stream image)
    {
        var serialized_message_type = JsonSerializer.Serialize(NetEvent.IMAGE);
        var serialized_data = JsonSerializer.Serialize(new DTO.Image(name, image));

        foreach (TcpClient cl in m_tcp_clients)
        {
            var stream = cl.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            {
                await sw.WriteLineAsync(serialized_message_type).ConfigureAwait(false);
                await sw.WriteLineAsync(serialized_data).ConfigureAwait(false);
                await sw.FlushAsync().ConfigureAwait(false);
            }
            m_logger.Information($"Sent UserList to {cl}");
        }
    }

    public void OnImageReceive(OnImageReceiveCallback callback)
    {
        m_OnImageReceiveCallback += callback;
    }
}