using System.Diagnostics;

namespace ChestCounter;

/// <summary>
/// Performance mode (Settings), for low-end PCs: the counter only does what is needed to count the chests.
/// <list type="bullet">
/// <item>No live reading of the replay during a match (the heaviest work while playing): the chests are counted a few
/// seconds after the match, the overlay shows the total and the red dot meanwhile.</item>
/// <item>Idle process priority (instead of below normal): the game always gets the processor first.</item>
/// <item>Minimizing the window stops its web engine (its processes end) instead of suspending it.</item>
/// <item>The window refreshes every 5 s instead of 2 s, without animations, and leaves out the extras.</item>
/// <item>A lean report (totals, chances, treasures, matches), refreshed every 60 s, and no OBS overlay page.</item>
/// </list>
/// </summary>
public static class Performance
{
    public static volatile bool On;

    public static void Apply(Config config)
    {
        On = config.PerformanceMode;
        SetPriority();
    }

    /// <summary>Idle in performance mode, below normal otherwise (the game always comes first).</summary>
    public static void SetPriority()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        try { Process.GetCurrentProcess().PriorityClass = On ? ProcessPriorityClass.Idle : ProcessPriorityClass.BelowNormal; } catch { }
    }

    /// <summary>A PC that would benefit from it: 4 logical processors or fewer, or 8 GB of memory or less.</summary>
    public static bool Recommended
    {
        get
        {
            try
            {
                var memory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
                return Environment.ProcessorCount <= 4 || (memory > 0 && memory <= 8L * 1024 * 1024 * 1024);
            }
            catch
            {
                return false;
            }
        }
    }
}
