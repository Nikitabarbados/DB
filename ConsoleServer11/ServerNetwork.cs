using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServerNetwork
{
    private TcpListener _listener = null!;
    private TcpClient? _client;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    private GameState _state = new();
    private DateTime _startTime;
    private bool _clientConnected = false;



    public async Task StartAsync()
    {
        _listener = new TcpListener(IPAddress.Any, 27015);
        _listener.Start();
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Waiting for client...");

        _client = await _listener.AcceptTcpClientAsync();
        var stream = _client.GetStream();
        _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        _reader = new StreamReader(stream, Encoding.UTF8);
        _clientConnected = true;

        Console.WriteLine("Client connected! Game starting...");

        // Отправляем начальное состояние
        await _writer.WriteLineAsync(_state.Serialize());

        _startTime = DateTime.Now;

        // Запускаем чтение от клиента и управление сервером параллельно
        var readTask = ReadFromClientAsync();
        var inputTask = HandleServerInputAsync();
        var timerTask = TimerAsync();

        await Task.WhenAny(readTask, timerTask);
    }

    private async Task ReadFromClientAsync()
    {
        try
        {
            while (true)
            {
                string? line = await _reader!.ReadLineAsync();
                if (line == null) break;

                // Формат от клиента: "X Y"
                var parts = line.Trim().Split(' ');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int nx) &&
                    int.TryParse(parts[1], out int ny))
                {
                    TryMoveBlue(nx, ny);
                    await SendStateAsync();
                }
            }
        }
        catch { }
    }

    private async Task HandleServerInputAsync()
    {
        while (!_state.GameOver)
        {
            if (!Console.KeyAvailable) { await Task.Delay(50); continue; }

            var key = Console.ReadKey(true).Key;
            int nx = _state.RedX, ny = _state.RedY;

            switch (key)
            {
                case ConsoleKey.LeftArrow: nx--; break;
                case ConsoleKey.RightArrow: nx++; break;
                case ConsoleKey.UpArrow: ny--; break;
                case ConsoleKey.DownArrow: ny++; break;
                default: continue;
            }

            TryMoveRed(nx, ny);
            RenderMap();
            if (_clientConnected) await SendStateAsync();
        }
    }

    private async Task TimerAsync()
    {
        while (true)
        {
            await Task.Delay(1000);
            var elapsed = DateTime.Now - _startTime;
            int remaining = 60 - (int)elapsed.TotalSeconds;

            if (remaining <= 0)
            {
                EndGame();
                RenderMap();
                if (_clientConnected) await SendStateAsync();
                break;
            }

            RenderMap(remaining);
        }
    }

    private void TryMoveBlue(int nx, int ny)
    {
        if (!IsWalkable(nx, ny)) return;
        if (nx == _state.RedX && ny == _state.RedY) return; // блокировка

        _state.BlueX = nx;
        _state.BlueY = ny;
        CheckTreasure(nx, ny, true);

        if (_state.Map[ny, nx] == 'F') { _state.BlueDone = true; EndGame(); }
    }

    private void TryMoveRed(int nx, int ny)
    {
        if (!IsWalkable(nx, ny)) return;
        if (nx == _state.BlueX && ny == _state.BlueY) return; // блокировка

        _state.RedX = nx;
        _state.RedY = ny;
        CheckTreasure(nx, ny, false);

        if (_state.Map[ny, nx] == 'F') { _state.RedDone = true; EndGame(); }
    }

    private bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 || x >= GameMap.Width || y >= GameMap.Height) return false;
        char c = _state.Map[y, x];
        return c != '#';
    }

    private void CheckTreasure(int x, int y, bool isBlue)
    {
        if (_state.Map[y, x] == 'T')
        {
            _state.Map[y, x] = ' ';
            if (isBlue) _state.BlueScore++;
            else _state.RedScore++;
        }
    }

    private void EndGame()
    {
        _state.GameOver = true;
        if (_state.BlueDone && !_state.RedDone)
            _state.Result = "BLUE_WINS_FINISH";
        else if (_state.RedDone && !_state.BlueDone)
            _state.Result = "RED_WINS_FINISH";
        else if (_state.BlueScore > _state.RedScore)
            _state.Result = "BLUE_WINS_SCORE";
        else if (_state.RedScore > _state.BlueScore)
            _state.Result = "RED_WINS_SCORE";
        else
            _state.Result = "DRAW";
    }

    private async Task SendStateAsync()
    {
        try { await _writer!.WriteLineAsync(_state.Serialize()); }
        catch { }
    }

    public void RenderMap(int secondsLeft = 0)
    {
        Console.Clear();
        Console.WriteLine($"=== MAZE GAME === Time: {secondsLeft}s | Red: {_state.RedScore} pts | Blue: {_state.BlueScore} pts");
        Console.WriteLine("Red = YOU (arrows) | Blue = CLIENT");

        for (int y = 0; y < GameMap.Height; y++)
        {
            for (int x = 0; x < GameMap.Width; x++)
            {
                if (x == _state.RedX && y == _state.RedY)
                { Console.ForegroundColor = ConsoleColor.Red; Console.Write("😊"); Console.ResetColor(); }
                else if (x == _state.BlueX && y == _state.BlueY)
                { Console.ForegroundColor = ConsoleColor.Blue; Console.Write("😊"); Console.ResetColor(); }
                else
                {
                    char c = _state.Map[y, x];
                    Console.ForegroundColor = c switch
                    {
                        '#' => ConsoleColor.DarkGray,
                        'T' => ConsoleColor.Yellow,
                        'F' => ConsoleColor.Green,
                        _ => ConsoleColor.White
                    };
                    Console.Write(c == '#' ? "██" : c == 'T' ? "📦" : c == 'F' ? "🏁" : "  ");
                    Console.ResetColor();
                }
            }
            Console.WriteLine();
        }

        if (_state.GameOver)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n GAME OVER! {_state.Result.Replace('_', ' ')}");
            Console.ResetColor();
        }
    }
}