namespace Content.Server.RPSX.Craft.StationGoals.Graph;

internal enum ExecuteState
{
    Idle,
    InProgress,
    WaitingDelay,
    Finished,
    Interrupted,
    InnerInterrupted
}
