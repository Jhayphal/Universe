#if DEBUG
#define LOG_TIME
#endif

using System.Diagnostics;

namespace Universe;

internal struct Chronograph
{
  private static int id = 0;

  private Stopwatch stopwatch;
  private string message;

  public Chronograph(string message)
  {
    this.message = message;
  }

  private Stopwatch Stopwatch => stopwatch ??= new Stopwatch();

  private string Message => message ??= "chronograph " + ++id;

  public void Start()
  {
#if LOG_TIME
    Stopwatch.Start();
#endif
  }

  public void Stop()
  {
#if LOG_TIME
    Stopwatch.Stop();
    Debug.WriteLine(Message + Stopwatch.Elapsed.TotalMilliseconds);
    Stopwatch.Reset();
#endif
  }
}