namespace PseudoFTP.Client.Utils;

class ProgressHandler<TResult>
{
    private const string Animation = @"|/-\";
    private readonly string _prompt;
    private readonly Task<TResult> _task;

    public ProgressHandler(string prompt, Task<TResult> task)
    {
        _prompt = prompt;
        _task = task;
    }

    public TResult Perform()
    {
        int i = 0;
        Console.Write($"{_prompt}... ");

        while (!_task.IsCompleted)
        {
            Console.Write($"\b{Animation[i]}");
            i = (i + 1) % Animation.Length;
            Thread.Sleep(100);
        }

        Console.WriteLine($"\r{_prompt}... Completed");

        return _task.Result;
    }
}