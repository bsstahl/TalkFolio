namespace TalkFolio.Api;

using Microsoft.Extensions.Logging;

internal static partial class ProgramLog
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Handling GET /talks request.")]
    public static partial void HandlingGetTalksRequest(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Returning {TalkCount} talks from GET /talks.")]
    public static partial void ReturningTalksFromGetTalks(ILogger logger, int talkCount);

    [LoggerMessage(EventId = 3, Level = LogLevel.Trace, Message = "Returning talk payload for GET /talks with talk IDs {TalkIds}.")]
    public static partial void ReturningTalkPayloadForGetTalks(ILogger logger, Guid[] talkIds);
}
